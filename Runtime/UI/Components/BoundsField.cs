using System;
using UnityEngine;
using UnityEngine.UIElements;
#if ENABLE_RUNTIME_DATA_BINDINGS
using Unity.Properties;
#endif

namespace Unity.AppUI.UI
{
    /// <summary>
    /// Bounds Field UI element.
    /// </summary>
#if ENABLE_UXML_SERIALIZED_DATA
    [UxmlElement]
#endif
    public partial class BoundsField : BaseVisualElement, IInputElement<Bounds>, ISizeableElement, INotifyValueChanging<Bounds>
    {
#if ENABLE_RUNTIME_DATA_BINDINGS
        internal static readonly BindingId valueProperty = nameof(value);

        internal static readonly BindingId sizeProperty = nameof(size);

        internal static readonly BindingId invalidProperty = nameof(invalid);

        internal static readonly BindingId validateValueProperty = nameof(validateValue);
#endif
        /// <summary>
        /// The BoundsField main styling class.
        /// </summary>
        public const string ussClassName = "appui-boundsfield";

        /// <summary>
        /// The BoundsField row styling class.
        /// </summary>
        public const string rowUssClassName = ussClassName + "__row";

        /// <summary>
        /// The BoundsField size styling class.
        /// </summary>
        [EnumName("GetSizeUssClassName", typeof(Size))]
        public const string sizeUssClassName = ussClassName + "--size-";

        /// <summary>
        /// The BoundsField X NumericalField styling class.
        /// </summary>
        public const string xFieldUssClassName = ussClassName + "__x-field";

        /// <summary>
        /// The BoundsField Y NumericalField styling class.
        /// </summary>
        public const string yFieldUssClassName = ussClassName + "__y-field";

        /// <summary>
        /// The BoundsField Z NumericalField styling class.
        /// </summary>
        public const string zFieldUssClassName = ussClassName + "__z-field";

        /// <summary>
        /// The BoundsField X NumericalField styling class.
        /// </summary>
        public const string sxFieldUssClassName = ussClassName + "__sx-field";

        /// <summary>
        /// The BoundsField Y NumericalField styling class.
        /// </summary>
        public const string syFieldUssClassName = ussClassName + "__sy-field";

        /// <summary>
        /// The BoundsField Z NumericalField styling class.
        /// </summary>
        public const string szFieldUssClassName = ussClassName + "__sz-field";

        /// <summary>
        /// The BoundsField Label styling class.
        /// </summary>
        public const string labelUssClassName = ussClassName + "__label";

        Size m_Size;

        Bounds m_Value;

        Func<Bounds, bool> m_ValidateValue;

        readonly FloatField m_CXField;

        readonly FloatField m_CYField;

        readonly FloatField m_CZField;

        readonly FloatField m_SXField;

        readonly FloatField m_SYField;

