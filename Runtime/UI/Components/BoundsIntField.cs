using System;
using UnityEngine;
using UnityEngine.UIElements;
#if ENABLE_RUNTIME_DATA_BINDINGS
using Unity.Properties;
#endif

namespace Unity.AppUI.UI
{
    /// <summary>
    /// BoundsInt Field UI element.
    /// </summary>
#if ENABLE_UXML_SERIALIZED_DATA
    [UxmlElement]
#endif
    public partial class BoundsIntField : BaseVisualElement, IInputElement<BoundsInt>, ISizeableElement, INotifyValueChanging<BoundsInt>
    {
#if ENABLE_RUNTIME_DATA_BINDINGS
        internal static readonly BindingId valueProperty = nameof(value);

        internal static readonly BindingId sizeProperty = nameof(size);

        internal static readonly BindingId invalidProperty = nameof(invalid);

        internal static readonly BindingId validateValueProperty = nameof(validateValue);
#endif
        /// <summary>
        /// The BoundsIntField main styling class.
        /// </summary>
        public const string ussClassName = "appui-boundsfield";

        /// <summary>
        /// The BoundsIntField row styling class.
        /// </summary>
        public const string rowUssClassName = ussClassName + "__row";

        /// <summary>
        /// The BoundsIntField size styling class.
        /// </summary>
        [EnumName("GetSizeUssClassName", typeof(Size))]
        public const string sizeUssClassName = ussClassName + "--size-";

        /// <summary>
        /// The BoundsIntField X NumericalField styling class.
        /// </summary>
        public const string xFieldUssClassName = ussClassName + "__x-field";

        /// <summary>
        /// The BoundsIntField Y NumericalField styling class.
        /// </summary>
        public const string yFieldUssClassName = ussClassName + "__y-field";

        /// <summary>
        /// The BoundsIntField Z NumericalField styling class.
        /// </summary>
        public const string zFieldUssClassName = ussClassName + "__z-field";

        /// <summary>
        /// The BoundsIntField X NumericalField styling class.
        /// </summary>
        public const string sxFieldUssClassName = ussClassName + "__sx-field";

        /// <summary>
        /// The BoundsIntField Y NumericalField styling class.
        /// </summary>
        public const string syFieldUssClassName = ussClassName + "__sy-field";

        /// <summary>
        /// The BoundsIntField Z NumericalField styling class.
        /// </summary>
        public const string szFieldUssClassName = ussClassName + "__sz-field";

        /// <summary>
        /// The BoundsIntField Label styling class.
        /// </summary>
        public const string labelUssClassName = ussClassName + "__label";

        Size m_Size;

        BoundsInt m_Value;

        Func<BoundsInt, bool> m_ValidateValue;

        readonly IntField m_CXField;

        readonly IntField m_CYField;

        readonly IntField m_CZField;

        readonly IntField m_SXField;

        readonly IntField m_SYField;

