using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Unity.AppUI.Core;
using UnityEditor;
using UnityEditor.Toolbars;
using UnityEditor.UIElements;
using Unity.AppUI.UI;
using UnityEngine;
using UnityEngine.UIElements;
using TextField = UnityEngine.UIElements.TextField;
using Toggle = UnityEngine.UIElements.Toggle;
using Toolbar = UnityEditor.UIElements.Toolbar;
#if UNITY_LOCALIZATION_PRESENT
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
#endif
#if UNITY_2022_1_OR_NEWER
using FloatField = UnityEngine.UIElements.FloatField;
#else
using FloatField = UnityEditor.UIElements.FloatField;
#endif

namespace Unity.AppUI.Editor
{
    /// <summary>
    /// This class defines a property for a StoryBookComponent.
    /// </summary>
    public class StoryBookComponentProperty
    {
        /// <summary>
        /// The name of the property.
        /// </summary>
        public string name { get; protected set; }

        /// <summary>
        /// Get the input element for the property.
        /// </summary>
        /// <param name="uiElement"> The UI element. </param>
        /// <returns> The input element. </returns>
        internal virtual VisualElement GetInputElement(VisualElement uiElement)
        {
            return null;
        }
    }

    /// <summary>
    /// This class defines a Enum property for a StoryBookComponent.
    /// </summary>
    /// <typeparam name="T"> The type of the Enum. </typeparam>
    public class StoryBookEnumProperty<T> : StoryBookComponentProperty
        where T : Enum
    {
        /// <summary>
        /// The getter of the property.
        /// </summary>
        public Func<VisualElement, T> getter { get; set; }

        /// <summary>
        /// The setter of the property.
        /// </summary>
        public Action<VisualElement, T> setter { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name"> The name of the property. </param>
        /// <param name="getter"> The getter of the property. </param>
        /// <param name="setter"> The setter of the property. </param>
        public StoryBookEnumProperty(string name, Func<VisualElement, T> getter, Action<VisualElement, T> setter)
        {
            this.name = name;
            this.getter = getter;
            this.setter = setter;
        }

        /// <inheritdoc/>
        internal override VisualElement GetInputElement(VisualElement uiElement)
        {
            var enumField = new EnumField(name, getter.Invoke(uiElement));
            enumField.SetValueWithoutNotify(getter.Invoke(uiElement));
            enumField.RegisterValueChangedCallback(evt => setter.Invoke(uiElement, (T)evt.newValue));
            return enumField;
        }
    }

    /// <summary>
    /// This class defines a float property for a StoryBookComponent.
    /// </summary>
    public class StoryBookFloatProperty : StoryBookComponentProperty
    {
        /// <summary>
        /// The getter of the property.
        /// </summary>
        public Func<VisualElement, float> getter { get; set; }

        /// <summary>
        /// The setter of the property.
        /// </summary>
        public Action<VisualElement, float> setter { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name"> The name of the property. </param>
        /// <param name="getter"> The getter of the property. </param>
        /// <param name="setter"> The setter of the property. </param>
        public StoryBookFloatProperty(string name, Func<VisualElement, float> getter, Action<VisualElement, float> setter)
        {
            this.name = name;
            this.getter = getter;
            this.setter = setter;
        }

        /// <inheritdoc/>
        internal override VisualElement GetInputElement(VisualElement uiElement)
        {
            var floatField = new FloatField(name);
            floatField.SetValueWithoutNotify(getter?.Invoke(uiElement) ?? 0);
            floatField.RegisterValueChangedCallback(evt => setter?.Invoke(uiElement, evt.newValue));
            return floatField;
        }
    }

    /// <summary>
    /// This class defines a boolean property for a StoryBookComponent.
    /// </summary>
    public class StoryBookBooleanProperty : StoryBookComponentProperty
    {
        /// <summary>
        /// The getter of the property.
        /// </summary>
        public Func<VisualElement, bool> getter { get; set; }

        /// <summary>
        /// The setter of the property.
        /// </summary>
        public Action<VisualElement, bool> setter { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name"> The name of the property. </param>
        /// <param name="getter"> The getter of the property. </param>
        /// <param name="setter"> The setter of the property. </param>
        public StoryBookBooleanProperty(string name, Func<VisualElement, bool> getter, Action<VisualElement, bool> setter)
        {
            this.name = name;
            this.getter = getter;
            this.setter = setter;
        }

        /// <inheritdoc/>
        internal override VisualElement GetInputElement(VisualElement uiElement)
        {
            var toggle = new Toggle(name);
            toggle.SetValueWithoutNotify(getter?.Invoke(uiElement) ?? false);
            toggle.RegisterValueChangedCallback(evt => setter?.Invoke(uiElement, evt.newValue));
            return toggle;
        }
    }

    /// <summary>
    /// This class defines a string property for a StoryBookComponent.
    /// </summary>
    public class StoryBookStringProperty : StoryBookComponentProperty
    {
        /// <summary>
        /// The getter of the property.
        /// </summary>
        public Func<VisualElement, string> getter { get; set; }

        /// <summary>
        /// The setter of the property.
        /// </summary>
        public Action<VisualElement, string> setter { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name"> The name of the property. </param>
        /// <param name="getter"> The getter of the property. </param>
        /// <param name="setter"> The setter of the property. </param>
        public StoryBookStringProperty(string name, Func<VisualElement, string> getter, Action<VisualElement, string> setter)
        {
            this.name = name;
            this.getter = getter;
            this.setter = setter;
        }

        /// <inheritdoc/>
        internal override VisualElement GetInputElement(VisualElement uiElement)
        {
            var textField = new TextField(name);
            textField.SetValueWithoutNotify(getter?.Invoke(uiElement));
            textField.RegisterValueChangedCallback(evt => setter?.Invoke(uiElement, evt.newValue));
            return textField;
        }
    }

    /// <summary>
    /// A StoryBookComponent is a component that can be used as a StoryBookPage.
    /// </summary>
    public abstract class StoryBookComponent
    {
        /// <summary>
        /// The type of the UI element.
        /// </summary>
        public virtual Type uiElementType { get; }

        /// <summary>
        /// Setup the component.
        /// </summary>
        /// <param name="element"> The element to setup. </param>
        public virtual void Setup(VisualElement element) {}

        /// <summary>
        /// The properties of the component.
        /// </summary>
        public IEnumerable<StoryBookComponentProperty> properties => m_Properties;

        /// <summary>
        /// The list of properties (used internally).
        /// </summary>
        protected readonly List<StoryBookComponentProperty> m_Properties = new List<StoryBookComponentProperty>();
    }

    /// <summary>
    /// A StoryBookStory is a story that can be used inside a StoryBookPage.
    /// </summary>
    public sealed class StoryBookStory
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name"> The name of the story. </param>
        /// <param name="createGUI"> The function that creates the GUI of the story. </param>
        public StoryBookStory(string name, Func<VisualElement> createGUI)
        {
            this.name = name;
            this.createGUI = createGUI;
        }

        /// <summary>
        /// The name of the story.
        /// </summary>
        public string name { get; }

        /// <summary>
        /// The function that creates the GUI of the story.
        /// </summary>
        public Func<VisualElement> createGUI { get; }
    }

    /// <summary>
    /// A StoryBookPage is a page that can be used inside a StoryBook.
    /// </summary>
    public abstract class StoryBookPage
    {
        /// <summary>
        /// The name of the page.
        /// </summary>
        public virtual string displayName { get; }

        /// <summary>
        /// The type of the component.
        /// </summary>
        public virtual Type componentType { get; }

        /// <summary>
        /// The stories of the page.
        /// </summary>
        public IEnumerable<StoryBookStory> stories => m_Stories;

        /// <summary>
        /// The list of stories (used internally).
        /// </summary>
        protected readonly List<StoryBookStory> m_Stories = new List<StoryBookStory>();
    }

    /// <summary>
    /// A StoryBook is a window that allows to preview the UI elements of the App UI package.
    /// </summary>
    public class Storybook : EditorWindow
    {
        const string k_DefaultTheme = "Packages/com.unity.dt.app-ui/PackageResources/Styles/Themes/App UI.tss";

        List<StoryBookPage> m_StoriesList = new List<StoryBookPage>();

        TwoPaneSplitView m_SplitView;

        ListView m_ListView;

        VisualElement m_Preview;

        ListView m_StoryListView;

        ScrollView m_Inspector;

        TwoPaneSplitView m_VerticalPane;

        string m_CurrentTheme = "dark";

        string m_CurrentScale = "medium";

        Dir m_CurrentLayoutDirection = Dir.Ltr;

        EditorToolbarDropdown m_ThemeDropdown;

        EditorToolbarDropdown m_ScaleDropdown;

        EditorToolbarDropdown m_LayoutDirectionDropdown;

        EditorToolbarDropdown m_LocaleToolbar;

        string m_CurrentLocaleId = "en";

        /// <summary>
        /// Open the StoryBook window.
        /// </summary>
        [UnityEditor.MenuItem("Window/App UI/ Storybook")]
        public static void OpenStoryBook()
        {
            var window = GetWindow<Storybook>("App UI - StoryBook");
            window.Show();
        }

        static IEnumerable<StoryBookPage> GetStories()
        {
            var stories = new List<StoryBookPage>();

            var storyBookPageType = typeof(StoryBookPage);
            foreach (var type in TypeCache.GetTypesDerivedFrom(storyBookPageType))
            {
                stories.Add(Activator.CreateInstance(type) as StoryBookPage);
            }

            return stories;
        }

#pragma warning disable CS1998
        async void OnEnable()
        {
#if UNITY_LOCALIZATION_PRESENT
            await LocalizationSettings.InitializationOperation.Task;
#endif
        }
#pragma warning restore CS1998

        void CreateGUI()
        {
            var root = rootVisualElement;

            m_SplitView = new TwoPaneSplitView(0, 200, TwoPaneSplitViewOrientation.Horizontal);
            m_StoriesList = new List<StoryBookPage>(GetStories().OrderBy(s => s.displayName));
            var listPane = new TwoPaneSplitView(0, 100, TwoPaneSplitViewOrientation.Horizontal);
            m_ListView = new ListView(m_StoriesList, -1f, MakeListVIewItem, BindListViewItem);
            m_StoryListView = new ListView(null, -1f, MakeListVIewItem, BindStoryListViewItem);
            m_Preview = CreateDetailPage();
#if UITK_SELECTED_INDICES_CHANGED
            m_ListView.selectedIndicesChanged += OnSelectionChanged;
            m_StoryListView.selectedIndicesChanged += OnStorySelectionChanged;
#else
            m_ListView.onSelectedIndicesChange += OnSelectionChanged;
            m_StoryListView.onSelectedIndicesChange += OnStorySelectionChanged;
#endif
            listPane.Add(m_ListView);
            listPane.Add(m_StoryListView);
            m_SplitView.Add(listPane);
            m_SplitView.Add(m_Preview);

            root.Add(m_SplitView);

            root.Bind(new SerializedObject(this));
        }

        void BindStoryListViewItem(VisualElement ve, int idx)
        {
            ((Label)ve).text = ((List<StoryBookStory>)m_StoryListView.itemsSource)[idx].name;
        }

        void OnSelectionChanged(IEnumerable<int> indices)
        {
            var enumerator = indices.GetEnumerator();

            if (!enumerator.MoveNext())
            {
                RefreshDetailPage(null);
                return;
            }

            var idx = enumerator.Current;
            var page = m_StoriesList[idx];
            RefreshStoryList(page);
        }

        void RefreshStoryList(StoryBookPage page)
        {
            var items = new List<StoryBookStory>(page.stories);
            if (page.componentType != null)
                items.Insert(0, new StoryBookStory("Canvas", null));
            m_StoryListView.itemsSource = items;
            m_StoryListView.Rebuild();
            m_StoryListView.SetSelection(new List<int> { 0 });
            OnStorySelectionChanged(new List<int> { 0 });
        }

        void OnStorySelectionChanged(IEnumerable<int> indices)
        {
            var enumerator = indices.GetEnumerator();

            if (!enumerator.MoveNext())
            {
                RefreshDetailPage(null);
                return;
            }

            var idx = enumerator.Current;
            var story = ((List<StoryBookStory>)m_StoryListView.itemsSource)[idx];

            RefreshDetailPage(story);
        }

        VisualElement CreateDetailPage()
        {
            var detailPage = new VisualElement();
            var toolbar = new Toolbar();
            var panel = new Panel
            {
                theme = m_CurrentTheme,
                scale = m_CurrentScale,
                layoutDirection = m_CurrentLayoutDirection,
                lang = m_CurrentLocaleId,
            };
            panel.AddToClassList("unity-editor");
            m_ThemeDropdown = new EditorToolbarDropdown("Theme", () =>
            {
                var menu = new GenericMenu();
                menu.AddItem(new GUIContent("Dark"), m_CurrentTheme == "dark", () =>
                {
                    m_CurrentTheme = "dark";
                    panel.theme = m_CurrentTheme;
                });
                menu.AddItem(new GUIContent("Light"), m_CurrentTheme == "light", () =>
                {
                    m_CurrentTheme = "light";
                    panel.theme = m_CurrentTheme;
                });
                menu.AddItem(new GUIContent("Editor Dark"), m_CurrentTheme == "editor-dark", () =>
                {
                    m_CurrentTheme = "editor-dark";
                    panel.theme = m_CurrentTheme;
                });
                menu.AddItem(new GUIContent("Editor Light"), m_CurrentTheme == "editor-light", () =>
                {
                    m_CurrentTheme = "editor-light";
                    panel.theme = m_CurrentTheme;
                });
                menu.DropDown(m_ThemeDropdown.worldBound);
            });
            toolbar.Add(m_ThemeDropdown);

            m_ScaleDropdown = new EditorToolbarDropdown("Scale", () =>
            {
                var menu = new GenericMenu();
                menu.AddItem(new GUIContent("Small"), m_CurrentScale == "small", () =>
                {
                    m_CurrentScale = "small";
                    panel.scale = m_CurrentScale;
                });
                menu.AddItem(new GUIContent("Medium"), m_CurrentScale == "medium", () =>
                {
                    m_CurrentScale = "medium";
                    panel.scale = m_CurrentScale;
                });
                menu.AddItem(new GUIContent("Large"), m_CurrentScale == "large", () =>
                {
                    m_CurrentScale = "large";
                    panel.scale = m_CurrentScale;
                });
                menu.DropDown(m_ScaleDropdown.worldBound);
            });
            toolbar.Add(m_ScaleDropdown);

            m_LayoutDirectionDropdown = new EditorToolbarDropdown("Layout Direction", () =>
            {
                var menu = new GenericMenu();
                menu.AddItem(new GUIContent("Left To Right"), m_CurrentLayoutDirection == Dir.Ltr, () =>
                {
                    m_CurrentLayoutDirection = Dir.Ltr;
                    panel.layoutDirection = m_CurrentLayoutDirection;
                });
                menu.AddItem(new GUIContent("Right To Left"), m_CurrentLayoutDirection == Dir.Rtl, () =>
                {
                    m_CurrentLayoutDirection = Dir.Rtl;
                    panel.layoutDirection = m_CurrentLayoutDirection;
                });
                menu.DropDown(m_LayoutDirectionDropdown.worldBound);
            });
            toolbar.Add(m_LayoutDirectionDropdown);

#if UNITY_LOCALIZATION_PRESENT
            var locales = LocalizationSettings.AvailableLocales.Locales;
            if (locales.Count > 0)
            {
                m_LocaleToolbar = new EditorToolbarDropdown("Locale", () =>
                {
                    var menu = new GenericMenu();
                    foreach (var locale in locales)
                    {
                        menu.AddItem(new GUIContent(locale.LocaleName), m_CurrentLocaleId == locale.Identifier.Code, () =>
                        {
                            m_CurrentLocaleId = locale.Identifier.Code;
                            panel.lang = m_CurrentLocaleId;
                        });
                    }
                    menu.DropDown(m_LocaleToolbar.worldBound);
                });
                toolbar.Add(m_LocaleToolbar);
            }
#endif

            detailPage.Add(toolbar);

            m_VerticalPane = new TwoPaneSplitView(1, 150, TwoPaneSplitViewOrientation.Vertical);

            var canvas = new VisualElement();
            canvas.styleSheets.Add(AssetDatabase.LoadAssetAtPath<ThemeStyleSheet>(k_DefaultTheme));
            var container = new VisualElement { name = "canvas-container" };
            container.style.alignItems = Align.Center;
            canvas.Add(panel);
            panel.Add(container);
            container.StretchToParentSize();
            m_VerticalPane.Add(canvas);

            m_Inspector = new ScrollView(ScrollViewMode.Vertical)
            {
                style =
                {
                    paddingBottom = 8,
                    paddingLeft = 8,
                    paddingTop = 8,
                    paddingRight = 8,
                }
            };
            var inspectorTitle = new Label("Properties")
            {
                style = { unityFontStyleAndWeight = FontStyle.Bold }
            };
            m_Inspector.Add(inspectorTitle);
            var inspectorContainer = new VisualElement { name = "inspector-container" };
            m_Inspector.Add(inspectorContainer);
            m_VerticalPane.Add(m_Inspector);

            detailPage.Add(m_VerticalPane);

            return detailPage;
        }

        void RefreshDetailPage(StoryBookStory story)
        {
            var container = rootVisualElement.Q<VisualElement>("canvas-container");
            var parent = container.parent;
            container.RemoveFromHierarchy();
            container = new VisualElement { name = "canvas-container" };
            container.style.alignItems = Align.Center;
            container.StretchToParentSize();
            parent.Add(container);

            var inspectorContainer = rootVisualElement.Q<VisualElement>("inspector-container");
            inspectorContainer.Clear();

            if (story == null)
            {
                m_VerticalPane.CollapseChild(1);
                return;
            }

            if (story.createGUI == null)
            {
                m_VerticalPane.UnCollapse();

                // Update component page
                var component = (StoryBookComponent)Activator.CreateInstance(m_StoriesList[m_ListView.selectedIndex].componentType);
                var uiElement = (VisualElement)Activator.CreateInstance(component.uiElementType);
                container.Add(uiElement);
                component.Setup(uiElement);

                foreach (var prop in component.properties)
                {
                    var field = prop.GetInputElement(uiElement);
                    if (field != null)
                        inspectorContainer.Add(field);
                }
            }
            else
            {
                m_VerticalPane.CollapseChild(1);
                var uiElement = story.createGUI();
                container.Add(uiElement);
            }

            var panel = container.GetFirstAncestorOfType<Panel>();
            panel.theme = m_CurrentTheme;
            panel.scale = m_CurrentScale;
            panel.layoutDirection = m_CurrentLayoutDirection;
            panel.lang = m_CurrentLocaleId;
        }

        void BindListViewItem(VisualElement ve, int idx)
        {
            var label = (Label)ve;
            label.text = m_StoriesList[idx].displayName;
        }

        VisualElement MakeListVIewItem()
        {
            return new Label()
            {
                style = {
                    unityTextAlign = TextAnchor.MiddleLeft,
                    paddingLeft = 8,
                }
            };
        }
    }
}
