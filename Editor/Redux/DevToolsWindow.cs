using System;
using UnityEditor;
using UnityEngine;

namespace Unity.AppUI.Editor.Redux.DevTools
{
    /// <summary>
    /// A window that provides a set of tools for debugging and inspecting the state of the store.
    /// </summary>
    class DevToolsWindow : EditorWindow, IHasCustomMenu
    {
        /// <summary>
        /// Show a Redux DevTools editor window.
        /// </summary>
        [MenuItem("Window/App UI/Redux DevTools", priority = 2161)]
        static void ShowWindow()
        {
            var window = CreateWindow<DevToolsWindow>();
            window.titleContent = new UnityEngine.GUIContent("DevTools");
            window.Show();
        }

        void CreateGUI()
        {
            var root = rootVisualElement;
            var devToolsGUI = new DevToolsView();
            root.Add(devToolsGUI);
        }

        void IHasCustomMenu.AddItemsToMenu(GenericMenu menu)
        {
            //todo
        }
    }
}
