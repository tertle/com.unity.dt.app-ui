using System;
using UnityEngine;
using UnityEngine.UIElements;
#if ENABLE_RUNTIME_DATA_BINDINGS
using Unity.Properties;
#endif

namespace Unity.AppUI.UI
{
    /// <summary>
    /// Vector3Int Field UI element.
    /// </summary>
#if ENABLE_UXML_SERIALIZED_DATA
    [UxmlElement]
#endif
    public partial class Vector3IntField : BaseVisualElement, IInputElement<Vector3Int>, ISizeableElement, INotifyValueChanging<Vector3Int>, IFormattable<int>
    {
#if ENABLE_RUNTIME_DATA_BINDINGS

        internal static readonly BindingId valueProperty = new BindingId(nameof(value));

        internal static readonly BindingId invalidProperty = new BindingId(nameof(invalid));

        internal static readonly BindingId sizeProperty = new BindingId(nameof(size));

        internal static readonly BindingId validateValueProperty = new BindingId(nameof(validateValue));

        internal static readonly BindingId formatStringProperty = new BindingId(nameof(formatString));

        internal static readonly BindingId formatFunctionProperty = new BindingId(nameof(formatFunction));

#endif

        /// <summary>
        /// The Vector3Field main styling class.
        /// </summary>
        public const string ussClassName = "appui-vector3field";

        /// <summary>
        /// The Vector3Field size styling class.
        /// </summary>
        [EnumName("GetSizeUssClassName", typeof(Size))]
        public const string sizeUssClassName = ussClassName + "--size-";

        /// <summary>
        /// The Vector3Field container styling class.
        /// </summary>
        public const string containerUssClassName = ussClassName + "__container";

        /// <summary>
        /// The Vector3Field X NumericalField styling class.
        /// </summary>
        public const string xFieldUssClassName = ussClassName + "__x-field";

        /// <summary>
        /// The Vector3Field Y NumericalField styling class.
        /// </summary>
        public const string yFieldUssClassName = ussClassName + "__y-field";

        /// <summary>
        /// The Vector3Field Z NumericalField styling class.
        /// </summary>
        public const string zFieldUssClassName = ussClassName + "__z-field";

        Size m_Size;

        Vector3Int m_Value;

        readonly IntField m_XField;

        readonly IntField m_YField;

        readonly IntField m_ZField;

        Vector3Int m_LastValue;

        Func<Vector3Int, bool> m_ValidateValue;

        string m_FormatString;

        FormatFunction<int> m_FormatFunction;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public Vector3IntField()
        {
            AddToClassList(ussClassName);

            var container = new VisualElement { name = containerUssClassName };
            container.AddToClassList(containerUssClassName);

            var xFieldContainer = new VisualElement { pickingMode = PickingMode.Ignore };
            m_XField = new IntField { name = xFieldUssClassName, unit = "X" };
            xFieldContainer.AddToClassList(xFieldUssClassName);
            xFieldContainer.Add(m_XField);

            var yFieldContainer = new VisualElement { pickingMode = PickingMode.Ignore };
            m_YField = new IntField { name = yFieldUssClassName, unit = "Y" };
            yFieldContainer.AddToClassList(yFieldUssClassName);
            yFieldContainer.Add(m_YField);

            var zFieldContainer = new VisualElement { pickingMode = PickingMode.Ignore };
            m_ZField = new IntField { name = zFieldUssClassName, unit = "Z" };
            zFieldContainer.AddToClassList(zFieldUssClassName);
            zFieldContainer.Add(m_ZField);

            container.Add(xFieldContainer);
            container.Add(yFieldContainer);
            container.Add(zFieldContainer);

            hierarchy.Add(container);

            size = Size.M;
            SetValueWithoutNotify(Vector3Int.zero);

            m_XField.RegisterValueChangingCallback(OnXFieldChanging);
            m_YField.RegisterValueChangingCallback(OnYFieldChanging);
            m_ZField.RegisterValueChangingCallback(OnZFieldChanging);

            m_XField.RegisterValueChangedCallback(OnXFieldChanged);
            m_YField.RegisterValueChangedCallback(OnYFieldChanged);
            m_ZField.RegisterValueChangedCallback(OnZFieldChanged);
        }

        /// <summary>
        /// The content container of the Vector3IntField.
        /// </summary>
        public override VisualElement contentContainer => null;

