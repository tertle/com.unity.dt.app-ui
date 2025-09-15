#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using Unity.AppUI.Core;
using UnityEngine;

namespace Unity.AppUI.Editor
{
    class AppUISettingsProvider : SettingsProvider, IDisposable
    {
        public const string kSettingsPath = "Project/App UI";

        class Styles
        {
            public static readonly GUIContent editorOnlyContent = new GUIContent("Editor Only", "Enable this options to prevent the App UI system from running in the player builds.");
            public static readonly GUIContent autoScaleUIContent = new GUIContent("Auto Scale UI", "Enable this options to correct the scale of UIDocuments, depending on the target platform and screen dpi.");
            public static readonly GUIContent useCustomEditorUpdateFrequencyContent = new GUIContent("Use Custom Loop Frequency", "Enable this option to override the default update loop frequency (the default frequency is the one used by the Editor loop).");
            public static readonly GUIContent editorUpdateFrequencyContent = new GUIContent("Update Loop Frequency", "Configure how frequently you want to run the main App UI process loop (to handle queued messages for examples). Default is 60Hz.");
            public static readonly GUIContent autoOverrideAndroidManifestContent = new GUIContent("Auto Override Android Manifest", "");
            public static readonly GUIContent enableMacOSGestureRecognitionContent = new GUIContent("Enable Gesture Recognition", "");
            public static readonly GUIContent includeShadersInPlayerBuildContent = new GUIContent("Include Shaders in Player Build", "");

            public static readonly GUIContent createSettingsAssetContent = new GUIContent("Create Settings Asset", "Create a new App UI settings asset in the project.");
            public static readonly GUIContent settingsAssetFieldContent = new GUIContent("Settings Asset", "The App UI settings asset to use for this project. If not set, a default one will be created.");

            public static readonly GUIContent editorGroup = new GUIContent("Editor");
            public static readonly GUIContent runtimeGroup = new GUIContent("Runtime");
            public static readonly GUIContent androidGroup = new GUIContent("Android");
            public static readonly GUIContent macOSGroup = new GUIContent("MacOS");

            public static readonly GUIContent androidManifestOverrideWarning = new GUIContent("In order to get all features working properly on Android, you need to override the default Android manifest file with the one provided by App UI.");
            public static readonly GUIContent macOSGestureRecognitionWarning = new GUIContent("Enabling gesture recognition on MacOS will allow you to use gestures such as pinch, pan, and rotate. However, some gestures are synthesized as mouse events and can't be avoided, such as the Pan gesture.");
        }

        public static void Open()
        {
            SettingsService.OpenProjectSettings(kSettingsPath);
        }

        [SettingsProvider]
        public static SettingsProvider CreateAppUISettingsProvider()
        {
            return new AppUISettingsProvider(kSettingsPath, SettingsScope.Project);
        }

        AppUISettingsProvider(string path, SettingsScope scopes)
            : base(path, scopes)
        {
            label = "App UI";
            keywords = GetSearchKeywordsFromGUIContentProperties<Styles>();
            s_Instance = this;
        }

        public void Dispose()
        {
            m_SettingsObject?.Dispose();
        }

        static bool IsInReadOnlyPackage(AppUISettings settings)
        {
            if (!settings)
                return false;

            var path = AssetDatabase.GetAssetPath(settings);
            if (string.IsNullOrEmpty(path))
                return false;

            // Check if the asset is in a read-only package.
            var packageInfo = UnityEditor.PackageManager.PackageInfo.FindForAssetPath(path);
            return packageInfo is
            {
                source: UnityEditor.PackageManager.PackageSource.BuiltIn
                or UnityEditor.PackageManager.PackageSource.Embedded
            };
        }

        static bool IsEditableSettings(AppUISettings settings)
        {
            return settings && EditorUtility.IsPersistent(settings) && !IsInReadOnlyPackage(settings);
        }