        readonly IntField m_SZField;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public BoundsIntField()
        {
            AddToClassList(ussClassName);

            var cxFieldContainer = new VisualElement { pickingMode = PickingMode.Ignore };
            m_CXField = new IntField { name = xFieldUssClassName, unit = "X" };
            cxFieldContainer.AddToClassList(xFieldUssClassName);
            cxFieldContainer.Add(m_CXField);

            var cyFieldContainer = new VisualElement { pickingMode = PickingMode.Ignore };
            m_CYField = new IntField { name = yFieldUssClassName, unit = "Y" };
            cyFieldContainer.AddToClassList(yFieldUssClassName);
            cyFieldContainer.Add(m_CYField);

            var czFieldContainer = new VisualElement { pickingMode = PickingMode.Ignore };
            m_CZField = new IntField { name = zFieldUssClassName, unit = "Z" };
            czFieldContainer.AddToClassList(zFieldUssClassName);
            czFieldContainer.Add(m_CZField);

            var sxFieldContainer = new VisualElement { pickingMode = PickingMode.Ignore };
            m_SXField = new IntField { name = sxFieldUssClassName, unit = "X" };
            sxFieldContainer.AddToClassList(sxFieldUssClassName);
            sxFieldContainer.Add(m_SXField);

            var syFieldContainer = new VisualElement { pickingMode = PickingMode.Ignore };
            m_SYField = new IntField { name = syFieldUssClassName, unit = "Y" };
            syFieldContainer.AddToClassList(syFieldUssClassName);
            syFieldContainer.Add(m_SYField);

            var szFieldContainer = new VisualElement { pickingMode = PickingMode.Ignore };
            m_SZField = new IntField { name = szFieldUssClassName, unit = "Z" };
            szFieldContainer.AddToClassList(szFieldUssClassName);
            szFieldContainer.Add(m_SZField);

            var centerLabel = new Text("Position") { size = TextSize.S, pickingMode = PickingMode.Ignore };
            centerLabel.AddToClassList(labelUssClassName);
            var sizeLabel = new Text("Size") { size = TextSize.S, pickingMode = PickingMode.Ignore };
            sizeLabel.AddToClassList(labelUssClassName);

            var centerRow = new VisualElement { name = rowUssClassName, pickingMode = PickingMode.Ignore };
            centerRow.AddToClassList(rowUssClassName);
            centerRow.Add(centerLabel);
            centerRow.Add(cxFieldContainer);
            centerRow.Add(cyFieldContainer);
            centerRow.Add(czFieldContainer);

            var sizeRow = new VisualElement { name = rowUssClassName, pickingMode = PickingMode.Ignore };
            sizeRow.AddToClassList(rowUssClassName);
            sizeRow.Add(sizeLabel);
            sizeRow.Add(sxFieldContainer);
            sizeRow.Add(syFieldContainer);
            sizeRow.Add(szFieldContainer);

            hierarchy.Add(centerRow);
            hierarchy.Add(sizeRow);

            size = Size.M;
            SetValueWithoutNotify(new BoundsInt());

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
        /// The content container of the BoundsIntField. Always null.
        /// </summary>
        public override VisualElement contentContainer => null;

        /// <summary>
        /// The BoundsIntField size.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
        [Header("BoundsInt Field")]
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
        /// Sets the BoundsIntField value without notifying any change event listeners.
        /// </summary>
        /// <param name="newValue"> The new value to set. </param>
        public void SetValueWithoutNotify(BoundsInt newValue)
        {
            m_Value = newValue;
            m_CXField.SetValueWithoutNotify(m_Value.position.x);
            m_CYField.SetValueWithoutNotify(m_Value.position.y);
            m_CZField.SetValueWithoutNotify(m_Value.position.z);
            m_SXField.SetValueWithoutNotify(m_Value.size.x);
            m_SYField.SetValueWithoutNotify(m_Value.size.y);
            m_SZField.SetValueWithoutNotify(m_Value.size.z);
            if (validateValue != null) invalid = !validateValue(m_Value);
        }

        /// <summary>
        /// The BoundsIntField value.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public BoundsInt value
        {
            get => m_Value;
            set
            {
                if (m_Value == value)
                    return;
                using var evt = ChangeEvent<BoundsInt>.GetPooled(m_Value, value);
                evt.target = this;
                SetValueWithoutNotify(value);
                SendEvent(evt);

#if ENABLE_RUNTIME_DATA_BINDINGS
                NotifyPropertyChanged(in valueProperty);
#endif
            }
        }

        /// <summary>
        /// Set the validation state of the BoundsIntField.
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
        /// The validation function to use on the BoundsIntField value.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
        public Func<BoundsInt, bool> validateValue
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

        void OnCZFieldChanged(ChangeEvent<int> evt)
        {
            value = new BoundsInt(new Vector3Int(value.position.x, value.position.y, evt.newValue), value.size);
        }

        void OnCYFieldChanged(ChangeEvent<int> evt)
        {
            value = new BoundsInt(new Vector3Int(value.position.x, evt.newValue, value.position.z), value.size);
        }

        void OnCXFieldChanged(ChangeEvent<int> evt)
        {
            value = new BoundsInt(new Vector3Int(evt.newValue, value.position.y, value.position.z), value.size);
        }

        void OnSXFieldChanged(ChangeEvent<int> evt)
        {
            value = new BoundsInt(value.position, new Vector3Int(evt.newValue, value.size.y, value.size.z));
        }

        void OnSYFieldChanged(ChangeEvent<int> evt)
        {
            value = new BoundsInt(value.position, new Vector3Int(value.size.x, evt.newValue, value.size.z));
        }

        void OnSZFieldChanged(ChangeEvent<int> evt)
        {
            value = new BoundsInt(value.position, new Vector3Int(value.size.x, value.size.y, evt.newValue));
        }

        void OnCZFieldChanging(ChangingEvent<int> evt)
        {
            evt.StopPropagation();
            TrySendChangingEvent(new BoundsInt(new Vector3Int(value.position.x, value.position.y, evt.newValue), value.size));
        }

        void OnCYFieldChanging(ChangingEvent<int> evt)
        {
            evt.StopPropagation();
            TrySendChangingEvent(new BoundsInt(new Vector3Int(value.position.x, evt.newValue, value.position.z), value.size));
        }

        void OnCXFieldChanging(ChangingEvent<int> evt)
        {
            evt.StopPropagation();
            TrySendChangingEvent(new BoundsInt(new Vector3Int(evt.newValue, value.position.y, value.position.z), value.size));
        }

        void OnSXFieldChanging(ChangingEvent<int> evt)
        {
            evt.StopPropagation();
            TrySendChangingEvent(new BoundsInt(value.position, new Vector3Int(evt.newValue, value.size.y, value.size.z)));
        }

        void OnSYFieldChanging(ChangingEvent<int> evt)
        {
            evt.StopPropagation();
            TrySendChangingEvent(new BoundsInt(value.position, new Vector3Int(value.size.x, evt.newValue, value.size.z)));
        }

        void OnSZFieldChanging(ChangingEvent<int> evt)
        {
            evt.StopPropagation();
            TrySendChangingEvent(new BoundsInt(value.position, new Vector3Int(value.size.x, value.size.y, evt.newValue)));
        }

        void TrySendChangingEvent(BoundsInt newVal)
        {
            var previousValue = m_Value;
            m_Value = newVal;

            if (m_Value != previousValue)
            {
                if (validateValue != null) invalid = !validateValue(m_Value);

                using var changeEvent = ChangingEvent<BoundsInt>.GetPooled();
                changeEvent.target = this;
                changeEvent.previousValue = previousValue;
                changeEvent.newValue = m_Value;
                SendEvent(changeEvent);
            }
        }

#if ENABLE_UXML_TRAITS

        /// <summary>
        /// Class to instantiate a <see cref="BoundsIntField"/> using the data read from a UXML file.
        /// </summary>
        public new class UxmlFactory : UxmlFactory<BoundsIntField, UxmlTraits> { }

        /// <summary>
        /// Class containing the <see cref="UxmlTraits"/> for the <see cref="BoundsIntField"/>.
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

                var element = (BoundsIntField)ve;
                element.size = m_Size.GetValueFromBag(bag, cc);


            }
        }
#endif
    }
}