        /// <summary>
        /// The size of the Vector3IntField.
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
                RemoveFromClassList(GetSizeUssClassName(m_Size));
                m_Size = value;
                AddToClassList(GetSizeUssClassName(m_Size));
                m_XField.size = m_Size;
                m_YField.size = m_Size;
                m_ZField.size = m_Size;
            }
        }

        /// <summary>
        /// Sets the value of the Vector3IntField without notifying any listeners.
        /// </summary>
        /// <param name="newValue"> The new value to set. </param>
        public void SetValueWithoutNotify(Vector3Int newValue)
        {
            m_Value = newValue;
            m_LastValue = m_Value;
            m_XField.SetValueWithoutNotify(m_Value.x);
            m_YField.SetValueWithoutNotify(m_Value.y);
            m_ZField.SetValueWithoutNotify(m_Value.z);
            if (validateValue != null) invalid = !validateValue(m_Value);
        }

        /// <summary>
        /// The value of the Vector3IntField.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public Vector3Int value
        {
            get => m_Value;
            set
            {
                if (m_LastValue == m_Value && m_Value == value)
                    return;
                using var evt = ChangeEvent<Vector3Int>.GetPooled(m_Value, value);
                evt.target = this;
                SetValueWithoutNotify(value);
                SendEvent(evt);
            }
        }

        /// <summary>
        /// The invalid state of the Vector3IntField.
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
                EnableInClassList(Styles.invalidUssClassName, value);

                m_XField.EnableInClassList(Styles.invalidUssClassName, value);
                m_YField.EnableInClassList(Styles.invalidUssClassName, value);
                m_ZField.EnableInClassList(Styles.invalidUssClassName, value);
            }
        }

        /// <summary>
        /// The validation function of the Vector3IntField.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
        public Func<Vector3Int, bool> validateValue
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
        /// The format string of the element.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public string formatString
        {
            get => m_FormatString;
            set
            {
                var changed = m_FormatString != value;
                m_FormatString = value;
                m_XField.formatString = m_FormatString;
                m_YField.formatString = m_FormatString;
                m_ZField.formatString = m_FormatString;
                SetValueWithoutNotify(this.value);

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in formatStringProperty);
#endif
            }
        }

        /// <summary>
        /// The format function of the element.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
        public FormatFunction<int> formatFunction
        {
            get => m_FormatFunction;
            set
            {
                var changed = m_FormatFunction != value;
                m_FormatFunction = value;
                m_XField.formatFunction = m_FormatFunction;
                m_YField.formatFunction = m_FormatFunction;
                m_ZField.formatFunction = m_FormatFunction;
                SetValueWithoutNotify(this.value);

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in formatStringProperty);
#endif
            }
        }

        void OnXFieldChanging(ChangingEvent<int> evt)
        {

            evt.StopPropagation();
            TrySendChangingEvent(new Vector3Int(evt.newValue, m_Value.y, m_Value.z));
        }

        void OnYFieldChanging(ChangingEvent<int> evt)
        {

            evt.StopPropagation();
            TrySendChangingEvent(new Vector3Int(m_Value.x, evt.newValue, m_Value.z));
        }

        void OnZFieldChanging(ChangingEvent<int> evt)
        {

            evt.StopPropagation();
            TrySendChangingEvent(new Vector3Int(m_Value.x, m_Value.y, evt.newValue));
        }

        void TrySendChangingEvent(Vector3Int newVector)
        {
            var previousValue = m_Value;
            m_Value = newVector;

            if (m_Value != previousValue)
            {
                if (validateValue != null) invalid = !validateValue(m_Value);

                using var changeEvent = ChangingEvent<Vector3Int>.GetPooled();
                changeEvent.target = this;
                changeEvent.previousValue = previousValue;
                changeEvent.newValue = m_Value;
                SendEvent(changeEvent);
            }
        }

        void OnZFieldChanged(ChangeEvent<int> evt)
        {
            value = new Vector3Int(value.x, value.y, evt.newValue);
        }

        void OnYFieldChanged(ChangeEvent<int> evt)
        {
            value = new Vector3Int(value.x, evt.newValue, value.z);
        }

        void OnXFieldChanged(ChangeEvent<int> evt)
        {
            value = new Vector3Int(evt.newValue, value.y, value.z);
        }

#if ENABLE_UXML_TRAITS

        /// <summary>
        /// Factory class to instantiate a <see cref="Vector3IntField"/> using the data read from a UXML file.
        /// </summary>
        public new class UxmlFactory : UxmlFactory<Vector3IntField, UxmlTraits> { }

        /// <summary>
        /// Class containing the <see cref="UxmlTraits"/> for the <see cref="Vector3IntField"/>.
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

                var element = (Vector3IntField)ve;
                element.size = m_Size.GetValueFromBag(bag, cc);
            }
        }

#endif
    }
}
