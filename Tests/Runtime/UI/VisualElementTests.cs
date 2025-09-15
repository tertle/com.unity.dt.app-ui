using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using Unity.AppUI.Core;
using Unity.AppUI.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Unity.AppUI.Tests.UI
{
    class VisualElementTests<T> where T : VisualElement, new()
    {
        VisualElement m_VisualElement;

        protected virtual string componentName => typeof(T).Name;

        protected virtual string mainUssClassName => null;

        protected virtual bool uxmlConstructable => true;

        protected virtual IEnumerable<string> uxmlTestCases
        {
            get { yield return "<" + uxmlNamespaceName + ":" + componentName + " />"; }
        }

        protected virtual string uxmlNamespaceName => "appui";

        protected record StoryContext(UIDocument document, Panel panel)
        {
            public UIDocument document { get; } = document;

            public Panel panel { get; } = panel;
        }

        protected record Story(string name, Func<StoryContext, VisualElement> setup)
        {
            public string name { get; } = name;

            public Func<StoryContext, VisualElement> setup { get; } = setup;
        }

        protected virtual IEnumerable<Story> stories
        {
            get { yield return new Story("Default", (ctx) => new T()); }
        }

        protected T element => m_VisualElement as T;

        protected UIDocument m_TestUI;

        protected Panel m_Panel;

        protected bool m_SetupDone;

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            if (!m_SetupDone)
            {
                // Load new scene
                var scene = SceneManager.CreateScene("ComponentTestScene-" + Random.Range(1, 1000000));
                while (!SceneManager.SetActiveScene(scene))
                {
                    yield return null;
                }
                yield return Utils.WaitForLocalizationPreloaded();
                m_TestUI = Utils.ConstructTestUI();
                Screen.SetResolution(1200, 600, FullScreenMode.Windowed);
                TimeUtils.timeOverride = 1f;
            }
            m_TestUI.rootVisualElement.Clear();
            m_SetupDone = true;
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            TimeUtils.timeOverride = null;
            if (m_TestUI)
                Object.Destroy(m_TestUI.gameObject);

            m_TestUI = null;
            m_SetupDone = false;
#pragma warning disable CS0618
            SceneManager.UnloadScene(SceneManager.GetActiveScene());
#pragma warning restore CS0618
        }

        [Test]
        [Order(1)]
        public void Constructor_ShouldSucceed()
        {
            m_VisualElement = null;
            Assert.DoesNotThrow(() => m_VisualElement = new T());
            Assert.IsNotNull(m_VisualElement);
            if (!string.IsNullOrEmpty(mainUssClassName))
                Assert.IsTrue(m_VisualElement.ClassListContains(mainUssClassName));
        }

        [UnityTest]
        [Order(2)]
        [ConditionalIgnore("IgnoreInPlayer", "UXML construction is supported only in Editor")]
        public IEnumerator UxmlConstruction_ShouldSucceed()
        {
            if (!uxmlConstructable)
            {
                Assert.Ignore("UXML construction not supported for this type");
                yield break;
            }

            var cases = uxmlTestCases;

            if (cases == null)
            {
                Assert.Ignore("No UXML test cases defined");
                yield break;
            }

            foreach (var uxmlTestCase in cases)
            {
                Assert.DoesNotThrow(() =>
                {
                    var content = "<ui:UXML xmlns:ui=\"UnityEngine.UIElements\" xmlns:appui=\"Unity.AppUI.UI\" xmlns:nav=\"Unity.AppUI.Navigation\" >" + uxmlTestCase + "</ui:UXML>";
                    var asset = Utils.LoadUxmlTemplateFromString(content);
                    m_TestUI.visualTreeAsset = asset;
                });

                yield return null;
            }
        }

        [UnityTest]
        [Order(3)]
        public IEnumerator CreateSnapshot()
        {
            var capture = !Application.isEditor && !string.IsNullOrEmpty(Utils.snapshotsOutputDir);

            foreach (var story in stories)
            {
                foreach (var theme in Utils.themes)
                {
                    foreach (var scale in Utils.scales)
                    {
                        foreach (var dir in Enum.GetValues(typeof(Dir)))
                        {
                            yield return CreateSnapshotInternal(story, theme, scale, (Dir)dir, capture);
                        }
                    }
                }
            }
        }

        IEnumerator CreateSnapshotInternal(Story story, string theme, string scale, Dir dir, bool capture)
        {
#if !UNITY_EDITOR
            Screen.SetResolution(1200, 600, FullScreenMode.Windowed);
            m_TestUI.rootVisualElement.styleSheets.Add(Resources.Load<ThemeStyleSheet>("Themes/App UI"));
#else
            m_TestUI.rootVisualElement.styleSheets.Add(UnityEditor.AssetDatabase.LoadAssetAtPath<ThemeStyleSheet>(
                "Packages/com.unity.dt.app-ui/PackageResources/Styles/Themes/App UI.tss"));
#endif
            var panel = new Panel
            {
                theme = theme,
                scale = scale,
                layoutDirection = dir,
                lang = "en",
            };
            m_TestUI.rootVisualElement.Clear();

            m_TestUI.rootVisualElement.Add(panel);
            var container = new VisualElement
            {
                style =
                {
                    paddingBottom = 10,
                    paddingLeft = 10,
                    paddingRight = 10,
                    paddingTop = 10,
                }
            };
            panel.Add(container);
            var context = new StoryContext(m_TestUI, panel);
            var component = story.setup(context);
            container.Add(component);
            yield return null;
            const int waitFrames = 3;
            for (var i = 0; i < waitFrames; i++)
            {
                yield return null;
            }

            if (capture)
            {
                yield return new WaitForEndOfFrame();
                var outputFilePath = Path.GetFullPath(
                    Path.Combine(Utils.snapshotsOutputDir ?? Application.dataPath,
                        $"{componentName}.{story.name}.{theme}.{dir.ToString().ToLower()}.{scale}.png"));
                ScreenCapture.CaptureScreenshot(outputFilePath);
                yield return null;
                yield return new WaitUntilOrTimeOut(
                    () => Utils.FileAvailable(outputFilePath),
                    false,
                    TimeSpan.FromSeconds(3));
                Assert.IsTrue(Utils.FileAvailable(outputFilePath));
            }
        }
    }
}
