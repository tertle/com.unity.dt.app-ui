using System;
using UnityEngine;

namespace Unity.AppUI.Navigation
{
    /// <summary>
    /// Available animations for navigation.
    /// </summary>
    public enum NavigationAnimation
    {
        /// <summary>
        /// No animation.
        /// </summary>
        None,

        /// <summary>
        /// The destination will scale up from the center of the screen.
        /// </summary>
        ScaleUpFadeOut,

        /// <summary>
        /// The destination will scale down to the center of the screen.
        /// </summary>
        ScaleDownFadeIn,

        /// <summary>
        /// The destination will fade in.
        /// </summary>
        FadeIn,

        /// <summary>
        /// The destination will fade out.
        /// </summary>
        FadeOut,
    }

    /// <summary>
    /// Strategy for popping up to a destination.
    /// </summary>
    public enum PopupToStrategy
    {
        /// <summary>
        /// The back stack will not be popped.
        /// </summary>
        None,

        /// <summary>
        /// The back stack will be popped up to the destination with the given ID.
        /// </summary>
        SpecificRoute,

        /// <summary>
        /// The back stack will be popped up to the current graph's start destination.
        /// </summary>
        CurrentStartDestination,
    }

    /// <summary>
    /// NavOptions stores special options for navigate actions
    /// </summary>
    [Serializable]
    public class NavOptions
    {
        [SerializeField]
        PopupToStrategy m_PopUpToStrategy;

        [SerializeField]
        NavDestination m_PopUpToRoute;

        [SerializeField]
        bool m_PopUpToInclusive;

        [SerializeField]
        NavigationAnimation m_EnterAnim;

        [SerializeField]
        NavigationAnimation m_ExitAnim;

        [SerializeField]
        NavigationAnimation m_PopEnterAnim;

        [SerializeField]
        NavigationAnimation m_PopExitAnim;

        [SerializeField]
        bool m_PopUpToSaveState;

        [SerializeField]
        bool m_RestoreState;

        [SerializeField]
        bool m_LaunchSingleTop;

        /// <summary>
        /// Strategy for popping up to a destination.
        /// </summary>
        public PopupToStrategy popUpToStrategy
        {
            get => m_PopUpToStrategy;
            set => m_PopUpToStrategy = value;
        }

        /// <summary>
        /// Route for the destination to pop up to before navigating.
        /// When set, all non-matching destinations should be popped from the back stack.
        /// </summary>
        public NavDestination popUpToRoute
        {
            get => m_PopUpToRoute;
            set => m_PopUpToRoute = value;
        }

        /// <summary>
        /// Whether the target destination in PopUpTo should be popped from the back stack.
        /// </summary>
        public bool popUpToInclusive
        {
            get => m_PopUpToInclusive;
            set => m_PopUpToInclusive = value;
        }

        /// <summary>
        /// The animation to use when navigating to the destination.
        /// </summary>
        public NavigationAnimation enterAnim
        {
            get => m_EnterAnim;
            set => m_EnterAnim = value;
        }

        /// <summary>
        /// The animation to use when navigating away from the destination.
        /// </summary>
        public NavigationAnimation exitAnim
        {
            get => m_ExitAnim;
            set => m_ExitAnim = value;
        }

        /// <summary>
        /// The custom enter Animation/Animator that should be run when this destination is popped from the back stack.
        /// </summary>
        public NavigationAnimation popEnterAnim
        {
            get => m_PopEnterAnim;
            set => m_PopEnterAnim = value;
        }

        /// <summary>
        /// The custom exit Animation/Animator that should be run when this destination is popped from the back stack.
        /// </summary>
        public NavigationAnimation popExitAnim
        {
            get => m_PopExitAnim;
            set => m_PopExitAnim = value;
        }

        /// <summary>
        /// Whether the back stack and the state of all destinations between the current destination
        /// and popUpToId should be saved for later restoration
        /// </summary>
        public bool popUpToSaveState
        {
            get => m_PopUpToSaveState;
            set => m_PopUpToSaveState = value;
        }

        /// <summary>
        /// Whether this navigation action should restore any state previously saved by
        /// Builder.setPopUpTo or the popUpToSaveState attribute.
        /// </summary>
        public bool restoreState
        {
            get => m_RestoreState;
            set => m_RestoreState = value;
        }

        /// <summary>
        /// Whether this navigation action should launch as single-top
        /// (i.e., there will be at most one copy of a given destination on the top of the back stack).
        /// </summary>
        public bool launchSingleTop
        {
            get => m_LaunchSingleTop;
            set => m_LaunchSingleTop = value;
        }
    }
}
