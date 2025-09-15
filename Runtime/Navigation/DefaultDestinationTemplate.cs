using System;
using UnityEngine;

namespace Unity.AppUI.Navigation
{
    /// <summary>
    /// Represents a default destination template for navigation within the application.
    /// </summary>
    /// <seealso cref="NavigationScreen"/>
    /// <seealso cref="NavDestinationTemplate"/>
    [Serializable]
    public class DefaultDestinationTemplate : NavDestinationTemplate
    {
        [SerializeField]
        [Tooltip("The screen to use when navigating to this destination. This is the Assembly Qualified Name of the view class.")]
        string m_ScreenType;

        [SerializeField]
        [Tooltip("Weathers or not to show the bottom navigation bar when navigating to this destination.")]
        bool m_ShowBottomNavBar = true;

        [SerializeField]
        [Tooltip("Weathers or not to show the app bar when navigating to this destination.")]
        bool m_ShowAppBar = true;

        [SerializeField]
        [Tooltip("Weathers or not to show the back button when navigating to this destination." +
            "This property is ignored if showAppBar is false.")]
        bool m_ShowBackButton = true;

        [SerializeField]
        [Tooltip("Weathers or not to show the drawer when navigating to this destination.")]
        bool m_ShowDrawer = true;

        [SerializeField]
        [Tooltip("Weathers or not to show the navigation rail when navigating to this destination.")]
        bool m_ShowNavigationRail = true;

        /// <summary>
        /// The type of screen to use when navigating to this destination.
        /// </summary>
        public string template
        {
            get => m_ScreenType;
            set => m_ScreenType = value;
        }

        /// <summary>
        /// Weathers or not to show the bottom navigation bar when navigating to this destination.
        /// </summary>
        public bool showBottomNavBar
        {
            get => m_ShowBottomNavBar;
            set => m_ShowBottomNavBar = value;
        }

        /// <summary>
        /// Weathers or not to show the app bar when navigating to this destination.
        /// </summary>
        public bool showAppBar
        {
            get => m_ShowAppBar;
            set => m_ShowAppBar = value;
        }

        /// <summary>
        /// Weathers or not to show the back button when navigating to this destination.
        /// </summary>
        /// <remarks> This property is ignored if <see cref="showAppBar"/> is false. </remarks>
        public bool showBackButton
        {
            get => m_ShowBackButton;
            set => m_ShowBackButton = value;
        }

        /// <summary>
        /// Weathers or not to show the drawer when navigating to this destination.
        /// </summary>
        public bool showDrawer
        {
            get => m_ShowDrawer;
            set => m_ShowDrawer = value;
        }

        /// <summary>
        /// Weathers or not to show the navigation rail when navigating to this destination.
        /// </summary>
        public bool showNavigationRail
        {
            get => m_ShowNavigationRail;
            set => m_ShowNavigationRail = value;
        }

        /// <inheritdoc />
        public override INavigationScreen CreateScreen(NavHost host)
        {
            NavigationScreen screen = null;
            var type = Type.GetType(template);
            if (type == null)
            {
                var msg = string.IsNullOrEmpty(template) ? "The template type is not set." : $"The template type '{template}' is not valid.";
                Debug.LogWarning($"{msg} Falling back to default screen type.");
                screen = new NavigationScreen();
            }
            else
            {
                screen = Activator.CreateInstance(type) as NavigationScreen;
            }

            if (screen == null)
            {
                throw new InvalidOperationException($"The template '{template}' could not be instantiated. " +
                                                    "Ensure that the type is a valid NavigationScreen and is accessible " +
                                                    "and has a parameterless constructor.");
            }

            screen.showBottomNavBar = showBottomNavBar;
            screen.showAppBar = showAppBar;
            screen.showBackButton = showBackButton;
            screen.showDrawer = showDrawer;
            screen.showNavigationRail = showNavigationRail;

            return screen;
        }
    }
}
