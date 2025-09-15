using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace Unity.AppUI.Navigation.Editor
{
    [CustomPropertyDrawer(typeof(DefaultDestinationTemplate))]
    class DefaultDestinationTemplateDrawer : PropertyDrawer
    {
        static readonly Dictionary<string, string> k_Templates = TypeCache
            .GetTypesDerivedFrom<NavigationScreen>()
            .Append(typeof(NavigationScreen))
            .ToDictionary(t => t.AssemblyQualifiedName, t => t.Name + " (" + t.Namespace + ")");

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var container = new VisualElement();

            var foldout = new Foldout
            {
                text = property.displayName
            };
            container.Add(foldout);

            var templateField = new PopupField<string>(
                "Screen Type",
                k_Templates.Keys.ToList(),
                0,
                s => string.IsNullOrEmpty(s) ? "None" : k_Templates[s],
                s => k_Templates[s]);
            foldout.Add(templateField);
            templateField.bindingPath = "m_ScreenType";

            var showBottomNavBar = new Toggle("Show Bottom Nav Bar");
            foldout.Add(showBottomNavBar);
            showBottomNavBar.bindingPath = "m_ShowBottomNavBar";

            var showAppBar = new Toggle("Show App Bar");
            foldout.Add(showAppBar);
            showAppBar.bindingPath = "m_ShowAppBar";

            var showBackButton = new Toggle("Show Back Button");
            foldout.Add(showBackButton);
            showBackButton.bindingPath = "m_ShowBackButton";

            var showDrawer = new Toggle("Show Drawer");
            foldout.Add(showDrawer);
            showDrawer.bindingPath = "m_ShowDrawer";

            var showNavigationRail = new Toggle("Show Navigation Rail");
            foldout.Add(showNavigationRail);
            showNavigationRail.bindingPath = "m_ShowNavigationRail";

            return container;
        }
    }
}
