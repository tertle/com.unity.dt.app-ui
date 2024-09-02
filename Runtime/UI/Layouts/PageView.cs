using System.Collections.Generic;
using Unity.AppUI.Core;
using UnityEngine;
using UnityEngine.UIElements;
#if ENABLE_RUNTIME_DATA_BINDINGS
using Unity.Properties;
#endif

namespace Unity.AppUI.UI
{
    /// <summary>
    /// A PageView is a container that displays a single child at a time and provides a UI to
    /// navigate between them. It is similar to a <see cref="ScrollView"/> but here children are
    /// snapped to the container's edges.
    /// </summary>
#if ENABLE_UXML_SERIALIZED_DATA
    [UxmlElement]
#endif
    public partial class PageView : BaseVisualElement
    {
#if ENABLE_RUNTIME_DATA_BINDINGS

        internal static readonly BindingId directionProperty = new BindingId(nameof(direction));

        internal static readonly BindingId animationSpeedProperty = new BindingId(nameof(snapAnimationSpeed));

        internal static readonly BindingId skipAnimationThresholdProperty = new BindingId(nameof(skipAnimationThreshold));

        internal static readonly BindingId wrapProperty = new BindingId(nameof(wrap));

        internal static readonly BindingId visibilityCountProperty = new BindingId(nameof(visibilityCount));

        internal static readonly BindingId autoPlayDurationProperty = new BindingId(nameof(autoPlayDuration));
#endif

        /// <summary>
        /// The main styling class of the PageView. This is the class that is used in the USS file.
        /// </summary>
        public const string ussClassName = "appui-pageview";

        /// <summary>
        /// The styling class applied to the SwipeView.
        /// </summary>
        public const string swipeViewUssClassName = ussClassName + "__swipeview";

        /// <summary>
        /// The styling class applied to the PageIndicator.
        /// </summary>
        public const string pageIndicatorUssClassName = ussClassName + "__page-indicator";

        /// <summary>
        /// The styling class applied to the PageView depending on its direction.
        /// </summary>
        [EnumName("GetDirectionUssClassName", typeof(Direction))]
        public const string variantUssClassName = ussClassName + "--";

        readonly SwipeView m_SwipeView;

        readonly PageIndicator m_PageIndicator;

        Dir m_CurrentDirection;

        /// <summary>
        /// The content container of the PageView.
        /// </summary>
        public override VisualElement contentContainer => m_SwipeView.contentContainer;

        /// <summary>
        /// The speed of the animation when snapping to a page.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public float snapAnimationSpeed
        {
            get => m_SwipeView.snapAnimationSpeed;
            set
            {
                var changed = !Mathf.Approximately(m_SwipeView.snapAnimationSpeed, value);
                m_SwipeView.snapAnimationSpeed = value;

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in animationSpeedProperty);
#endif
            }
        }

