using System;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Unity.AppUI.Core
{
    /// <summary>
    /// A dummy object used to store some data in the editor.
    /// </summary>
    class AppUISystemObject : ScriptableObject
    {
        public string settings;
    }

    /// <summary>
    /// The main entry point for the App UI system.
    /// </summary>
#if UNITY_EDITOR
    [InitializeOnLoad]
#endif
    public static class AppUI
    {
        /// <summary>
        /// The id of the touchpad button in synthesized mouse events.
        /// </summary>
        public const int touchPadId = 3;

        static AppUISystemObject s_SystemObject;

        internal static AppUIManager s_Manager;

        internal static AppUIManagerBehaviour gameObject
        {
            get
            {
                if (!AppUIManagerBehaviour.instance)
                {
                    if (Application.isPlaying)
                    {
                        AppUIManagerBehaviour.Create();
                        Assert.IsNotNull(AppUIManagerBehaviour.instance);
                    }
                    else
                    {
                        Debug.LogError("Trying to access AppUIManagerBehaviour.instance in edit mode. " +
                            "This instance is only available in play mode.");
                    }
                }

                return AppUIManagerBehaviour.instance;
            }
        }

        /// <summary>
        /// Initialize the App UI system.
        /// </summary>
        static AppUI()
        {
#if UNITY_EDITOR
            InitializeInEditor();
#else
            InitializeInPlayer();
#endif
        }

        /// <summary>
        /// The main looper of the App UI system.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if the App UI system is not ready.</exception>
        public static Looper mainLooper
        {
            get
            {
                if (s_Manager == null)
                    throw new InvalidOperationException("The App UI Manager is not ready");

                return s_Manager.mainLooper;
            }
        }

        /// <summary>
        /// The notification manager of the App UI system.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if the App UI system is not ready.</exception>
        internal static NotificationManager notificationManager
        {
            get
            {
                if (s_Manager == null)
                    throw new InvalidOperationException("The App UI Manager is not ready");

                return s_Manager.notificationManager;
            }
        }

        /// <summary>
        /// Dummy method to ensure the App UI system is initialized.
        /// </summary>
        internal static void EnsureInitialized() { }

        /// <summary>
        /// The update method that must be called every frame.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if the App UI system is not ready.</exception>
        internal static void Update()
        {
            if (s_Manager == null)
                throw new InvalidOperationException("The App UI Manager is not ready");

            s_Manager.Update();
        }

        internal static void RegisterUpdateCallback(VisualElement updatableElement)
        {
            s_Manager?.RegisterUpdateCallback(updatableElement);
        }

        internal static void UnregisterUpdateCallback(VisualElement updatableElement)
        {
            s_Manager?.UnregisterUpdateCallback(updatableElement);
        }

        /// <summary>
        /// Manage internal App UI features when the application has gained or lost focus.
        /// </summary>
        /// <param name="hasFocus"></param>
        internal static void OnApplicationFocus(bool hasFocus)
        {
            s_Manager?.OnApplicationFocus(hasFocus);
        }

#if UNITY_EDITOR

        static uint s_UpdateFrame = 0;

        const uint k_EditorUpdateDelay = 20;

        /// <summary>
        /// Initialize the App UI system in the editor.
        /// </summary>
        static void InitializeInEditor()
        {
            Reset();

            var existingSystemObjects = Resources.FindObjectsOfTypeAll<AppUISystemObject>();
            if (existingSystemObjects is {Length: > 0})
            {
                s_SystemObject = existingSystemObjects[0];
                // here we can restore some state saved inside the system object
            }
            else
            {
                s_SystemObject = ScriptableObject.CreateInstance<AppUISystemObject>();
                s_SystemObject.hideFlags = HideFlags.HideAndDontSave;
            }

            AppUISettings newSettings = null;
            if (EditorBuildSettings.TryGetConfigObject(AppUISettings.configName,
                    out AppUISettings settingsAsset))
            {
                newSettings = settingsAsset;
            }
            else
            {
                // nothing found yet, we can try to find the asset database
                var settingsGuids = AssetDatabase.FindAssets($"t:{nameof(AppUISettings)} a:all");
                if (settingsGuids.Length > 0)
                {
                    var settingsPath = AssetDatabase.GUIDToAssetPath(settingsGuids[0]);
                    newSettings = AssetDatabase.LoadAssetAtPath<AppUISettings>(settingsPath);
                }
            }

            if (newSettings)
            {
                if (s_Manager.m_Settings.hideFlags == HideFlags.HideAndDontSave)
                    Object.DestroyImmediate(s_Manager.m_Settings);
                s_Manager.m_Settings = newSettings;
                // here we can apply new settings on managers
                s_Manager.ApplySettings();
            }
        }

        static void Reset()
        {
            s_Manager?.Shutdown();

            var newSettings = ScriptableObject.CreateInstance<AppUISettings>();
            newSettings.hideFlags = HideFlags.HideAndDontSave;

            s_Manager = new AppUIManager();
            s_Manager.Initialize(newSettings);

            // here we can reset others managers

            EditorApplication.projectChanged -= OnProjectChange;
            EditorApplication.playModeStateChanged -= OnPlayModeChange;
            EditorApplication.projectChanged += OnProjectChange;
            EditorApplication.playModeStateChanged += OnPlayModeChange;
            EditorApplication.update -= EditorUpdate;
            EditorApplication.update += EditorUpdate;
#if UNITY_2022_2_OR_NEWER
            EditorApplication.focusChanged -= OnApplicationFocus;
            EditorApplication.focusChanged += OnApplicationFocus;
#endif
        }

        static void OnPlayModeChange(PlayModeStateChange change)
        {
            switch (change)
            {
                case PlayModeStateChange.EnteredEditMode:
                    if (!string.IsNullOrEmpty(s_SystemObject.settings))
                    {
                        JsonUtility.FromJsonOverwrite(s_SystemObject.settings, settings);
                        s_SystemObject.settings = null;
                        settings.OnChange();
                    }
                    break;
                case PlayModeStateChange.ExitingEditMode:
                    s_SystemObject.settings = JsonUtility.ToJson(settings);
                    break;
                case PlayModeStateChange.EnteredPlayMode:
                    break;
                case PlayModeStateChange.ExitingPlayMode:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(change), change, null);
            }
        }

        static void OnProjectChange()
        {
            if (EditorUtility.InstanceIDToObject(settings.GetInstanceID()) == null)
            {
                var newSettings = ScriptableObject.CreateInstance<AppUISettings>();
                newSettings.hideFlags = HideFlags.HideAndDontSave;
                settings = newSettings;
            }
        }

        static double s_PreviousTime = 0;

        static void EditorUpdate()
        {
            if (s_UpdateFrame < k_EditorUpdateDelay)
                s_UpdateFrame++;

            if (s_UpdateFrame < k_EditorUpdateDelay && s_Manager == null)
                return;

            if (EditorApplication.isCompiling)
            {
                s_Manager.Cleanup();
                return;
            }

            if (settings.useCustomEditorUpdateFrequency)
            {
                var currentTime = Time.realtimeSinceStartupAsDouble;
                var delta = currentTime - s_PreviousTime;
                if (delta >= 1.0 / settings.editorUpdateFrequency)
                {
                    s_Manager.Update();
                    s_PreviousTime = currentTime;
                }
            }
            else
            {
                s_Manager.Update();
            }
        }
#else

        static void InitializeInPlayer()
        {
            var settings = Resources.FindObjectsOfTypeAll<AppUISettings>();
            var newSettings = settings.Length > 0 ? settings[0] : null;
            if (!newSettings)
            {
                Debug.LogWarning("<b>[App UI]</b> Unable to find an AppUISettings instance, creating a default one...");
                newSettings = ScriptableObject.CreateInstance<AppUISettings>();
            }

            s_Manager = new AppUIManager();
            s_Manager.Initialize(newSettings);
        }

#endif

        [RuntimeInitializeOnLoadMethod(loadType: RuntimeInitializeLoadType.SubsystemRegistration)]
        static void RunInitializeInPlayer()
        {
            // IL2CPP has a bug that causes the class constructor to not be run when
            // the RuntimeInitializeOnLoadMethod is invoked. So we need an explicit check
            // here until that is fixed (case 1014293).
#if !UNITY_EDITOR
            if (s_Manager == null)
                InitializeInPlayer();
#endif
        }

        /// <summary>
        /// The settings used by the App UI system.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown when the value is null.</exception>
        public static AppUISettings settings
        {
            get => s_Manager.settings;
            internal set => s_Manager.settings = value;
        }
    }
}
