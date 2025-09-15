using System;
using Unity.AppUI.Bridge;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UIElements.Experimental;

namespace Unity.AppUI.UI
{
    /// <summary>
    /// This manipulator allows panning and zooming of the content.
    /// </summary>
    /// <remarks>
    /// This manipulator is used on the <see cref="Canvas"/> UI element.
    /// </remarks>
    public partial class PanAndZoomable : Manipulator
    {
        [EnumName("GetGrabModeUssClassName", typeof(GrabMode))]
        const string k_CursorUssClassName = Styles.cursorUsClassNamePrefix;

        const CanvasControlScheme k_DefaultControlScheme = CanvasControlScheme.Modern;

        const CanvasManipulator k_DefaultPrimaryManipulator = CanvasManipulator.None;

        const int k_DefaultDampingEffectDurationMs = 750;

        const float k_DefaultScrollSpeed = 2f;

        const float k_DefaultMinZoom = 0.1f;

        const float k_DefaultMaxZoom = 100.0f;

        const float k_DefaultZoomSpeed = 0.075f;

        const float k_DefaultZoomMultiplier = 2f;

        const float k_DefaultPanMultiplier = 3f;

        const bool k_DefaultUseSpaceBar = true;

        int m_PointerId = PointerId.invalidPointerId;

        Vector2 m_Velocity;

        Vector3 m_PointerPosition;

        long m_LastTimestamp;

        bool m_SpaceBarPressed;

        float m_ZoomMultiplier;

        ValueAnimation<float> m_DampingEffect;

        GrabMode m_GrabMode = GrabMode.None;

        CanvasManipulator m_PrimaryManipulator = k_DefaultPrimaryManipulator;

        readonly ZoomChangedDelegate m_ZoomChanged;

        readonly ScrollOffsetChangedDelegate m_ScrollOffsetChanged;

        readonly GrabModeChangedDelegate m_GrabModeChanged;

        /// <summary>
        /// The control scheme used to interact with the canvas.
        /// </summary>
        /// <seealso cref="CanvasControlScheme"/>
        public CanvasControlScheme controlScheme { get; set; } = k_DefaultControlScheme;

        /// <summary>
        /// The speed at which the canvas scrolls.
        /// </summary>
        public float scrollSpeed { get; set; } = k_DefaultScrollSpeed;

        /// <summary>
        /// The minimum zoom level.
        /// </summary>
        public float minZoom { get; set; } = k_DefaultMinZoom;

        /// <summary>
        /// The maximum zoom level.
        /// </summary>
        public float maxZoom { get; set; } = k_DefaultMaxZoom;

        /// <summary>
        /// The speed at which the manipulator zooms.
        /// </summary>
        public float zoomSpeed { get; set; } = k_DefaultZoomSpeed;

        /// <summary>
        /// The zoom multiplier.
        /// </summary>
        public float zoomMultiplier { get; set; } = k_DefaultZoomMultiplier;

        /// <summary>
        /// The pan multiplier.
        /// </summary>
        public float panMultiplier { get; set; } = k_DefaultPanMultiplier;

        /// <summary>
        /// Whether to use the space bar to pan the canvas.
        /// </summary>
        public bool useSpaceBar { get; set; } = k_DefaultUseSpaceBar;

        /// <summary>
        /// The duration of the damping effect in milliseconds.
        /// </summary>
        public int dampingEffectDurationMs { get; set; } = k_DefaultDampingEffectDurationMs;

        /// <summary>
        /// The target's viewport. It is automatically set to the parent of the target's content container.
        /// </summary>
        internal VisualElement viewport => target.contentContainer.hierarchy.parent;

        /// <summary>
        /// The scroll offset.
        /// </summary>
        public Vector2 scrollOffset
        {
#if UNITY_6000_2_OR_NEWER
            get => viewport.resolvedStyle.translate * -1;
#else
            get => viewport.transform.position * -1;
#endif
            set
            {
                var newPosition = new Vector3(-value.x, -value.y, 0);
#if UNITY_6000_2_OR_NEWER
                var previousPosition = viewport.resolvedStyle.translate;
#else
                var previousPosition = viewport.transform.position;
#endif
                var changed =
                    !Mathf.Approximately(previousPosition.x, newPosition.x) ||
                    !Mathf.Approximately(previousPosition.y, newPosition.y);
                if (changed)
                {
                    SetScrollOffsetWithoutNotify(value);
                    m_ScrollOffsetChanged?.Invoke(previousPosition, newPosition);
                }
            }
        }

