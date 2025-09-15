using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.AppUI.UI;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UIElements;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Build;
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

        readonly List<WeakReference<VisualElement>> m_UpdateCallbacks = new ();

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
        }

        /// <summary>
        /// Shutdown the AppUIManager.
        /// </summary>
        internal void Shutdown()
        {
            Cleanup();
            m_MainLooper.Quit();
        }

        /// <summary>
        /// Applies the current settings.
        /// </summary>
        internal void ApplySettings()
        {
#if UNITY_EDITOR
            if (EditorUtility.IsPersistent(m_Settings))
                EditorBuildSettings.AddConfigObject(AppUISettings.configName, m_Settings, true);

            EditorApplication.delayCall += () =>
            {
                const string kEditorOnlyDefine = "APP_UI_EDITOR_ONLY";
                var group = EditorUserBuildSettings.selectedBuildTargetGroup;
                var target = NamedBuildTarget.FromBuildTargetGroup(group);
                var defineStr = PlayerSettings.GetScriptingDefineSymbols(target);
                var defines = new HashSet<string>(defineStr.Split(";"));
                if (m_Settings.editorOnly)
                    defines.Add(kEditorOnlyDefine);
                else
                    defines.Remove(kEditorOnlyDefine);
                var newDefineStr = string.Join(";", defines);
                if (newDefineStr.Length != defineStr.Length)
                    PlayerSettings.SetScriptingDefineSymbols(target, newDefineStr);
            };
#endif
        }

        /// <summary>
        /// Clean up the AppUIManager.
        /// </summary>
        internal void Cleanup()
        {
            m_UpdateCallbacks.Clear();
            m_MainLooper?.RemoveCallbacksAndMessages(null);
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
            m_MainLooper.LoopOnce();
            InvokeUpdateCallbacks();
        }

        internal void RegisterUpdateCallback(VisualElement updatableElement)
        {
            // check if the callback is already registered
            foreach (var t in m_UpdateCallbacks)
            {
                if (t.TryGetTarget(out var updatable) && updatable == updatableElement)
                    return;
            }

            m_UpdateCallbacks.Add(new WeakReference<VisualElement>(updatableElement));
        }

        internal void UnregisterUpdateCallback(VisualElement updatableElement)
        {
            for (var i = 0; i < m_UpdateCallbacks.Count; i++)
            {
                if (m_UpdateCallbacks[i].TryGetTarget(out var updatable) && updatable == updatableElement)
                {
                    m_UpdateCallbacks.RemoveAt(i);
                    return;
                }
            }
        }

        void InvokeUpdateCallbacks()
        {
            m_UpdateCallbacks.RemoveAll(wr =>
            {
                if (wr.TryGetTarget(out var updatable))
                {
                    using var evt = UpdateEvent.GetPooled();
                    evt.target = updatable;
                    updatable.SendEvent(evt);
                    return false;
                }
                return true;
            });
        }
    }
}
