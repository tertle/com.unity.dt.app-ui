using System;
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
    /// Drawer UI element. A drawer is a UI element that slides in from the side of the screen. It can be used to display
    /// additional content or to display a menu.
    /// </summary>
#if ENABLE_UXML_SERIALIZED_DATA
    [UxmlElement]
#endif
    public partial class Drawer : BaseVisualElement
    {
#if ENABLE_RUNTIME_DATA_BINDINGS

        internal static readonly BindingId swipeableProperty = new BindingId(nameof(swipeable));

        internal static readonly BindingId isOpenProperty = new BindingId(nameof(isOpen));

        internal static readonly BindingId distanceProperty = new BindingId(nameof(distance));

        internal static readonly BindingId swipeAreaWidthProperty = new BindingId(nameof(swipeAreaWidth));

        internal static readonly BindingId hysteresisProperty = new BindingId(nameof(hysteresis));

        internal static readonly BindingId elevationProperty = new BindingId(nameof(elevation));

        internal static readonly BindingId anchorProperty = new BindingId(nameof(anchor));

        internal static readonly BindingId variantProperty = new BindingId(nameof(variant));

        internal static readonly BindingId backdropTransitionEnabledProperty = new BindingId(nameof(backdropTransitionEnabled));

        internal static readonly BindingId hideBackdropProperty = new BindingId(nameof(hideBackdrop));

        internal static readonly BindingId backdropFinalOpacityProperty = new BindingId(nameof(backdropFinalOpacity));

        internal static readonly BindingId transitionDurationMsProperty = new BindingId(nameof(transitionDurationMs));

#endif

        /// <summary>
        /// The Drawer main styling class.
        /// </summary>
        public const string ussClassName = "appui-drawer";

        /// <summary>
        /// The Drawer backdrop styling class.
        /// </summary>
        public const string backdropUssClassName = ussClassName + "__backdrop";

        /// <summary>
        /// The Drawer element styling class.
        /// </summary>
        public const string drawerUssClassName = ussClassName + "__drawer";

        /// <summary>
        /// The Drawer container styling class.
        /// </summary>
        public const string drawerContainerUssClassName = ussClassName + "__drawer-container";

        /// <summary>
        /// The Drawer variant styling class.
        /// </summary>
        [EnumName("GetVariantUssClassName", typeof(DrawerVariant))]
        [EnumName("GetAnchorUssClassName", typeof(DrawerAnchor))]
        public const string variantUssClassName = ussClassName + "--";

        /// <summary>
        /// The elevation styling class prefix.
        /// </summary>
        public const string elevationUssClassName = Styles.elevationUssClassName;

        readonly VisualElement m_Backdrop;

        readonly VisualElement m_DrawerElement;

        readonly ExVisualElement m_DrawerContainer;

        DrawerAnchor m_Anchor;

        DrawerVariant m_Variant;

        readonly Scrollable m_SwipeManipulator;

        bool m_InSwipeAreaToOpen;

        Vector2 m_SwipeToOpenVector;

        bool m_Swipeable;

        float m_SwipeAreaWidth;

        bool m_IsOpen;

        float m_Elevation;

        Vector2 m_DownPosition;

        Vector2 m_UpPosition;

        int m_TransitionDurationMs;

        bool m_BackdropTransitionEnabled;

        float m_Hysteresis;

        float m_BackdropFinalOpacity;

        /// <summary>
        /// Event fired when the drawer is closed.
        /// </summary>
        public event Action<Drawer> closed;

        /// <summary>
        /// Event fired when the drawer is opened.
        /// </summary>
        public event Action<Drawer> opened;

        /// <summary>
        /// The opacity of the backdrop when the drawer is open.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public float backdropFinalOpacity
        {
            get => m_BackdropFinalOpacity;
            set
            {
                var changed = !Mathf.Approximately(m_BackdropFinalOpacity, value);
                m_BackdropFinalOpacity = value;

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in backdropFinalOpacityProperty);
#endif
            }
        }

        /// <summary>
        /// Ability to swipe the drawer to open it or close it.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public bool swipeable
        {
            get
            {
                return m_Swipeable && variant == DrawerVariant.Temporary;
            }

            set
            {
                if (m_Swipeable == value)
                    return;

                m_Swipeable = value;
                m_SwipeManipulator?.Cancel();

#if ENABLE_RUNTIME_DATA_BINDINGS
                NotifyPropertyChanged(in swipeableProperty);
#endif
            }
        }

        /// <summary>
        /// Check if the drawer is open.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
        public bool isOpen
        {
            get => m_IsOpen || variant == DrawerVariant.Permanent;
            set
            {
                if (value == m_IsOpen || variant == DrawerVariant.Permanent)
                    return;

                SetOpenState(value); // calls NotifyPropertyChanged(in isOpenProperty) and others events

                if (value)
                {
                    if (anchor == DrawerAnchor.Left)
                        m_DrawerElement.style.left = 0;
                    else
                        m_DrawerElement.style.right = 0;
                    if (!hideBackdrop)
                        m_Backdrop.style.opacity = backdropFinalOpacity;
                }
                else
                {
                    var size = float.IsNaN(m_DrawerElement.localBound.width) ? 16000 : m_DrawerElement.localBound.width;
                    if (anchor == DrawerAnchor.Left)
                        m_DrawerElement.style.left = -size;
                    else
                        m_DrawerElement.style.right = -size;
                    m_Backdrop.style.opacity = 0;
                }

#if ENABLE_RUNTIME_DATA_BINDINGS
                NotifyPropertyChanged(in distanceProperty);
#endif
            }
        }

        /// <summary>
        /// The duration of the transition when opening or closing the drawer in milliseconds.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public int transitionDurationMs
        {
            get => m_TransitionDurationMs;
            set
            {
                if (m_TransitionDurationMs == value)
                    return;

                m_TransitionDurationMs = value;

#if ENABLE_RUNTIME_DATA_BINDINGS
                NotifyPropertyChanged(in transitionDurationMsProperty);
#endif
            }
        }

        /// <summary>
        /// Enable or disable the transition animation for the backdrop when opening or closing the drawer.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public bool backdropTransitionEnabled
        {
            get => m_BackdropTransitionEnabled;
            set
            {
                if (m_BackdropTransitionEnabled == value)
                    return;

                m_BackdropTransitionEnabled = value;

#if ENABLE_RUNTIME_DATA_BINDINGS
                NotifyPropertyChanged(in backdropTransitionEnabledProperty);
#endif
            }
        }

        /// <summary>
        /// Show or hide the backdrop of this drawer.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public bool hideBackdrop
        {
            get => m_Backdrop.ClassListContains(Styles.hiddenUssClassName);
            set
            {
                var changed = m_Backdrop.ClassListContains(Styles.hiddenUssClassName) != value;
                m_Backdrop.EnableInClassList(Styles.hiddenUssClassName, value);

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in hideBackdropProperty);
#endif
            }
        }

        /// <summary>
        /// The content container of the drawer.
        /// </summary>
        public override VisualElement contentContainer => m_DrawerContainer;

        /// <summary>
        /// The normalized distance of the drawer from the edge of the screen. 0 means the drawer is closed, 1 means the
        /// drawer is fully open.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty(ReadOnly = true)]
