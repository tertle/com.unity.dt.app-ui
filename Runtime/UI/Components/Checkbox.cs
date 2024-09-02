using System;
using UnityEngine.UIElements;
#if ENABLE_RUNTIME_DATA_BINDINGS
using Unity.Properties;
#endif

namespace Unity.AppUI.UI
{
    /// <summary>
    /// The possible states for a <see cref="Checkbox"/>.
    /// </summary>
    public enum CheckboxState
    {
        /// <summary>
        /// The <see cref="Checkbox"/> is completely unchecked.
        /// </summary>
        Unchecked,

        /// <summary>
        /// The <see cref="Checkbox"/> is unchecked but at least one of its dependencies is checked.
        /// </summary>
        Intermediate,

        /// <summary>
        /// The <see cref="Checkbox"/> is checked.
        /// </summary>
        Checked
    }

    /// <summary>
    /// Checkbox UI element.
    /// </summary>
#if ENABLE_UXML_SERIALIZED_DATA
    [UxmlElement]
#endif
    public partial class Checkbox : BaseVisualElement, IInputElement<CheckboxState>, IPressable
    {
#if ENABLE_RUNTIME_DATA_BINDINGS

        internal static readonly BindingId valueProperty = nameof(value);

        internal static readonly BindingId emphasizedProperty = nameof(emphasized);

        internal static readonly BindingId labelProperty = nameof(label);

        internal static readonly BindingId invalidProperty = nameof(invalid);

        internal static readonly BindingId validateValueProperty = nameof(validateValue);
#endif

        /// <summary>
        /// The Checkbox main styling class.
        /// </summary>
        public const string ussClassName = "appui-checkbox";

        /// <summary>
        /// The Checkbox size styling class.
        /// </summary>
        public const string sizeUssClassName = ussClassName + "--size-";

        /// <summary>
        /// The Checkbox emphasized mode styling class.
        /// </summary>
        public const string emphasizedUssClassName = ussClassName + "--emphasized";

        /// <summary>
        /// The Checkbox box styling class.
        /// </summary>
        public const string boxUssClassName = ussClassName + "__box";

        /// <summary>
        /// The Checkbox checkmark styling class.
        /// </summary>
        public const string checkmarkUssClassName = ussClassName + "__checkmark";

        /// <summary>
        /// The Checkbox partial checkmark styling class.
        /// </summary>
        public const string partialCheckmarkUssClassName = ussClassName + "__partialcheckmark";

        /// <summary>
        /// The Checkbox label styling class.
        /// </summary>
        public const string labelUssClassName = ussClassName + "__label";

        readonly LocalizedTextElement m_Label;

        CheckboxState m_Value;

        Pressable m_Clickable;

        readonly ExVisualElement m_Box;

        Func<CheckboxState, bool> m_ValidateValue;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public Checkbox()
        {
            AddToClassList(ussClassName);

            clickable = new Pressable(OnClicked);
            pickingMode = PickingMode.Position;
            focusable = true;
            tabIndex = 0;

            var checkmark = new Icon { name = checkmarkUssClassName, iconName = "check", variant = IconVariant.Bold, pickingMode = PickingMode.Ignore };
            checkmark.AddToClassList(checkmarkUssClassName);
            var partialCheckmark = new Icon { name = partialCheckmarkUssClassName, iconName = "minus", variant = IconVariant.Bold, pickingMode = PickingMode.Ignore };
            partialCheckmark.AddToClassList(partialCheckmarkUssClassName);
            m_Box = new ExVisualElement { name = boxUssClassName, pickingMode = PickingMode.Ignore, passMask = 0 };
            m_Box.AddToClassList(boxUssClassName);
            m_Label = new LocalizedTextElement { name = labelUssClassName, pickingMode = PickingMode.Ignore };
            m_Label.AddToClassList(labelUssClassName);

            m_Box.hierarchy.Add(checkmark);
            m_Box.hierarchy.Add(partialCheckmark);
            hierarchy.Add(m_Box);
            hierarchy.Add(m_Label);

            emphasized = false;
            invalid = false;
            label = null;
            SetValueWithoutNotify(CheckboxState.Unchecked);

            this.AddManipulator(new KeyboardFocusController(OnKeyboardFocus, OnPointerFocus));
        }