        /// <summary>
        /// The zoom level.
        /// </summary>
        public Vector2 zoom
        {
#if UNITY_6000_2_OR_NEWER
            get => viewport.resolvedStyle.scale.value;
#else
            get => viewport.transform.scale;
#endif
            set
            {
#if UNITY_6000_2_OR_NEWER
                var previousScale = viewport.resolvedStyle.scale.value;
#else
                var previousScale = viewport.transform.scale;
#endif
                var newScale = new Vector3(Mathf.Clamp(value.x, minZoom, maxZoom), Mathf.Clamp(value.y, minZoom, maxZoom), 1);
                var changed =
                    !Mathf.Approximately(previousScale.x, newScale.x) ||
                    !Mathf.Approximately(previousScale.y, newScale.y);
#if UNITY_6000_2_OR_NEWER
                viewport.style.scale = new Scale(newScale);
#else
                viewport.transform.scale = newScale;
#endif
                if (changed)
                    m_ZoomChanged?.Invoke(previousScale, newScale);
            }
        }

        /// <summary>
        /// The grab mode.
        /// </summary>
        public GrabMode grabMode
        {
            get => m_GrabMode;
            private set
            {
                if (m_GrabMode == value)
                    return;

                var previousGrabMode = m_GrabMode;
                target.RemoveFromClassList(GetGrabModeUssClassName(m_GrabMode));
                m_GrabMode = value;
                target.AddToClassList(GetGrabModeUssClassName(m_GrabMode));
                m_GrabModeChanged?.Invoke(previousGrabMode, m_GrabMode);
            }
        }

        /// <summary>
        /// The primary manipulator. This manipulator will be used without any modifier key.
        /// </summary>
        /// <seealso cref="CanvasManipulator"/>
        public CanvasManipulator primaryManipulator
        {
            get => m_PrimaryManipulator;
            set
            {
                var changed = m_PrimaryManipulator != value;
                m_PrimaryManipulator = value;
                grabMode = m_PrimaryManipulator switch
                {
                    CanvasManipulator.Pan => GrabMode.Grab,
                    _ => GrabMode.None
                };
            }
        }

        /// <summary>
        /// A delegate that is called when the zoom level changes.
        /// </summary>
        /// <param name="previousZoom"> The previous zoom level. </param>
        /// <param name="newZoom"> The new zoom level. </param>
        public delegate void ZoomChangedDelegate(Vector2 previousZoom, Vector2 newZoom);

        /// <summary>
        /// A delegate that is called when the scroll offset changes.
        /// </summary>
        /// <param name="previousScrollOffset"> The previous scroll offset. </param>
        /// <param name="newScrollOffset"> The new scroll offset. </param>
        public delegate void ScrollOffsetChangedDelegate(Vector2 previousScrollOffset, Vector2 newScrollOffset);

        /// <summary>
        /// A delegate that is called when the grab mode changes.
        /// </summary>
        /// <param name="previousGrabMode"> The previous grab mode. </param>
        /// <param name="newGrabMode"> The new grab mode. </param>
        public delegate void GrabModeChangedDelegate(GrabMode previousGrabMode, GrabMode newGrabMode);

        /// <summary>
        /// Constructs a new instance of <see cref="PanAndZoomable"/>.
        /// </summary>
        /// <param name="zoomChanged"> A delegate that is called when the zoom level changes. </param>
        /// <param name="scrollOffsetChanged"> A delegate that is called when the scroll offset changes. </param>
        /// <param name="grabModeChanged"> A delegate that is called when the grab mode changes. </param>
        public PanAndZoomable(ZoomChangedDelegate zoomChanged, ScrollOffsetChangedDelegate scrollOffsetChanged, GrabModeChangedDelegate grabModeChanged)
        {
            m_ZoomChanged = zoomChanged;
            m_ScrollOffsetChanged = scrollOffsetChanged;
            m_GrabModeChanged = grabModeChanged;
        }