#endif
        public float distance
        {
            get
            {
                var size = m_DrawerElement.localBound.width;
                return anchor switch
                {
                    DrawerAnchor.Left => (size + m_DrawerElement.resolvedStyle.left) / size,
                    _ => (size + m_DrawerElement.resolvedStyle.right) / size,
                };
            }
        }

        /// <summary>
        /// The size of the swipe area to open the drawer.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public float swipeAreaWidth
        {
            get => m_SwipeAreaWidth;

            set
            {
                var changed = !Mathf.Approximately(m_SwipeAreaWidth, value);
                m_SwipeAreaWidth = value;
                if (m_Variant == DrawerVariant.Temporary)
                    style.width = m_SwipeAreaWidth;

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in swipeAreaWidthProperty);
#endif
            }
        }

        /// <summary>
        /// The distance threshold to interact with the drawer when swiping.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public float hysteresis
        {
            get => m_Hysteresis;
            set
            {
                var changed = !Mathf.Approximately(m_Hysteresis, value);
                m_Hysteresis = value;

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in hysteresisProperty);
#endif
            }
        }

        /// <summary>
        /// The elevation level of the drawer.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public float elevation
        {
            get => m_Elevation;
            set
            {
                var changed = !Mathf.Approximately(m_Elevation, value);
                m_DrawerContainer.RemoveFromClassList(MemoryUtils.Concatenate(elevationUssClassName, m_Elevation.ToString()));
                m_Elevation = value;
                m_DrawerContainer.passMask = m_Elevation > 0
                    ? ExVisualElement.Passes.Clear | ExVisualElement.Passes.OutsetShadows
                    : 0;
                m_DrawerContainer.AddToClassList(MemoryUtils.Concatenate(elevationUssClassName, m_Elevation.ToString()));
                if (anchor == DrawerAnchor.Left)
                {
                    m_DrawerElement.style.paddingRight = m_Elevation;
                    m_DrawerElement.style.paddingLeft = 0;
                }
                else
                {
                    m_DrawerElement.style.paddingLeft = m_Elevation;
                    m_DrawerElement.style.paddingRight = 0;
                }

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in elevationProperty);
#endif
            }
        }

        /// <summary>
        /// The anchor of the drawer. The drawer will be anchored to the left or right side of the screen.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public DrawerAnchor anchor
        {
            get => m_Anchor;
            set
            {
                var changed = m_Anchor != value;
                RemoveFromClassList(GetAnchorUssClassName(m_Anchor));
                m_Anchor = value;

                if (m_Anchor == DrawerAnchor.Left)
                {
                    style.left = 0;
                    style.top = 0;
                    style.bottom = 0;
                    style.right = new StyleLength(StyleKeyword.Auto);
                    m_DrawerElement.style.left = -640;
                    m_DrawerElement.style.right = new StyleLength(StyleKeyword.Auto);
                    m_DrawerElement.style.paddingRight = m_Elevation;
                    m_DrawerElement.style.paddingLeft = 0;
                }
                else
                {
                    style.left = new StyleLength(StyleKeyword.Auto);
                    style.top = 0;
                    style.bottom = 0;
                    style.right = 0;
                    m_DrawerElement.style.right = -640;
                    m_DrawerElement.style.left = new StyleLength(StyleKeyword.Auto);
                    m_DrawerElement.style.paddingLeft = m_Elevation;
                    m_DrawerElement.style.paddingRight = 0;
                }
                AddToClassList(GetAnchorUssClassName(m_Anchor));

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                {
                    NotifyPropertyChanged(in anchorProperty);
                    NotifyPropertyChanged(in distanceProperty);
                }
#endif
            }
        }

        /// <summary>
        /// The variant of the drawer. Permanent drawers are always open and cannot be closed. Temporary drawers can be
        /// opened and closed.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public DrawerVariant variant
        {
            get => m_Variant;
            set
            {
                var changed = m_Variant != value;
                RemoveFromClassList(GetVariantUssClassName(m_Variant));
                m_Variant = value;
                AddToClassList(GetVariantUssClassName(m_Variant));
                if (m_Variant == DrawerVariant.Permanent)
                {
                    isOpen = true;
                    style.width = new StyleLength(StyleKeyword.Auto);
                    if (anchor == DrawerAnchor.Left)
                    {
                        m_DrawerElement.style.left = 0;
                        m_DrawerElement.style.right = new StyleLength(StyleKeyword.Auto);
                    }
                    else
                    {
                        m_DrawerElement.style.right = 0;
                        m_DrawerElement.style.left = new StyleLength(StyleKeyword.Auto);
                    }
                }
                else
                {
                    isOpen = false;
                    if (anchor == DrawerAnchor.Left)
                        m_DrawerElement.style.right = new StyleLength(StyleKeyword.Auto);
                    else
                        m_DrawerElement.style.left = new StyleLength(StyleKeyword.Auto);
                }

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                {
                    NotifyPropertyChanged(in variantProperty);
                    NotifyPropertyChanged(in isOpenProperty);
                    NotifyPropertyChanged(in distanceProperty);
                }
#endif
            }
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public Drawer()
        {
            pickingMode = PickingMode.Position;
            AddToClassList(ussClassName);

            m_Backdrop = new VisualElement
            {
                name = backdropUssClassName,
                pickingMode = PickingMode.Ignore,
                usageHints = UsageHints.DynamicColor,
            };
            m_DrawerElement = new VisualElement
            {
                name = drawerUssClassName,
                pickingMode = PickingMode.Ignore,
            };
            m_DrawerElement.EnableDynamicTransform(true);
            m_DrawerContainer = new ExVisualElement
            {
                name = drawerContainerUssClassName,
                pickingMode = PickingMode.Ignore,
                passMask = 0
            };

            m_Backdrop.AddToClassList(backdropUssClassName);
            m_DrawerElement.AddToClassList(drawerUssClassName);
            m_DrawerContainer.AddToClassList(drawerContainerUssClassName);

            hierarchy.Add(m_Backdrop);
            hierarchy.Add(m_DrawerElement);
            m_DrawerElement.hierarchy.Add(m_DrawerContainer);

            m_SwipeManipulator = new Scrollable(OnDrag, OnUp, OnDown, OnCancel)
            {
                direction = ScrollViewMode.Horizontal,
                threshold = 0,
            };
            this.AddManipulator(m_SwipeManipulator);

            anchor = DrawerAnchor.Left;
            variant = DrawerVariant.Temporary;
            swipeable = true;
            backdropTransitionEnabled = true;
            swipeAreaWidth = 16;
            elevation = 16;
            backdropFinalOpacity = 0.33f;
            transitionDurationMs = 150;
            hysteresis = 8;
            SetOpenState(false);
        }

        /// <summary>
        /// Open the drawer.
        /// </summary>
        public void Open()
        {
            if (isOpen || variant == DrawerVariant.Permanent)
                return;

            if (anchor == DrawerAnchor.Left)
                m_DrawerElement.style.left = -m_DrawerElement.localBound.width;
            else
                m_DrawerElement.style.right = -m_DrawerElement.localBound.width;

#if ENABLE_RUNTIME_DATA_BINDINGS
            NotifyPropertyChanged(in distanceProperty);
#endif

            SetOpenState(true);
            if (m_SwipeToOpenVector.sqrMagnitude <= 0)
                FinishOpenAnimation();
        }

        void FinishOpenAnimation()
        {
            // Animation
            m_DrawerElement.experimental.animation.Start(distance, 1, transitionDurationMs, (element, f) =>
            {
                if (anchor == DrawerAnchor.Left)
                    element.style.left = (1 - f) * -m_DrawerElement.localBound.width;
                else
                    element.style.right = (1 - f) * m_DrawerElement.localBound.width;

#if ENABLE_RUNTIME_DATA_BINDINGS
                NotifyPropertyChanged(in distanceProperty);
#endif
            }).Ease(Easing.OutQuad);

            if (backdropTransitionEnabled && !hideBackdrop)
            {
                m_Backdrop.experimental.animation.Start(m_Backdrop.resolvedStyle.opacity, backdropFinalOpacity, transitionDurationMs, (element, f) =>
                {
                    element.style.opacity = f;
                }).Ease(Easing.OutQuad);
            }
            else if (!hideBackdrop)
            {
                m_Backdrop.style.opacity = backdropFinalOpacity;
            }
        }

        /// <summary>
        /// Close the drawer.
        /// </summary>
        public void Close()
        {
            if (variant == DrawerVariant.Permanent)
                return;

            var size = m_DrawerElement.localBound.width;
            var d = anchor == DrawerAnchor.Left ? distance : (size - m_DrawerElement.resolvedStyle.right) / size;
            m_DrawerElement.experimental.animation.Start(d, 0, transitionDurationMs,
                (element, f) =>
                {
                    if (anchor == DrawerAnchor.Left)
                        element.style.left = (1 - f) * -m_DrawerElement.localBound.width;
                    else
                        element.style.right = (1 - f) * -m_DrawerElement.localBound.width;

#if ENABLE_RUNTIME_DATA_BINDINGS
                    NotifyPropertyChanged(in distanceProperty);
#endif
                }).Ease(Easing.OutQuad).OnCompleted(OnCloseAnimationFinished);

            if (backdropTransitionEnabled && !hideBackdrop)
            {
                m_Backdrop.experimental.animation.Start(m_Backdrop.resolvedStyle.opacity, 0, transitionDurationMs, (element, f) =>
                {
                    element.style.opacity = f;
                }).Ease(Easing.OutQuad);
            }
            else if (!hideBackdrop)
            {
                m_Backdrop.style.opacity = 0;
            }
        }

        /// <summary>
        /// Toggle the drawer. If it is open, close it. If it is closed, open it.
        /// </summary>
        public void Toggle()
        {
            if (isOpen)
                Close();
            else
                Open();
        }

        bool InSwipeArea(Vector2 pos)
        {
            if (anchor == DrawerAnchor.Left)
                return pos.x < swipeAreaWidth;
            else
                return pos.x > contentRect.width - swipeAreaWidth;
        }

        void OnDown(Scrollable manipulator)
        {
            m_InSwipeAreaToOpen = false;
            m_SwipeToOpenVector = Vector2.zero;
            m_DownPosition = manipulator.position;

            if (!isOpen && swipeable && variant == DrawerVariant.Temporary)
            {
                m_InSwipeAreaToOpen = InSwipeArea(manipulator.localPosition);
            }
        }

        bool IsDirectionToClose()
        {
            if (anchor == DrawerAnchor.Left)
                return m_UpPosition.x - m_DownPosition.x < 0;
            else
                return m_UpPosition.x - m_DownPosition.x > 0;
        }

        void OnUp(Scrollable manipulator)
        {
            m_UpPosition = manipulator.position;
            var closing = m_SwipeToOpenVector.sqrMagnitude == 0 && !m_InSwipeAreaToOpen && isOpen && IsDirectionToClose();
            m_InSwipeAreaToOpen = false;
            m_SwipeToOpenVector = Vector2.zero;

            if (variant == DrawerVariant.Permanent)
                return;

            if (!manipulator.hasMoved)
            {
                OnClick();
                return;
            }

            if (closing)
            {
                Close();
            }
            else if (isOpen)
            {
                FinishOpenAnimation();
            }
        }

        void OnCancel(Scrollable manipulator)
        {
            OnUp(manipulator);
        }

        void OnDrag(Scrollable manipulator)
        {
            if (!swipeable)
                return;

            if (isOpen)
            {
                // move the drawer
                float d;
                var size = m_DrawerElement.localBound.width;
                if (anchor == DrawerAnchor.Left)
                {
                    var newLeftValue = Mathf.Min(0, m_DrawerElement.resolvedStyle.left + manipulator.deltaPos.x);
                    m_DrawerElement.style.left = newLeftValue;
                    d = (size + newLeftValue) / size;
                }
                else
                {
                    var newRightValue = Mathf.Min(0, -m_DrawerElement.resolvedStyle.right - manipulator.deltaPos.x);
                    m_DrawerElement.style.right = newRightValue;
                    d = (size + newRightValue) / size;
                }

#if ENABLE_RUNTIME_DATA_BINDINGS
                NotifyPropertyChanged(in distanceProperty);
#endif

                if (backdropTransitionEnabled && !hideBackdrop)
                    m_Backdrop.style.opacity = Mathf.Lerp(0, backdropFinalOpacity, d);
            }
            else
            {
                // check if the pointer is in the swipe area to open the drawer
                if (m_InSwipeAreaToOpen)
                {
                    m_SwipeToOpenVector += manipulator.deltaPos;
                    if ((anchor == DrawerAnchor.Left && m_SwipeToOpenVector.x > hysteresis)
                        || (anchor == DrawerAnchor.Right && m_SwipeToOpenVector.x < -hysteresis))
                        Open();
                }
            }
        }

        void OnClick()
        {
            if (m_SwipeManipulator.hasMoved)
                return;

            // Close the Drawer if it is open
            var mousePos = this.LocalToWorld(m_SwipeManipulator.localPosition);
            var hoverDrawer = m_DrawerElement.ContainsPoint(m_DrawerElement.WorldToLocal(mousePos));
            if (isOpen && !hoverDrawer)
                Close();
        }

        void OnCloseAnimationFinished()
        {
            SetOpenState(false);
        }

        void SetOpenState(bool value)
        {
            if (value)
            {
                AddToClassList(Styles.openUssClassName);
                m_IsOpen = true;
                m_Backdrop.pickingMode = PickingMode.Position;
                style.width = new Length(100, LengthUnit.Percent);
                opened?.Invoke(this);
            }
            else
            {
                RemoveFromClassList(Styles.openUssClassName);
                m_IsOpen = false;
                m_Backdrop.pickingMode = PickingMode.Ignore;
                style.width = swipeAreaWidth;
                closed?.Invoke(this);
            }

#if ENABLE_RUNTIME_DATA_BINDINGS
            NotifyPropertyChanged(in isOpenProperty);
            NotifyPropertyChanged(in distanceProperty);
#endif
        }

#if ENABLE_UXML_TRAITS

        /// <summary>
        /// Factory class to instantiate a <see cref="Drawer"/> using the data read from a UXML file.
        /// </summary>
        public new class UxmlFactory : UxmlFactory<Drawer, UxmlTraits> { }

        /// <summary>
        /// Class containing the <see cref="UxmlTraits"/> for the <see cref="Drawer"/>.
        /// </summary>
        public new class UxmlTraits : BaseVisualElement.UxmlTraits
        {
            readonly UxmlEnumAttributeDescription<DrawerAnchor> m_Anchor =
                new UxmlEnumAttributeDescription<DrawerAnchor>()
                {
                    name = "anchor",
                    defaultValue = DrawerAnchor.Left,
                };

            readonly UxmlEnumAttributeDescription<DrawerVariant> m_Variant =
                new UxmlEnumAttributeDescription<DrawerVariant>()
                {
                    name = "variant",
                    defaultValue = DrawerVariant.Temporary,
                };

            readonly UxmlBoolAttributeDescription m_Swipeable =
                new UxmlBoolAttributeDescription()
                {
                    name = "swipeable",
                    defaultValue = true,
                };

            readonly UxmlBoolAttributeDescription m_HideBackdrop =
                new UxmlBoolAttributeDescription()
                {
                    name = "hide-backdrop",
                    defaultValue = false,
                };

            readonly UxmlBoolAttributeDescription m_BackdropTransitionEnabled =
                new UxmlBoolAttributeDescription()
                {
                    name = "backdrop-transition",
                    defaultValue = true,
                };

            readonly UxmlFloatAttributeDescription m_BackdropFinalOpacity =
                new UxmlFloatAttributeDescription()
                {
                    name = "backdrop-opacity",
                    defaultValue = 0.33f,
                };

            readonly UxmlFloatAttributeDescription m_SwipeAreaWidth =
                new UxmlFloatAttributeDescription()
                {
                    name = "swipe-area-width",
                    defaultValue = 16f,
                };

            readonly UxmlIntAttributeDescription m_TransitionDuration =
                new UxmlIntAttributeDescription()
                {
                    name = "transition-duration",
                    defaultValue = 150,
                };

            readonly UxmlFloatAttributeDescription m_Hysteresis =
                new UxmlFloatAttributeDescription()
                {
                    name = "hysteresis",
                    defaultValue = 8f,
                };

            readonly UxmlIntAttributeDescription m_Elevation =
                new UxmlIntAttributeDescription()
                {
                    name = "elevation",
                    defaultValue = 16,
                };

            /// <summary>
            /// Initializes the VisualElement from the UXML attributes.
            /// </summary>
            /// <param name="ve"> The <see cref="VisualElement"/> to initialize.</param>
            /// <param name="bag"> The <see cref="IUxmlAttributes"/> bag to use to initialize the <see cref="VisualElement"/>.</param>
            /// <param name="cc"> The <see cref="CreationContext"/> to use to initialize the <see cref="VisualElement"/>.</param>
            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);

                var el = (Drawer)ve;
                el.anchor = m_Anchor.GetValueFromBag(bag, cc);
                el.variant = m_Variant.GetValueFromBag(bag, cc);
                el.swipeable = m_Swipeable.GetValueFromBag(bag, cc);
                el.hideBackdrop = m_HideBackdrop.GetValueFromBag(bag, cc);
                el.backdropTransitionEnabled = m_BackdropTransitionEnabled.GetValueFromBag(bag, cc);
                el.backdropFinalOpacity = m_BackdropFinalOpacity.GetValueFromBag(bag, cc);
                el.swipeAreaWidth = m_SwipeAreaWidth.GetValueFromBag(bag, cc);
                el.transitionDurationMs = m_TransitionDuration.GetValueFromBag(bag, cc);
                el.hysteresis = m_Hysteresis.GetValueFromBag(bag, cc);
                el.elevation = m_Elevation.GetValueFromBag(bag, cc);
            }
        }

#endif
    }

    /// <summary>
    /// The variant of the Drawer.
    /// </summary>
    public enum DrawerVariant
    {
        /// <summary>
        /// The Drawer is temporary and will be dismissed when the user clicks outside of it.
        /// </summary>
        Temporary,
        /// <summary>
        /// The Drawer is permanent and will not be dismissed when the user clicks outside of it.
        /// </summary>
        Permanent
    }

    /// <summary>
    /// The anchor of the Drawer. The Drawer will be anchored to the left or right side of the screen.
    /// </summary>
    [GenerateLowerCaseStrings]
    public enum DrawerAnchor
    {
        /// <summary>
        /// The Drawer will be anchored to the left side of the screen.
        /// </summary>
        Left,
        /// <summary>
        /// The Drawer will be anchored to the right side of the screen.
        /// </summary>
        Right,
    }
}
