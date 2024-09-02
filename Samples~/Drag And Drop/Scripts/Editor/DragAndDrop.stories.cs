using System;
using Unity.AppUI.Editor;
using Unity.AppUI.UI;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.AppUI.Sample.Editor
{
    public class DragAndDropPage : StoryBookPage
    {
        public override string displayName => "Drag And Drop";

        public override Type componentType => null;

        public DragAndDropPage()
        {
            m_Stories.Add(new StoryBookStory("Main", MainStory));
            m_Stories.Add(new StoryBookStory("Over Elements", OverElementsStory));
        }

        static VisualElement MainStory()
        {
            var element = new VisualElement();
            var tree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(AssetDatabase.GUIDToAssetPath("febb74e948b94540999325bf7cf13cd3"));
            var styleSheet = AssetDatabase.LoadAssetAtPath<ThemeStyleSheet>(AssetDatabase.GUIDToAssetPath("2a9862e95ebae4adaa5eb143ed0b4c98"));
            tree.CloneTree(element);
            var root = element.Q<VisualElement>("main-root");
            var desc = root.Q<Text>("dnd-desc");
            desc.text += "\nYou can also drag items from others panels of the Editor into the destination list to generate items based on dragged paths.";
            root.styleSheets.Add(styleSheet);
            var script = new Unity.AppUI.Samples.DragAndDropSample.DragAndDropSampleScript();
            script.Start(root);
            return root;
        }

        static VisualElement OverElementsStory()
        {
            var element = new VisualElement();
            var tree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(AssetDatabase.GUIDToAssetPath("3c2afccb5abd4b5896196ba58ac8af96"));
            var styleSheet = AssetDatabase.LoadAssetAtPath<ThemeStyleSheet>(AssetDatabase.GUIDToAssetPath("2a9862e95ebae4adaa5eb143ed0b4c98"));
            tree.CloneTree(element);
            var root = element.Q<VisualElement>("main-root");
            root.styleSheets.Add(styleSheet);
            var script = new Unity.AppUI.Samples.DragAndDropOverElementsSample.DragAndDropOverElementsSampleScript();
            script.Start(root);
            return root;
        }
    }
}
