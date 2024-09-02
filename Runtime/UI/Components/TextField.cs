using System;
using Unity.AppUI.Bridge;
using UnityEngine.UIElements;
#if ENABLE_RUNTIME_DATA_BINDINGS
using Unity.Properties;
#endif

namespace Unity.AppUI.UI
{
    /// <summary>
    /// Text Field UI element.
    /// </summary>
#if ENABLE_UXML_SERIALIZED_DATA
    [UxmlElement]
#endif
    public partial class TextField : ExVisualElement, IInputElement<string>, INotifyValueChanging<string>
    {
#if ENABLE_RUNTIME_DATA_BINDINGS

        internal static readonly BindingId valueProperty = nameof(value);

        internal static readonly BindingId isReadOnlyProperty = nameof(isReadOnly);

        internal static readonly BindingId maxLengthProperty = nameof(maxLength);

        internal static readonly BindingId placeholderProperty = nameof(placeholder);

        internal static readonly BindingId validateValueProperty = nameof(validateValue);

        internal static readonly BindingId invalidProperty = nameof(invalid);

        internal static readonly BindingId isPasswordProperty = nameof(isPassword);

        internal static readonly BindingId maskCharProperty = nameof(maskChar);

        internal static readonly BindingId sizeProperty = nameof(size);

        internal static readonly BindingId leadingIconNameProperty = nameof(leadingIconName);

        internal static readonly BindingId trailingIconNameProperty = nameof(trailingIconName);

#endif

        /// <summary>
        /// The TextField main styling class.
        /// </summary>
        public const string ussClassName = "appui-textfield";

        /// <summary>
        /// The TextField size styling class.
        /// </summary>
        [EnumName("GetSizeUssClassName", typeof(Size))]
        public const string sizeUssClassName = ussClassName + "--size-";

        /// <summary>
        /// The TextField leading container styling class.
        /// </summary>
        public const string leadingContainerUssClassName = ussClassName + "__leadingcontainer";

        /// <summary>
        /// The TextField leading icon styling class.
        /// </summary>
        public const string leadingIconUssClassName = ussClassName + "__leadingicon";

        /// <summary>
        /// The TextField input container styling class.
        /// </summary>
        public const string inputContainerUssClassName = ussClassName + "__inputcontainer";

        /// <summary>
        /// The TextField input styling class.
        /// </summary>
        public const string inputUssClassName = ussClassName + "__input";

        /// <summary>
        /// The TextField placeholder styling class.
        /// </summary>
        public const string placeholderUssClassName = ussClassName + "__placeholder";

        /// <summary>
        /// The TextField trailing container styling class.
        /// </summary>
        public const string trailingContainerUssClassName = ussClassName + "__trailingcontainer";

        /// <summary>
        /// The TextField trailing icon styling class.
        /// </summary>
        public const string trailingIconUssClassName = ussClassName + "__trailingicon";

        const bool k_IsPasswordDefault = false;

        const bool k_IsReadOnlyDefault = false;

        const char k_MaskCharDefault = '*';

        const int k_MaxLengthDefault = -1;

        readonly UnityEngine.UIElements.TextField m_InputField;

        readonly VisualElement m_LeadingContainer;

        readonly LocalizedTextElement m_Placeholder;

        Size m_Size;

        readonly VisualElement m_TrailingContainer;

        string m_Value;

        string m_PreviousValue;

        VisualElement m_LeadingElement;

        VisualElement m_TrailingElement;

        Func<string, bool> m_ValidateValue;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public TextField()
            : this(null) { }

