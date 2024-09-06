using System;
using Unity.AppUI.Bridge;
using Unity.AppUI.Core;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UIElements.Experimental;
#if ENABLE_RUNTIME_DATA_BINDINGS
using Unity.Properties;
#endif

namespace Unity.AppUI.UI
{
    /// <summary>
    /// The direction of the scroll.
    /// </summary>
    public enum ScrollDirection
    {
        /// <summary>
        /// The natural scroll direction.
        /// </summary>
        Natural,

        /// <summary>
        /// The inversed scroll direction.
        /// </summary>
        Inverse,
    }

    /// <summary>
    /// The current Grab Mode.
    /// </summary>
    public enum GrabMode
    {
        /// <summary>
        /// No grab possible for the moment.
        /// </summary>
        None,

        /// <summary>
        /// Elements can be grabbed.
        /// </summary>
        Grab,

        /// <summary>
        /// Elements are currently grabbed.
        /// </summary>
        Grabbing,
    }

    /// <summary>
    /// The current Canvas control scheme.
    /// </summary>
    public enum CanvasControlScheme
    {
        /// <summary>
        /// The default control scheme, similar to others Unity Editor tools.
        /// </summary>
        Editor,

        /// <summary>
        /// The alternate control scheme, similar to Figma, Sketch, etc.
        /// </summary>
        Modern,
    }

    /// <summary>
    /// The current Canvas manipulator for the primary pointer.
    /// </summary>
    public enum CanvasManipulator
    {
        /// <summary>
        /// The pointer has no manipulator when no modifier is pressed.
        /// </summary>
        None,

        /// <summary>
        /// The pointer is used to pan into the <see cref="Canvas"/> when no modifier is pressed.
        /// </summary>
        Pan,

        /// <summary>
        /// The pointer is used to zoom into the <see cref="Canvas"/> when no modifier is pressed.
        /// </summary>
        Zoom,
    }

    /// <summary>
    /// A Canvas is a VisualElement that can be used to group other VisualElements.
    /// You can use it to create a scrollable area inside a window.
    /// </summary>
#if ENABLE_UXML_SERIALIZED_DATA
    [UxmlElement]
#endif
    public partial class Canvas : BaseVisualElement
    {
#if ENABLE_RUNTIME_DATA_BINDINGS
        internal static readonly BindingId frameContainerProperty = nameof(frameContainer);

        internal static readonly BindingId scrollOffsetProperty = nameof(scrollOffset);

        internal static readonly BindingId scrollSpeedProperty = nameof(scrollSpeed);

        internal static readonly BindingId minZoomProperty = nameof(minZoom);

        internal static readonly BindingId maxZoomProperty = nameof(maxZoom);

        internal static readonly BindingId zoomSpeedProperty = nameof(zoomSpeed);

        internal static readonly BindingId zoomMultiplierProperty = nameof(zoomMultiplier);

        internal static readonly BindingId panMultiplierProperty = nameof(panMultiplier);

        internal static readonly BindingId scrollDirectionProperty = nameof(scrollDirection);

        internal static readonly BindingId zoomProperty = nameof(zoom);

        internal static readonly BindingId dampingEffectDurationProperty = nameof(dampingEffectDuration);

        internal static readonly BindingId frameMarginProperty = nameof(frameMargin);

        internal static readonly BindingId useSpaceBarProperty = nameof(useSpaceBar);

        internal static readonly BindingId grabModeProperty = nameof(grabMode);

        internal static readonly BindingId controlSchemeProperty = nameof(controlScheme);

        internal static readonly BindingId primaryManipulatorProperty = nameof(primaryManipulator);

#endif
        /// <summary>
        /// USS class name of elements of this type.
        /// </summary>
        public const string ussClassName = "appui-canvas";

        /// <summary>
        /// USS class name prefix for the cursor.
        /// </summary>
        [EnumName("GetGrabModeUssClassName", typeof(GrabMode))]
        public const string cursorUssClassName = Styles.cursorUsClassNamePrefix;

        /// <summary>
        /// USS class name of the background element of this type.
        /// </summary>
        public const string backgroundUssClassName = ussClassName + "__background";

        /// <summary>
        /// USS class name of the viewport element of this type.
        /// </summary>
        public const string viewportUssClassName = ussClassName + "__viewport";