        /// <summary>
        /// Clickable Manipulator for this Checkbox.
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
        /// The Checkbox emphasized mode.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public bool emphasized
        {
            get => ClassListContains(emphasizedUssClassName);
            set
            {
                var changed = emphasized != value;
                EnableInClassList(emphasizedUssClassName, value);

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in emphasizedProperty);
#endif
            }
        }

        /// <summary>
        /// The text displayed in the Checkbox label.
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
        /// The Checkbox invalid state.
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
                var changed = invalid != value;
                EnableInClassList(Styles.invalidUssClassName, value);

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in invalidProperty);
#endif
            }
        }

        /// <summary>
        /// The Checkbox validation function.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
        public Func<CheckboxState, bool> validateValue
        {
            get => m_ValidateValue;
            set
            {
                var changed = m_ValidateValue != value;
                m_ValidateValue = value;

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in validateValueProperty);
#endif
            }
        }

        /// <summary>
        /// Set the Checkbox value without notifying the change.
        /// </summary>
        /// <param name="newValue"> The new Checkbox value. </param>
        public void SetValueWithoutNotify(CheckboxState newValue)
        {
            m_Value = newValue;
            EnableInClassList(Styles.checkedUssClassName, m_Value == CheckboxState.Checked);
            EnableInClassList(Styles.intermediateUssClassName, m_Value == CheckboxState.Intermediate);
            if (validateValue != null) invalid = !validateValue(m_Value);
        }

        /// <summary>
        /// The Checkbox value.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public CheckboxState value
        {
            get => m_Value;
            set
            {
                if (m_Value == value)
                    return;
                using var evt = ChangeEvent<CheckboxState>.GetPooled(m_Value, value);
                evt.target = this;
                SetValueWithoutNotify(value);
                SendEvent(evt);

#if ENABLE_RUNTIME_DATA_BINDINGS
                NotifyPropertyChanged(in valueProperty);
#endif
            }
        }

        void OnClicked()
        {
            // when a user click on the UI element, it can't go into an intermediate state
            // intermediate state can be set programatically.
            value = value == CheckboxState.Checked ? CheckboxState.Unchecked : CheckboxState.Checked;
        }

        void OnPointerFocus(FocusInEvent evt)
        {
            m_Box.passMask = 0;
        }

        void OnKeyboardFocus(FocusInEvent evt)
        {
            m_Box.passMask = ExVisualElement.Passes.Clear | ExVisualElement.Passes.Outline;
        }

#if ENABLE_UXML_TRAITS

        /// <summary>
        /// UXML factory for the Checkbox.
        /// </summary>
        public new class UxmlFactory : UxmlFactory<Checkbox, UxmlTraits> { }

        /// <summary>
        /// Class containing the <see cref="UxmlTraits"/> for the <see cref="Checkbox"/>.
        /// </summary>
        public new class UxmlTraits : BaseVisualElement.UxmlTraits
        {
            readonly UxmlBoolAttributeDescription m_Emphasized = new UxmlBoolAttributeDescription
            {
                name = "emphasized",
                defaultValue = false
            };

            readonly UxmlStringAttributeDescription m_Label = new UxmlStringAttributeDescription
            {
                name = "label",
                defaultValue = null
            };

            readonly UxmlEnumAttributeDescription<CheckboxState> m_Value = new UxmlEnumAttributeDescription<CheckboxState>
            {
                name = "value",
                defaultValue = CheckboxState.Unchecked
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

                var element = (Checkbox)ve;
                element.emphasized = m_Emphasized.GetValueFromBag(bag, cc);
                element.value = m_Value.GetValueFromBag(bag, cc);
                element.label = m_Label.GetValueFromBag(bag, cc);

            }
        }
#endif
    }
}