        /// <summary>
        /// Construct a TextField with a predefined text value.
        /// </summary>
        /// <param name="value">A default text value.</param>
        /// <remarks>
        /// No event will be triggered when setting the text value during construction.
        /// </remarks>
        public TextField(string value)
        {
            AddToClassList(ussClassName);

            focusable = true;
            pickingMode = PickingMode.Position;
            passMask = 0;
            tabIndex = 0;
            this.SetIsCompositeRoot(true);
            this.SetExcludeFromFocusRing(true);
            delegatesFocus = true;

            m_LeadingContainer = new VisualElement { name = leadingContainerUssClassName, pickingMode = PickingMode.Ignore };
            m_LeadingContainer.AddToClassList(leadingContainerUssClassName);
            hierarchy.Add(m_LeadingContainer);

            var leadingIcon = new Icon { name = leadingIconUssClassName, iconName = null, pickingMode = PickingMode.Ignore };
            leadingIcon.AddToClassList(leadingIconUssClassName);
            m_LeadingContainer.hierarchy.Add(leadingIcon);

            var inputContainer = new VisualElement { name = inputContainerUssClassName, pickingMode = PickingMode.Ignore };
            inputContainer.AddToClassList(inputContainerUssClassName);
            hierarchy.Add(inputContainer);

            m_Placeholder = new LocalizedTextElement { name = placeholderUssClassName, pickingMode = PickingMode.Ignore, focusable = false };
            m_Placeholder.AddToClassList(placeholderUssClassName);
            inputContainer.hierarchy.Add(m_Placeholder);

            m_InputField = new UnityEngine.UIElements.TextField { name = inputUssClassName };
            m_InputField.AddToClassList(inputUssClassName);
            m_InputField.AddManipulator(new BlinkingCursor());
            inputContainer.hierarchy.Add(m_InputField);

            m_TrailingContainer = new VisualElement { name = trailingContainerUssClassName, pickingMode = PickingMode.Ignore };
            m_TrailingContainer.AddToClassList(trailingContainerUssClassName);
            hierarchy.Add(m_TrailingContainer);

            var trailingIcon = new Icon { name = trailingIconUssClassName, iconName = null, pickingMode = PickingMode.Ignore };
            trailingIcon.AddToClassList(trailingIconUssClassName);
            m_TrailingContainer.hierarchy.Add(trailingIcon);

            SetValueWithoutNotify(value);
            leadingElement = leadingIcon;
            trailingElement = trailingIcon;
            leadingIconName = null;
            trailingIconName = null;
            size = Size.M;
            isPassword = k_IsPasswordDefault;
            isReadOnly = k_IsReadOnlyDefault;
            maskChar = k_MaskCharDefault;
            maxLength = k_MaxLengthDefault;

            m_InputField.AddManipulator(new KeyboardFocusController(OnKeyboardFocusedIn, OnFocusedIn, OnFocusedOut));
            m_Placeholder.RegisterValueChangedCallback(OnPlaceholderValueChanged);
            m_InputField.RegisterValueChangedCallback(OnInputValueChanged);
        }

        void OnInputValueChanged(ChangeEvent<string> e)
        {
            e.StopPropagation();

            using var evt = ChangingEvent<string>.GetPooled();
            evt.target = this;
            evt.previousValue = m_Value;
            m_Value = e.newValue;
            evt.newValue = m_Value;

            if (validateValue != null) invalid = !validateValue(m_Value);
            RefreshUI();
            SendEvent(evt);
        }

        void OnPlaceholderValueChanged(ChangeEvent<string> evt)
        {
            evt.StopPropagation();
        }

        /// <summary>
        /// The content container of the TextField.
        /// </summary>
        public override VisualElement contentContainer => null;

        /// <summary>
        /// The TextField leading element.
        /// </summary>
        public VisualElement leadingElement
        {
            get => m_LeadingElement;
            set
            {
                if (m_LeadingElement == value)
                    return;

                if (m_LeadingElement != null)
                    m_LeadingContainer.Remove(m_LeadingElement);

                m_LeadingElement = value;

                if (m_LeadingElement != null)
                    m_LeadingContainer.Add(m_LeadingElement);
            }
        }

        /// <summary>
        /// The TextField trailing element.
        /// </summary>
        public VisualElement trailingElement
        {
            get => m_TrailingElement;
            set
            {
                if (m_TrailingElement == value)
                    return;

                if (m_TrailingElement != null)
                    m_TrailingContainer.Remove(m_TrailingElement);

                m_TrailingElement = value;

                if (m_TrailingElement != null)
                    m_TrailingContainer.Add(m_TrailingElement);
            }
        }

        /// <summary>
        /// Whether the TextField is a password field.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public bool isPassword
        {
            get => m_InputField.isPasswordField;
            set
            {
                var changed = m_InputField.isPasswordField != value;
                m_InputField.isPasswordField = value;

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in isPasswordProperty);
#endif
            }
        }

