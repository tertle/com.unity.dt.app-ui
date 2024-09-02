using UnityEngine.UIElements;

namespace Unity.AppUI.Navigation
{
    /// <summary>
    /// Extension methods for VisualElement.
    /// </summary>
    public static class VisualElementExtensions
    {
        /// <summary>
        /// Find the NavigationController used by the NavigationContainer that contains this visual element.
        /// </summary>
        /// <param name="element"> The reference element that must be a descendant of <see cref="NavHost"/> component. </param>
        /// <returns> The <see cref="NavController"/> used by the <see cref="NavHost"/> that contains this visual element. </returns>
        public static NavController FindNavController(this VisualElement element)
        {
            return element?.GetFirstAncestorOfType<NavHost>()?.navController;
        }
    }
}
