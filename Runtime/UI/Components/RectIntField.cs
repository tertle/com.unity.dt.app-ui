using System;
using UnityEngine;
using UnityEngine.UIElements;
#if ENABLE_RUNTIME_DATA_BINDINGS
using Unity.Properties;
#endif

namespace Unity.AppUI.UI
{
    /// <summary>
    /// RectInt Field UI element.
    /// </summary>
#if ENABLE_UXML_SERIALIZED_DATA
    [UxmlElement]
#endif
    public partial class RectIntField : BaseVisualElement, IInputElement<RectInt>, ISizeableElement, INotifyValueChanging<RectInt>
    {
#if ENABLE_RUNTIME_DATA_BINDINGS

        internal static readonly BindingId valueProperty = nameof(value);

        internal static readonly BindingId invalidProperty = nameof(invalid);

        internal static readonly BindingId validateValueProperty = nameof(validateValue);

        internal static readonly BindingId sizeProperty = nameof(size);

#endif

        /// <summary>
        /// The RectIntField main styling class.
        /// </summary>
        public const string ussClassName = "appui-rectfield";

        /// <summary>
        /// The RectIntField row styling class.
        /// </summary>
        public const string rowUssClassName = ussClassName + "__row";

        /// <summary>
        /// The RectIntField size styling class.
        /// </summary>
        [EnumName("GetSizeUssClassName", typeof(Size))]
        public const string sizeUssClassName = ussClassName + "--size-";

        /// <summary>
        /// The RectIntField X NumericalField styling class.
        /// </summary>
        public const string xFieldUssClassName = ussClassName + "__x-field";

        /// <summary>
        /// The RectIntField Y NumericalField styling class.
        /// </summary>
        public const string yFieldUssClassName = ussClassName + "__y-field";

        /// <summary>
        /// The RectIntField H NumericalField styling class.
        /// </summary>
        public const string hFieldUssClassName = ussClassName + "__h-field";

        /// <summary>
        /// The RectIntField W NumericalField styling class.
        /// </summary>
        public const string wFieldUssClassName = ussClassName + "__w-field";

        /// <summary>
        /// The RectIntField Label styling class.
        /// </summary>
        public const string labelUssClassName = ussClassName + "__label";

        Size m_Size;

        RectInt m_Value;

        readonly IntField m_WField;

        readonly IntField m_XField;

        readonly IntField m_YField;

        readonly IntField m_HField;

        Func<RectInt, bool> m_ValidateValue;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public RectIntField()
        {
            AddToClassList(ussClassName);

            var xFieldContainer = new VisualElement { pickingMode = PickingMode.Ignore };
            m_XField = new IntField { name = xFieldUssClassName, unit = "X" };
            xFieldContainer.AddToClassList(xFieldUssClassName);
            xFieldContainer.Add(m_XField);

            var yFieldContainer = new VisualElement { pickingMode = PickingMode.Ignore };
            m_YField = new IntField { name = yFieldUssClassName, unit = "Y" };
            yFieldContainer.AddToClassList(yFieldUssClassName);
            yFieldContainer.Add(m_YField);

            var wFieldContainer = new VisualElement { pickingMode = PickingMode.Ignore };
            m_WField = new IntField { name = wFieldUssClassName, unit = "W" };
            wFieldContainer.AddToClassList(wFieldUssClassName);
            wFieldContainer.Add(m_WField);

            var hFieldContainer = new VisualElement { pickingMode = PickingMode.Ignore };
            m_HField = new IntField { name = hFieldUssClassName, unit = "H" };
            hFieldContainer.AddToClassList(hFieldUssClassName);
            hFieldContainer.Add(m_HField);

            var positionLabel = new Text("Position") { size = TextSize.S, pickingMode = PickingMode.Ignore };
            positionLabel.AddToClassList(labelUssClassName);
            var sizeLabel = new Text("Size") { size = TextSize.S, pickingMode = PickingMode.Ignore };
            sizeLabel.AddToClassList(labelUssClassName);

            var positionRow = new VisualElement { name = rowUssClassName, pickingMode = PickingMode.Ignore };
            positionRow.AddToClassList(rowUssClassName);
            positionRow.Add(positionLabel);
            positionRow.Add(xFieldContainer);
            positionRow.Add(yFieldContainer);

            var sizeRow = new VisualElement { name = rowUssClassName, pickingMode = PickingMode.Ignore };
            sizeRow.AddToClassList(rowUssClassName);
            sizeRow.Add(sizeLabel);
            sizeRow.Add(wFieldContainer);
            sizeRow.Add(hFieldContainer);

            hierarchy.Add(positionRow);
            hierarchy.Add(sizeRow);

            size = Size.M;
            SetValueWithoutNotify(new RectInt(0, 0, 0, 0));

            m_XField.RegisterValueChangedCallback(OnXFieldChanged);
            m_YField.RegisterValueChangedCallback(OnYFieldChanged);
            m_HField.RegisterValueChangedCallback(OnHFieldChanged);
            m_WField.RegisterValueChangedCallback(OnWFieldChanged);

            m_XField.RegisterValueChangingCallback(OnXFieldChanging);
            m_YField.RegisterValueChangingCallback(OnYFieldChanging);
            m_HField.RegisterValueChangingCallback(OnHFieldChanging);
            m_WField.RegisterValueChangingCallback(OnWFieldChanging);
        }

