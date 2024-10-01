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
    /// Base class for all textual App UI components.
    /// </summary>
#if ENABLE_UXML_SERIALIZED_DATA
    [UxmlElement]
#endif
    public abstract partial class BaseTextElement : TextElement, IContextOverrideElement, IAdditionalDataHolder
    {
#if ENABLE_RUNTIME_DATA_BINDINGS
        internal static readonly BindingId preferredTooltipPlacementOverrideProperty = nameof(preferredTooltipPlacementOverride);

        internal static readonly BindingId tooltipDelayMsOverrideProperty = nameof(tooltipDelayMsOverride);

        internal static readonly BindingId scaleOverrideProperty = nameof(scaleOverride);

        internal static readonly BindingId themeOverrideProperty = nameof(themeOverride);

        internal static readonly BindingId langOverrideProperty = nameof(langOverride);

        internal static readonly BindingId layoutDirectionOverrideProperty = nameof(layoutDirectionOverride);
#endif

        /// <summary>
        /// The context prefix used as USS selector.
        /// </summary>
        [EnumName("GetLayoutDirectionUssClassName", typeof(Dir))]
        public const string contextPrefix = Panel.contextPrefix;

        VisualElementExtensions.AdditionalData IAdditionalDataHolder.additionalData { get; set; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        protected BaseTextElement()
        {
            preferredTooltipPlacementOverride = OptionalEnum<PopoverPlacement>.none;
            tooltipDelayMsOverride = Optional<int>.none;
            scaleOverride = Optional<string>.none;
            themeOverride = Optional<string>.none;
            langOverride = Optional<string>.none;
            layoutDirectionOverride = OptionalEnum<Dir>.none;
        }

        /// <summary>
        /// The scale to use in this part of the UI.
        /// </summary>
        [Tooltip("The scale to use in this part of the UI.")]
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute("scale")]
        [OptionalScaleDrawer]
#endif
        public Optional<string> scaleOverride
        {
            get => this.GetSelfContext<ScaleContext>() is {} ctx ?
                ctx.scale : Optional<string>.none;
            set
            {
                var previous = this.GetSelfContext<ScaleContext>();
                var newCtx = value.IsSet ? new ScaleContext(value.Value) : null;

                if (previous == newCtx)
                    return;

                if (!string.IsNullOrEmpty(previous?.scale))
                    RemoveFromClassList(MemoryUtils.Concatenate(Panel.contextPrefix, previous.scale));
                if (!string.IsNullOrEmpty(newCtx?.scale))
                    AddToClassList(MemoryUtils.Concatenate(Panel.contextPrefix, newCtx.scale));
                this.ProvideContext(newCtx);
#if ENABLE_RUNTIME_DATA_BINDINGS
                NotifyPropertyChanged(in scaleOverrideProperty);
#endif
            }
        }

        /// <summary>
        /// The theme to use in this part of the UI.
        /// </summary>
        [Tooltip("The theme to use in this part of the UI.")]
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute("theme")]
        [OptionalThemeDrawer]