        /// <summary>
        /// USS class name of the viewport container element of this type.
        /// </summary>
        public const string viewportContainerUssClassName = ussClassName + "__viewport-container";

        /// <summary>
        /// USS class name of the horizontal scroller element of this type.
        /// </summary>
        public const string horizontalScrollerUssClassName = ussClassName + "__horizontal-scroller";

        /// <summary>
        /// USS class name of the vertical scroller element of this type.
        /// </summary>
        public const string verticalScrollerUssClassName = ussClassName + "__vertical-scroller";

        /// <summary>
        /// Event that is triggered when the scroll position of the Canvas has changed.
        /// </summary>
        public event Action scrollOffsetChanged;

        /// <summary>
        /// Event that is triggered when the zoom factor of the Canvas has changed.
        /// </summary>
        public event Action zoomChanged;

        const float k_DefaultScrollSpeed = 2f;

        const float k_DefaultMinZoom = 0.1f;

        const float k_DefaultMaxZoom = 100.0f;

        const float k_DefaultZoomSpeed = 0.075f;

        const float k_DefaultZoomMultiplier = 2f;

        const float k_DefaultPanMultiplier = 3f;

        const float k_DefaultFrameMargin = 12f;

        const bool k_DefaultUseSpaceBar = true;

        Vector2 m_Velocity;

        long m_LastTimestamp;

        ValueAnimation<float> m_DampingEffect;

        const int k_DefaultDampingEffectDurationMs = 750;

        int m_DampingEffectDurationMs = k_DefaultDampingEffectDurationMs;

        const ScrollDirection k_DefaultScrollDirection = ScrollDirection.Natural;

        const CanvasControlScheme k_DefaultControlScheme = CanvasControlScheme.Modern;

        const CanvasManipulator k_DefaultPrimaryManipulator = CanvasManipulator.None;

        readonly CanvasBackground m_Background;

        readonly VisualElement m_Viewport;

        readonly VisualElement m_ViewportContainer;

        readonly Scroller m_HorizontalScroller;

        readonly Scroller m_VerticalScroller;

        Vector3 m_PointerPosition;

        GrabMode m_GrabMode = GrabMode.None;

        bool m_UpdatingScrollers;

        Vector2 m_LastScrollersPosition;

        int m_PointerId = -1;

        bool m_SpaceBarPressed;

        CanvasManipulator m_PrimaryManipulator = k_DefaultPrimaryManipulator;

        Optional<Rect> m_FrameContainer = Optional<Rect>.none;

        float m_ScrollSpeed = k_DefaultScrollSpeed;

        float m_MinZoom = k_DefaultMinZoom;

        float m_MaxZoom = k_DefaultMaxZoom;

        float m_ZoomSpeed = k_DefaultZoomSpeed;

        float m_ZoomMultiplier = k_DefaultZoomMultiplier;

        float m_PanMultiplier = k_DefaultPanMultiplier;

        ScrollDirection m_ScrollDirection = k_DefaultScrollDirection;

        float m_FrameMargin = k_DefaultFrameMargin;

        bool m_UseSpaceBar = k_DefaultUseSpaceBar;

        CanvasControlScheme m_ControlScheme = k_DefaultControlScheme;

        /// <summary>
        /// The content container of the Canvas.
        /// </summary>
        public override VisualElement contentContainer => m_ViewportContainer;

        /// <summary>
        /// The container used for framing the Canvas.
        /// </summary>
        /// <remarks>
        /// The container rect value must be defined in the Canvas' local coordinates.
        /// </remarks>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public Optional<Rect> frameContainer
        {
            get => m_FrameContainer;
            set
            {
                var changed = m_FrameContainer != value;
                m_FrameContainer = value;

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in frameContainerProperty);
#endif
            }
        }

        /// <summary>
        /// The scroll coordinates of the Canvas.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
        public Vector2 scrollOffset
        {
            get => m_Viewport.transform.position * -1;
            set
            {
                SetScrollOffset(value);
                UpdateScrollers();
                scrollOffsetChanged?.Invoke();

#if ENABLE_RUNTIME_DATA_BINDINGS
                NotifyPropertyChanged(in scrollOffsetProperty);
#endif
            }
        }

