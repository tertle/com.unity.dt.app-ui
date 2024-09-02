using System;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.UIElements;
using Toggle = UnityEngine.UIElements.Toggle;

namespace Unity.AppUI.Navigation.Editor
{
    /// <summary>
    /// Editor window for creating and editing <see cref="NavGraphViewAsset"/> objects.
    /// </summary>
    public class NavigationGraphWindow : EditorWindow
    {
        static NavigationGraphWindow s_Instance;

        VisualElement m_GraphViewPane;

        NavGraphViewAsset m_LastGraphAsset;

        bool m_FollowNavigation = false;

        /// <summary>
        /// Initializes the window.
        /// </summary>
        [MenuItem("Window/App UI/Navigation Graph")]
        static void Init()
        {
            if (s_Instance == null)
            {
                s_Instance = GetWindow<NavigationGraphWindow>();
                s_Instance.titleContent = new GUIContent("Navigation Graph");
            }
        }

        /// <summary>
        /// CreateGUI is called when the EditorWindow's rootVisualElement is ready to be populated.
        /// </summary>
        void CreateGUI()
        {
            var root = rootVisualElement;
            root.styleSheets.Add(AssetDatabase.LoadAssetAtPath<StyleSheet>("Packages/com.unity.dt.app-ui/Editor/Navigation/NavigationGraphWindow.uss"));

            m_GraphViewPane = new VisualElement();
            m_GraphViewPane.AddToClassList("graph-view-pane");
            root.Add(m_GraphViewPane);

            var graphView = new NavigationGraphView();
            graphView.StretchToParentSize();
            m_GraphViewPane.Add(graphView);

            var breadcrumbs = new VisualElement();
            breadcrumbs.AddToClassList("breadcrumbs");
            m_GraphViewPane.Add(breadcrumbs);

            var generateCodeButton = new Button(OnGenerateCodeClicked)
            {
                text = "Generate Code"
            };
            generateCodeButton.AddToClassList("generate-code-button");
            m_GraphViewPane.Add(generateCodeButton);

            var followNavigationToggle = new Toggle("Follow Navigation");
            followNavigationToggle.AddToClassList("follow-navigation-toggle");
            followNavigationToggle.RegisterValueChangedCallback(evt =>
            {
                graphView.followNavigation = evt.newValue;
                m_FollowNavigation = evt.newValue;
            });
            followNavigationToggle.value = m_FollowNavigation;
            m_GraphViewPane.Add(followNavigationToggle);

            graphView.graphChanged += (graph) =>
            {
                breadcrumbs.Clear();
                var currentGraph = graph;
                var first = true;
                while (currentGraph)
                {
                    var label = new Label(currentGraph.name)
                    {
                        userData = currentGraph
                    };
                    label.AddToClassList("breadcrumb");
                    if (first)
                    {
                        label.style.unityFontStyleAndWeight = FontStyle.Bold;
                    }
                    else
                    {
                        label.RegisterCallback<ClickEvent>(evt =>
                        {
                            graphView.SetGraph(((Label)evt.target).userData as NavGraph);
                        });
                    }

                    breadcrumbs.Insert(0, label);
                    currentGraph = currentGraph.parent;
                    breadcrumbs.Insert(0, new Label(" > "));
                    first = false;
                }
                breadcrumbs.Insert(0, new Label(" ⌂ "));
            };

            m_GraphViewPane.SetEnabled(false);

            // load last graph
            if (m_LastGraphAsset)
            {
                graphView.SetGraphAsset(m_LastGraphAsset);
                m_GraphViewPane.SetEnabled(true);
                m_GraphViewPane.CapturePointer(PointerId.mousePointerId);
            }
        }

        /// <summary>
        /// Called when the Generate Code button is clicked.
        /// </summary>
        void OnGenerateCodeClicked()
        {
            var graphView = rootVisualElement.Q<NavigationGraphView>();
            var graph = graphView.graphAsset;
            if (graph)
                NavigationCodeGenerator.GenerateCode(graph);
        }

        /// <summary>
        /// Called when an asset is opened.
        /// </summary>
        /// <param name="instanceID"> The instance ID of the asset being opened. </param>
        /// <param name="line"> The line number where the asset is being opened. </param>
        /// <returns> True if the asset was opened, false otherwise. </returns>
        [OnOpenAsset(1, OnOpenAssetAttributeMode.Execute)]
        static bool OnOpenAsset(int instanceID, int line)
        {
            var asset = EditorUtility.InstanceIDToObject(instanceID) as NavGraphViewAsset;
            if (asset != null)
            {
                Init();
                var graphView = s_Instance.rootVisualElement.Q<NavigationGraphView>();
                graphView.SetGraphAsset(asset);
                s_Instance.m_GraphViewPane.SetEnabled(true);
                s_Instance.m_LastGraphAsset = asset;
                graphView.FrameAll();
                return true;
            }

            return false;
        }
    }
}