        /// <inheritdoc/>
        protected override void RegisterCallbacksOnTarget()
        {
            target.RegisterCallback<WheelEvent>(OnWheel);
            target.RegisterCallback<PointerDownEvent>(OnPointerDownTrickleDown, TrickleDown.TrickleDown);
            target.RegisterCallback<PointerDownEvent>(OnPointerDown);
#if !UNITY_2023_1_OR_NEWER
            target.RegisterCallback<MouseDownEvent>(OnMouseDown);
#endif
            target.RegisterCallback<PointerUpEvent>(OnPointerUp);
            target.RegisterCallback<PointerCancelEvent>(OnPointerCancel);
            target.RegisterCallback<PointerCaptureOutEvent>(OnPointerCaptureOut);
            target.RegisterCallback<PointerMoveEvent>(OnPointerMove);
            target.RegisterCallback<KeyDownEvent>(OnKeyDown);
            target.RegisterCallback<KeyUpEvent>(OnKeyUp);
            target.RegisterCallback<FocusOutEvent>(OnFocusOut);
        }

        /// <inheritdoc/>
        protected override void UnregisterCallbacksFromTarget()
        {
            target.UnregisterCallback<WheelEvent>(OnWheel);
            target.UnregisterCallback<PointerDownEvent>(OnPointerDownTrickleDown, TrickleDown.TrickleDown);
            target.UnregisterCallback<PointerDownEvent>(OnPointerDown);
#if !UNITY_2023_1_OR_NEWER
            target.UnregisterCallback<MouseDownEvent>(OnMouseDown);
#endif
            target.UnregisterCallback<PointerUpEvent>(OnPointerUp);
            target.UnregisterCallback<PointerCancelEvent>(OnPointerCancel);
            target.UnregisterCallback<PointerCaptureOutEvent>(OnPointerCaptureOut);
            target.UnregisterCallback<PointerMoveEvent>(OnPointerMove);
            target.UnregisterCallback<KeyDownEvent>(OnKeyDown);
            target.UnregisterCallback<KeyUpEvent>(OnKeyUp);
            target.UnregisterCallback<FocusOutEvent>(OnFocusOut);
        }

        void OnWheel(WheelEvent evt)
        {
            evt.StopImmediatePropagation();

            if (m_PointerId >= 0 && target.HasPointerCapture(m_PointerId))
                target.ReleasePointer(m_PointerId);

            // no support of touchpad App UI events in Alternate control scheme
            if (!Application.isMobilePlatform &&
                controlScheme == CanvasControlScheme.Editor &&
                evt.button == Unity.AppUI.Core.AppUI.touchPadId)
                return;

            var shouldZoom = controlScheme switch
            {
                CanvasControlScheme.Modern => evt.ctrlKey || evt.commandKey,
                CanvasControlScheme.Editor => true,
                _ => evt.ctrlKey || evt.commandKey
            };

            if (shouldZoom)
            {
                var multiplier = evt.shiftKey ? zoomMultiplier : 1f;
                // logarithmic zoom
                // TODO: We only support zooming in the same amount in both directions, but in the future we should
                // take in account both dimensions in evt.delta
                var zoomDelta = zoomSpeed * -evt.delta.y * multiplier;
                zoomDelta = (zoom.y * Mathf.Pow(2f, zoomDelta)) - zoom.y;
                ApplyZoom(m_PointerPosition, new Vector2(zoomDelta, zoomDelta));
            }
            else
            {
                var multiplier = evt.shiftKey ? panMultiplier : 1f;
                ApplyScrollOffset(evt.delta * multiplier * 2f);
            }
        }

        void OnPointerDownTrickleDown(PointerDownEvent evt)
        {
            m_Velocity = Vector2.zero;
            StopAnyDampingEffect();
        }

        void OnPointerDown(PointerDownEvent evt)
        {
            if (target.panel == null || target.panel.GetCapturingElement(evt.pointerId) != null)
                return;

            if (Application.isPlaying && Application.isMobilePlatform && m_PointerId != PointerId.invalidPointerId && evt.pointerId != m_PointerId)
            {
                evt.StopPropagation();
                if (target.HasPointerCapture(m_PointerId))
                    target.ReleasePointer(m_PointerId);
                return;
            }

            var hasModifierPressed = controlScheme switch
            {
                CanvasControlScheme.Modern => m_SpaceBarPressed,
                CanvasControlScheme.Editor => evt.altKey,
                _ => m_SpaceBarPressed
            } || primaryManipulator == CanvasManipulator.Pan;

            if (evt.button == (int)MouseButton.MiddleMouse ||
                (evt.button == (int)MouseButton.LeftMouse && hasModifierPressed) ||
                (evt.pointerId != PointerId.mousePointerId && (Application.isMobilePlatform ? evt.isPrimary : true)))
            {
                if (!target.HasPointerCapture(evt.pointerId))
                {
                    target.CapturePointer(evt.pointerId);
#if !UNITY_2023_1_OR_NEWER
                    if (evt.pointerId == PointerId.mousePointerId)
                        target.CaptureMouse();
#endif
                }

                evt.StopPropagation();
                m_PointerId = evt.pointerId;
                m_PointerPosition = evt.localPosition;
                m_LastTimestamp = evt.timestamp;
                grabMode = GrabMode.Grabbing;
            }
        }

