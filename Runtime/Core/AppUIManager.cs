using System;
using System.Collections.Generic;
using Unity.AppUI.Bridge;
using Unity.AppUI.UI;
using UnityEngine;
using UnityEngine.UIElements;
using PointerType = UnityEngine.PointerType;
#if UNITY_INPUTSYSTEM_PRESENT
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.EventSystems;
#endif

namespace Unity.AppUI.Core
{
    /// <summary>
    /// The main manager for the AppUI system.
    /// This class is responsible for managing the main looper, the notification manager and the settings.
    /// It also provides access to them.
    /// </summary>
    public class AppUIManager
    {
        // ReSharper disable once InconsistentNaming
        internal AppUISettings m_Settings;

        Looper m_MainLooper;

        readonly Dictionary<PanelSettings, HashSet<Panel>> m_PanelSettings = new Dictionary<PanelSettings, HashSet<Panel>>();

        readonly HashSet<Panel> m_Panels = new HashSet<Panel>();

        readonly Dictionary<IPanel, HashSet<Popup>> m_DismissablePopupsPerPanel = new Dictionary<IPanel, HashSet<Popup>>();

        NotificationManager m_NotificationManager;

        internal AppUISettings defaultSettings { get; private set; }

        /// <summary>
        /// The current settings.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when the settings are not ready.</exception>
        /// <exception cref="ArgumentNullException">Thrown when the provided settings object is null.</exception>
        internal AppUISettings settings
        {
            get
            {
                if (m_Settings)
                    return m_Settings;

                if (!defaultSettings)
                    defaultSettings = ScriptableObject.CreateInstance<AppUISettings>();

                return defaultSettings;
            }
            set
            {
                if (!value)
                    throw new ArgumentNullException(nameof(value));

                if (m_Settings == value)
                    return;

                m_Settings = value;
                ApplySettings();
            }
        }

        /// <summary>
        /// The main looper.
        /// </summary>
        internal Looper mainLooper => m_MainLooper;

        /// <summary>
        /// The notification manager.
        /// </summary>
        internal NotificationManager notificationManager => m_NotificationManager;

        /// <summary>
        /// Default constructor.
        /// </summary>
        internal AppUIManager()
        {

        }

        /// <summary>
        /// Initializes the AppUIManager.
        /// </summary>
        /// <param name="newSettings">The settings to use.</param>
        internal void Initialize(AppUISettings newSettings)
        {
            Debug.Assert(newSettings, "The provided settings object can't be null");

            m_Settings = newSettings;
            m_MainLooper = new Looper();
            m_NotificationManager = new NotificationManager(this);
            AppUIInput.Initialize();

            m_MainLooper.Loop();

            ApplySettings();

#if UNITY_INPUTSYSTEM_PRESENT && ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
            InputSystem.onActionChange -= OnActionChange;
            InputSystem.onActionChange += OnActionChange;
#endif
        }

#if ENABLE_INPUT_SYSTEM
#pragma warning disable 414
        Vector2 m_PointerPosition = Vector2.zero;
        bool m_PointerDown = false;
#pragma warning restore 414
#endif

#if UNITY_INPUTSYSTEM_PRESENT && ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER

        void OnActionChange(object arg1, InputActionChange arg2)
        {
            var module = EventSystem.current ? EventSystem.current.currentInputModule as InputSystemUIInputModule : null;
            if (arg2 == InputActionChange.ActionPerformed && arg1 is InputAction action && module)
            {
                if (action.name == module.leftClick.action.name)
                    m_PointerDown = true;
                else if (action.name == module.point.action.name)
                    m_PointerPosition = action.ReadValue<Vector2>();
            }
        }
#endif

        /// <summary>
        /// Shutdown the AppUIManager.
        /// </summary>
        internal void Shutdown()
        {
            m_MainLooper.Quit();
        }

        /// <summary>
        /// Applies the current settings.
        /// </summary>
        internal void ApplySettings()
        {
            Platform.scaleFactorChanged -= OnScaleFactorChanged;
            if (AppUI.settings.autoCorrectUiScale)
                Platform.scaleFactorChanged += OnScaleFactorChanged;
        }