        /// <summary>
        /// The content container of the RectIntField.
        /// </summary>
        public override VisualElement contentContainer => null;

        /// <summary>
        /// The size of the RectIntField.
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
                m_XField.size = m_Size;
                m_YField.size = m_Size;
                m_HField.size = m_Size;
                m_WField.size = m_Size;

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in sizeProperty);
#endif
            }
        }

        /// <summary>
        /// Set the value of the RectIntField without notifying the change.
        /// </summary>
        /// <param name="newValue"> The new value of the RectIntField. </param>
        public void SetValueWithoutNotify(RectInt newValue)
        {
            m_Value = newValue;
            m_XField.SetValueWithoutNotify(m_Value.x);
            m_YField.SetValueWithoutNotify(m_Value.y);
            m_HField.SetValueWithoutNotify(m_Value.height);
            m_WField.SetValueWithoutNotify(m_Value.width);
            if (validateValue != null) invalid = !validateValue(m_Value);
        }

        /// <summary>
        /// The value of the RectIntField.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public RectInt value
        {
            get => m_Value;
            set
            {
                if (m_Value.Equals(value))
                    return;
                using var evt = ChangeEvent<RectInt>.GetPooled(m_Value, value);
                evt.target = this;
                SetValueWithoutNotify(value);
                SendEvent(evt);

#if ENABLE_RUNTIME_DATA_BINDINGS
                NotifyPropertyChanged(in valueProperty);
#endif
            }
        }

        /// <summary>
        /// The invalid state of the RectIntField.
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

                m_XField.EnableInClassList(Styles.invalidUssClassName, value);
                m_YField.EnableInClassList(Styles.invalidUssClassName, value);
                m_HField.EnableInClassList(Styles.invalidUssClassName, value);
                m_WField.EnableInClassList(Styles.invalidUssClassName, value);

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in invalidProperty);
#endif
            }
        }

        /// <summary>
        /// The callback to validate the value of the RectIntField.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
        public Func<RectInt, bool> validateValue
        {
            get => m_ValidateValue;
            set
            {
                var changed = m_ValidateValue != value;
                m_ValidateValue = value;
                invalid = !validateValue?.Invoke(m_Value) ?? false;

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in validateValueProperty);
#endif
            }
        }

        void OnHFieldChanged(ChangeEvent<int> evt)
        {
            value = new RectInt(value.x, value.y, value.width, evt.newValue);
        }

        void OnWFieldChanged(ChangeEvent<int> evt)
        {
            value = new RectInt(value.x, value.y, evt.newValue, value.height);
        }

        void OnYFieldChanged(ChangeEvent<int> evt)
        {
            value = new RectInt(value.x, evt.newValue, value.width, value.height);
        }

        void OnXFieldChanged(ChangeEvent<int> evt)
        {
            value = new RectInt(evt.newValue, value.y, value.width, value.height);
        }

        void OnHFieldChanging(ChangingEvent<int> evt)
        {
            evt.StopPropagation();
            var val = new RectInt(value.x, value.y, value.width, evt.newValue);
            TrySendChangingEvent(val);
        }

        void OnWFieldChanging(ChangingEvent<int> evt)
        {
            evt.StopPropagation();
            var val = new RectInt(value.x, value.y, evt.newValue, value.height);
            TrySendChangingEvent(val);
        }

        void OnYFieldChanging(ChangingEvent<int> evt)
        {
            evt.StopPropagation();
            var val = new RectInt(value.x, evt.newValue, value.width, value.height);
            TrySendChangingEvent(val);
        }

        void OnXFieldChanging(ChangingEvent<int> evt)
        {
            evt.StopPropagation();
            var val = new RectInt(evt.newValue, value.y, value.width, value.height);
            TrySendChangingEvent(val);
        }

        void TrySendChangingEvent(RectInt newVal)
        {
            var previousValue = m_Value;
            m_Value = newVal;

            if (!m_Value.Equals(previousValue))
            {
                if (validateValue != null) invalid = !validateValue(m_Value);

                using var changeEvent = ChangingEvent<RectInt>.GetPooled();
                changeEvent.target = this;
                changeEvent.previousValue = previousValue;
                changeEvent.newValue = m_Value;
                SendEvent(changeEvent);
            }
        }

#if ENABLE_UXML_TRAITS

        /// <summary>
        /// Factory class to instantiate a <see cref="RectIntField"/> using the data read from a UXML file.
        /// </summary>
        public new class UxmlFactory : UxmlFactory<RectIntField, UxmlTraits> { }

        /// <summary>
        /// Class containing the <see cref="UxmlTraits"/> for the <see cref="RectIntField"/>.
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

                var element = (RectIntField)ve;
                element.size = m_Size.GetValueFromBag(bag, cc);
            }
        }

#endif
    }
}