        readonly FloatField m_SZField;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public BoundsField()
        {
            AddToClassList(ussClassName);

            var cXFieldContainer = new VisualElement { pickingMode = PickingMode.Ignore };
            m_CXField = new FloatField { name = xFieldUssClassName, unit = "X" };
            cXFieldContainer.AddToClassList(xFieldUssClassName);
            cXFieldContainer.Add(m_CXField);

            var cYFieldContainer = new VisualElement { pickingMode = PickingMode.Ignore };
            m_CYField = new FloatField { name = yFieldUssClassName, unit = "Y" };
            cYFieldContainer.AddToClassList(yFieldUssClassName);
            cYFieldContainer.Add(m_CYField);

            var cZFieldContainer = new VisualElement { pickingMode = PickingMode.Ignore };
            m_CZField = new FloatField { name = zFieldUssClassName, unit = "Z" };
            cZFieldContainer.AddToClassList(zFieldUssClassName);
            cZFieldContainer.Add(m_CZField);

            var sXFieldContainer = new VisualElement { pickingMode = PickingMode.Ignore };
            m_SXField = new FloatField { name = sxFieldUssClassName, unit = "X" };
            sXFieldContainer.AddToClassList(sxFieldUssClassName);
            sXFieldContainer.Add(m_SXField);

            var sYFieldContainer = new VisualElement { pickingMode = PickingMode.Ignore };
            m_SYField = new FloatField { name = syFieldUssClassName, unit = "Y" };
            sYFieldContainer.AddToClassList(syFieldUssClassName);
            sYFieldContainer.Add(m_SYField);

            var sZFieldContainer = new VisualElement { pickingMode = PickingMode.Ignore };
            m_SZField = new FloatField { name = szFieldUssClassName, unit = "Z" };
            sZFieldContainer.AddToClassList(szFieldUssClassName);
            sZFieldContainer.Add(m_SZField);

            var centerLabel = new Text("Center") { size = TextSize.S, pickingMode = PickingMode.Ignore };
            centerLabel.AddToClassList(labelUssClassName);
            var sizeLabel = new Text("Size") { size = TextSize.S, pickingMode = PickingMode.Ignore };
            sizeLabel.AddToClassList(labelUssClassName);

            var centerRow = new VisualElement { name = rowUssClassName, pickingMode = PickingMode.Ignore };
            centerRow.AddToClassList(rowUssClassName);
            centerRow.Add(centerLabel);
            centerRow.Add(cXFieldContainer);
            centerRow.Add(cYFieldContainer);
            centerRow.Add(cZFieldContainer);

            var sizeRow = new VisualElement { name = rowUssClassName, pickingMode = PickingMode.Ignore };
            sizeRow.AddToClassList(rowUssClassName);
            sizeRow.Add(sizeLabel);
            sizeRow.Add(sXFieldContainer);
            sizeRow.Add(sYFieldContainer);
            sizeRow.Add(sZFieldContainer);

            hierarchy.Add(centerRow);
            hierarchy.Add(sizeRow);

            size = Size.M;
            SetValueWithoutNotify(new Bounds());

            m_CXField.RegisterValueChangedCallback(OnCXFieldChanged);
            m_CYField.RegisterValueChangedCallback(OnCYFieldChanged);
            m_CZField.RegisterValueChangedCallback(OnCZFieldChanged);
            m_SXField.RegisterValueChangedCallback(OnSXFieldChanged);
            m_SYField.RegisterValueChangedCallback(OnSYFieldChanged);
            m_SZField.RegisterValueChangedCallback(OnSZFieldChanged);

            m_CXField.RegisterValueChangingCallback(OnCXFieldChanging);
            m_CYField.RegisterValueChangingCallback(OnCYFieldChanging);
            m_CZField.RegisterValueChangingCallback(OnCZFieldChanging);
            m_SXField.RegisterValueChangingCallback(OnSXFieldChanging);
            m_SYField.RegisterValueChangingCallback(OnSYFieldChanging);
            m_SZField.RegisterValueChangingCallback(OnSZFieldChanging);
        }

        /// <summary>
        /// The content container of the BoundsField. Always null.
        /// </summary>
        public override VisualElement contentContainer => null;

