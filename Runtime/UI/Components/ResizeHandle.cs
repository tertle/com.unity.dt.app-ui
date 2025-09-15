using System;
using UnityEngine;
using UnityEngine.UIElements;
#if ENABLE_RUNTIME_DATA_BINDINGS
using Unity.Properties;
#endif

namespace Unity.AppUI.UI
{
    /// <summary>
    /// An element that can be dragged to resize another element.
    /// </summary>
#if ENABLE_UXML_SERIALIZED_DATA
    [UxmlElement]
#endif
    public partial class ResizeHandle : VisualElement
    {
#if ENABLE_RUNTIME_DATA_BINDINGS

        internal static readonly BindingId dragDirectionProperty = nameof(dragDirection);

        internal static readonly BindingId thresholdProperty = nameof(threshold);

#endif

        /// <summary>
        /// Event triggered when the resize operation starts.
        /// </summary>
        public event Action<ResizeHandle> resizeStarted;

        /// <summary>
        /// Event triggered when the resize operation ends.
        /// </summary>
        public event Action<ResizeHandle> resizeEnded;

        /// <summary>
        /// The ResizeHandle main styling class.
        /// </summary>
        public const string ussClassName = "appui-resize-handle";

        /// <summary>
        /// The ResizeHandle variant styling class.
        /// </summary>
        [EnumName("GetDirectionUssClassName", typeof(Draggable.DragDirection))]
        public const string variantUssClassName = ussClassName + "--";

        readonly Draggable m_Draggable;

        Vector2 m_StartSize;

        bool m_Resizing;

        /// <summary>
        /// The direction in which the handle can be dragged.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public Draggable.DragDirection dragDirection
        {
            get => m_Draggable.dragDirection;
            set
            {
                var changed = m_Draggable.dragDirection != value;
                RemoveFromClassList(GetDirectionUssClassName(m_Draggable.dragDirection));
                m_Draggable.dragDirection = value;
                AddToClassList(GetDirectionUssClassName(value));

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in dragDirectionProperty);
#endif
            }
        }

        /// <summary>
        /// The threshold in pixels that the handle must be dragged before it starts to move.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public float threshold
        {
            get => m_Draggable.threshold;
            set
            {
                var changed = !Mathf.Approximately(m_Draggable.threshold, value);
                m_Draggable.threshold = value;

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in thresholdProperty);
#endif
            }
        }

        /// <summary>
        /// The target <see cref="VisualElement"/> that will be resized when the handle is dragged.
        /// </summary>
        public VisualElement target { get; set; }

        /// <summary>
        /// Default Constructor.
        /// </summary>
        public ResizeHandle()
        {
            pickingMode = PickingMode.Position;
            focusable = false;

            AddToClassList(ussClassName);

            m_Draggable = new Draggable(OnClick, OnDrag, OnUp, OnDown);
            this.AddManipulator(m_Draggable);

            dragDirection = Draggable.DragDirection.Vertical;
            threshold = 1f;
        }

        /// <summary>
        /// Constructor with a target <see cref="VisualElement"/>.
        /// </summary>
        /// <param name="target"> The element that will be resized when the handle is dragged. </param>
        public ResizeHandle(VisualElement target)
            : this()
        {
            this.target = target;
        }

        void OnClick()
        {
            // do nothing
        }

        void OnDown(Draggable draggable)
        {
            if (target == null)
                return;

            m_StartSize = target.layout.size;
        }

        void OnDrag(Draggable draggable)
        {
            if (target == null)
                return;

            if (!m_Resizing)
            {
                m_Resizing = true;
                resizeStarted?.Invoke(this);
            }

            var deltaFromStart = draggable.startPosition - draggable.position;
            if ((draggable.dragDirection & Draggable.DragDirection.Vertical) != 0)
                target.style.height = m_StartSize.y - deltaFromStart.y;
            if ((draggable.dragDirection & Draggable.DragDirection.Horizontal) != 0)
                target.style.width = m_StartSize.x - deltaFromStart.x;
        }

        void OnUp(Draggable draggable)
        {
            m_Resizing = false;
            resizeEnded?.Invoke(this);
        }

#if ENABLE_UXML_TRAITS

        /// <summary>
        /// Factory class to instantiate a <see cref="ResizeHandle"/> using the data read from a UXML file.
        /// </summary>
        public new class UxmlFactory : UxmlFactory<ResizeHandle, UxmlTraits> { }

#endif
    }
}
