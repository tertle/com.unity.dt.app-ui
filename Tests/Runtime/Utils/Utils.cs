using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using Unity.AppUI.Navigation;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UIElements;

#if UNITY_EDITOR
using UnityEditor;
#endif

#if UNITY_LOCALIZATION_PRESENT
using UnityEngine.Localization.Settings;
#endif

namespace Unity.AppUI.Tests
{
    static class Utils
    {
        static readonly MethodInfo k_ImportXmlFromString;

        static readonly object k_UxmlImporterImplInstance;

        static Utils()
        {
            ConditionalIgnoreAttribute.AddConditionalIgnoreMapping("IgnoreInEditor", Application.isEditor);
            ConditionalIgnoreAttribute.AddConditionalIgnoreMapping("IgnoreInPlayer", !Application.isEditor);
#if UNITY_EDITOR
            Type importerType = null;
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (type.FullName == "UnityEditor.UIElements.UXMLImporterImpl")
                    {
                        importerType = type;
                        break;
                    }
                }

                if (importerType != null)
                    break;
            }

            if (importerType == null)
            {
                Debug.LogError("Could not find UXMLImporterImpl type");
            }
            else
            {
                k_UxmlImporterImplInstance = importerType.GetConstructors(
                        BindingFlags.Instance | BindingFlags.NonPublic)
                    .First(c => !c.GetParameters().Any())
                    .Invoke(Array.Empty<object>());

                k_ImportXmlFromString = importerType.GetMethod("ImportXmlFromString",
                    BindingFlags.Instance | BindingFlags.NonPublic);

                if (k_ImportXmlFromString == null)
                    Debug.LogError("Could not find ImportXmlFromString method");
            }
#endif
        }

        internal static readonly string snapshotsOutputDir =
            Environment.GetEnvironmentVariable("SNAPSHOTS_OUTPUT_DIR") is {Length:>0} p ?
            Path.GetFullPath(p) : null;

        internal static IEnumerable<string> scales
        {
            get
            {
                yield return "small";
                yield return "medium";
            }
        }

        internal static IEnumerable<string> themes
        {
            get
            {
                yield return "dark";
                yield return "light";
            }
        }

        internal static bool FileAvailable(string path)
        {
            if (!File.Exists(path))
                return false;

            var file = new FileInfo(path);
            FileStream stream = null;

            try
            {
                stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None);
            }
            catch (IOException)
            {
                // Can be either:
                // - file is processed by another thread
                // - file is still being written to
                // - file does not really exist yet
                return false;
            }
            finally
            {
                stream?.Close();
            }

            return true;
        }

        static PanelSettings s_PanelSettingsInstance;

        internal static PanelSettings panelSettingsInstance
        {
            get
            {
                if (!s_PanelSettingsInstance)
                {
                    s_PanelSettingsInstance = ScriptableObject.CreateInstance<PanelSettings>();
                    s_PanelSettingsInstance.scaleMode = PanelScaleMode.ConstantPhysicalSize;
#if UNITY_EDITOR
                    s_PanelSettingsInstance.themeStyleSheet = AssetDatabase.LoadAssetAtPath<ThemeStyleSheet>(
                        "Packages/com.unity.dt.app-ui/PackageResources/Styles/Themes/App UI.tss");
#else
                    s_PanelSettingsInstance.themeStyleSheet = Resources.Load<ThemeStyleSheet>("Themes/App UI");
#endif
                }

                return s_PanelSettingsInstance;
            }
        }

        static NavGraphViewAsset s_NavGraphTestAsset;

        internal static NavGraphViewAsset navGraphTestAsset
        {
            get
            {
                if (!s_NavGraphTestAsset)
                {
#if UNITY_EDITOR
                    s_NavGraphTestAsset = AssetDatabase.LoadAssetAtPath<NavGraphViewAsset>(
                        "Packages/com.unity.dt.app-ui/Tests/Runtime/Navigation/NavGraphTestAsset.asset");
#endif
                }

                return s_NavGraphTestAsset;
            }
        }

        internal static IEnumerator WaitForLocalizationPreloaded()
        {
#if UNITY_LOCALIZATION_PRESENT
            var localizationSettings = LocalizationSettings.GetInstanceDontCreateDefault();
            if (localizationSettings)
            {
                yield return localizationSettings.GetInitializationOperation();
                yield return localizationSettings.GetSelectedLocaleAsync();
                Assert.IsTrue(localizationSettings.GetSelectedLocale());
            }
#else
            yield break;
#endif
        }

        internal static UIDocument ConstructTestUI()
        {
            var obj = new GameObject("TestUI");
            obj.AddComponent<Camera>();
            var doc = obj.AddComponent<UIDocument>();
            doc.panelSettings = panelSettingsInstance;

            return doc;
        }

        internal static VisualTreeAsset LoadUxmlTemplateFromString(string contents)
        {
            // ReSharper disable once RedundantAssignment
            VisualTreeAsset vta = null;
#if UNITY_EDITOR
            var args = new object[] {contents, null};
            k_ImportXmlFromString.Invoke(k_UxmlImporterImplInstance, args);
            vta = args[1] as VisualTreeAsset;
#endif
            return vta;
        }

        // ReSharper disable StringLiteralTypo
        internal static string loremIpsum = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod " +
            "tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation " +
            "ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in " +
            "voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non " +
            "proident, sunt in culpa qui officia deserunt mollit anim id est laborum.";
        // ReSharper restore StringLiteralTypo
    }
}
