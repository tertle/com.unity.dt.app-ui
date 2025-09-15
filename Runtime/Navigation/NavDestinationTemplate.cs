using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.AppUI.Navigation
{
    /// <summary>
    /// Represents a default destination template for navigation within the application.
    /// </summary>
    /// <seealso cref="NavDestination"/>
    [Serializable]
    public abstract class NavDestinationTemplate
    {
        /// <summary>
        /// Creates a new instance of the navigation screen defined by this template.
        /// </summary>
        /// <param name="host"> The navigation host that will manage the screen.</param>
        /// <returns> An instance of the navigation screen defined by this template.</returns>
        /// <remarks>
        /// The returned screen should be a subclass of <see cref="VisualElement"/>.
        /// </remarks>
        public abstract INavigationScreen CreateScreen(NavHost host);
    }
}