        void OnMouseDown(MouseDownEvent evt)
        {
            if (target.HasMouseCapture())
                evt.StopPropagation();
        }

        void OnPointerUp(PointerUpEvent evt)
        {
            if (target.HasPointerCapture(evt.pointerId))
                target.ReleasePointer(evt.pointerId);
        }

        void OnPointerCancel(PointerCancelEvent evt)
        {
            if (target.HasPointerCapture(evt.pointerId))
                target.ReleasePointer(evt.pointerId);
        }

        void OnPointerCaptureOut(PointerCaptureOutEvent evt)
        {
            if (evt.pointerId == m_PointerId)
            {
                m_PointerId = PointerId.invalidPointerId;
                m_LastTimestamp = 0;
                grabMode = m_SpaceBarPressed || m_PrimaryManipulator == CanvasManipulator.Pan ?
                    GrabMode.Grab : GrabMode.None;

                // damping the velocity
                StopAnyDampingEffect();
                if (dampingEffectDurationMs > 0)
                {
                    if (m_DampingEffect == null || m_DampingEffect.durationMs != dampingEffectDurationMs)
                    {
                        if (m_DampingEffect != null && !m_DampingEffect.IsRecycled())
                            m_DampingEffect.Recycle();
                        m_DampingEffect = target.experimental.animation
                            .Start(1f, 0f, dampingEffectDurationMs, DampingEffect)
                            .KeepAlive();
                    }
                    m_DampingEffect.Start();
                }
            }
        }

        void OnPointerMove(PointerMoveEvent evt)
        {
            if (Application.isPlaying && Application.isMobilePlatform && evt.pointerId != m_PointerId)
                return;

            if (target.HasPointerCapture(evt.pointerId))
            {
                evt.SetIsHandledByDraggable(true);
                grabMode = GrabMode.Grabbing;
                var oldScrollOffset = scrollOffset;
                var newScrollOffset = scrollOffset;
                newScrollOffset -= (Vector2) (evt.localPosition - m_PointerPosition);
                if (m_LastTimestamp == 0)
                    m_LastTimestamp = evt.timestamp;
                var deltaTime = evt.timestamp - m_LastTimestamp;
                m_LastTimestamp = evt.timestamp;
                m_Velocity = (newScrollOffset - oldScrollOffset) / deltaTime;
                scrollOffset = newScrollOffset;
            }

            m_PointerPosition = evt.localPosition;
        }

        void OnKeyDown(KeyDownEvent evt)
        {
            switch (evt.keyCode)
            {
                case KeyCode.Escape:
                {
                    if (m_PointerId >= 0)
                    {
                        evt.StopImmediatePropagation();
                        if (target.HasPointerCapture(m_PointerId))
                            target.ReleasePointer(m_PointerId);
                        m_PointerId = PointerId.invalidPointerId;
                    }
                    break;
                }
                case KeyCode.Space:
                    if (controlScheme != CanvasControlScheme.Editor && useSpaceBar)
                    {
                        evt.StopImmediatePropagation();
                        m_SpaceBarPressed = true;
                        grabMode = GrabMode.Grab;
                    }
                    break;
                case KeyCode.Minus when evt.actionKey:
                case KeyCode.KeypadMinus when evt.actionKey:
                    {
                        evt.StopImmediatePropagation();
                        // logarithmic zoom
                        var multiplier = evt.shiftKey ? zoomMultiplier : 1f;
                        var zoomDelta = zoomSpeed * -1f * multiplier;
                        var zoomDeltaX = (zoom.x * Mathf.Pow(2f, zoomDelta)) - zoom.x;
                        var zoomDeltaY = (zoom.y * Mathf.Pow(2f, zoomDelta)) - zoom.y;
                        ApplyZoom(new Vector2(target.contentRect.width * 0.5f, target.contentRect.height * 0.5f), new Vector2(zoomDeltaX, zoomDeltaY));
                    }
                    break;
                case KeyCode.Plus when evt.actionKey:
                case KeyCode.Equals when evt.actionKey:
                case KeyCode.KeypadPlus when evt.actionKey:
                    {
                        evt.StopImmediatePropagation();
                        // logarithmic zoom
                        var multiplier = evt.shiftKey ? zoomMultiplier : 1f;
                        var zoomDelta = zoomSpeed * 1f * multiplier;
                        var zoomDeltaX = (zoom.x * Mathf.Pow(2f, zoomDelta)) - zoom.x;
                        var zoomDeltaY = (zoom.y * Mathf.Pow(2f, zoomDelta)) - zoom.y;
                        ApplyZoom(new Vector2(target.contentRect.width * 0.5f, target.contentRect.height * 0.5f), new Vector2(zoomDeltaX, zoomDeltaY));
                    }
                    break;
                case KeyCode.Alpha0 when evt.actionKey:
                case KeyCode.Keypad0 when evt.actionKey:
                    {
                        evt.StopImmediatePropagation();
                        ApplyZoom(new Vector2(target.contentRect.width * 0.5f, target.contentRect.height * 0.5f), Vector2.one - zoom);
                    }
                    break;
            }
        }

