using System;
using Unity.AppUI.Core;
using UnityEngine.UIElements;
#if ENABLE_RUNTIME_DATA_BINDINGS
using Unity.Properties;
#endif

namespace Unity.AppUI.UI
{
    /// <summary>
    /// A floating action button.
    /// </summary>
#if ENABLE_UXML_SERIALIZED_DATA
    [UxmlElement]
#endif
    public partial class FloatingActionButton : ExVisualElement, IPressable
    {
#if ENABLE_RUNTIME_DATA_BINDINGS

        internal static readonly BindingId sizeProperty = new BindingId(nameof(size));

        internal static readonly BindingId elevationProperty = new BindingId(nameof(elevation));

        internal static readonly BindingId accentProperty = new BindingId(nameof(accent));

#endif

        /// <summary>
        /// The Floating Action Button's USS class name.
        /// </summary>
        public const string ussClassName = "appui-fab";

        /// <summary>
        /// The Floating Action Button's elevation USS class name.
        /// </summary>
        public const string elevationUssClassName = Styles.elevationUssClassName;

        /// <summary>
        /// The Floating Action Button's size USS class name.
        /// </summary>
        [EnumName("GetSizeUssClassName", typeof(Size))]
        public const string sizeUssClassName = ussClassName + "--size-";

        /// <summary>
        /// The Floating Action Button's accent USS class name.
        /// </summary>
        public const string accentUssClassName = ussClassName + "--accent";

        Pressable m_Clickable;

        int m_Elevation;

        Size m_Size;

        /// <summary>
        /// The clickable manipulator used by this button.
        /// </summary>
        public Pressable clickable
        {
            get => m_Clickable;
            private set
            {
                if (m_Clickable != null && m_Clickable.target == this)
                    this.RemoveManipulator(m_Clickable);
                m_Clickable = value;
                if (m_Clickable == null)
                    return;
                this.AddManipulator(m_Clickable);
            }
        }

        /// <summary>
        /// Event fired when the button is clicked.
        /// </summary>
        public event Action clicked
        {
            add => clickable.clicked += value;
            remove => clickable.clicked -= value;
        }

        /// <summary>
        /// The elevation of this element.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public int elevation
        {
            get => m_Elevation;
            set
            {
                var changed = m_Elevation != value;
                RemoveFromClassList(MemoryUtils.Concatenate(elevationUssClassName, m_Elevation.ToString()));
                m_Elevation = value;
                AddToClassList(MemoryUtils.Concatenate(elevationUssClassName, m_Elevation.ToString()));

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in elevationProperty);
#endif
            }
        }

        /// <summary>
        /// The accent variant of this element.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public bool accent
        {
            get => ClassListContains(accentUssClassName);
            set
            {
                var changed = ClassListContains(accentUssClassName) != value;
                EnableInClassList(accentUssClassName, value);

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in accentProperty);
#endif
            }
        }

        /// <summary>
        /// The size of this element.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public Size size
        {
            get => m_Size;
            set
            {
                var changed = m_Size != value;
                RemoveFromClassList(GetSizeUssClassName(m_Size));
                m_Size = value;
                AddToClassList(GetSizeUssClassName(m_Size));

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in sizeProperty);
#endif
            }
        }

        /// <summary>
        /// The content container of this element.
        /// </summary>
        public override VisualElement contentContainer => this;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public FloatingActionButton() : this(null) { }

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="clickAction"> The action to perform when the button is clicked. </param>
        public FloatingActionButton(Action clickAction)
        {
            AddToClassList(ussClassName);

            clickable = new Pressable(clickAction);
            pickingMode = PickingMode.Position;
            focusable = true;
            tabIndex = 0;

            passMask = Passes.Clear | Passes.OutsetShadows;
            elevation = 12;
            size = Size.M;

            this.AddManipulator(new KeyboardFocusController(OnKeyboardFocus, OnPointerFocus));
        }

        void OnPointerFocus(FocusInEvent evt)
        {
            passMask = Passes.Clear | Passes.OutsetShadows;
        }

        void OnKeyboardFocus(FocusInEvent evt)
        {
            passMask = Passes.Clear | Passes.OutsetShadows | Passes.Outline;
        }

#if ENABLE_UXML_TRAITS

        /// <summary>
        /// Defines the UxmlFactory for the <see cref="FloatingActionButton"/>.
        /// </summary>
        public new class UxmlFactory : UxmlFactory<FloatingActionButton, UxmlTraits> { }

        /// <summary>
        /// Class containing the UXML traits for the <see cref="FloatingActionButton"/>.
        /// </summary>
        public new class UxmlTraits : ExVisualElement.UxmlTraits
        {
            readonly UxmlEnumAttributeDescription<Size> m_Size = new UxmlEnumAttributeDescription<Size>
            {
                name = "size",
                defaultValue = Size.M,
            };

            readonly UxmlIntAttributeDescription m_Elevation = new UxmlIntAttributeDescription
            {
                name = "elevation",
                defaultValue = 12,
            };


            readonly UxmlBoolAttributeDescription m_Accent = new UxmlBoolAttributeDescription
            {
                name = "accent",
                defaultValue = false,
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

                var element = (FloatingActionButton)ve;
                element.size = m_Size.GetValueFromBag(bag, cc);
                element.elevation = m_Elevation.GetValueFromBag(bag, cc);
                element.accent = m_Accent.GetValueFromBag(bag, cc);


            }
        }

#endif
    }
}