        /// <summary>
        /// The scroll speed of the Canvas.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public float scrollSpeed
        {
            get => m_ScrollSpeed;
            set
            {
                var changed = !Mathf.Approximately(m_ScrollSpeed, value);
                m_ScrollSpeed = value;

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in scrollSpeedProperty);
#endif
            }
        }

        /// <summary>
        /// The minimum zoom factor of the Canvas.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public float minZoom
        {
            get => m_MinZoom;
            set
            {
                var changed = !Mathf.Approximately(m_MinZoom, value);
                m_MinZoom = value;

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in minZoomProperty);
#endif
            }
        }

        /// <summary>
        /// The maximum zoom factor of the Canvas.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public float maxZoom
        {
            get => m_MaxZoom;
            set
            {
                var changed = !Mathf.Approximately(m_MaxZoom, value);
                m_MaxZoom = value;

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in maxZoomProperty);
#endif
            }
        }

        /// <summary>
        /// The zoom speed of the Canvas.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public float zoomSpeed
        {
            get => m_ZoomSpeed;
            set
            {
                var changed = !Mathf.Approximately(m_ZoomSpeed, value);
                m_ZoomSpeed = value;

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in zoomSpeedProperty);
#endif
            }
        }

        /// <summary>
        /// The zoom speed multiplier when Shift key is hold.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public float zoomMultiplier
        {
            get => m_ZoomMultiplier;
            set
            {
                var changed = !Mathf.Approximately(m_ZoomMultiplier, value);
                m_ZoomMultiplier = value;

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in zoomMultiplierProperty);
#endif
            }
        }

        /// <summary>
        /// The pan multiplier when Shift key is hold.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public float panMultiplier
        {
            get => m_PanMultiplier;
            set
            {
                var changed = !Mathf.Approximately(m_PanMultiplier, value);
                m_PanMultiplier = value;

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in panMultiplierProperty);
#endif
            }
        }

        /// <summary>
        /// The scroll direction of the Canvas. See <see cref="ScrollDirection"/> for more information.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public ScrollDirection scrollDirection
        {
            get => m_ScrollDirection;
            set
            {
                var changed = m_ScrollDirection != value;
                m_ScrollDirection = value;

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in scrollDirectionProperty);
#endif
            }
        }

        /// <summary>
        /// The zoom factor of the Canvas.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public float zoom
        {
            get => m_Viewport.transform.scale.x;
            set
            {
                var changed = !Mathf.Approximately(m_Viewport.transform.scale.x, value);
                m_Viewport.transform.scale = new Vector3(value, value, 1);
                m_Background.scale = value;
                UpdateScrollers();

                if (changed)
                    zoomChanged?.Invoke();

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in zoomProperty);
#endif
            }
        }

        /// <summary>
        /// The damping effect duration in milliseconds.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public int dampingEffectDuration
        {
            get => m_DampingEffectDurationMs;
            set
            {
                var changed = m_DampingEffectDurationMs != value;
                m_DampingEffectDurationMs = value;

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in dampingEffectDurationProperty);
#endif
            }
        }

        /// <summary>
        /// The margin applied when framing the Canvas.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public float frameMargin
        {
            get => m_FrameMargin;
            set
            {
                var changed = !Mathf.Approximately(m_FrameMargin, value);
                m_FrameMargin = value;

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in frameMarginProperty);
#endif
            }
        }

        /// <summary>
        /// Whether the Canvas should use the Space bar to pan.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public bool useSpaceBar
        {
            get => m_UseSpaceBar;
            set
            {
                var changed = m_UseSpaceBar != value;
                m_UseSpaceBar = value;

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in useSpaceBarProperty);
#endif
            }
        }

        /// <summary>
        /// The current grab state of the canvas (to pan).
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty(ReadOnly = true)]
#endif
        public GrabMode grabMode
        {
            get => m_GrabMode;
            private set
            {
                if (m_GrabMode == value)
                    return;

                RemoveFromClassList(GetGrabModeUssClassName(m_GrabMode));
                m_GrabMode = value;
                AddToClassList(GetGrabModeUssClassName(m_GrabMode));

#if ENABLE_RUNTIME_DATA_BINDINGS
                NotifyPropertyChanged(in grabModeProperty);
#endif
            }
        }

        /// <summary>
        /// The current control scheme of the canvas.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public CanvasControlScheme controlScheme
        {
            get => m_ControlScheme;
            set
            {
                var changed = m_ControlScheme != value;
                m_ControlScheme = value;

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in controlSchemeProperty);
#endif
            }
        }

        /// <summary>
        /// The current manipulator of the canvas for the primary pointer without modifier.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
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

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in primaryManipulatorProperty);
#endif
            }
        }

        /// <summary>
        /// Instantiates a <see cref="Canvas"/> element.
        /// </summary>
        public Canvas()
        {
            AddToClassList(ussClassName);
            pickingMode = PickingMode.Position;
            focusable = true;

            m_Background = new CanvasBackground {name = backgroundUssClassName, pickingMode = PickingMode.Ignore};
            m_Background.AddToClassList(backgroundUssClassName);
            hierarchy.Add(m_Background);

            m_Viewport = new VisualElement
            {
                name = viewportUssClassName,
                pickingMode = PickingMode.Ignore,
                usageHints = UsageHints.DynamicTransform
            };
            m_Viewport.AddToClassList(viewportUssClassName);
            hierarchy.Add(m_Viewport);

            m_ViewportContainer = new VisualElement
            {
                name = viewportContainerUssClassName,
                pickingMode = PickingMode.Ignore,
                usageHints = UsageHints.GroupTransform
            };
            m_ViewportContainer.AddToClassList(viewportContainerUssClassName);
            m_Viewport.hierarchy.Add(m_ViewportContainer);

            m_VerticalScroller = new Scroller
            {
                name = verticalScrollerUssClassName,
                direction = SliderDirection.Vertical
            };
            m_VerticalScroller.AddToClassList(verticalScrollerUssClassName);
            m_VerticalScroller.slider.RegisterValueChangedCallback(OnVerticalScrollValueChanged);
            m_VerticalScroller.slider.RegisterCallback<PointerCaptureEvent>(OnScrollerPointerCapture);
            m_VerticalScroller.slider.RegisterCallback<PointerCaptureOutEvent>(OnScrollerPointerCaptureOut);
            hierarchy.Add(m_VerticalScroller);

            m_HorizontalScroller = new Scroller
            {
                name = horizontalScrollerUssClassName,
                direction = SliderDirection.Horizontal
            };
            m_HorizontalScroller.AddToClassList(horizontalScrollerUssClassName);
            m_HorizontalScroller.slider.RegisterValueChangedCallback(OnHorizontalScrollValueChanged);
            m_HorizontalScroller.slider.RegisterCallback<PointerCaptureEvent>(OnScrollerPointerCapture);
            m_HorizontalScroller.slider.RegisterCallback<PointerCaptureOutEvent>(OnScrollerPointerCaptureOut);
            hierarchy.Add(m_HorizontalScroller);

            RegisterCallback<WheelEvent>(OnWheel);
            RegisterCallback<PointerDownEvent>(OnPointerDown, TrickleDown.TrickleDown);
#if !UNITY_2023_1_OR_NEWER
            RegisterCallback<MouseDownEvent>(OnMouseDown, TrickleDown.TrickleDown);
#endif
            RegisterCallback<PointerUpEvent>(OnPointerUp);
            RegisterCallback<PointerCancelEvent>(OnPointerCancel);
            RegisterCallback<PointerCaptureOutEvent>(OnPointerCaptureOut);
            RegisterCallback<PointerMoveEvent>(OnPointerMove);
            RegisterCallback<KeyDownEvent>(OnKeyDown);
            RegisterCallback<KeyUpEvent>(OnKeyUp);
            RegisterCallback<FocusOutEvent>(OnFocusOut);
            RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);
        }

        /// <summary>
        /// Frame the Canvas to the given world area. The area is in world coordinates.
        /// </summary>
        /// <param name="worldRect"> The area to frame. </param>
        public void FrameWorldRect(Rect worldRect)
        {
            if (worldRect.size.sqrMagnitude == 0)
                return;

            var container = frameContainer.IsSet ? frameContainer.Value : contentRect;
            var containerCenter = new Vector2(container.width * 0.5f, container.height * 0.5f) + container.position;

            var localRect = this.WorldToLocal(worldRect);
            var zoomRatio = Mathf.Min(
                (container.width - frameMargin * 2f) / localRect.width,
                (container.height - frameMargin * 2f) / localRect.height);
            var newZoom = zoom * zoomRatio;

            var centerDelta = localRect.center - containerCenter;
            scrollOffset += centerDelta;

            var zoomDelta = newZoom - zoom;
            ApplyZoom(containerCenter, zoomDelta);
        }

        /// <summary>
        /// Frame the Canvas to the given area. The area is in the Viewport's local coordinates.
        /// </summary>
        /// <param name="viewportArea"> The area to frame. </param>
        public void FrameArea(Rect viewportArea)
        {
            if (viewportArea.size.sqrMagnitude == 0)
                return;

            var worldRect = m_Viewport.LocalToWorld(viewportArea);
            FrameWorldRect(worldRect);
        }

        /// <summary>
        /// Frame the Canvas to the given element. The element is in the Viewport's local coordinates.
        /// </summary>
        /// <param name="element"> The element to frame. </param>
        public void FrameElement(VisualElement element)
        {
            if (element == null)
                return;

            var boundingBox = element.GetWorldBoundingBox();
            if (boundingBox.size.sqrMagnitude == 0)
                return;

            FrameArea(m_Viewport.WorldToLocal(boundingBox));
        }

        /// <summary>
        /// Frame the Canvas to see all elements.
        /// </summary>
        public void FrameAll()
        {
            FrameElement(m_ViewportContainer);
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
                        if (m_PointerId >= 0 && this.HasPointerCapture(m_PointerId))
                            this.ReleasePointer(m_PointerId);
                    }
                    break;
                }
            }
        }

        void OnFocusOut(FocusOutEvent evt)
        {
            m_SpaceBarPressed = false;
            grabMode = m_PrimaryManipulator == CanvasManipulator.Pan ? GrabMode.Grab : GrabMode.None;
            m_PointerId = -1;
        }

        void OnKeyDown(KeyDownEvent evt)
        {
            switch (evt.keyCode)
            {
                case KeyCode.F:
                    evt.StopImmediatePropagation();
                    FrameAll();
                    break;
                case KeyCode.Escape:
                {
                    if (m_PointerId >= 0)
                    {
                        evt.StopImmediatePropagation();
                        if (this.HasPointerCapture(m_PointerId))
                            this.ReleasePointer(m_PointerId);
                        m_PointerId = -1;
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
                        zoomDelta = (zoom * Mathf.Pow(2f, zoomDelta)) - zoom;
                        ApplyZoom(new Vector2(contentRect.width * 0.5f, contentRect.height * 0.5f), zoomDelta);
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
                        zoomDelta = (zoom * Mathf.Pow(2f, zoomDelta)) - zoom;
                        ApplyZoom(new Vector2(contentRect.width * 0.5f, contentRect.height * 0.5f), zoomDelta);
                    }
                    break;
                case KeyCode.Alpha0 when evt.actionKey:
                case KeyCode.Keypad0 when evt.actionKey:
                    {
                        evt.StopImmediatePropagation();
                        ApplyZoom(new Vector2(contentRect.width * 0.5f, contentRect.height * 0.5f), 1f - zoom);
                    }
                    break;
            }
        }

        void OnPointerDown(PointerDownEvent evt)
        {
            m_Velocity = Vector2.zero;
            StopAnyDampingEffect();

            if (panel == null || panel.GetCapturingElement(evt.pointerId) != null)
                return;

            if (Application.isPlaying && Application.isMobilePlatform && m_PointerId >= 0 && evt.pointerId != m_PointerId)
            {
                evt.StopPropagation();
                if (this.HasPointerCapture(m_PointerId))
                    this.ReleasePointer(m_PointerId);
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
                (evt.pointerId != PointerId.mousePointerId && evt.isPrimary))
            {
                if (!this.HasPointerCapture(evt.pointerId))
                {
                    this.CapturePointer(evt.pointerId);
#if !UNITY_2023_1_OR_NEWER
                    if (evt.pointerId == PointerId.mousePointerId)
                        this.CaptureMouse();
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
            if (this.HasMouseCapture())
                evt.StopPropagation();
        }

        void OnPointerUp(PointerUpEvent evt)
        {
            if (this.HasPointerCapture(evt.pointerId))
                this.ReleasePointer(evt.pointerId);
        }

        void OnPointerCancel(PointerCancelEvent evt)
        {
            if (this.HasPointerCapture(evt.pointerId))
                this.ReleasePointer(evt.pointerId);
        }

        void OnPointerCaptureOut(PointerCaptureOutEvent evt)
        {
            if (evt.pointerId == m_PointerId)
            {
                m_PointerId = -1;
                m_LastTimestamp = 0;
                grabMode = m_SpaceBarPressed || m_PrimaryManipulator == CanvasManipulator.Pan ?
                    GrabMode.Grab : GrabMode.None;

                // damping the velocity
                StopAnyDampingEffect();
                if (m_DampingEffectDurationMs > 0)
                {
                    m_DampingEffect = experimental.animation
                        .Start(1f, 0f, m_DampingEffectDurationMs, DampingEffect)
                        .KeepAlive();
                    m_DampingEffect.Start();
                }
            }
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

        void StopAnyDampingEffect()
        {
            m_DampingEffect?.Stop();
            m_DampingEffect?.Recycle();
            m_DampingEffect = null;
        }

        void OnPointerMove(PointerMoveEvent evt)
        {
            if (Application.isPlaying && Application.isMobilePlatform && evt.pointerId != m_PointerId)
                return;

            if (this.HasPointerCapture(evt.pointerId))
            {
                evt.SetIsHandledByDraggable(true);
                grabMode = GrabMode.Grabbing;
                var oldScrollOffset = scrollOffset;
                var newScrollOffset = scrollOffset;
                newScrollOffset += (Vector2)(evt.localPosition - m_PointerPosition) *
                                (scrollDirection == ScrollDirection.Natural ? -1f : 1f);
                if (m_LastTimestamp == 0)
                    m_LastTimestamp = evt.timestamp;
                var deltaTime = evt.timestamp - m_LastTimestamp;
                m_LastTimestamp = evt.timestamp;
                m_Velocity = (newScrollOffset - oldScrollOffset) / deltaTime;
                scrollOffset = newScrollOffset;
            }

            m_PointerPosition = evt.localPosition;
        }

        void OnWheel(WheelEvent evt)
        {

            evt.StopImmediatePropagation();

            if (m_PointerId >= 0 && this.HasPointerCapture(m_PointerId))
                this.ReleasePointer(m_PointerId);

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
                var zoomDelta = zoomSpeed * -evt.delta.y * multiplier;
                zoomDelta = (zoom * Mathf.Pow(2f, zoomDelta)) - zoom;
                ApplyZoom(m_PointerPosition, zoomDelta);
            }
            else
            {
                var multiplier = evt.shiftKey ? panMultiplier : 1f;
                ApplyScrollOffset(evt.delta * multiplier * 2f);
            }
        }

        void OnVerticalScrollValueChanged(ChangeEvent<float> evt)
        {
            if (m_UpdatingScrollers)
                return;

            var delta = evt.newValue - m_LastScrollersPosition.y;
            SetScrollOffset(new Vector2(scrollOffset.x, scrollOffset.y + delta));
            m_LastScrollersPosition.y = evt.newValue;
            scrollOffsetChanged?.Invoke();
        }

        void OnHorizontalScrollValueChanged(ChangeEvent<float> evt)
        {
            if (m_UpdatingScrollers)
                return;

            var delta = evt.newValue - m_LastScrollersPosition.x;
            SetScrollOffset(new Vector2(scrollOffset.x - delta, scrollOffset.y));
            m_LastScrollersPosition.x = evt.newValue;
            scrollOffsetChanged?.Invoke();
        }

        void OnGeometryChanged(GeometryChangedEvent evt)
        {
            if (!evt.newRect.IsValid())
                return;

            UpdateScrollers();
        }

        void OnScrollerPointerCapture(PointerCaptureEvent evt)
        {
            if (evt.target == m_HorizontalScroller.slider || evt.target == m_VerticalScroller.slider)
            {
                m_LastScrollersPosition = new Vector2(m_HorizontalScroller.slider.value, m_VerticalScroller.slider.value);
            }
        }

        void OnScrollerPointerCaptureOut(PointerCaptureOutEvent evt)
        {
            if (evt.target == m_HorizontalScroller.slider || evt.target == m_VerticalScroller.slider)
            {
                UpdateScrollers();
            }
        }

        void UpdateScrollers()
        {
            var viewportRect = contentRect;
            if (!viewportRect.IsValid())
                return;

            var canvasContentRect = this.WorldToLocal(m_ViewportContainer.GetWorldBoundingBox());

            // Shrinking the canvas content rect by 1 pixel on each side to avoid rounding issues
            canvasContentRect.xMin += 1;
            canvasContentRect.yMin += 1;
            canvasContentRect.xMax -= 1;
            canvasContentRect.yMax -= 1;

            // Encapsulating the canvas content rect by the viewport rect
            canvasContentRect.xMin = Mathf.Min(canvasContentRect.xMin, 0);
            canvasContentRect.yMin = Mathf.Min(canvasContentRect.yMin, 0);
            canvasContentRect.xMax = Mathf.Max(canvasContentRect.xMax, viewportRect.width);
            canvasContentRect.yMax = Mathf.Max(canvasContentRect.yMax, viewportRect.height);

            // Compute the ratio between the viewport and the canvas content size
            var xRatio = canvasContentRect.width > 0 ? viewportRect.width / canvasContentRect.width : 1f;
            xRatio = Mathf.Approximately(1f, xRatio) ? 1f : xRatio;
            var yRatio = canvasContentRect.height > 0 ? viewportRect.height / canvasContentRect.height : 1f;
            yRatio = Mathf.Approximately(1f, yRatio) ? 1f : yRatio;

            m_UpdatingScrollers = true;

            if (xRatio < 1f)
            {
                m_HorizontalScroller.lowValue = float.MinValue;
                m_HorizontalScroller.highValue = float.MaxValue;

                var min = 0;
                var max = Mathf.Max(min, canvasContentRect.width - viewportRect.width);
                var newValue = max - Mathf.Clamp(-canvasContentRect.xMin, min, max);

                m_HorizontalScroller.slider.SetValueWithoutNotify(newValue);
                m_HorizontalScroller.lowValue = min;
                m_HorizontalScroller.highValue = max;
            }

            if (yRatio < 1f)
            {
                m_VerticalScroller.lowValue = float.MinValue;
                m_VerticalScroller.highValue = float.MaxValue;

                var min = 0;
                var max = Mathf.Max(min, canvasContentRect.height - viewportRect.height);
                var newValue = Mathf.Clamp(-canvasContentRect.yMin, min, max);

                m_VerticalScroller.slider.SetValueWithoutNotify(newValue);
                m_VerticalScroller.lowValue = min;
                m_VerticalScroller.highValue = max;
            }

            m_HorizontalScroller.Adjust(xRatio);
            m_VerticalScroller.Adjust(yRatio);

            m_UpdatingScrollers = false;
        }

        void ApplyScrollOffset(Vector2 delta)
        {
            if (delta == Vector2.zero)
                return;

            var newScrollOffset = scrollOffset;
            newScrollOffset += (delta * scrollSpeed) * (scrollDirection == ScrollDirection.Natural ? -1f : 1f);
            scrollOffset = newScrollOffset;
        }

        void ApplyZoom(Vector2 pivot, float delta)
        {
            if (delta == 0)
                return;

            var newZoom = zoom;
            newZoom += delta;
            newZoom = Mathf.Clamp(newZoom, minZoom, maxZoom);
            var zoomRatio = newZoom / zoom;
            zoom = newZoom;

            var newScrollOffset = scrollOffset;
            newScrollOffset = (newScrollOffset + pivot) * zoomRatio - pivot;
            scrollOffset = newScrollOffset;
        }

        void SetScrollOffset(Vector2 newValue)
        {
            m_Viewport.transform.position = new Vector3(-newValue.x, -newValue.y, 0);
            m_Background.offset = m_Viewport.transform.position;
        }

#if ENABLE_UXML_TRAITS

        /// <summary>
        /// Defines the UxmlFactory for the Canvas.
        /// </summary>
        public new class UxmlFactory : UxmlFactory<Canvas, UxmlTraits> { }

        /// <summary>
        /// Class containing the UXML traits for the <see cref="Canvas"/>.
        /// </summary>
        public new class UxmlTraits : BaseVisualElement.UxmlTraits
        {
            readonly UxmlFloatAttributeDescription m_ScrollSpeed = new UxmlFloatAttributeDescription
            {
                name = "scroll-speed",
                defaultValue = k_DefaultScrollSpeed
            };

            readonly UxmlFloatAttributeDescription m_MinZoom = new UxmlFloatAttributeDescription
            {
                name = "min-zoom",
                defaultValue = k_DefaultMinZoom
            };

            readonly UxmlFloatAttributeDescription m_MaxZoom = new UxmlFloatAttributeDescription
            {
                name = "max-zoom",
                defaultValue = k_DefaultMaxZoom
            };

            readonly UxmlFloatAttributeDescription m_ZoomSpeed = new UxmlFloatAttributeDescription
            {
                name = "zoom-speed",
                defaultValue = k_DefaultZoomSpeed
            };

            readonly UxmlFloatAttributeDescription m_ZoomMultiplier = new UxmlFloatAttributeDescription
            {
                name = "zoom-multiplier",
                defaultValue = k_DefaultZoomMultiplier
            };

            readonly UxmlFloatAttributeDescription m_PanMultiplier = new UxmlFloatAttributeDescription
            {
                name = "pan-multiplier",
                defaultValue = k_DefaultPanMultiplier
            };

            readonly UxmlIntAttributeDescription m_DampingEffectDuration = new UxmlIntAttributeDescription
            {
                name = "damping-effect-duration",
                defaultValue = k_DefaultDampingEffectDurationMs
            };

            readonly UxmlFloatAttributeDescription m_FrameMargin = new UxmlFloatAttributeDescription
            {
                name = "frame-margin",
                defaultValue = k_DefaultFrameMargin
            };

            readonly UxmlEnumAttributeDescription<ScrollDirection> m_ScrollDirection = new UxmlEnumAttributeDescription<ScrollDirection>
            {
                name = "scroll-direction",
                defaultValue = k_DefaultScrollDirection
            };

            readonly UxmlEnumAttributeDescription<CanvasControlScheme> m_ControlScheme = new UxmlEnumAttributeDescription<CanvasControlScheme>
            {
                name = "control-scheme",
                defaultValue = k_DefaultControlScheme
            };

            readonly UxmlBoolAttributeDescription m_UseSpaceBar = new UxmlBoolAttributeDescription
            {
                name = "use-space-bar",
                defaultValue = k_DefaultUseSpaceBar
            };




            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);

                var canvas = (Canvas)ve;

                var floatVal = 0f;
                if (m_ScrollSpeed.TryGetValueFromBag(bag, cc, ref floatVal))
                    canvas.scrollSpeed = floatVal;

                if (m_MinZoom.TryGetValueFromBag(bag, cc, ref floatVal))
                    canvas.minZoom = floatVal;

                if (m_MaxZoom.TryGetValueFromBag(bag, cc, ref floatVal))
                    canvas.maxZoom = floatVal;

                if (m_ZoomSpeed.TryGetValueFromBag(bag, cc, ref floatVal))
                    canvas.zoomSpeed = floatVal;

                if (m_ZoomMultiplier.TryGetValueFromBag(bag, cc, ref floatVal))
                    canvas.zoomMultiplier = floatVal;

                if (m_PanMultiplier.TryGetValueFromBag(bag, cc, ref floatVal))
                    canvas.panMultiplier = floatVal;

                var intVal = 0;
                if (m_DampingEffectDuration.TryGetValueFromBag(bag, cc, ref intVal))
                    canvas.dampingEffectDuration = intVal;

                if (m_FrameMargin.TryGetValueFromBag(bag, cc, ref floatVal))
                    canvas.frameMargin = floatVal;

                var scrollDirectionVal = ScrollDirection.Natural;
                if (m_ScrollDirection.TryGetValueFromBag(bag, cc, ref scrollDirectionVal))
                    canvas.scrollDirection = scrollDirectionVal;

                var controlSchemeVal = CanvasControlScheme.Modern;
                if (m_ControlScheme.TryGetValueFromBag(bag, cc, ref controlSchemeVal))
                    canvas.controlScheme = controlSchemeVal;

                var boolVal = false;
                if (m_UseSpaceBar.TryGetValueFromBag(bag, cc, ref boolVal))
                    canvas.useSpaceBar = boolVal;
            }
        }
#endif
    }
}
