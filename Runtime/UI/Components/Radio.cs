using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;
#if ENABLE_RUNTIME_DATA_BINDINGS
using Unity.Properties;
#endif

namespace Unity.AppUI.UI
{
    /// <summary>
    /// Radio UI element.
    /// </summary>
#if ENABLE_UXML_SERIALIZED_DATA
    [UxmlElement]
#endif
    public partial class Radio : BaseVisualElement, IValidatableElement<bool>, IPressable
    {
#if ENABLE_RUNTIME_DATA_BINDINGS

        internal static readonly BindingId sizeProperty = nameof(size);

        internal static readonly BindingId emphasizedProperty = nameof(emphasized);

        internal static readonly BindingId labelProperty = nameof(label);

        internal static readonly BindingId invalidProperty = nameof(invalid);

        internal static readonly BindingId validateValueProperty = nameof(validateValue);

        internal static readonly BindingId valueProperty = nameof(value);

        internal static readonly BindingId keyProperty = nameof(key);

#endif

        /// <summary>
        /// The Radio main styling class.
        /// </summary>
        public const string ussClassName = "appui-radio";

        /// <summary>
        /// The Radio size styling class.
        /// </summary>
        [EnumName("GetSizeUssClassName", typeof(Size))]
        public const string sizeUssClassName = ussClassName + "--size-";

        /// <summary>
        /// The Radio emphasized mode styling class.
        /// </summary>
        public const string emphasizedUssClassName = ussClassName + "--emphasized";

        /// <summary>
        /// The Radio button styling class.
        /// </summary>
        public const string boxUssClassName = ussClassName + "__button";

        /// <summary>
        /// The Radio checkmark styling class.
        /// </summary>
        public const string checkmarkUssClassName = ussClassName + "__checkmark";

        /// <summary>
        /// The Radio label styling class.
        /// </summary>
        public const string labelUssClassName = ussClassName + "__label";

        readonly LocalizedTextElement m_Label;

        Size m_Size;

        bool m_Value;

        Pressable m_Clickable;

        readonly ExVisualElement m_Box;

        Func<bool, bool> m_ValidateValue;

        string m_Key;

        RadioGroup m_Group;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public Radio()
        {
            AddToClassList(ussClassName);

            clickable = new Pressable(OnClick);
            focusable = true;
            pickingMode = PickingMode.Position;
            tabIndex = 0;

            var radioIcon = new VisualElement { name = checkmarkUssClassName, pickingMode = PickingMode.Ignore };
            radioIcon.AddToClassList(checkmarkUssClassName);
            m_Box = new ExVisualElement { name = boxUssClassName, pickingMode = PickingMode.Ignore, passMask = 0 };
            m_Box.AddToClassList(boxUssClassName);
            m_Label = new LocalizedTextElement { name = labelUssClassName, pickingMode = PickingMode.Ignore };
            m_Label.AddToClassList(labelUssClassName);

            m_Box.hierarchy.Add(radioIcon);
            hierarchy.Add(m_Box);
            hierarchy.Add(m_Label);

            size = Size.M;
            emphasized = false;
            invalid = false;
            key = null;
            SetValueWithoutNotify(false);

            this.AddManipulator(new KeyboardFocusController(OnKeyboardFocusIn, OnPointerFocusIn));

            RegisterCallback<AttachToPanelEvent>(OnAttachToPanel);
            RegisterCallback<DetachFromPanelEvent>(OnDetachFromPanel);
        }

        /// <summary>
        /// Clickable Manipulator for this Radio.
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
        /// The Radio key.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public string key
        {
            get => m_Key;
            set
            {
                var changed = m_Key != value;
                m_Key = value;
                TryAddToGroup();

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in keyProperty);
#endif
            }
        }

        /// <summary>
        /// The Radio size.
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
        /// The Radio emphasized mode.
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
        /// The Radio label.
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

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in labelProperty);
#endif
            }
        }

        /// <summary>
        /// The Radio invalid state.
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
        /// The Radio validation function.
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
        /// Sets the Radio value without notifying the listeners.
        /// </summary>
        /// <param name="newValue"> The new value. </param>
        public void SetValueWithoutNotify(bool newValue)
        {
            m_Value = newValue;
            EnableInClassList(Styles.checkedUssClassName, m_Value);
            if (validateValue != null) invalid = !validateValue(m_Value);
        }

        /// <summary>
        /// The Radio value.
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

        void OnClick()
        {
            value = true;
        }

        void OnPointerFocusIn(FocusInEvent evt)
        {
            m_Box.passMask = 0;
        }

        void OnKeyboardFocusIn(FocusInEvent evt)
        {
            m_Box.passMask = ExVisualElement.Passes.Clear | ExVisualElement.Passes.Outline;
        }

        void OnAttachToPanel(AttachToPanelEvent evt)
        {
            TryAddToGroup();
        }

        void TryAddToGroup()
        {
            m_Group?.RemoveRadio(this);
            var group = GetFirstAncestorOfType<RadioGroup>();
            if (group != null && m_Group != group)
            {
                m_Group = group;
                m_Group.AddRadio(this);
            }
        }

        void OnDetachFromPanel(DetachFromPanelEvent evt)
        {
            m_Group?.RemoveRadio(this);
        }

#if ENABLE_UXML_TRAITS

        /// <summary>
        /// Factory class to instantiate a <see cref="Radio"/> using the data read from a UXML file.
        /// </summary>
        public new class UxmlFactory : UxmlFactory<Radio, UxmlTraits> { }

        /// <summary>
        /// Class containing the <see cref="UxmlTraits"/> for the <see cref="Radio"/>.
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

            readonly UxmlEnumAttributeDescription<Size> m_Size = new UxmlEnumAttributeDescription<Size>
            {
                name = "size",
                defaultValue = Size.M,
            };

            readonly UxmlBoolAttributeDescription m_Value = new UxmlBoolAttributeDescription
            {
                name = "value",
                defaultValue = false
            };

            readonly UxmlStringAttributeDescription m_Key = new UxmlStringAttributeDescription
            {
                name = "key",
                defaultValue = null
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

                var element = (Radio)ve;
                element.size = m_Size.GetValueFromBag(bag, cc);
                element.emphasized = m_Emphasized.GetValueFromBag(bag, cc);
                element.value = m_Value.GetValueFromBag(bag, cc);
                element.label = m_Label.GetValueFromBag(bag, cc);
                element.key = m_Key.GetValueFromBag(bag, cc);
            }
        }

#endif
    }
}