        /// <summary>
        /// The BoundsField size.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
        [Header("Bounds Field")]
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
                m_CXField.size = m_Size;
                m_CYField.size = m_Size;
                m_CZField.size = m_Size;
                m_SXField.size = m_Size;
                m_SYField.size = m_Size;
                m_SZField.size = m_Size;

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in sizeProperty);
#endif
            }
        }

        /// <summary>
        /// Sets the value of the BoundsField without notifying the value changed callback.
        /// </summary>
        /// <param name="newValue"> The new value of the BoundsField. </param>
        public void SetValueWithoutNotify(Bounds newValue)
        {
            m_Value = newValue;
            m_CXField.SetValueWithoutNotify(m_Value.center.x);
            m_CYField.SetValueWithoutNotify(m_Value.center.y);
            m_CZField.SetValueWithoutNotify(m_Value.center.z);
            m_SXField.SetValueWithoutNotify(m_Value.size.x);
            m_SYField.SetValueWithoutNotify(m_Value.size.y);
            m_SZField.SetValueWithoutNotify(m_Value.size.z);
            if (validateValue != null) invalid = !validateValue(m_Value);
        }

        /// <summary>
        /// The value of the BoundsField.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public Bounds value
        {
            get => m_Value;
            set
            {
                if (m_Value == value)
                    return;
                using var evt = ChangeEvent<Bounds>.GetPooled(m_Value, value);
                evt.target = this;
                SetValueWithoutNotify(value);
                SendEvent(evt);

#if ENABLE_RUNTIME_DATA_BINDINGS
                NotifyPropertyChanged(in valueProperty);
#endif
            }
        }

        /// <summary>
        /// Set the validation state of the BoundsField.
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

                m_CXField.EnableInClassList(Styles.invalidUssClassName, value);
                m_CYField.EnableInClassList(Styles.invalidUssClassName, value);
                m_CZField.EnableInClassList(Styles.invalidUssClassName, value);
                m_SXField.EnableInClassList(Styles.invalidUssClassName, value);
                m_SYField.EnableInClassList(Styles.invalidUssClassName, value);
                m_SZField.EnableInClassList(Styles.invalidUssClassName, value);

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in invalidProperty);
#endif
            }
        }

        /// <summary>
        /// The validation function of the BoundsField.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
        public Func<Bounds, bool> validateValue
        {
            get => m_ValidateValue;
            set
            {
                var changed = m_ValidateValue != value;
                m_ValidateValue = value;
                if (validateValue != null)
                    invalid = !validateValue(m_Value);

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in validateValueProperty);
#endif
            }
        }

        void OnCZFieldChanged(ChangeEvent<float> evt)
        {
            value = new Bounds(new Vector3(value.center.x, value.center.y, evt.newValue), value.size);
        }

        void OnCYFieldChanged(ChangeEvent<float> evt)
        {
            value = new Bounds(new Vector3(value.center.x, evt.newValue, value.center.z), value.size);
        }

        void OnCXFieldChanged(ChangeEvent<float> evt)
        {
            value = new Bounds(new Vector3(evt.newValue, value.center.y, value.center.z), value.size);
        }

        void OnSXFieldChanged(ChangeEvent<float> evt)
        {
            value = new Bounds(value.center, new Vector3(evt.newValue, value.size.y, value.size.z));
        }

        void OnSYFieldChanged(ChangeEvent<float> evt)
        {
            value = new Bounds(value.center, new Vector3(value.size.x, evt.newValue, value.size.z));
        }

        void OnSZFieldChanged(ChangeEvent<float> evt)
        {
            value = new Bounds(value.center, new Vector3(value.size.x, value.size.y, evt.newValue));
        }

        void OnCZFieldChanging(ChangingEvent<float> evt)
        {
            evt.StopPropagation();
            var val = new Bounds(new Vector3(value.center.x, value.center.y, evt.newValue), value.size);
            TrySendChangingEvent(val);
        }

        void OnCYFieldChanging(ChangingEvent<float> evt)
        {
            evt.StopPropagation();
            var val = new Bounds(new Vector3(value.center.x, evt.newValue, value.center.z), value.size);
            TrySendChangingEvent(val);
        }

        void OnCXFieldChanging(ChangingEvent<float> evt)
        {
            evt.StopPropagation();
            var val = new Bounds(new Vector3(evt.newValue, value.center.y, value.center.z), value.size);
            TrySendChangingEvent(val);
        }

        void OnSXFieldChanging(ChangingEvent<float> evt)
        {
            evt.StopPropagation();
            var val = new Bounds(value.center, new Vector3(evt.newValue, value.size.y, value.size.z));
            TrySendChangingEvent(val);
        }

        void OnSYFieldChanging(ChangingEvent<float> evt)
        {
            evt.StopPropagation();
            var val = new Bounds(value.center, new Vector3(value.size.x, evt.newValue, value.size.z));
            TrySendChangingEvent(val);
        }

        void OnSZFieldChanging(ChangingEvent<float> evt)
        {
            evt.StopPropagation();
            var val = new Bounds(value.center, new Vector3(value.size.x, value.size.y, evt.newValue));
            TrySendChangingEvent(val);
        }

        void TrySendChangingEvent(Bounds newVal)
        {
            var previousValue = m_Value;
            m_Value = newVal;

            if (m_Value != previousValue)
            {
                if (validateValue != null) invalid = !validateValue(m_Value);

                using var changeEvent = ChangingEvent<Bounds>.GetPooled();
                changeEvent.target = this;
                changeEvent.previousValue = previousValue;
                changeEvent.newValue = m_Value;
                SendEvent(changeEvent);
            }
        }

#if ENABLE_UXML_TRAITS
        /// <summary>
        /// Class to instantiate a <see cref="BoundsField"/> using the data read from a UXML file.
        /// </summary>
        public new class UxmlFactory : UxmlFactory<BoundsField, UxmlTraits> { }

        /// <summary>
        /// Class containing the <see cref="UxmlTraits"/> for the <see cref="BoundsField"/>.
        /// </summary>
        public new class UxmlTraits : BaseVisualElement.UxmlTraits
        {
            readonly UxmlEnumAttributeDescription<Size> m_Size = new UxmlEnumAttributeDescription<Size>
            {
                name = "size",
                defaultValue = Size.M,
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

                var element = (BoundsField)ve;
                element.size = m_Size.GetValueFromBag(bag, cc);


            }
        }
#endif
    }
}
