using System;
using System.Collections;
using System.Collections.Generic;
using Unity.AppUI.Bridge;
using Unity.AppUI.Core;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UIElements;
#if ENABLE_RUNTIME_DATA_BINDINGS
using Unity.Properties;
#endif

namespace Unity.AppUI.UI
{
    /// <summary>
    /// This is the main UI element of any Runtime App. The <see cref="Panel"/> class will create different
    /// UI layers for the main user-interface, popups, notifications and tooltips.
    /// </summary>
#if ENABLE_UXML_SERIALIZED_DATA
    [UxmlElement]
#endif
    public partial class Panel : VisualElement
    {
#if ENABLE_RUNTIME_DATA_BINDINGS
        internal static readonly BindingId scaleProperty = nameof(scale);

        internal static readonly BindingId themeProperty = nameof(theme);

        internal static readonly BindingId layoutDirectionProperty = nameof(layoutDirection);

        internal static readonly BindingId langProperty = nameof(lang);

        internal static readonly BindingId tooltipPlacementProperty = nameof(preferredTooltipPlacement);

        internal static readonly BindingId tooltipDelayMsProperty = nameof(tooltipDelayMs);

        internal static readonly BindingId forceUseTooltipSystemProperty = nameof(forceUseTooltipSystem);
#endif

        /// <summary>
        /// Main Uss Class Name.
        /// </summary>
        public const string ussClassName = "appui";

        /// <summary>
        /// Prefix used in App UI context USS classes.
        /// </summary>
        [EnumName("GetLayoutDirectionUssClassName", typeof(Dir))]
        public const string contextPrefix = "appui--";

        /// <summary>
        /// The name of the main UI layer.
        /// </summary>
        public const string mainContainerName = "main-container";

        /// <summary>
        /// The name of the Popups layer.
        /// </summary>
        public const string popupContainerName = "popup-container";

        /// <summary>
        /// The name of the Notifications layer.
        /// </summary>
        public const string notificationContainerName = "notification-container";

        /// <summary>
        /// The name of the Tooltip layer.
        /// </summary>
        public const string tooltipContainerName = "tooltip-container";

        /// <summary>
        /// The default language for this panel.
        /// </summary>
        internal const string defaultLang = "en";

        /// <summary>
        /// The default scale for this panel.
        /// </summary>
        internal const string defaultScale = "medium";

        /// <summary>
        /// The default theme for this panel.
        /// </summary>
        internal const string defaultTheme = "dark";

        /// <summary>
        /// The default layout direction for this panel.
        /// </summary>
        internal const Dir defaultDir = Dir.Ltr;

        string m_PreviousTheme;

        string m_PreviousScale;

        Dir m_PreviousDir;

        string m_PreviousLang;

        readonly VisualElement m_MainContainer;

        readonly VisualElement m_NotificationContainer;

        readonly VisualElement m_PopupContainer;

        readonly VisualElement m_TooltipContainer;

        TooltipManipulator m_TooltipManipulator;

        bool m_ForceUseTooltipSystem;

        float m_PreviousDpi = 96f;

        Vector2 m_PrimaryPointerPosition = Vector2.negativeInfinity;

        static readonly ObjectPool<Event> k_EventPool = new ObjectPool<Event>(() => new Event());

        static readonly EventCallback<UpdateEvent> k_UpdateCallback = new EventCallback<UpdateEvent>(OnUpdate);

#if UNITY_LOCALIZATION_PRESENT
        SelectedLocaleListener m_SelectedLocaleListener;
#endif