        /// <summary>
        /// A limit number of pages to keep animating the transition between pages.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public int skipAnimationThreshold
        {
            get => m_SwipeView.skipAnimationThreshold;
            set
            {
                var changed = m_SwipeView.skipAnimationThreshold != value;
                m_SwipeView.skipAnimationThreshold = value;

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in skipAnimationThresholdProperty);
#endif
            }
        }

        /// <summary>
        /// Whether the PageView should wrap around when reaching the end of the list.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public bool wrap
        {
            get => m_SwipeView.wrap;
            set
            {
                var changed = m_SwipeView.wrap != value;
                m_SwipeView.wrap = value;

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in wrapProperty);
#endif
            }
        }

        /// <summary>
        /// The number of milliseconds between each automatic swipe.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public int autoPlayDuration
        {
            get => m_SwipeView.autoPlayDuration;
            set
            {
                var changed = m_SwipeView.autoPlayDuration != value;
                m_SwipeView.autoPlayDuration = value;

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in autoPlayDurationProperty);
#endif
            }
        }

        /// <summary>
        /// The orientation of the PageView.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public Direction direction
        {
            get => m_SwipeView.direction;
            set
            {
                var changed = m_SwipeView.direction != value;
                RemoveFromClassList(GetDirectionUssClassName(m_SwipeView.direction));
                m_SwipeView.direction = value;
                m_PageIndicator.direction = value;
                AddToClassList(GetDirectionUssClassName(m_SwipeView.direction));

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in directionProperty);
#endif
            }
        }

        /// <summary>
        /// The number of pages that are visible at the same time.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public int visibilityCount
        {
            get => m_SwipeView.visibleItemCount;
            set
            {
                var changed = m_SwipeView.visibleItemCount != value;
                m_SwipeView.visibleItemCount = value;

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in visibilityCountProperty);
#endif
            }
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public PageView()
        {
            AddToClassList(ussClassName);

            m_SwipeView = new SwipeView { name = swipeViewUssClassName };
            m_SwipeView.AddToClassList(swipeViewUssClassName);

            m_PageIndicator = new PageIndicator { name = pageIndicatorUssClassName };
            m_PageIndicator.AddToClassList(pageIndicatorUssClassName);

            hierarchy.Add(m_SwipeView);
            hierarchy.Add(m_PageIndicator);

            m_SwipeView.RegisterValueChangedCallback(OnSwipeValueChanged);
            m_PageIndicator.RegisterValueChangedCallback(OnPageIndicatorValueChanged);
            RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);

            direction = Direction.Horizontal;
            snapAnimationSpeed = 0.5f;
            skipAnimationThreshold = 2;
            wrap = false;
            visibilityCount = 1;
            autoPlayDuration = SwipeView.noAutoPlayDuration;

            this.RegisterContextChangedCallback<DirContext>(OnDirContextChanged);
        }

        void OnDirContextChanged(ContextChangedEvent<DirContext> evt)
        {
            m_CurrentDirection = evt.context?.dir ?? Dir.Ltr;
            m_PageIndicator.count = m_SwipeView.childCount;
            var newValue = m_PageIndicator.count > 0 ? 0 : -1;
            m_SwipeView.value = newValue;
        }

        void OnPageIndicatorValueChanged(ChangeEvent<int> evt)
        {
            m_SwipeView.SetValueWithoutNotify(evt.newValue);
        }

        void OnSwipeValueChanged(ChangeEvent<int> evt)
        {
            m_PageIndicator.count = m_SwipeView.childCount;
            m_PageIndicator.SetValueWithoutNotify(evt.newValue);
        }

        void OnGeometryChanged(GeometryChangedEvent evt)
        {
            m_PageIndicator.count = m_SwipeView.childCount;
            m_PageIndicator.SetValueWithoutNotify(m_SwipeView.value);
            m_SwipeView.SetValueWithoutNotify(m_SwipeView.value);
        }

#if ENABLE_UXML_TRAITS

        /// <summary>
        /// Class used to create a PageView from UXML.
        /// </summary>
        public new class UxmlFactory : UxmlFactory<PageView, UxmlTraits> { }

        /// <summary>
        /// Class containing the <see cref="UxmlTraits"/> for the <see cref="PageView"/>.
        /// </summary>
        public new class UxmlTraits : BaseVisualElement.UxmlTraits
        {
            readonly UxmlEnumAttributeDescription<Direction> m_Direction = new UxmlEnumAttributeDescription<Direction>
            {
                name = "direction",
                defaultValue = Direction.Horizontal,
            };

            readonly UxmlFloatAttributeDescription m_AnimationSpeed = new UxmlFloatAttributeDescription()
            {
                name = "animation-speed",
                defaultValue = 0.5f,
            };

            readonly UxmlIntAttributeDescription m_SkipAnim = new UxmlIntAttributeDescription()
            {
                name = "skip-animation-threshold",
                defaultValue = 2,
            };

            readonly UxmlBoolAttributeDescription m_Wrap = new UxmlBoolAttributeDescription()
            {
                name = "wrap",
                defaultValue = false,
            };

            readonly UxmlIntAttributeDescription m_VisibilityCount = new UxmlIntAttributeDescription()
            {
                name = "visibility-count",
                defaultValue = 1,
            };

            readonly UxmlIntAttributeDescription m_AutoPlayDuration = new UxmlIntAttributeDescription()
            {
                name = "auto-play-duration",
                defaultValue = SwipeView.noAutoPlayDuration,
            };

            /// <summary>
            /// Returns an enumerable containing UxmlChildElementDescription(typeof(VisualElement)), since VisualElements can contain other VisualElements.
            /// </summary>
            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription =>
                new[]
                {
                    new UxmlChildElementDescription(typeof(SwipeViewItem))
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

                var el = (PageView)ve;
                el.direction = m_Direction.GetValueFromBag(bag, cc);
                el.wrap = m_Wrap.GetValueFromBag(bag, cc);
                el.visibilityCount = m_VisibilityCount.GetValueFromBag(bag, cc);
                el.skipAnimationThreshold = m_SkipAnim.GetValueFromBag(bag, cc);
                el.snapAnimationSpeed = m_AnimationSpeed.GetValueFromBag(bag, cc);
                el.autoPlayDuration = m_AutoPlayDuration.GetValueFromBag(bag, cc);
            }
        }

#endif
    }
}