        /// <summary>
        /// Clean up the AppUIManager.
        /// </summary>
        internal void Cleanup()
        {
            m_MainLooper?.RemoveCallbacksAndMessages(null);
            UnregisterAllPanels();
            UnregisterAllPopups();
        }

        void OnScaleFactorChanged(float _)
        {
            if (!m_Settings.autoCorrectUiScale)
                return;

            var dpi = Platform.referenceDpi;
            foreach (var panelSettings in m_PanelSettings.Keys)
            {
                if (panelSettings is null)
                    continue;

                var previousDpi = panelSettings.referenceDpi;
                var dpiChanged = !Mathf.Approximately(panelSettings.referenceDpi, dpi);

                if (dpiChanged)
                    panelSettings.referenceDpi = dpi;

                foreach (var panel in m_PanelSettings[panelSettings])
                {
                    if (panel is not {panel: not null})
                        continue;

                    if (dpiChanged)
                    {
                        using var evt = DpiChangedEvent.GetPooled();
                        evt.previousValue = previousDpi;
                        evt.newValue = dpi;
                        evt.target = panel;
                        panel.SendEvent(evt);
                    }
                }
            }
        }

        /// <summary>
        /// Called when the application focus changes.
        /// </summary>
        /// <param name="hasFocus"> Whether the application has focus or not. </param>
        internal void OnApplicationFocus(bool hasFocus)
        {
            AppUIInput.Reset();
        }

        /// <summary>
        /// Update method that should be called every frame.
        /// </summary>
        internal void Update()
        {
            Platform.Update();
            AppUIInput.PollEvents();

#if ENABLE_LEGACY_INPUT_MANAGER
            var mousePosition = Input.mousePosition;
            var mouseLeftButtonDown = Input.GetMouseButtonDown(0);
#elif ENABLE_INPUT_SYSTEM
            var mousePosition = m_PointerPosition;
            var mouseLeftButtonDown = m_PointerDown;
            m_PointerDown = false;
#endif
            mousePosition = new Vector2(mousePosition.x, UnityEngine.Device.Screen.height - mousePosition.y);

            if (m_Panels is {Count: > 0})
            {
                foreach (var panel in m_Panels)
                {
                    if (panel is not {panel: { } iPanel})
                        continue;

                    VisualElement pickedElement = null;
                    var needToFindPickedElement = mouseLeftButtonDown || AppUIInput.pinchGestureChangedThisFrame;
                    if (needToFindPickedElement && iPanel.contextType == ContextType.Player)
                    {
                        var coord = RuntimePanelUtils.ScreenToPanel(iPanel, mousePosition);
                        pickedElement = iPanel.Pick(coord);
                    }

                    if (mouseLeftButtonDown && pickedElement == null)
                        DismissAnyPopups(iPanel, DismissType.OutOfBounds);

#if UNITY_EDITOR
                    if (needToFindPickedElement && iPanel.contextType == ContextType.Editor &&
                        UnityEditor.EditorWindow.focusedWindow &&
                        UnityEditor.EditorWindow.focusedWindow.rootVisualElement?.panel == iPanel)
                        pickedElement ??= iPanel.focusController.focusedElement as VisualElement;
#endif
                    if (AppUIInput.pinchGestureChangedThisFrame && pickedElement != null)
                    {
                        using var evt = PinchGestureEvent.GetPooled();
                        evt.gesture = AppUIInput.pinchGesture;
                        evt.target = pickedElement;
                        panel.SendEvent(evt);

                        var systemEvent = new Event()
                        {
                            type = EventType.ScrollWheel,
                            pointerType = PointerType.Mouse,
                            modifiers = EventModifiers.Control,
                            mousePosition = mousePosition,
                            delta = evt.gesture.scrollDelta,
                            button = AppUI.touchPadId,
                            clickCount = 0,
                        };
                        using var evt2 = WheelEvent.GetPooled(systemEvent);
                        evt2.target = pickedElement;
                        panel.SendEvent(evt2);
                    }
                }
            }

            m_MainLooper.LoopOnce();
        }

