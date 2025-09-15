using UnityEngine;

namespace Unity.AppUI.Navigation
{
    /// <summary>
    /// Interface for a navigation screen. A navigation screen is a screen that can be navigated to and from.
    /// </summary>
    /// <seealso cref="NavigationScreen"/>
    public interface INavigationScreen
    {
        /// <summary>
        /// Called when the navigation controller enters a destination.
        /// </summary>
        /// <param name="controller"> The navigation controller that is entering the destination.</param>
        /// <param name="destination"> The destination that is being entered.</param>
        /// <param name="args"> The arguments passed to the destination.</param>
        void OnEnter(NavController controller, NavDestination destination, Argument[] args);

        /// <summary>
        /// Called when the navigation controller exits a destination.
        /// </summary>
        /// <param name="controller"> The navigation controller that is exiting the destination.</param>
        /// <param name="destination"> The destination that is being exited.</param>
        /// <param name="args"> The arguments passed to the destination.</param>
        void OnExit(NavController controller, NavDestination destination, Argument[] args);
    }
}