        /// <summary>
        /// Default constructor.
        /// </summary>
        public Panel()
        {
            AddToClassList(ussClassName);

            // Add a layer for the main UI
            m_MainContainer = new VisualElement { name = mainContainerName, pickingMode = PickingMode.Ignore };
            SetFixedFullScreen(m_MainContainer);
            hierarchy.Add(m_MainContainer);

            // Add a layer for popups stack (popovers, modals, trays)
            m_PopupContainer = new VisualElement { name = popupContainerName, pickingMode = PickingMode.Ignore };
            SetFixedFullScreen(m_PopupContainer);
            hierarchy.Add(m_PopupContainer);

            // Add a layer for notifications (snackbars, toasts)
            m_NotificationContainer = new VisualElement { name = notificationContainerName, pickingMode = PickingMode.Ignore };
            SetFixedFullScreen(m_NotificationContainer);
            m_NotificationContainer.style.flexDirection = FlexDirection.Column;
            m_NotificationContainer.style.alignItems = Align.Center;
            m_NotificationContainer.style.justifyContent = Justify.Center;
            hierarchy.Add(m_NotificationContainer);

            // Add a layer for tooltips
            m_TooltipContainer = new VisualElement { name = tooltipContainerName, pickingMode = PickingMode.Ignore };
            SetFixedFullScreen(m_TooltipContainer);
            hierarchy.Add(m_TooltipContainer);

            RegisterCallback<AttachToPanelEvent>(OnAttachedToPanel);
            RegisterCallback<DetachFromPanelEvent>(OnDetachedFromPanel);
            RegisterCallback<FocusOutEvent>(OnFocusOut);
            RegisterCallback<PointerMoveEvent>(OnPointerMoved);
            RegisterCallback<PointerLeaveEvent>(OnPointerLeft);

            this.RegisterContextChangedCallback<ThemeContext>(OnThemeContextChanged);
            this.RegisterContextChangedCallback<ScaleContext>(OnScaleContextChanged);
            this.RegisterContextChangedCallback<DirContext>(OnDirContextChanged);
            this.RegisterContextChangedCallback<LangContext>(OnLangContextChanged);

            lang = defaultLang;
            scale = defaultScale;
            theme = defaultTheme;
            layoutDirection = defaultDir;
            preferredTooltipPlacement = Tooltip.defaultPlacement;
            tooltipDelayMs = TooltipManipulator.defaultDelayMs;
        }

        void OnThemeContextChanged(ContextChangedEvent<ThemeContext> evt)
        {
            // only handle the event if it comes from this panel
            if (this.GetContextProvider<ThemeContext>() != this)
                return;

            var newTheme = evt.context?.theme;
            if (m_PreviousTheme != newTheme)
            {
                if (m_PreviousTheme != null)
                    RemoveFromClassList(MemoryUtils.Concatenate(contextPrefix, m_PreviousTheme));
                if (newTheme != null)
                    AddToClassList(MemoryUtils.Concatenate(contextPrefix, newTheme));

                m_PreviousTheme = newTheme;
            }
        }

        void OnScaleContextChanged(ContextChangedEvent<ScaleContext> evt)
        {
            // only handle the event if it comes from this panel
            if (this.GetContextProvider<ScaleContext>() != this)
                return;

            var newScale = evt.context?.scale;
            if (m_PreviousScale != newScale)
            {
                if (m_PreviousScale != null)
                    RemoveFromClassList(MemoryUtils.Concatenate(contextPrefix, m_PreviousScale));
                if (newScale != null)
                    AddToClassList(MemoryUtils.Concatenate(contextPrefix, newScale));

                m_PreviousScale = newScale;
            }
        }

        void OnDirContextChanged(ContextChangedEvent<DirContext> evt)
        {
            // only handle the event if it comes from this panel
            if (this.GetContextProvider<DirContext>() != this)
                return;

            var newDir = evt.context?.dir ?? defaultDir;
            // cannot check if previous value is different than the new one here because its an enum
            // but we don't need to check if class list contains the new/old values because it is already
            // checked by UIElements API
            AddToClassList(GetLayoutDirectionUssClassName(newDir));
            if (m_PreviousDir != newDir)
                RemoveFromClassList(GetLayoutDirectionUssClassName(m_PreviousDir));

            m_PreviousDir = newDir;
        }