#endif
        public Optional<string> themeOverride
        {
            get => this.GetSelfContext<ThemeContext>() is {} ctx ?
                ctx.theme : Optional<string>.none;
            set
            {
                var previous = this.GetSelfContext<ThemeContext>();
                var newCtx = value.IsSet ? new ThemeContext(value.Value) : null;

                if (previous == newCtx)
                    return;

                if (!string.IsNullOrEmpty(previous?.theme))
                    RemoveFromClassList(MemoryUtils.Concatenate(Panel.contextPrefix, previous.theme));
                if (!string.IsNullOrEmpty(newCtx?.theme))
                    AddToClassList(MemoryUtils.Concatenate(Panel.contextPrefix, newCtx.theme));
                this.ProvideContext(newCtx);
#if ENABLE_RUNTIME_DATA_BINDINGS
                NotifyPropertyChanged(in themeOverrideProperty);
#endif
            }
        }

        /// <summary>
        /// The language to use in this part of the UI.
        /// </summary>
        [Tooltip("The language to use in this part of the UI.")]
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute("lang")]
#endif
        public Optional<string> langOverride
        {
            get => this.GetSelfContext<LangContext>() is {} ctx ?
                ctx.lang : Optional<string>.none;
            set
            {
                var previous = this.GetSelfContext<LangContext>();
                var newCtx = value.IsSet ? new LangContext(value.Value) : null;

                if (previous == newCtx)
                    return;

                if (!string.IsNullOrEmpty(previous?.lang))
                    RemoveFromClassList(MemoryUtils.Concatenate(Panel.contextPrefix, previous.lang));
                if (!string.IsNullOrEmpty(newCtx?.lang))
                    AddToClassList(MemoryUtils.Concatenate(Panel.contextPrefix, newCtx.lang));
                this.ProvideContext(newCtx);
#if ENABLE_RUNTIME_DATA_BINDINGS
                NotifyPropertyChanged(in langOverrideProperty);
#endif
            }
        }

        /// <summary>
        /// The layout direction to use in this part of the UI.
        /// </summary>
        [Tooltip("The layout direction to use in this part of the UI.")]
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute("dir")]
#endif
        public OptionalEnum<Dir> layoutDirectionOverride
        {
            get => this.GetSelfContext<DirContext>() is {} ctx ?
                ctx.dir : OptionalEnum<Dir>.none;
            set
            {
                var previous = this.GetSelfContext<DirContext>();
                var newCtx = value.IsSet ? new DirContext(value.Value) : null;

                if (previous == newCtx)
                    return;

                if (previous != null)
                    RemoveFromClassList(GetLayoutDirectionUssClassName(previous.dir));
                if (newCtx != null)
                    AddToClassList(GetLayoutDirectionUssClassName(newCtx.dir));
                this.ProvideContext(newCtx);
#if ENABLE_RUNTIME_DATA_BINDINGS
                NotifyPropertyChanged(in layoutDirectionOverrideProperty);
#endif
            }
        }

        /// <summary>
        /// Preferred placement for tooltips.
        /// </summary>
        [Tooltip("Preferred placement for tooltips.\n" +
            "Note that this is only a hint and the tooltip may be placed differently if there is not enough space.")]
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute("preferred-tooltip-placement")]
#endif
        public OptionalEnum<PopoverPlacement> preferredTooltipPlacementOverride
        {
            get => this.GetSelfContext<TooltipPlacementContext>() is {} ctx ?
                ctx.placement : OptionalEnum<PopoverPlacement>.none;
            set
            {
                var previous = this.GetSelfContext<TooltipPlacementContext>();
                var newCtx = value.IsSet ? new TooltipPlacementContext(value.Value) : null;

                if (previous == newCtx)
                    return;

                this.ProvideContext(newCtx);
#if ENABLE_RUNTIME_DATA_BINDINGS
                NotifyPropertyChanged(in preferredTooltipPlacementOverrideProperty);
#endif
            }
        }

        /// <summary>
        /// Delay in milliseconds before showing a tooltip.
        /// </summary>
        [Tooltip("Delay in milliseconds before showing a tooltip.")]
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute("tooltip-delay-ms")]
#endif
        public Optional<int> tooltipDelayMsOverride
        {
            get => this.GetSelfContext<TooltipDelayContext>() is {} ctx ?
                ctx.tooltipDelayMs : Optional<int>.none;
            set
            {
                var previous = this.GetSelfContext<TooltipDelayContext>();
                var newCtx = value.IsSet ? new TooltipDelayContext(value.Value) : null;

                if (previous == newCtx)
                    return;

                this.ProvideContext(newCtx);
#if ENABLE_RUNTIME_DATA_BINDINGS
                NotifyPropertyChanged(in tooltipDelayMsOverrideProperty);
#endif
            }
        }

#if ENABLE_ENABLED_UXML_PROPERTY
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute("enabled")]
#endif
        bool enabledOverride
        {
            get => enabledSelf;
            set => SetEnabled(value);
        }
#endif