        public override void OnGUI(string searchContext)
        {
            var currentSettingsAsset = m_Settings && EditorUtility.IsPersistent(m_Settings) ? m_Settings : null;
            var newSettingsAsset = EditorGUILayout.ObjectField(
                Styles.settingsAssetFieldContent,
                currentSettingsAsset,
                typeof(AppUISettings),
                false) as AppUISettings;

            var removed = !newSettingsAsset && currentSettingsAsset;
            if (newSettingsAsset != currentSettingsAsset)
                Core.AppUI.settings = newSettingsAsset ? newSettingsAsset : ScriptableObject.CreateInstance<AppUISettings>();

            InitializeWithCurrentSettingsIfNecessary(removed);

            Debug.Assert(m_Settings);

            EditorGUIUtility.labelWidth = 200;

            if (m_AvailableAppUISettingsAssets.Count == 0 || !EditorUtility.IsPersistent(m_Settings))
            {
                var notPersistentMessage = EditorUtility.IsPersistent(m_Settings) ? "" : "You have not selected one yet. Please pick one from the field above or create a new one.";
                EditorGUILayout.HelpBox(
                    $"Settings for App UI are stored in an asset. {notPersistentMessage}\n\nClick the button below to create a settings asset you can edit.",
                    MessageType.Info);
                if (GUILayout.Button(Styles.createSettingsAssetContent, GUILayout.Height(30)))
                    CreateNewSettingsAsset();
                GUILayout.Space(20);
            }

            if (m_Settings && EditorUtility.IsPersistent(m_Settings) && IsInReadOnlyPackage(m_Settings))
            {
                EditorGUILayout.HelpBox(
                    $"The App UI settings asset is stored in a read-only package. If you want to edit it, please create a new App UI settings asset in your project.\n\nClick the button below to create a new settings asset you can edit.",
                    MessageType.Warning);
                if (GUILayout.Button(Styles.createSettingsAssetContent, GUILayout.Height(30)))
                    CreateNewSettingsAsset();
                GUILayout.Space(20);
            }

            using (new EditorGUI.DisabledScope(!IsEditableSettings(m_Settings)))
            {
                EditorGUILayout.Space();
                EditorGUILayout.Separator();
                EditorGUILayout.Space();

                EditorGUI.BeginChangeCheck();

                EditorGUILayout.LabelField(Styles.editorGroup, EditorStyles.boldLabel);

                EditorGUI.indentLevel++;

                EditorGUILayout.PropertyField(m_EditorOnly, Styles.editorOnlyContent);

                EditorGUILayout.PropertyField(m_UseCustomEditorUpdateFrequency, Styles.useCustomEditorUpdateFrequencyContent);

                using (new EditorGUI.DisabledScope(!m_UseCustomEditorUpdateFrequency.boolValue))
                {
                    EditorGUILayout.PropertyField(m_EditorUpdateFrequency, Styles.editorUpdateFrequencyContent);
                }

                EditorGUI.indentLevel--;

                EditorGUILayout.Space();

                EditorGUILayout.LabelField(Styles.runtimeGroup, EditorStyles.boldLabel);

                EditorGUI.indentLevel++;

                EditorGUILayout.PropertyField(m_AutoScaleUI, Styles.autoScaleUIContent);

                using (new EditorGUI.DisabledScope(m_EditorOnly.boolValue))
                {
                    EditorGUILayout.PropertyField(m_IncludeShadersInPlayerBuild, Styles.includeShadersInPlayerBuildContent);
                }

                EditorGUI.indentLevel--;

                EditorGUILayout.Space();

                EditorGUILayout.LabelField(Styles.androidGroup, EditorStyles.boldLabel);

                EditorGUI.indentLevel++;

                EditorGUILayout.HelpBox(Styles.androidManifestOverrideWarning.text, MessageType.Warning);

                using (new EditorGUI.DisabledScope(m_EditorOnly.boolValue))
                {
                    EditorGUILayout.PropertyField(m_AutoOverrideAndroidManifest, Styles.autoOverrideAndroidManifestContent);
                }

                EditorGUI.indentLevel--;

                EditorGUILayout.Space();

                EditorGUILayout.LabelField(Styles.macOSGroup, EditorStyles.boldLabel);

                EditorGUI.indentLevel++;

                EditorGUILayout.PropertyField(m_EnableMacOSGestureRecognition, Styles.enableMacOSGestureRecognitionContent);

                EditorGUILayout.HelpBox(Styles.macOSGestureRecognitionWarning.text, MessageType.Info);

                EditorGUI.indentLevel--;

                EditorGUILayout.Space();

                if (EditorGUI.EndChangeCheck())
                    Apply();
            }
        }

        internal static void CreateNewSettingsAsset(string relativePath)
        {
            var existingGuid = AssetDatabase.AssetPathToGUID(relativePath, AssetPathToGUIDOptions.OnlyExistingAssets);
            if (string.IsNullOrEmpty(existingGuid))
            {
                // Create settings file.
                var settings = ScriptableObject.CreateInstance<AppUISettings>();
                AssetDatabase.CreateAsset(settings, relativePath);
                EditorGUIUtility.PingObject(settings);
                // Install the settings. This will lead to an AppUI.settingsChanged event which in turn
                // will cause us to re-initialize.
                Core.AppUI.settings = settings;
            }
        }