        void OnLangContextChanged(ContextChangedEvent<LangContext> evt)
        {
            // only handle the event if it comes from this panel
            if (this.GetContextProvider<LangContext>() != this)
                return;

            var newLang = evt.context?.lang;
            if (m_PreviousLang != newLang)
            {
                if (m_PreviousLang != null)
                    RemoveFromClassList(MemoryUtils.Concatenate(contextPrefix, m_PreviousLang));
                if (newLang != null)
                    AddToClassList(MemoryUtils.Concatenate(contextPrefix, newLang));

                m_PreviousLang = newLang;
            }
        }

        /// <summary>
        /// The default language for this panel.
        /// </summary>
        [Tooltip("The default language for this panel.")]
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
        [Header("Panel")]
#endif
        public string lang
        {
            get => this.GetSelfContext<LangContext>()?.lang ?? defaultLang;
            set
            {
                var previous = this.GetSelfContext<LangContext>();
                if (previous == null || previous.lang != value)
                {
                    this.ProvideContext(string.IsNullOrEmpty(value) ? null : new LangContext(value));
#if ENABLE_RUNTIME_DATA_BINDINGS
                    NotifyPropertyChanged(in langProperty);
#endif
                }
            }
        }

        /// <summary>
        /// The default scale for this panel.
        /// </summary>
        [Tooltip("The default scale for this panel.")]
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public string scale
        {
            get => this.GetSelfContext<ScaleContext>()?.scale ?? defaultScale;
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw new ArgumentException("Scale cannot be null or empty on a Panel element.");
                var previous = this.GetSelfContext<ScaleContext>();
                if (previous?.scale != value)
                {
                    this.ProvideContext(new ScaleContext(value));
#if ENABLE_RUNTIME_DATA_BINDINGS
                    NotifyPropertyChanged(in scaleProperty);
#endif
                }
            }
        }