        /// <summary>
        /// Dismisses all popups.
        /// </summary>
        /// <param name="panel"> The panel that owns the popups. </param>
        /// <param name="reason"> The reason for dismissing the popups. </param>
        internal void DismissAnyPopups(IPanel panel, DismissType reason)
        {
            if (panel == null)
                return;

            if (m_DismissablePopupsPerPanel.TryGetValue(panel, out var popups))
            {
                foreach (var popup in popups)
                {
                    popup?.Dismiss(reason);
                }
                popups.Clear();
            }
        }

        /// <summary>
        /// Registers a panel.
        /// </summary>
        /// <param name="element">The panel to register.</param>
        /// <exception cref="ArgumentNullException">Thrown when the provided panel is null.</exception>
        internal void RegisterPanel(Panel element)
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));

            m_Panels.Add(element);

            var panelSettings = element.panel?.GetPanelSettings();
            if (panelSettings == null)
                return;

            if (m_PanelSettings.TryGetValue(panelSettings, out var setting))
                setting.Add(element);
            else
                m_PanelSettings.Add(panelSettings, new HashSet<Panel> { element });

            OnScaleFactorChanged(1.0f);
        }

        /// <summary>
        /// Unregisters a panel.
        /// </summary>
        /// <param name="iPanel">The UITK Panel that owns the panel.</param>
        /// <param name="panel">The panel to unregister.</param>
        /// <exception cref="ArgumentNullException">Thrown when the provided panel is null.</exception>
        internal void UnregisterPanel(IPanel iPanel, Panel panel)
        {
            if (panel == null)
                throw new ArgumentNullException(nameof(panel));

            UnregisterPopups(iPanel);
            m_Panels.Remove(panel);

            var panelSettings = iPanel?.GetPanelSettings();
            if (panelSettings == null)
                return;

            if (m_PanelSettings.ContainsKey(panelSettings))
            {
                m_PanelSettings[panelSettings].Remove(panel);
                if (m_PanelSettings[panelSettings].Count == 0)
                    m_PanelSettings.Remove(panelSettings);
            }
        }

        /// <summary>
        /// Unregisters all panels.
        /// </summary>
        internal void UnregisterAllPanels()
        {
            m_PanelSettings.Clear();
            m_Panels.Clear();
        }

        /// <summary>
        /// Registers a popup.
        /// </summary>
        /// <param name="panel"> The panel that owns the popup. </param>
        /// <param name="popup"> The popup to register. </param>
        internal void RegisterPopup(IPanel panel, Popup popup)
        {
            if (panel == null || popup == null)
                return;

            if (!m_DismissablePopupsPerPanel.TryGetValue(panel, out var popups))
            {
                popups = new HashSet<Popup>();
                m_DismissablePopupsPerPanel.Add(panel, popups);
            }
            popups.Add(popup);
        }

        /// <summary>
        /// Unregisters a popup.
        /// </summary>
        /// <param name="panel"> The panel that owns the popup. </param>
        /// <param name="popup"> The popup to unregister. </param>
        internal void UnregisterPopup(IPanel panel, Popup popup)
        {
            if (panel == null || popup == null)
                return;

            if (m_DismissablePopupsPerPanel.TryGetValue(panel, out var popups))
            {
                popups.Remove(popup);
                if (popups.Count == 0)
                    m_DismissablePopupsPerPanel.Remove(panel);
            }
        }

        /// <summary>
        /// Unregisters all popups.
        /// </summary>
        internal void UnregisterAllPopups()
        {
            m_DismissablePopupsPerPanel.Clear();
        }

        /// <summary>
        /// Unregisters all popups for a panel.
        /// </summary>
        /// <param name="panel"> The panel that owns the popups. </param>
        internal void UnregisterPopups(IPanel panel)
        {
            if (panel == null)
                return;

            m_DismissablePopupsPerPanel.Remove(panel);
        }
    }
}
