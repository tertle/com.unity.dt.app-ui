using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Unity.AppUI.Navigation.Editor
{
    /// <summary>
    /// Navigation options drawer for the UIElements backend.
    /// </summary>
    [CustomPropertyDrawer(typeof(NavOptions))]
    public class NavOptionsDrawerUIE : PropertyDrawer
    {
        PropertyField m_PopUpToRouteField;
        PropertyField m_PopUpToStrategyField;
        PropertyField m_PopUpToInclusiveField;

        /// <summary>
        /// Override this method to make your own UI Toolkit based GUI for the property.
        /// </summary>
        /// <param name="property">The SerializedProperty to make the custom GUI for.</param>
        /// <returns> The VisualElement representing the custom GUI for the given property.</returns>
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var container = new Foldout
            {
                text = "Options"
            };

            var popUpToStrategy = property.FindPropertyRelative("m_PopUpToStrategy");
            m_PopUpToStrategyField = new PropertyField(popUpToStrategy);
            container.Add(m_PopUpToStrategyField);

            var popUpToRoute = property.FindPropertyRelative("m_PopUpToRoute");
            m_PopUpToRouteField = new PropertyField(popUpToRoute);
            container.Add(m_PopUpToRouteField);

            var popUpToInclusive = property.FindPropertyRelative("m_PopUpToInclusive");
            m_PopUpToInclusiveField = new PropertyField(popUpToInclusive);
            container.Add(m_PopUpToInclusiveField);

            var enterAnim = property.FindPropertyRelative("m_EnterAnim");
            var enterAnimField = new PropertyField(enterAnim);
            container.Add(enterAnimField);

            var exitAnim = property.FindPropertyRelative("m_ExitAnim");
            var exitAnimField = new PropertyField(exitAnim);
            container.Add(exitAnimField);

            var popExitAnim = property.FindPropertyRelative("m_PopExitAnim");
            var popExitAnimField = new PropertyField(popExitAnim);
            container.Add(popExitAnimField);

            var popEnterAnim = property.FindPropertyRelative("m_PopEnterAnim");
            var popEnterAnimField = new PropertyField(popEnterAnim);
            container.Add(popEnterAnimField);

            var popUpToSaveState = property.FindPropertyRelative("m_PopUpToSaveState");
            var popUpToSaveStateField = new PropertyField(popUpToSaveState);
            container.Add(popUpToSaveStateField);

            var popUpToRestoreState = property.FindPropertyRelative("m_RestoreState");
            var popUpToRestoreStateField = new PropertyField(popUpToRestoreState);
            container.Add(popUpToRestoreStateField);

            var launchSingleTop = property.FindPropertyRelative("m_LaunchSingleTop");
            var launchSingleTopField = new PropertyField(launchSingleTop);
            container.Add(launchSingleTopField);

            m_PopUpToStrategyField.RegisterValueChangeCallback(evt =>
            {
                Refresh(property);
            });

            Refresh(property);

            return container;
        }

        /// <summary>
        /// Refreshes the drawer based on the current state of the property.
        /// </summary>
        /// <param name="property"> The property used to refresh.</param>
        void Refresh(SerializedProperty property)
        {
            var popUpToStrategy = property.FindPropertyRelative("m_PopUpToStrategy");
            var strategy = (PopupToStrategy)popUpToStrategy.enumValueIndex;
            m_PopUpToRouteField.SetEnabled(strategy == PopupToStrategy.SpecificRoute);
            m_PopUpToInclusiveField.SetEnabled(strategy != PopupToStrategy.None);
        }
    }
}