#if ENABLE_UXML_TRAITS
        /// <summary>
        /// Class containing the UXML traits for the TextElement class.
        /// </summary>
        public new class UxmlTraits : TextElement.UxmlTraits
        {
            readonly UxmlEnumAttributeDescription<PopoverPlacement> m_PreferredTooltipPlacement =
                new UxmlEnumAttributeDescription<PopoverPlacement>
                {
                    defaultValue = Tooltip.defaultPlacement,
                    name = "preferred-tooltip-placement"
                };

            readonly UxmlIntAttributeDescription m_TooltipDelayMs =
                new UxmlIntAttributeDescription
                {
                    defaultValue = TooltipManipulator.defaultDelayMs,
                    name = "tooltip-delay-ms"
                };

            readonly UxmlStringAttributeDescription m_Scale =
                new UxmlStringAttributeDescription
                {
                    defaultValue = null,
                    name = "scale",
                    restriction = new UxmlEnumeration
                    {
                        values = new[] {"small", "medium", "large"}
                    }
                };

            readonly UxmlStringAttributeDescription m_Theme =
                new UxmlStringAttributeDescription
                {
                    defaultValue = null,
                    name = "theme",
                    restriction = new UxmlEnumeration
                    {
                        values = new []{ "dark", "light", "editor-dark", "editor-light" }
                    }
                };

            readonly UxmlStringAttributeDescription m_Lang =
                new UxmlStringAttributeDescription
                {
                    defaultValue = null,
                    name = "lang",
                };

            readonly UxmlEnumAttributeDescription<Dir> m_Dir =
                new UxmlEnumAttributeDescription<Dir>
                {
                    defaultValue = Dir.Ltr,
                    name = "dir"
                };

#if ENABLE_ENABLED_UXML_PROPERTY
            readonly UxmlBoolAttributeDescription m_EnabledOverride =
                new UxmlBoolAttributeDescription
                {
                    defaultValue = true,
                    name = "enabled"
                };
#endif

            /// <summary>
            /// Initializes the VisualElement from the UXML attributes.
            /// </summary>
            /// <param name="ve"> The <see cref="VisualElement"/> to initialize.</param>
            /// <param name="bag"> The <see cref="IUxmlAttributes"/> bag to use to initialize the <see cref="VisualElement"/>.</param>
            /// <param name="cc"> The <see cref="CreationContext"/> to use to initialize the <see cref="VisualElement"/>.</param>
            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                var isFocusable = ve.focusable;
                base.Init(ve, bag, cc);

                var element = (BaseTextElement)ve;

                // small hack because UITK override the currently focusable state when building the element from UXML
                if (isFocusable)
                    ve.focusable = true;

                var preferredTooltipPlacement = Tooltip.defaultPlacement;
                if (m_PreferredTooltipPlacement.TryGetValueFromBag(bag, cc, ref preferredTooltipPlacement))
                    ve.SetPreferredTooltipPlacement(preferredTooltipPlacement);

                var tooltipDelayMs = TooltipManipulator.defaultDelayMs;
                if (m_TooltipDelayMs.TryGetValueFromBag(bag, cc, ref tooltipDelayMs))
                    element.tooltipDelayMsOverride = tooltipDelayMs;

                var scale = string.Empty;
                if (m_Scale.TryGetValueFromBag(bag, cc, ref scale) && !string.IsNullOrEmpty(scale))
                    element.scaleOverride = scale;

                var theme = string.Empty;
                if (m_Theme.TryGetValueFromBag(bag, cc, ref theme) && !string.IsNullOrEmpty(theme))
                    element.themeOverride = theme;

                var lang = string.Empty;
                if (m_Lang.TryGetValueFromBag(bag, cc, ref lang) && !string.IsNullOrEmpty(lang))
                    element.langOverride = lang;

                var dir = Dir.Ltr;
                if (m_Dir.TryGetValueFromBag(bag, cc, ref dir))
                    element.layoutDirectionOverride = dir;

#if ENABLE_ENABLED_UXML_PROPERTY
                var enabled = true;
                if (m_EnabledOverride.TryGetValueFromBag(bag, cc, ref enabled))
                    element.enabledOverride = enabled;
#endif
            }
        }
#endif
    }
}