        /// <summary>
        /// Whether the TextField is read-only.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public bool isReadOnly
        {
            get => m_InputField.isReadOnly;
            set
            {
                var changed = m_InputField.isReadOnly != value;
                m_InputField.isReadOnly = value;

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in isReadOnlyProperty);
#endif
            }
        }

        /// <summary>
        /// The TextField mask character.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public char maskChar
        {
            get => m_InputField.maskChar;
            set
            {
                var changed = m_InputField.maskChar != value;
                m_InputField.maskChar = value;

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in maskCharProperty);
#endif
            }
        }

        /// <summary>
        /// The TextField max length.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public int maxLength
        {
            get => m_InputField.maxLength;
            set
            {
                var changed = m_InputField.maxLength != value;
                m_InputField.maxLength = value;

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in maxLengthProperty);
#endif
            }
        }

        /// <summary>
        /// The TextField placeholder text.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public string placeholder
        {
            get => m_Placeholder.text;
            set
            {
                var changed = m_Placeholder.text != value;
                m_Placeholder.text = value;

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in placeholderProperty);
#endif
            }
        }

        /// <summary>
        /// The trailing icon name.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public string trailingIconName
        {
            get => (trailingElement as Icon)?.iconName;
            set
            {
                if (trailingElement is not Icon icon)
                    return;

                var changed = icon.iconName != value;
                icon.iconName = value;
                m_TrailingContainer.EnableInClassList(Styles.hiddenUssClassName, string.IsNullOrEmpty(icon.iconName));

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in trailingIconNameProperty);
#endif
            }
        }

        /// <summary>
        /// The leading icon name.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public string leadingIconName
        {
            get => (leadingElement as Icon)?.iconName;
            set
            {
                if (leadingElement is not Icon icon)
                    return;

                var changed = icon.iconName != value;
                icon.iconName = value;
                m_LeadingContainer.EnableInClassList(Styles.hiddenUssClassName, string.IsNullOrEmpty(icon.iconName));

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in leadingIconNameProperty);
#endif
            }
        }

        /// <summary>
        /// The TextField size.
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

                switch (leadingElement)
                {
                    case ISizeableElement leadingSizeable:
                        leadingSizeable.size = m_Size;
                        break;
                    case Icon leadingIcon:
                        leadingIcon.size = m_Size switch
                        {
                            Size.S => IconSize.S,
                            Size.M => IconSize.S,
                            Size.L => IconSize.M,
                            _ => IconSize.S
                        };
                        break;
                }

                switch (trailingElement)
                {
                    case ISizeableElement trailingSizeable:
                        trailingSizeable.size = m_Size;
                        break;
                    case Icon trailingIcon:
                        trailingIcon.size = m_Size switch
                        {
                            Size.S => IconSize.S,
                            Size.M => IconSize.S,
                            Size.L => IconSize.M,
                            _ => IconSize.S
                        };
                        break;
                }

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in sizeProperty);
#endif
            }
        }

        /// <summary>
        /// The validation function for the TextField.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
        public Func<string, bool> validateValue
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
        /// The invalid state of the TextField.
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
        /// Set the TextField value without notifying the change.
        /// </summary>
        /// <param name="newValue"> The new value of the TextField. </param>
        public void SetValueWithoutNotify(string newValue)
        {
            m_Value = newValue;
            m_InputField.SetValueWithoutNotify(m_Value);
            RefreshUI();
            if (validateValue != null) invalid = !validateValue(m_Value);
        }

        /// <summary>
        /// The TextField value.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public string value
        {
            get => m_InputField.value;
            set
            {
                if (m_Value == value && m_PreviousValue == value)
                {
                    RefreshUI();
                    return;
                }

                using var evt = ChangeEvent<string>.GetPooled(m_PreviousValue, value);
                m_PreviousValue = m_Value;
                evt.target = this;
                SetValueWithoutNotify(value);
                SendEvent(evt);

#if ENABLE_RUNTIME_DATA_BINDINGS
                NotifyPropertyChanged(in valueProperty);
#endif
            }
        }

        void OnFocusedOut(FocusOutEvent e)
        {
            RemoveFromClassList(Styles.focusedUssClassName);
            RemoveFromClassList(Styles.keyboardFocusUssClassName);
            value = m_InputField.value;
#if UNITY_2022_1_OR_NEWER
            m_InputField.cursorIndex = 0;
#endif
        }

        void OnFocusedIn(FocusInEvent evt)
        {
            AddToClassList(Styles.focusedUssClassName);
            passMask = 0;
            m_PreviousValue = m_Value;
        }

        void OnKeyboardFocusedIn(FocusInEvent evt)
        {
            AddToClassList(Styles.focusedUssClassName);
            AddToClassList(Styles.keyboardFocusUssClassName);
            passMask = Passes.Clear | Passes.Outline;
            m_PreviousValue = m_Value;
        }

        void RefreshUI()
        {
            m_Placeholder.EnableInClassList(Styles.hiddenUssClassName, !string.IsNullOrEmpty(m_Value));
        }

