using System;
using Unity.AppUI.Core;
using UnityEngine;
using UnityEngine.UIElements;
#if ENABLE_RUNTIME_DATA_BINDINGS
using Unity.Properties;
#endif

namespace Unity.AppUI.UI
{
    /// <summary>
    /// Sizing values for <see cref="Icon"/> elements.
    /// </summary>
    public enum IconSize
    {
        /// <summary>
        /// Extra extra small
        /// </summary>
        XXS,

        /// <summary>
        /// Extra small
        /// </summary>
        XS,

        /// <summary>
        /// Small
        /// </summary>
        S,

        /// <summary>
        /// Medium
        /// </summary>
        M,

        /// <summary>
        /// Large
        /// </summary>
        L
    }

    /// <summary>
    /// Variant values for <see cref="Icon"/> elements.
    /// </summary>
    [GenerateLowerCaseStrings]
    public enum IconVariant
    {
        /// <summary>
        /// Regular
        /// </summary>
        Regular = 1,

        /// <summary>
        /// Bold
        /// </summary>
        Bold,

        /// <summary>
        /// DuoTone
        /// </summary>
        DuoTone,

        /// <summary>
        /// Light
        /// </summary>
        Light,

        /// <summary>
        /// Fill
        /// </summary>
        Fill,

        /// <summary>
        /// Thin
        /// </summary>
        Thin
    }

    /// <summary>
    /// Icon UI component.
    /// </summary>
#if ENABLE_UXML_SERIALIZED_DATA
    [UxmlElement]
#endif
    public partial class Icon : Image
    {
#if ENABLE_RUNTIME_DATA_BINDINGS

        internal static readonly BindingId iconNameProperty = new BindingId(nameof(iconName));

        internal static readonly BindingId primaryProperty = new BindingId(nameof(primary));

        internal static readonly BindingId sizeProperty = new BindingId(nameof(size));

        internal static readonly BindingId variantProperty = new BindingId(nameof(variant));

#endif

        /// <summary>
        /// The Icon main styling class.
        /// </summary>
        public new const string ussClassName = "appui-icon";

        /// <summary>
        /// The Icon primary variant styling class.
        /// </summary>
        public const string primaryUssClassName = ussClassName + "--primary";

        /// <summary>
        /// The Icon size styling class.
        /// </summary>
        [EnumName("GetSizeUssClassName", typeof(IconSize))]
        public const string sizeUssClassName = ussClassName + "--size-";

        string m_IconName;

        IconSize m_Size;

        IconVariant m_Variant = IconVariant.Regular;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public Icon()
        {
            AddToClassList(ussClassName);

            focusable = false;
            pickingMode = PickingMode.Ignore;
            usageHints |= UsageHints.DynamicColor;

            iconName = "info";
            size = IconSize.M;
            primary = true;
            scaleMode = ScaleMode.ScaleToFit;
        }

        /// <summary>
        /// The primary variant of the Icon.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public bool primary
        {
            get => ClassListContains(primaryUssClassName);
            set
            {
                var changed = ClassListContains(primaryUssClassName) != value;
                EnableInClassList(primaryUssClassName, value);

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in primaryProperty);
#endif
            }
        }

        /// <summary>
        /// The size of the Icon.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public IconSize size
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
        /// The name of the Icon.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public string iconName
        {
            get => m_IconName;
            set
            {
                var changed = m_IconName != value;
                RemoveFromClassList(MemoryUtils.Concatenate(ussClassName, "--", m_IconName, "--", m_Variant.ToLowerCase()));
                RemoveFromClassList(MemoryUtils.Concatenate(ussClassName, "--", m_IconName));
                m_IconName = value;
                AddToClassList(MemoryUtils.Concatenate(ussClassName, "--", m_IconName, "--", m_Variant.ToLowerCase()));
                AddToClassList(MemoryUtils.Concatenate(ussClassName, "--", m_IconName));

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in iconNameProperty);
#endif
            }
        }

        /// <summary>
        /// The variant of the Icon.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public IconVariant variant
        {
            get => m_Variant;
            set
            {
                var changed = m_Variant != value;
                RemoveFromClassList(MemoryUtils.Concatenate(ussClassName, "--", m_IconName, "--", m_Variant.ToLowerCase()));
                RemoveFromClassList(MemoryUtils.Concatenate(ussClassName, "--", m_IconName));
                m_Variant = value;
                AddToClassList(MemoryUtils.Concatenate(ussClassName, "--", m_IconName, "--", m_Variant.ToLowerCase()));
                AddToClassList(MemoryUtils.Concatenate(ussClassName, "--", m_IconName));

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in variantProperty);
#endif
            }
        }

#if ENABLE_UXML_TRAITS

        /// <summary>
        /// Factory class to instantiate a <see cref="Icon"/> using the data read from a UXML file.
        /// </summary>
        public new class UxmlFactory : UxmlFactory<Icon, UxmlTraits> { }

        /// <summary>
        /// Class containing the <see cref="UxmlTraits"/> for the <see cref="Icon"/>.
        /// </summary>
        public new class UxmlTraits : Image.UxmlTraits
        {

            readonly UxmlStringAttributeDescription m_IconName = new UxmlStringAttributeDescription
            {
                name = "icon-name",
                defaultValue = "info",
            };

            readonly UxmlBoolAttributeDescription m_Primary = new UxmlBoolAttributeDescription
            {
                name = "primary",
                defaultValue = true,
            };

            readonly UxmlEnumAttributeDescription<IconVariant> m_Variant = new UxmlEnumAttributeDescription<IconVariant>
            {
                name = "variant",
                defaultValue = IconVariant.Regular,
            };

            readonly UxmlEnumAttributeDescription<IconSize> m_Size = new UxmlEnumAttributeDescription<IconSize>
            {
                name = "size",
                defaultValue = IconSize.M,
            };

            /// <summary>
            /// Initializes the VisualElement from the UXML attributes.
            /// </summary>
            /// <param name="ve"> The <see cref="VisualElement"/> to initialize.</param>
            /// <param name="bag"> The <see cref="IUxmlAttributes"/> bag to use to initialize the <see cref="VisualElement"/>.</param>
            /// <param name="cc"> The <see cref="CreationContext"/> to use to initialize the <see cref="VisualElement"/>.</param>
            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                m_PickingMode.defaultValue = PickingMode.Ignore;
                base.Init(ve, bag, cc);

                var element = (Icon)ve;
                element.primary = m_Primary.GetValueFromBag(bag, cc);
                element.size = m_Size.GetValueFromBag(bag, cc);
                element.iconName = m_IconName.GetValueFromBag(bag, cc);
                element.variant = m_Variant.GetValueFromBag(bag, cc);

            }
        }

#endif
    }
}
