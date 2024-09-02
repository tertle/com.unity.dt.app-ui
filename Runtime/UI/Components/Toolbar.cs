using System;
using UnityEngine.UIElements;
#if ENABLE_RUNTIME_DATA_BINDINGS
using Unity.Properties;
#endif

namespace Unity.AppUI.UI
{
    /// <summary>
    /// The dock mode of the Toolbar.
    /// </summary>
    public enum ToolbarDockMode
    {
        /// <summary>
        /// The Toolbar is floating.
        /// </summary>
        Floating = 0,

        /// <summary>
        /// The Toolbar is docked to the top.
        /// </summary>
        Top,

        /// <summary>
        /// The Toolbar is docked to the bottom.
        /// </summary>
        Bottom,

        /// <summary>
        /// The Toolbar is docked to the left.
        /// </summary>
        Left,

        /// <summary>
        /// The Toolbar is docked to the right.
        /// </summary>
        Right
    }

    /// <summary>
    /// Accordion visual element.
    /// </summary>
#if ENABLE_UXML_SERIALIZED_DATA
    [UxmlElement]
#endif
    public partial class Toolbar : BaseVisualElement
    {
#if ENABLE_RUNTIME_DATA_BINDINGS

        internal static readonly BindingId dockModeProperty = new BindingId(nameof(dockMode));

        internal static readonly BindingId draggableProperty = new BindingId(nameof(draggable));

        internal static readonly BindingId directionProperty = new BindingId(nameof(direction));

#endif


        /// <summary>
        /// The Toolbar's USS class name.
        /// </summary>
        public const string ussClassName = "appui-toolbar";

        /// <summary>
        /// The Toolbar's drag bar USS class name.
        /// </summary>
        public const string dragBarUssClassName = ussClassName + "__drag-bar";

        /// <summary>
        /// The Toolbar's drag bar indicator USS class name.
        /// </summary>
        public const string dragBarIndicatorUssClassName = dragBarUssClassName + "-indicator";

        /// <summary>
        /// The Toolbar's container USS class name.
        /// </summary>
        public const string containerUssClassName = ussClassName + "__container";

        /// <summary>
        /// The Toolbar's variant USS class name.
        /// </summary>
        [EnumName("GetDockModeUssClassName", typeof(ToolbarDockMode))]
        [EnumName("GetDirectionUssClassName", typeof(Direction))]
        public const string variantUssClassName = ussClassName + "--";

        /// <summary>
        /// The Toolbar's draggable USS class name.
        /// </summary>
        public const string draggableUssClassName = ussClassName + "--draggable";

        readonly VisualElement m_Container;

        readonly VisualElement m_DragBar;

        readonly VisualElement m_DragBarIndicator;

        readonly Draggable m_DragBarPressable;

        ToolbarDockMode m_DockMode;

        Direction m_Direction;

        /// <summary>
        /// The container of the Toolbar's content.
        /// </summary>
        public override VisualElement contentContainer => m_Container;

        /// <summary>
        /// The dock mode of the Toolbar.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public ToolbarDockMode dockMode
        {
            get => m_DockMode;
            set
            {
                var changed = m_DockMode != value;
                RemoveFromClassList(GetDockModeUssClassName(m_DockMode));
                m_DockMode = value;
                AddToClassList(GetDockModeUssClassName(m_DockMode));

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in dockModeProperty);
#endif
            }
        }

        /// <summary>
        /// Whether the Toolbar is draggable.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public bool draggable
        {
            get => ClassListContains(draggableUssClassName);
            set
            {
                var changed = ClassListContains(draggableUssClassName) != value;
                EnableInClassList(draggableUssClassName, value);

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in draggableProperty);
#endif
            }
        }

        /// <summary>
        /// The direction of the Toolbar.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public Direction direction
        {
            get => m_Direction;
            set
            {
                var changed = m_Direction != value;
                RemoveFromClassList(GetDirectionUssClassName(m_Direction));
                m_Direction = value;
                AddToClassList(GetDirectionUssClassName(m_Direction));

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in directionProperty);
#endif
            }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public Toolbar()
        {
            AddToClassList(ussClassName);

            m_DragBar = new VisualElement
            {
                name = dragBarUssClassName,
                pickingMode = PickingMode.Position
            };
            m_DragBar.AddToClassList(dragBarUssClassName);
            hierarchy.Add(m_DragBar);

            m_DragBarPressable = new Draggable(null, null, null);
            m_DragBar.AddManipulator(m_DragBarPressable);

            m_DragBarIndicator = new VisualElement
            {
                name = dragBarIndicatorUssClassName,
                pickingMode = PickingMode.Ignore
            };
            m_DragBarIndicator.AddToClassList(dragBarIndicatorUssClassName);
            m_DragBar.Add(m_DragBarIndicator);

            m_Container = new VisualElement
            {
                name = containerUssClassName,
                pickingMode = PickingMode.Ignore
            };
            m_Container.AddToClassList(containerUssClassName);
            hierarchy.Add(m_Container);

            draggable = false;
            dockMode = ToolbarDockMode.Floating;
            direction = Direction.Horizontal;
        }

#if ENABLE_UXML_TRAITS

        /// <summary>
        /// UXML Factory for Toolbar.
        /// </summary>
        public new class UxmlFactory : UxmlFactory<Toolbar, UxmlTraits> { }

        /// <summary>
        /// UXML Traits for Toolbar.
        /// </summary>
        public new class UxmlTraits : BaseVisualElement.UxmlTraits
        {
            readonly UxmlEnumAttributeDescription<ToolbarDockMode> m_DockMode = new UxmlEnumAttributeDescription<ToolbarDockMode> { name = "dock-mode", defaultValue = ToolbarDockMode.Floating };

            readonly UxmlBoolAttributeDescription m_Draggable = new UxmlBoolAttributeDescription { name = "draggable", defaultValue = false };

            readonly UxmlEnumAttributeDescription<Direction> m_Direction = new UxmlEnumAttributeDescription<Direction> { name = "direction", defaultValue = Direction.Horizontal };

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);

                var toolbar = (Toolbar)ve;

                toolbar.dockMode = m_DockMode.GetValueFromBag(bag, cc);
                toolbar.draggable = m_Draggable.GetValueFromBag(bag, cc);
                toolbar.direction = m_Direction.GetValueFromBag(bag, cc);
            }
        }

#endif
    }
}