#if ENABLE_UXML_TRAITS

        /// <summary>
        /// Factory class to instantiate a <see cref="TextField"/> using the data read from a UXML file.
        /// </summary>
        public new class UxmlFactory : UxmlFactory<TextField, UxmlTraits> { }

        /// <summary>
        /// Class containing the <see cref="UxmlTraits"/> for the <see cref="TextField"/>.
        /// </summary>
        public new class UxmlTraits : ExVisualElement.UxmlTraits
        {
            readonly UxmlStringAttributeDescription m_LeadingIconName = new UxmlStringAttributeDescription
            {
                name = "leading-icon-name",
                defaultValue = null
            };

            readonly UxmlStringAttributeDescription m_Placeholder = new UxmlStringAttributeDescription
            {
                name = "placeholder",
                defaultValue = null
            };

            readonly UxmlEnumAttributeDescription<Size> m_Size = new UxmlEnumAttributeDescription<Size>
            {
                name = "size",
                defaultValue = Size.M,
            };

            readonly UxmlStringAttributeDescription m_TrailingIconName = new UxmlStringAttributeDescription
            {
                name = "trailing-icon-name",
                defaultValue = null
            };

            readonly UxmlStringAttributeDescription m_Value = new UxmlStringAttributeDescription
            {
                name = "value",
                defaultValue = null
            };

            readonly UxmlBoolAttributeDescription m_IsPassword = new UxmlBoolAttributeDescription
            {
                name = "is-password",
                defaultValue = k_IsPasswordDefault
            };

            readonly UxmlBoolAttributeDescription m_IsReadOnly = new UxmlBoolAttributeDescription
            {
                name = "is-read-only",
                defaultValue = k_IsReadOnlyDefault
            };

            readonly UxmlStringAttributeDescription m_MaskChar = new UxmlStringAttributeDescription
            {
                name = "mask-char",
                defaultValue = k_MaskCharDefault.ToString()
            };

            readonly UxmlIntAttributeDescription m_MaxLength = new UxmlIntAttributeDescription
            {
                name = "max-length",
                defaultValue = k_MaxLengthDefault
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

                var el = (TextField)ve;

                var size = Size.M;
                if (m_Size.TryGetValueFromBag(bag, cc, ref size))
                    el.size = size;

                var placeholder = string.Empty;
                if (m_Placeholder.TryGetValueFromBag(bag, cc, ref placeholder))
                    el.placeholder = placeholder;

                var value = string.Empty;
                if (m_Value.TryGetValueFromBag(bag, cc, ref value))
                    el.value = value;

                var leadingIconName = string.Empty;
                if (m_LeadingIconName.TryGetValueFromBag(bag, cc, ref leadingIconName))
                    el.leadingIconName = leadingIconName;

                var trailingIconName = string.Empty;
                if (m_TrailingIconName.TryGetValueFromBag(bag, cc, ref trailingIconName))
                    el.trailingIconName = trailingIconName;

                var isPassword = k_IsPasswordDefault;
                if (m_IsPassword.TryGetValueFromBag(bag, cc, ref isPassword))
                    el.isPassword = isPassword;

                var isReadOnly = k_IsReadOnlyDefault;
                if (m_IsReadOnly.TryGetValueFromBag(bag, cc, ref isReadOnly))
                    el.isReadOnly = isReadOnly;

                var maskChar = k_MaskCharDefault.ToString();
                if (m_MaskChar.TryGetValueFromBag(bag, cc, ref maskChar))
                    el.maskChar = string.IsNullOrEmpty(maskChar) ? k_MaskCharDefault : maskChar[0];

                var maxLength = k_MaxLengthDefault;
                if (m_MaxLength.TryGetValueFromBag(bag, cc, ref maxLength))
                    el.maxLength = maxLength;


            }
        }

#endif
    }
}
