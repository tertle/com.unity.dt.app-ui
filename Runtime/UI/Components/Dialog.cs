using System;
using Unity.AppUI.Core;
using UnityEngine.UIElements;
#if ENABLE_RUNTIME_DATA_BINDINGS
using Unity.Properties;
#endif

namespace Unity.AppUI.UI
{
    /// <summary>
    /// Base class for Dialogs (<see cref="Dialog"/>, <see cref="AlertDialog"/>, etc).
    /// </summary>
#if ENABLE_UXML_SERIALIZED_DATA
    [UxmlElement]
#endif
    public abstract partial class BaseDialog : BaseVisualElement, ISizeableElement
    {
#if ENABLE_RUNTIME_DATA_BINDINGS

        internal static readonly BindingId titlePropertyKey = new BindingId(nameof(title));

        internal static readonly BindingId descriptionPropertyKey = new BindingId(nameof(description));

        internal static readonly BindingId sizePropertyKey = new BindingId(nameof(size));

#endif

        /// <summary>
        /// The Dialog main styling class.
        /// </summary>
        public const string ussClassName = "appui-dialog";

        /// <summary>
        /// The Dialog variant styling class.
        /// </summary>
        [EnumName("GetVariantUssClassName", typeof(AlertSemantic))]
        public const string variantUssClassName = ussClassName + "--";

        /// <summary>
        /// The Dialog size styling class.
        /// </summary>
        [EnumName("GetSizeUssClassName", typeof(Size))]
        public const string sizeUssClassName = ussClassName + "--size-";

        /// <summary>
        /// The Dialog heading styling class.
        /// </summary>
        public const string headingUssClassName = ussClassName + "__heading";

        /// <summary>
        /// The Dialog header styling class.
        /// </summary>
        public const string headerUssClassName = ussClassName + "__header";

        /// <summary>
        /// The Dialog divider styling class.
        /// </summary>
        public const string dividerUssClassName = ussClassName + "__divider";

        /// <summary>
        /// The Dialog content styling class.
        /// </summary>
        public const string contentUssClassName = ussClassName + "__content";

        /// <summary>
        /// The Dialog button group styling class.
        /// </summary>
        public const string buttonGroupUssClassName = ussClassName + "__buttongroup";

        /// <summary>
        /// The container for the Dialog actions (buttons).
        /// </summary>
        protected readonly VisualElement m_ActionContainer;

        /// <summary>
        /// The Dialog content.
        /// </summary>
        protected readonly LocalizedTextElement m_Content;

        /// <summary>
        /// The Dialog header divider.
        /// </summary>
        protected readonly Divider m_Divider;

        /// <summary>
        /// The Dialog header.
        /// </summary>
        protected readonly Heading m_Header;

        /// <summary>
        /// The Dialog heading.
        /// </summary>
        protected readonly VisualElement m_Heading;

        Size m_Size;

        /// <summary>
        /// Default constructor.
        /// </summary>
        protected BaseDialog()
        {
            AddToClassList(ussClassName);

            m_Heading = new VisualElement { name = headingUssClassName };
            m_Heading.AddToClassList(headingUssClassName);
            m_Header = new Heading { name = headerUssClassName };
            m_Header.AddToClassList(headerUssClassName);
            m_Divider = new Divider { name = dividerUssClassName };
            m_Divider.AddToClassList(dividerUssClassName);
            m_Content = new LocalizedTextElement { name = contentUssClassName };
            m_Content.AddToClassList(contentUssClassName);
            m_ActionContainer = new VisualElement { name = buttonGroupUssClassName };
            m_ActionContainer.AddToClassList(buttonGroupUssClassName);

            m_Heading.hierarchy.Add(m_Header);

            hierarchy.Add(m_Heading);
            hierarchy.Add(m_Divider);
            hierarchy.Add(m_Content);
            hierarchy.Add(m_ActionContainer);

            size = Size.M;
            title = null;
        }

        /// <summary>
        /// The Dialog content container.
        /// </summary>
        public override VisualElement contentContainer => m_Content;

        /// <summary>
        /// The Dialog action container.
        /// </summary>
        public VisualElement actionContainer => m_ActionContainer;

        /// <summary>
        /// The Dialog title.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public string title
        {
            get => m_Header.text;
            set
            {
                var changed = m_Header.text != value;
                m_Header.text = value;
                RefreshHeading();

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in titlePropertyKey);
#endif
            }
        }

        /// <summary>
        /// Check if the heading should be hidden. Override this method to change the default behavior.
        /// By default, the heading is hidden if the title is null or empty.
        /// </summary>
        /// <returns> True if the heading should be hidden, false otherwise.</returns>
        protected virtual bool ShouldHideHeading() => string.IsNullOrEmpty(title);

