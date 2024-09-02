using System;
using Unity.AppUI.Core;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.AppUI.UI
{
    /// <summary>
    /// AssetTarget Field UI element.
    /// </summary>
    // todo This has to work with an AssetReferencePicker
#if ENABLE_UXML_SERIALIZED_DATA
    [UxmlElement]
#endif
    partial class AssetTargetField : BaseVisualElement, IInputElement<AssetReference>, ISizeableElement, IPressable
    {
#if ENABLE_RUNTIME_DATA_BINDINGS
        internal static readonly BindingId sizeProperty = nameof(size);

        internal static readonly BindingId valueProperty = nameof(value);

        internal static readonly BindingId invalidProperty = nameof(invalid);

        internal static readonly BindingId validateValueProperty = nameof(validateValue);
#endif
        const string k_DefaultIconName = "scene";

        /// <summary>
        /// The AssetTargetField main styling class.
        /// </summary>
        public const string ussClassName = "appui-assettargetfield";

        /// <summary>
        /// The AssetTargetField icon styling class.
        /// </summary>
        public const string iconUssClassName = ussClassName + "__icon";

        /// <summary>
        /// The AssetTargetField label styling class.
        /// </summary>
        public const string labelUssClassName = ussClassName + "__label";

        /// <summary>
        /// The AssetTargetField type label styling class.
        /// </summary>
        public const string typeLabelUssClassName = ussClassName + "__typelabel";

        /// <summary>
        /// The AssetTargetField size styling class.
        /// </summary>
        [EnumName("GetSizeUssClassName", typeof(Size))]
        public const string sizeUssClassName = ussClassName + "--size-";

        readonly Icon m_IconElement;

        readonly LocalizedTextElement m_LabelElement;

        readonly LocalizedTextElement m_TypeLabelElement;

        AssetReference m_AssetReference;

        Size m_Size;

        Type m_Type;

        Pressable m_Clickable;

        Func<AssetReference, bool> m_ValidateValue;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public AssetTargetField()
        {
            AddToClassList(ussClassName);

            focusable = true;
            pickingMode = PickingMode.Position;
            tabIndex = 0;
            clickable = new Pressable();

            m_IconElement = new Icon
            {
                name = iconUssClassName,
                pickingMode = PickingMode.Ignore,
                iconName = k_DefaultIconName
            };
            m_IconElement.AddToClassList(iconUssClassName);

            m_LabelElement = new LocalizedTextElement
            {
                name = labelUssClassName,
                pickingMode = PickingMode.Ignore
            };
            m_LabelElement.AddToClassList(labelUssClassName);

            m_TypeLabelElement = new LocalizedTextElement
            {
                name = typeLabelUssClassName,
                pickingMode = PickingMode.Ignore
            };
            m_TypeLabelElement.AddToClassList(typeLabelUssClassName);

            hierarchy.Add(m_IconElement);
            hierarchy.Add(m_LabelElement);
            hierarchy.Add(m_TypeLabelElement);

            size = Size.M;
            type = typeof(GameObject);
            SetValueWithoutNotify(null);
        }

        public override VisualElement contentContainer => null;

        /// <summary>
        /// Clickable Manipulator for this AssetTargetField.
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

        public Type type
        {
            get => m_Type;
            set
            {
                m_Type = value;
                if (m_AssetReference != null && m_Type != null)
                {
                    var valueType = m_AssetReference.GetType();
                    if (!m_Type.IsAssignableFrom(valueType))
                        this.value = null;
                }

                m_IconElement.iconName = m_Type?.Name.ToLower();
                m_TypeLabelElement.text = m_Type?.Name.ToUpper();
            }
        }

        public Size size
        {
            get => m_Size;
            set
            {
                RemoveFromClassList(GetSizeUssClassName(m_Size));
                m_Size = value;
                AddToClassList(GetSizeUssClassName(m_Size));
            }
        }

        public bool invalid
        {
            get => ClassListContains(Styles.invalidUssClassName);
            set => EnableInClassList(Styles.invalidUssClassName, value);
        }

        public Func<AssetReference, bool> validateValue
        {
            get => m_ValidateValue;
            set
            {
                var changed = m_ValidateValue != value;
                m_ValidateValue = value;
                invalid = !m_ValidateValue?.Invoke(this.value) ?? false;

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in validateValueProperty);
#endif
            }
        }

        public void SetValueWithoutNotify(AssetReference newValue)
        {
            m_AssetReference = newValue;
            m_LabelElement.text = m_AssetReference?.name ?? "<None>";
            if (validateValue != null) invalid = !validateValue(m_AssetReference);
        }

        public AssetReference value
        {
            get => m_AssetReference;
            set
            {
                if (m_AssetReference == value)
                    return;
                using var evt = ChangeEvent<AssetReference>.GetPooled(m_AssetReference, value);
                evt.target = this;
                SetValueWithoutNotify(value);
                SendEvent(evt);
            }
        }

#if ENABLE_UXML_TRAITS
        public new class UxmlFactory : UxmlFactory<AssetTargetField, UxmlTraits> { }

        /// <summary>
        /// Class containing the <see cref="UxmlTraits"/> for the <see cref="AssetTargetField"/>.
        /// </summary>
        public new class UxmlTraits : BaseVisualElement.UxmlTraits
        {

            readonly UxmlBoolAttributeDescription m_Invalid = new UxmlBoolAttributeDescription
            {
                name = "invalid",
                defaultValue = false
            };

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

                var element = (AssetTargetField)ve;
                element.size = m_Size.GetValueFromBag(bag, cc);
                element.invalid = m_Invalid.GetValueFromBag(bag, cc);

            }
        }
#endif
    }
}
