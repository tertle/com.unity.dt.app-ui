using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using Unity.AppUI.Core;
using UnityEngine;

namespace Unity.AppUI.Editor
{
    /// <summary>
    /// Unity build processor to add AppUISettings object to preloaded assets.
    /// </summary>
    public class AppUIBuildSetup : IPreprocessBuildWithReport, IPostprocessBuildWithReport
    {
        /// <summary>
        /// Callback order used internally by Unity.
        /// </summary>
        public int callbackOrder => 0;

        /// <summary>
        /// Called before build starts.
        /// </summary>
        /// <param name="report"> Unity Build report. </param>
        public void OnPreprocessBuild(BuildReport report)
        {
            if (Core.AppUI.settings == null)
            {
                throw new System.InvalidOperationException("There's no App UI Settings set in your project./n" +
                    "Please go to Edit > Project Settings > App UI and create a new Settings asset.");
            }

            EnsureAppUISettingsArePreloaded();

            if (report.summary.platform == BuildTarget.Android && Core.AppUI.settings.autoOverrideAndroidManifest)
                BuildAndroidManifest();
        }

        static void EnsureAppUISettingsArePreloaded()
        {
            // If we operate on temporary object instead of input setting asset,
            // adding temporary asset would result in preloadedAssets containing null object "{fileID: 0}".
            // Hence we ignore adding temporary objects to preloaded assets.
            if (EditorUtility.IsPersistent(Core.AppUI.settings))
            {
                // Add AppUISettings object assets, if it's not in there already.
                var preloadedAssets = PlayerSettings.GetPreloadedAssets().ToList();
                if (!preloadedAssets.Contains(Core.AppUI.settings))
                {
                    preloadedAssets.Add(Core.AppUI.settings);
                    PlayerSettings.SetPreloadedAssets(preloadedAssets.ToArray());
                }
            }
            else
            {
                var persistentAppUISettings = Resources.FindObjectsOfTypeAll<AppUISettings>()?.ToList() ?? new List<AppUISettings>();
                if (persistentAppUISettings.Count > 0)
                {
                    Debug.LogWarning("App UI Settings is not a persistent object. Skipping adding it to preloaded assets.\n" +
                        "If this is not intended, please make sure the App UI Settings object is saved to disk and selected in " +
                        "the Project Settings (Edit > Project Settings > App UI).");
                }
            }
        }

        static void BuildAndroidManifest()
        {
            const string androidNamespace = "http://schemas.android.com/apk/res/android";
            var manifest = Path.Combine(Application.dataPath, "Plugins", "Android", "AndroidManifest.xml");
            if (!File.Exists(manifest))
            {
                if (!Directory.Exists(Path.GetDirectoryName(manifest)))
                    Directory.CreateDirectory(Path.GetDirectoryName(manifest)!);

                var content = AssetDatabase.LoadAssetAtPath<TextAsset>(
                    "Packages/com.unity.dt.app-ui/Runtime/Core/Platform/Android/Plugins/Android/AndroidManifest.xml");

                File.WriteAllText(manifest, content.text);
            }

            var doc = new XmlDocument();
            doc.Load(manifest);

            var nsManager = new XmlNamespaceManager(doc.NameTable);
            nsManager.AddNamespace("android", androidNamespace);
            var manifestNode = (XmlElement)doc.SelectSingleNode("/manifest");
            var activity = (XmlElement)doc.SelectSingleNode("/manifest/application/activity");

#if UNITY_2023_2_OR_NEWER
            var activityName = PlayerSettings.Android.applicationEntry.HasFlag(AndroidApplicationEntry.GameActivity) ?
                "com.unity3d.player.appui.AppUIGameActivity" : "com.unity3d.player.appui.AppUIActivity";
            activity!.SetAttribute("name", androidNamespace, activityName);
#else
            activity!.SetAttribute("name", androidNamespace, "com.unity3d.player.appui.AppUIActivity");
#endif

            var vibratePermission = doc.SelectSingleNode("/manifest/uses-permission[@android:name='android.permission.VIBRATE']", nsManager) as XmlElement;
            if (vibratePermission == null)
            {
                vibratePermission = doc.CreateElement("uses-permission");
                vibratePermission.SetAttribute("name", androidNamespace, "android.permission.VIBRATE");
                manifestNode.AppendChild(vibratePermission);
            }
            doc.Save(manifest);
        }

        /// <summary>
        /// Called after build finishes.
        /// </summary>
        /// <param name="report"> Unity Build report. </param>
        public void OnPostprocessBuild(BuildReport report)
        {
            // Revert back to original state by removing all input settings from preloaded assets.
            var preloadedAssets = PlayerSettings.GetPreloadedAssets().ToList();
            PlayerSettings.SetPreloadedAssets(preloadedAssets.Where(x => x is not AppUISettings).ToArray());
        }
    }
}
