using Unity.AppUI.UI;

namespace Unity.AppUI.Navigation
{
    /// <summary>
    /// Interface for a navigation visual controller. A navigation visual controller is responsible for
    /// setting up the visual elements of a navigation graph. This includes the bottom navigation bar,
    /// the app bar, the drawer and the navigation rail.
    /// </summary>
    public interface INavVisualController
    {
        /// <summary>
        /// Setup the bottom navigation bar.
        /// </summary>
        /// <param name="bottomNavBar"> The bottom navigation bar to setup. </param>
        /// <param name="destination"> The destination to setup the bottom navigation bar for. </param>
        /// <param name="navController"> The navigation controller. </param>
        void SetupBottomNavBar(BottomNavBar bottomNavBar, NavDestination destination, NavController navController);

        /// <summary>
        /// Setup the app bar.
        /// </summary>
        /// <param name="appBar"> The app bar to setup. </param>
        /// <param name="destination"> The destination to setup the app bar for. </param>
        /// <param name="navController"> The navigation controller. </param>
        void SetupAppBar(AppBar appBar, NavDestination destination, NavController navController);

        /// <summary>
        /// Setup the drawer.
        /// </summary>
        /// <param name="drawer"> The drawer to setup. </param>
        /// <param name="destination"> The destination to setup the drawer for. </param>
        /// <param name="navController"> The navigation controller. </param>
        void SetupDrawer(Drawer drawer, NavDestination destination, NavController navController);

        /// <summary>
        /// Setup the navigation rail.
        /// </summary>
        /// <param name="navigationRail"> The navigation rail to setup. </param>
        /// <param name="destination"> The destination to setup the navigation rail for. </param>
        /// <param name="navController"> The navigation controller. </param>
        void SetupNavigationRail(NavigationRail navigationRail, NavDestination destination, NavController navController);
    }
}