        /// <summary>
        /// The default theme for this panel.
        /// </summary>
        [Tooltip("The default theme for this panel.")]
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public string theme
        {
            get => this.GetSelfContext<ThemeContext>()?.theme ?? defaultTheme;
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw new ArgumentException("Theme cannot be null or empty on a Panel element.");
                var previous = this.GetSelfContext<ThemeContext>();
                if (previous?.theme != value)
                {
                    this.ProvideContext(new ThemeContext(value));
#if ENABLE_RUNTIME_DATA_BINDINGS
                    NotifyPropertyChanged(in themeProperty);
#endif
                }
            }
        }

        /// <summary>
        /// The default layout direction for this panel.
        /// </summary>
        [Tooltip("The default layout direction for this panel.")]
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public Dir layoutDirection
        {
            get => this.GetSelfContext<DirContext>()?.dir ?? Dir.Ltr;
            set
            {
                var previous = this.GetSelfContext<DirContext>();
                if (previous == null || previous.dir != value)
                {
                    this.ProvideContext(new DirContext(value));
#if ENABLE_RUNTIME_DATA_BINDINGS
                    NotifyPropertyChanged(in layoutDirectionProperty);
#endif
                }
            }
        }

        /// <summary>
        /// The default preferred tooltip placement for this panel.
        /// </summary>
        /// <remarks>
        /// Note that this is just the ideal placement, the tooltip will be placed on the opposite side if there is not enough space.
        /// </remarks>
        [Tooltip("The default preferred tooltip placement for this panel.\n" +
            "Note that this is just the ideal placement, the tooltip will be placed on the opposite side if there is not enough space.")]
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public PopoverPlacement preferredTooltipPlacement
        {
            get => this.GetSelfContext<TooltipPlacementContext>()?.placement ?? Tooltip.defaultPlacement;
            set
            {
                var previous = this.GetSelfContext<TooltipPlacementContext>();
                if (previous == null || previous.placement != value)
                {
                    this.ProvideContext(new TooltipPlacementContext(value));
#if ENABLE_RUNTIME_DATA_BINDINGS
                    NotifyPropertyChanged(in tooltipPlacementProperty);
#endif
                }
            }
        }

        /// <summary>
        /// The default tooltip delay in milliseconds for this panel.
        /// </summary>
        [Tooltip("The default tooltip delay in milliseconds for this panel.")]
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public int tooltipDelayMs
        {
            get => this.GetSelfContext<TooltipDelayContext>()?.tooltipDelayMs ?? TooltipManipulator.defaultDelayMs;
            set
            {
                var previous = this.GetSelfContext<TooltipDelayContext>();
                if (previous == null || previous.tooltipDelayMs != value)
                {
                    this.ProvideContext(new TooltipDelayContext(value));
#if ENABLE_RUNTIME_DATA_BINDINGS
                    NotifyPropertyChanged(in tooltipDelayMsProperty);
#endif
                }
            }
        }

        /// <summary>
        /// If true, the panel will use the tooltip system, even if the default UI-Toolkit tooltips are enabled.
        /// </summary>
        [Tooltip("Force the use of the tooltip system, even if the default UI-Toolkit tooltips are enabled.")]
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public bool forceUseTooltipSystem
        {
            get => m_ForceUseTooltipSystem;
            set
            {
                m_ForceUseTooltipSystem = value;
                if (m_TooltipManipulator != null)
                    m_TooltipManipulator.force = value;

#if ENABLE_RUNTIME_DATA_BINDINGS
                NotifyPropertyChanged(in forceUseTooltipSystemProperty);
#endif
            }
        }

        /// <summary>
        /// If true, this panel is the root panel of the application.
        /// </summary>
        internal bool isRootPanel { get; private set; }

        /// <summary>
        /// The main UI layer container.
        /// </summary>
        public override VisualElement contentContainer => m_MainContainer;

        /// <summary>
        /// The Popups layer container.
        /// </summary>
        public VisualElement popupContainer => m_PopupContainer;

        /// <summary>
        /// The Notifications layer container.
        /// </summary>
        public VisualElement notificationContainer => m_NotificationContainer;

        /// <summary>
        /// The Tooltip layer container.
        /// </summary>
        public VisualElement tooltipContainer => m_TooltipContainer;

        void OnDetachedFromPanel(DetachFromPanelEvent evt)
        {
            UnregisterCallback<UpdateEvent>(OnUpdate);
            Platform.scaleFactorChanged -= OnScaleFactorChanged;
            DismissAllPopups();
            isRootPanel = false;
            if (m_TooltipManipulator != null)
                this.RemoveManipulator(m_TooltipManipulator);
#if UNITY_LOCALIZATION_PRESENT
            if (m_SelectedLocaleListener != null)
                this.RemoveManipulator(m_SelectedLocaleListener);
#endif
        }

        void OnFocusOut(FocusOutEvent evt)
        {
            if (!isRootPanel)
                return;

            // we receive focus out event even when calling Focus() on a child element,
            // we need to filter out those cases
            if (evt.relatedTarget is VisualElement target && target.FindCommonAncestor(this) == this)
                return;

            DismissAllPopups(DismissType.OutOfBounds);
        }

        internal void DismissAllPopups(DismissType reason = DismissType.Manual)
        {
            for (var i = m_PopupContainer.childCount - 1; i >= 0; i--)
            {
                var popupElement = m_PopupContainer[i];
                if (popupElement.userData is Popup popup)
                {
                    var shouldDismiss = reason == DismissType.Manual ||
                        (reason == DismissType.OutOfBounds && popup.focusOutDismissable);
                    if (shouldDismiss)
                        popup.Dismiss(reason);
                }
            }
        }

        void OnPointerMoved(PointerMoveEvent evt)
        {
            if (evt.pointerId == PointerId.mousePointerId || evt.isPrimary)
                m_PrimaryPointerPosition = evt.position;
        }

        void OnPointerLeft(PointerLeaveEvent evt)
        {
            if (evt.pointerId == PointerId.mousePointerId || evt.isPrimary)
                m_PrimaryPointerPosition = Vector2.negativeInfinity;
        }

        void OnAttachedToPanel(AttachToPanelEvent evt)
        {
            if (evt.destinationPanel != null)
            {
                m_TooltipManipulator ??= new TooltipManipulator();
                this.AddManipulator(m_TooltipManipulator);
                m_TooltipManipulator.force = forceUseTooltipSystem;

                isRootPanel = !this.HasAncestorsOfType<Panel>();
                if (isRootPanel)
                {
                    if (global::Unity.AppUI.Core.AppUI.settings.autoCorrectUiScale &&
                        evt.destinationPanel.contextType == ContextType.Player)
                    {
                        m_PreviousDpi = panel.GetPanelSettings().referenceDpi;
                        // we wait to let a chance fo others panels to set their previous DPI correctly
                        schedule.Execute(() => OnScaleFactorChanged(m_PreviousDpi));
                    }
                    Platform.scaleFactorChanged -= OnScaleFactorChanged;
                    Platform.scaleFactorChanged += OnScaleFactorChanged;
                    this.RegisterUpdateCallback(k_UpdateCallback);
#if UNITY_LOCALIZATION_PRESENT
                    m_SelectedLocaleListener ??= new SelectedLocaleListener();
                    this.AddManipulator(m_SelectedLocaleListener);
#endif
                }
            }
        }

        static void OnUpdate(UpdateEvent e)
        {
            if (e.target is not Panel panelElement)
                return;

            // skip sending event to unattached panels
            var iPanel = panelElement.panel;
            if (iPanel == null)
                return;

            var shouldPickElement = AppUIInput.pinchGestureChangedThisFrame;
            var pickedElement = shouldPickElement ? iPanel.Pick(panelElement.m_PrimaryPointerPosition) : null;
            var shouldHandleGestures = pickedElement != null;

            if (AppUIInput.pinchGestureChangedThisFrame)
            {
                if (shouldHandleGestures)
                {
                    using var evt = PinchGestureEvent.GetPooled();
                    evt.gesture = AppUIInput.pinchGesture;
                    evt.target = pickedElement;
                    panelElement.SendEvent(evt);

                    var systemEvent = k_EventPool.Get();
                    systemEvent.type = EventType.ScrollWheel;
                    systemEvent.pointerType = UnityEngine.PointerType.Mouse;
                    systemEvent.modifiers = EventModifiers.Control;
                    systemEvent.mousePosition = panelElement.m_PrimaryPointerPosition;
                    systemEvent.delta = evt.gesture.scrollDelta;
                    systemEvent.button = global::Unity.AppUI.Core.AppUI.touchPadId;
                    systemEvent.clickCount = 0;

                    using var evt2 = WheelEvent.GetPooled(systemEvent);
                    evt2.target = pickedElement;
                    panelElement.SendEvent(evt2);

                    k_EventPool.Release(systemEvent);
                }
            }
        }

        void OnScaleFactorChanged(float _)
        {
            if (!isRootPanel ||
                panel is not {contextType: ContextType.Player} ||
                !global::Unity.AppUI.Core.AppUI.settings.autoCorrectUiScale)
                return;

            try
            {
                var dpi = Platform.referenceDpi;
                var panelSettings = panel?.GetPanelSettings();
                if (panelSettings)
                {
                    var isValidDpi = dpi > 0;
                    var dpiChanged = !Mathf.Approximately(m_PreviousDpi, dpi);

                    if (dpiChanged && isValidDpi)
                    {
                        panelSettings.referenceDpi = dpi;
                        // send event
                        using var evt = DpiChangedEvent.GetPooled();
                        evt.previousValue = m_PreviousDpi;
                        evt.newValue = dpi;
                        evt.target = this;
                        SendEvent(evt);
                        m_PreviousDpi = dpi;
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        /// <summary>
        /// Utility method to quickly find the current application's Notification layer.
        /// </summary>
        /// <param name="element">An element present in the application visual tree.</param>
        /// <returns>The Notification layer container.</returns>
        public static VisualElement FindNotificationLayer(VisualElement element)
        {
            if (element is Panel app)
                return app.notificationContainer;
            return element.GetFirstAncestorOfType<Panel>()?.notificationContainer;
        }

        /// <summary>
        /// Utility method to quickly find the current application's Popup layer.
        /// </summary>
        /// <param name="element">An element present in the application visual tree.</param>
        /// <returns>The Popup layer container.</returns>
        public static VisualElement FindPopupLayer(VisualElement element)
        {
            if (element is Panel app)
                return app.popupContainer;
            return element.GetFirstAncestorOfType<Panel>()?.popupContainer;
        }

        /// <summary>
        /// Utility method to quickly find the current application's Tooltip layer.
        /// </summary>
        /// <param name="element">An element present in the application visual tree.</param>
        /// <returns>The Tooltip layer container.</returns>
        public static VisualElement FindTooltipLayer(VisualElement element)
        {
            if (element is Panel app)
                return app.tooltipContainer;
            return element.GetFirstAncestorOfType<Panel>()?.tooltipContainer;
        }

        static void SetFixedFullScreen(VisualElement element)
        {
            element.style.position = Position.Absolute;
            element.style.top = 0;
            element.style.bottom = 0;
            element.style.left = 0;
            element.style.right = 0;
        }

#if ENABLE_UXML_TRAITS
        /// <summary>
        /// Class used to create instances of <see cref="Panel"/> from UXML.
        /// </summary>
        public new class UxmlFactory : UxmlFactory<Panel, UxmlTraits> { }

        /// <summary>
        /// Class containing the <see cref="UxmlTraits"/> for the <see cref="Panel"/>.
        /// </summary>
        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            readonly UxmlStringAttributeDescription m_Lang = new UxmlStringAttributeDescription
            {
                name = "lang",
                defaultValue = defaultLang
            };

            readonly UxmlStringAttributeDescription m_Scale = new UxmlStringAttributeDescription
            {
                name = "scale",
                defaultValue = defaultScale,
                restriction = new UxmlEnumeration
                {
                    values = new[] { "small", "medium", "large" }
                }
            };

            readonly UxmlStringAttributeDescription m_Theme = new UxmlStringAttributeDescription
            {
                name = "theme",
                defaultValue = defaultTheme,
                restriction = new UxmlEnumeration
                {
                    values = new[] { "light", "dark", "editor-dark", "editor-light" }
                }
            };

            readonly UxmlEnumAttributeDescription<Dir> m_Dir = new UxmlEnumAttributeDescription<Dir>
            {
                name = "dir",
                defaultValue = defaultDir
            };

            readonly UxmlEnumAttributeDescription<PopoverPlacement> m_PreferredTooltipPlacement = new UxmlEnumAttributeDescription<PopoverPlacement>
            {
                name = "preferred-tooltip-placement",
                defaultValue = Tooltip.defaultPlacement
            };

            readonly UxmlIntAttributeDescription m_TooltipDelayMs = new UxmlIntAttributeDescription
            {
                name = "tooltip-delay-ms",
                defaultValue = TooltipManipulator.defaultDelayMs
            };

            readonly UxmlBoolAttributeDescription m_ForceUseTooltipSystem = new UxmlBoolAttributeDescription
            {
                name = "force-use-tooltip-system",
                defaultValue = false
            };

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);

                var panel = (Panel)ve;

                panel.lang = m_Lang.GetValueFromBag(bag, cc);
                panel.scale = m_Scale.GetValueFromBag(bag, cc);
                panel.theme = m_Theme.GetValueFromBag(bag, cc);
                panel.layoutDirection = m_Dir.GetValueFromBag(bag, cc);
                panel.preferredTooltipPlacement = m_PreferredTooltipPlacement.GetValueFromBag(bag, cc);
                panel.tooltipDelayMs = m_TooltipDelayMs.GetValueFromBag(bag, cc);
                panel.forceUseTooltipSystem = m_ForceUseTooltipSystem.GetValueFromBag(bag, cc);
            }
        }
#endif
    }
}
