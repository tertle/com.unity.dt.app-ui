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

        const ScrollDirection k_DefaultScrollDirection = ScrollDirection.Natural;

        const CanvasControlScheme k_DefaultControlScheme = CanvasControlScheme.Modern;

        readonly CanvasBackground m_Background;

        readonly VisualElement m_Viewport;

        readonly VisualElement m_ViewportContainer;

        readonly Scroller m_HorizontalScroller;

        readonly Scroller m_VerticalScroller;

        Vector3 m_PointerPosition;

        bool m_UpdatingScrollers;

        Vector2 m_LastScrollersPosition;

        Optional<Rect> m_FrameContainer = Optional<Rect>.none;

        float m_FrameMargin = k_DefaultFrameMargin;

        readonly PanAndZoomable m_Manipulator;

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
            get => m_Manipulator.scrollOffset;
            set => m_Manipulator.scrollOffset = value;
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
            get => m_Manipulator.scrollSpeed;
            set
            {
                var changed = !Mathf.Approximately(m_Manipulator.scrollSpeed, value);
                m_Manipulator.scrollSpeed = value;

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
            get => m_Manipulator.minZoom;
            set
            {
                var changed = !Mathf.Approximately(m_Manipulator.minZoom, value);
                m_Manipulator.minZoom = value;

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
            get => m_Manipulator.maxZoom;
            set
            {
                var changed = !Mathf.Approximately(m_Manipulator.maxZoom, value);
                m_Manipulator.maxZoom = value;

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
            get => m_Manipulator.zoomSpeed;
            set
            {
                var changed = !Mathf.Approximately(m_Manipulator.zoomSpeed, value);
                m_Manipulator.zoomSpeed = value;

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
            get => m_Manipulator.zoomMultiplier;
            set
            {
                var changed = !Mathf.Approximately(m_Manipulator.zoomMultiplier, value);
                m_Manipulator.zoomMultiplier = value;

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
            get => m_Manipulator.panMultiplier;
            set
            {
                var changed = !Mathf.Approximately(m_Manipulator.panMultiplier, value);
                m_Manipulator.panMultiplier = value;

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
            get => m_Manipulator.scrollDirection;
            set
            {
                var changed = m_Manipulator.scrollDirection != value;
                m_Manipulator.scrollDirection = value;

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
            get => m_Manipulator.zoom.y;
            set => m_Manipulator.zoom = new Vector2(value, value);
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
            get => m_Manipulator.dampingEffectDurationMs;
            set
            {
                var changed = m_Manipulator.dampingEffectDurationMs != value;
                m_Manipulator.dampingEffectDurationMs = value;

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
            get => m_Manipulator.useSpaceBar;
            set
            {
                var changed = m_Manipulator.useSpaceBar != value;
                m_Manipulator.useSpaceBar = value;

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
        public GrabMode grabMode => m_Manipulator.grabMode;

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
            get => m_Manipulator.controlScheme;
            set
            {
                var changed = m_Manipulator.controlScheme != value;
                m_Manipulator.controlScheme = value;

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
            get => m_Manipulator.primaryManipulator;
            set
            {
                var changed = m_Manipulator.primaryManipulator != value;
                m_Manipulator.primaryManipulator = value;

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

            m_Manipulator = new PanAndZoomable(OnZoomChanged, OnScrollOffsetChanged, OnGrabModeChanged);
            this.AddManipulator(m_Manipulator);

            RegisterCallback<KeyDownEvent>(OnKeyDown);
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
            m_Manipulator.ApplyZoom(containerCenter, new Vector2(zoomDelta, zoomDelta));
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

        void OnKeyDown(KeyDownEvent evt)
        {
            if (evt.keyCode == KeyCode.F)
            {
                evt.StopImmediatePropagation();
                FrameAll();
            }
        }

        void OnZoomChanged(Vector2 previousZoom, Vector2 newZoom)
        {
            UpdateScrollers();
            m_Background.scale = newZoom.y;
            zoomChanged?.Invoke();
#if ENABLE_RUNTIME_DATA_BINDINGS
            NotifyPropertyChanged(in zoomProperty);
#endif
        }

        void OnScrollOffsetChanged(Vector2 previousScrollOffset, Vector2 newScrollOffset)
        {
            UpdateScrollers();
            m_Background.offset = m_Viewport.transform.position;
            scrollOffsetChanged?.Invoke();
#if ENABLE_RUNTIME_DATA_BINDINGS
            NotifyPropertyChanged(in scrollOffsetProperty);
#endif
        }

        void OnGrabModeChanged(GrabMode previousGrabMode, GrabMode newGrabMode)
        {
#if ENABLE_RUNTIME_DATA_BINDINGS
            NotifyPropertyChanged(in grabModeProperty);
#endif
        }

        void OnVerticalScrollValueChanged(ChangeEvent<float> evt)
        {
            if (m_UpdatingScrollers)
                return;

            var delta = evt.newValue - m_LastScrollersPosition.y;
            m_Manipulator.SetScrollOffsetWithoutNotify(new Vector2(scrollOffset.x, scrollOffset.y + delta));
            m_Background.offset = m_Viewport.transform.position;
            m_LastScrollersPosition.y = evt.newValue;
            scrollOffsetChanged?.Invoke();
        }

        void OnHorizontalScrollValueChanged(ChangeEvent<float> evt)
        {
            if (m_UpdatingScrollers)
                return;

            var delta = evt.newValue - m_LastScrollersPosition.x;
            m_Manipulator.SetScrollOffsetWithoutNotify(new Vector2(scrollOffset.x - delta, scrollOffset.y));
            m_Background.offset = m_Viewport.transform.position;
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