        static void CreateNewSettingsAsset()
        {
            // Query for file name.
            var path = EditorUtility.SaveFilePanel("Create App UI Settings File", "Assets",
                "App UI Settings", "asset");
            if (string.IsNullOrEmpty(path))
                return;

            // Make sure the path is in the Assets/ folder.
            path = path.Replace("\\", "/"); // Make sure we only get '/' separators.
            var dataPath = Application.dataPath + "/";
            if (!path.StartsWith(dataPath, StringComparison.CurrentCultureIgnoreCase))
            {
                Debug.LogError($"App UI settings must be stored in Assets folder of the project (got: '{path}')");
                return;
            }

            // Make sure it ends with .asset.
            var extension = Path.GetExtension(path);
            if (string.Compare(extension, ".asset", StringComparison.InvariantCultureIgnoreCase) != 0)
                path += ".asset";

            // Create settings file.
            var relativePath = "Assets/" + path.Substring(dataPath.Length);
            CreateNewSettingsAsset(relativePath);
        }

        void InitializeWithCurrentSettingsIfNecessary(bool removed)
        {
            if (Core.AppUI.settings == m_Settings && m_Settings && m_SettingsDirtyCount == EditorUtility.GetDirtyCount(m_Settings))
                return;

            InitializeWithCurrentSettings(removed);
        }

        /// <summary>
        /// Grab <see cref="AppUISettings"/> and set it up for editing.
        /// </summary>
        void InitializeWithCurrentSettings(bool removed)
        {
            // Find the set of available assets in the project.
            m_AvailableAppUISettingsAssets = new List<string>(FindAppUISettingsInProject());

            // See which is the active one.
            m_Settings = Core.AppUI.settings;
            m_SettingsDirtyCount = EditorUtility.GetDirtyCount(m_Settings);
            if (!EditorUtility.IsPersistent(m_Settings))
            {
                if (m_AvailableAppUISettingsAssets.Count != 0 && !removed)
                {
                    m_Settings = AssetDatabase.LoadAssetAtPath<AppUISettings>(m_AvailableAppUISettingsAssets[0]);
                    Core.AppUI.settings = m_Settings;
                }
            }

            // Look up properties.
            m_SettingsObject = new SerializedObject(m_Settings);

            m_EditorOnly = m_SettingsObject.FindProperty("m_EditorOnly");
            m_AutoScaleUI = m_SettingsObject.FindProperty("m_AutoCorrectUiScale");
            m_UseCustomEditorUpdateFrequency = m_SettingsObject.FindProperty("m_UseCustomEditorUpdateFrequency");
            m_EditorUpdateFrequency = m_SettingsObject.FindProperty("m_EditorUpdateFrequency");
            m_AutoOverrideAndroidManifest = m_SettingsObject.FindProperty("m_AutoOverrideAndroidManifest");
            m_EnableMacOSGestureRecognition = m_SettingsObject.FindProperty("m_EnableMacOSGestureRecognition");
            m_IncludeShadersInPlayerBuild = m_SettingsObject.FindProperty("m_IncludeShadersInPlayerBuild");
        }

        void Apply()
        {
            if (!m_Settings)
                return;

            m_SettingsObject.ApplyModifiedPropertiesWithoutUndo();
            m_SettingsObject.Update();
            m_Settings.OnChange();
        }

        /// <summary>
        /// Find all <see cref="AppUISettings"/> stored in assets in the current project.
        /// </summary>
        /// <returns>List of AppUI settings in project.</returns>
        static IEnumerable<string> FindAppUISettingsInProject()
        {
            var guids = AssetDatabase.FindAssets($"t:{nameof(AppUISettings)} a:all");

            var paths = new List<string>();

            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                paths.Add(path);
            }

            return paths;
        }

        [SerializeField] AppUISettings m_Settings;

        [NonSerialized] int m_SettingsDirtyCount;
        [NonSerialized] SerializedObject m_SettingsObject;
        [NonSerialized] SerializedProperty m_EditorOnly;
        [NonSerialized] SerializedProperty m_AutoScaleUI;
        [NonSerialized] SerializedProperty m_UseCustomEditorUpdateFrequency;
        [NonSerialized] SerializedProperty m_EditorUpdateFrequency;
        [NonSerialized] SerializedProperty m_AutoOverrideAndroidManifest;
        [NonSerialized] SerializedProperty m_EnableMacOSGestureRecognition;
        [NonSerialized] SerializedProperty m_IncludeShadersInPlayerBuild;

        [NonSerialized] List<string> m_AvailableAppUISettingsAssets;

        static AppUISettingsProvider s_Instance;

        internal static void ForceReload()
        {
            if (s_Instance != null)
            {
                // Force next OnGUI() to re-initialize.
                s_Instance.m_Settings = null;

                // Request repaint.
                SettingsService.NotifySettingsProviderChanged();
            }
        }
    }
}
#endif // UNITY_EDITOR
