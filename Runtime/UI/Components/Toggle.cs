using System;
using UnityEngine.UIElements;
#if ENABLE_RUNTIME_DATA_BINDINGS
using Unity.Properties;
#endif

namespace Unity.AppUI.UI
{
    /// <summary>
    /// Toggle UI element.
    /// </summary>
#if ENABLE_UXML_SERIALIZED_DATA
    [UxmlElement]
#endif
    public partial class Toggle : BaseVisualElement, IInputElement<bool>, IPressable
    {
#if ENABLE_RUNTIME_DATA_BINDINGS

        internal static readonly BindingId valueProperty = nameof(value);

        internal static readonly BindingId labelProperty = nameof(label);

        internal static readonly BindingId invalidProperty = nameof(invalid);

        internal static readonly BindingId validateValueProperty = nameof(validateValue);

#endif

        /// <summary>
        /// The Toggle main styling class.
        /// </summary>
        public const string ussClassName = "appui-toggle";

        /// <summary>
        /// The Toggle size styling class.
        /// </summary>
        public const string sizeUssClassName = ussClassName + "--size-";

        /// <summary>
        /// The Toggle box styling class.
        /// </summary>
        public const string boxUssClassName = ussClassName + "__box";

        /// <summary>
        /// The Toggle box padded styling class.
        /// </summary>
        public const string paddedBoxUssClassName = ussClassName + "__boxpadded";

        /// <summary>
        /// The Toggle checkmark container styling class.
        /// </summary>
        public const string checkmarkContainerUssClassName = ussClassName + "__checkmarkcontainer";

        /// <summary>
        /// The Toggle checkmark styling class.
        /// </summary>
        public const string checkmarkUssClassName = ussClassName + "__checkmark";

        /// <summary>
        /// The Toggle label styling class.
        /// </summary>
        public const string labelUssClassName = ussClassName + "__label";

        readonly LocalizedTextElement m_Label;

        bool m_Value;

        Pressable m_Clickable;

        readonly ExVisualElement m_Box;

        Func<bool, bool> m_ValidateValue;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public Toggle()
        {
            AddToClassList(ussClassName);

            pickingMode = PickingMode.Position;
            focusable = true;
            tabIndex = 0;
            clickable = new Pressable(OnClick);

            var checkmark = new VisualElement
            {
                name = checkmarkUssClassName,
                pickingMode = PickingMode.Ignore,
                usageHints = UsageHints.DynamicTransform,
            };
            checkmark.AddToClassList(checkmarkUssClassName);
            var checkmarkContainer = new VisualElement { name = checkmarkContainerUssClassName, pickingMode = PickingMode.Ignore };
            checkmarkContainer.AddToClassList(checkmarkContainerUssClassName);
            var boxPadded = new VisualElement { name = paddedBoxUssClassName, pickingMode = PickingMode.Ignore };
            boxPadded.AddToClassList(paddedBoxUssClassName);
            m_Box = new ExVisualElement { name = boxUssClassName, pickingMode = PickingMode.Ignore, passMask = 0 };
            m_Box.AddToClassList(boxUssClassName);
            m_Label = new LocalizedTextElement { name = labelUssClassName, pickingMode = PickingMode.Ignore };
            m_Label.AddToClassList(labelUssClassName);

            checkmarkContainer.hierarchy.Add(checkmark);
            boxPadded.hierarchy.Add(checkmarkContainer);
            m_Box.hierarchy.Add(boxPadded);
            hierarchy.Add(m_Box);
            hierarchy.Add(m_Label);

            label = null;
            invalid = false;
            SetValueWithoutNotify(false);

            this.AddManipulator(new KeyboardFocusController(OnKeyboardFocusIn, OnPointerFocusIn));
        }

        /// <summary>
        /// Clickable Manipulator for this Toggle.
        /// </summary>
        public Pressable clickable
        {
            get => m_Clickable;
            set
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
        /// The Toggle label.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public string label
        {
            get => m_Label.text;
            set
            {
                var changed = m_Label.text != value;
                m_Label.text = value;
                m_Label.EnableInClassList(Styles.hiddenUssClassName, string.IsNullOrEmpty(value));

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in labelProperty);
#endif
            }
        }

        /// <summary>
        /// The invalid state of the Toggle.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public bool invalid
        {
            get => ClassListContains(Styles.invalidUssClassName);
            set
            {
                var changed = ClassListContains(Styles.invalidUssClassName) != value;
                EnableInClassList(Styles.invalidUssClassName, value);

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in invalidProperty);
#endif
            }
        }

        /// <summary>
        /// The validation function for the Toggle.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
        public Func<bool, bool> validateValue
        {
            get => m_ValidateValue;
            set
            {
                var changed = m_ValidateValue != value;
                m_ValidateValue = value;
                invalid = !m_ValidateValue?.Invoke(m_Value) ?? false;

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in validateValueProperty);
#endif
            }
        }

        /// <summary>
        /// Set the Toggle value without notifying the change.
        /// </summary>
        /// <param name="newValue"> The new value. </param>
        public void SetValueWithoutNotify(bool newValue)
        {
            m_Value = newValue;
            EnableInClassList(Styles.checkedUssClassName, m_Value);
            if (validateValue != null) invalid = !validateValue(m_Value);
        }

        /// <summary>
        /// The Toggle value.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public bool value
        {
            get => m_Value;
            set
            {
                if (m_Value == value)
                    return;
                using var evt = ChangeEvent<bool>.GetPooled(m_Value, value);
                evt.target = this;
                SetValueWithoutNotify(value);
                SendEvent(evt);

#if ENABLE_RUNTIME_DATA_BINDINGS
                NotifyPropertyChanged(in valueProperty);
#endif
            }
        }

        void OnPointerFocusIn(FocusInEvent evt)
        {
            m_Box.passMask = 0;
        }

        void OnKeyboardFocusIn(FocusInEvent evt)
        {
            m_Box.passMask = ExVisualElement.Passes.Clear | ExVisualElement.Passes.Outline;
        }

        void OnClick()
        {
            value = !value;
        }

#if ENABLE_UXML_TRAITS

        /// <summary>
        /// Factory class to instantiate a <see cref="Toggle"/> using the data read from a UXML file.
        /// </summary>
        public new class UxmlFactory : UxmlFactory<Toggle, UxmlTraits> { }

        /// <summary>
        /// Class containing the <see cref="UxmlTraits"/> for the <see cref="Toggle"/>.
        /// </summary>
        public new class UxmlTraits : BaseVisualElement.UxmlTraits
        {
            readonly UxmlStringAttributeDescription m_Label = new UxmlStringAttributeDescription
            {
                name = "label",
                defaultValue = null
            };

            readonly UxmlBoolAttributeDescription m_Value = new UxmlBoolAttributeDescription
            {
                name = "value",
                defaultValue = false
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

                var element = (Toggle)ve;
                element.value = m_Value.GetValueFromBag(bag, cc);
                element.label = m_Label.GetValueFromBag(bag, cc);


            }
        }

#endif
    }
}