        /// <summary>
        /// Refresh the heading visibility.
        /// </summary>
        protected void RefreshHeading()
        {
            var hidden = ShouldHideHeading();
            m_Heading.EnableInClassList(Styles.hiddenUssClassName, hidden);
            m_Divider.EnableInClassList(Styles.hiddenUssClassName, hidden);
        }

        /// <summary>
        /// The Dialog description. This is the text displayed in the content container.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public string description
        {
            get => m_Content.text;
            set
            {
                var changed = m_Content.text != value;
                m_Content.text = value;

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in descriptionPropertyKey);
#endif
            }
        }

        /// <summary>
        /// The Dialog size.
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
                    NotifyPropertyChanged(in sizePropertyKey);
#endif
            }
        }

#if ENABLE_UXML_TRAITS

        /// <summary>
        /// Class containing the <see cref="UxmlTraits"/> for the <see cref="BaseDialog"/>.
        /// </summary>
        public new class UxmlTraits : BaseVisualElement.UxmlTraits
        {
            readonly UxmlEnumAttributeDescription<Size> m_Size = new UxmlEnumAttributeDescription<Size>
            {
                name = "size",
                defaultValue = Size.M,
            };

            readonly UxmlStringAttributeDescription m_Title = new UxmlStringAttributeDescription
            {
                name = "title",
                defaultValue = ""
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

                var element = (BaseDialog)ve;
                element.size = m_Size.GetValueFromBag(bag, cc);
                element.title = m_Title.GetValueFromBag(bag, cc);
            }
        }

#endif
    }

    /// <summary>
    /// Dialog UI element.
    /// </summary>
#if ENABLE_UXML_SERIALIZED_DATA
    [UxmlElement]
#endif
    public partial class Dialog : BaseDialog, IDismissInvocator
    {
#if ENABLE_RUNTIME_DATA_BINDINGS

        internal static readonly BindingId dismissablePropertyKey = new BindingId(nameof(dismissable));

#endif

        /// <summary>
        /// The Dialog close button styling class.
        /// </summary>
        public const string closeButtonUssClassName = ussClassName + "__closebutton";

        /// <summary>
        /// The Dialog dismissable mode styling class.
        /// </summary>
        public const string dismissableUssClassName = ussClassName + "--dismissable";

        /// <summary>
        /// Default constructor.
        /// </summary>
        public Dialog()
        {
            closeButton = new Button(OnCloseButtonClicked) { name = closeButtonUssClassName, leadingIcon = "x" };
            closeButton.AddToClassList(closeButtonUssClassName);
            m_Heading.hierarchy.Add(closeButton);

            dismissable = false;
        }

        /// <summary>
        /// The close button.
        /// </summary>
        /// <remarks>
        /// The button is only visible if <see cref="dismissable"/> is `True`.
        /// </remarks>
        public Button closeButton { get; }

        /// <summary>
        /// Set the <see cref="Dialog"/> dismissable by itself using a <see cref="closeButton"/>.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public bool dismissable
        {
            get => ClassListContains(dismissableUssClassName);
            set
            {
                var changed = ClassListContains(dismissableUssClassName) != value;
                EnableInClassList(dismissableUssClassName, value);
                RefreshHeading();

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in dismissablePropertyKey);
#endif
            }
        }

        /// <inheritdoc cref="BaseDialog.ShouldHideHeading"/>
        protected override bool ShouldHideHeading() => string.IsNullOrEmpty(title) && !dismissable;

        /// <summary>
        /// Event fired when the <see cref="Dialog"/> is dismissed.
        /// </summary>
        public event Action<DismissType> dismissRequested;

        void OnCloseButtonClicked()
        {
            if (dismissable)
                dismissRequested?.Invoke(DismissType.Manual);
        }

#if ENABLE_UXML_TRAITS

        /// <summary>
        /// Factory class to instantiate a <see cref="Dialog"/> using the data read from a UXML file.
        /// </summary>
        public new class UxmlFactory : UxmlFactory<Dialog, UxmlTraits> { }

        /// <summary>
        /// Class containing the <see cref="UxmlTraits"/> for the <see cref="Dialog"/>.
        /// </summary>
        public new class UxmlTraits : BaseDialog.UxmlTraits
        {
            readonly UxmlBoolAttributeDescription m_Dismissable = new UxmlBoolAttributeDescription
            {
                name = "dismissable",
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

                var element = (Dialog)ve;
                element.dismissable = m_Dismissable.GetValueFromBag(bag, cc);
            }
        }
#endif
    }
}