        void OnKeyUp(KeyUpEvent evt)
        {
            switch (evt.keyCode)
            {
                case KeyCode.Space:
                {
                    if (controlScheme != CanvasControlScheme.Editor && useSpaceBar)
                    {
                        m_SpaceBarPressed = false;
                        if (m_PrimaryManipulator != CanvasManipulator.Pan)
                            grabMode = GrabMode.None;
                        if (m_PointerId >= 0 && target.HasPointerCapture(m_PointerId))
                            target.ReleasePointer(m_PointerId);
                    }
                    break;
                }
            }
        }

        void OnFocusOut(FocusOutEvent evt)
        {
            m_SpaceBarPressed = false;
            grabMode = m_PrimaryManipulator == CanvasManipulator.Pan ? GrabMode.Grab : GrabMode.None;
            m_PointerId = PointerId.invalidPointerId;
        }


        void ApplyScrollOffset(Vector2 delta)
        {
            if (delta == Vector2.zero)
                return;

            var newScrollOffset = scrollOffset;
            newScrollOffset += delta * scrollSpeed;
            scrollOffset = newScrollOffset;
        }

        /// <summary>
        /// Applies a zoom delta to the target.
        /// </summary>
        /// <param name="pivot"> The pivot point. </param>
        /// <param name="delta"> The zoom delta. </param>
        public void ApplyZoom(Vector2 pivot, Vector2 delta)
        {
            if (Mathf.Approximately(0f, delta.x) && Mathf.Approximately(0f, delta.y))
                return;

            var newZoom = zoom;
            newZoom += delta;
            newZoom.x = Mathf.Clamp(newZoom.x, minZoom, maxZoom);
            newZoom.y = Mathf.Clamp(newZoom.y, minZoom, maxZoom);
            var zoomRatio = newZoom / zoom;
            zoom = newZoom;

            var newScrollOffset = scrollOffset;
            newScrollOffset = (newScrollOffset + pivot) * zoomRatio - pivot;
            scrollOffset = newScrollOffset;
        }

        /// <summary>
        /// Sets the scroll offset without notifying the scroll offset changed delegate.
        /// </summary>
        /// <param name="newValue"> The new scroll offset. </param>
        public void SetScrollOffsetWithoutNotify(Vector2 newValue)
        {
#if UNITY_6000_2_OR_NEWER
            viewport.style.translate = new Translate(-newValue.x, -newValue.y, 0);
#else
            viewport.transform.position = new Vector3(-newValue.x, -newValue.y, 0);
#endif
        }

        void StopAnyDampingEffect()
        {
            if (m_DampingEffect != null && !m_DampingEffect.IsRecycled())
                m_DampingEffect.Stop();
        }

        void DampingEffect(VisualElement element, float inverseNormalizedTime)
        {
            if (m_Velocity == Vector2.zero)
            {
                StopAnyDampingEffect();
                return;
            }

            var newScrollOffset = scrollOffset;
            newScrollOffset += m_Velocity * inverseNormalizedTime;
            scrollOffset = newScrollOffset;
        }
    }
}
