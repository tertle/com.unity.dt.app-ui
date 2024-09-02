using System;
using System.Collections.Generic;
using UnityEngine;

namespace Unity.AppUI.Navigation
{
    /// <summary>
    /// <para>NavDestination represents one node within an overall navigation graph.</para>
    /// <para>
    /// Destinations declare a set of actions that they support. These actions form a navigation API for the destination;
    /// the same actions declared on different destinations that fill similar roles
    /// allow application code to navigate based on semantic intent.
    /// </para>
    /// <para>
    /// Each destination has a set of arguments that will be applied when navigating to that destination.
    /// Any default values for those arguments can be overridden at the time of navigation.
    /// </para>
    /// </summary>
    [Serializable]
    public class NavDestination : NavGraphViewHierarchicalNode
    {
        [SerializeField]
        [Tooltip("The screen to use when navigating to this destination. This is the Assembly Qualified Name of the view class.")]
        string m_Template;

        [SerializeField]
        [Tooltip("A dictionary of arguments to apply when navigating to this destination." +
                 "The key is the name of the argument and the value is the default value of the argument.")]
        List<NavArgumentKeyValuePair> m_Arguments;

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
        /// The screen to use when navigating to this destination.
        /// </summary>
        public string template
        {
            get => m_Template;
            set => m_Template = value;
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

        /// <summary>
        /// The arguments supported by this destination.
        /// </summary>
        public List<NavArgumentKeyValuePair> arguments
        {
            get => m_Arguments;
            set => m_Arguments = value;
        }
    }
}
