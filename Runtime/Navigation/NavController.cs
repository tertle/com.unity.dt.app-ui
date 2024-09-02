using System;
using System.Collections.Generic;
using Unity.AppUI.Core;

namespace Unity.AppUI.Navigation
{
    /// <summary>
    /// The NavController manages navigation within a NavHost. It is responsible for
    /// keeping track of the navigation stack and the current destination.
    /// </summary>
    public class NavController
    {
        const int k_ActionNavigate = 1234;
        const int k_ActionPopBack = 12345;

        /// <summary>
        /// Event that is invoked when an action is triggered.
        /// </summary>
        public static event Action<NavController, NavAction> actionTriggered;

        /// <summary>
        /// Event that is invoked when the current destination changes.
        /// </summary>
        public static event Action<NavController, NavDestination> destinationChanged;

        NavGraphViewAsset m_GraphAsset;

        /// <summary>
        /// Returns true if there is a destination on the back stack that can be popped.
        /// </summary>
        public bool canGoBack => m_BackStack.Count > 0;

        /// <summary>
        /// The used graph asset.
        /// </summary>
        public NavGraphViewAsset graphAsset
        {
            get => m_GraphAsset;
        }

        Argument[] m_CurrentArgs;

        /// <summary>
        /// The current destination.
        /// </summary>
        public NavDestination currentDestination
        {
            get => m_CurrentDestination;
            set
            {
                if (m_CurrentDestination == value)
                    return;

                m_CurrentDestination = value;
                destinationChanged?.Invoke(this, m_CurrentDestination);
            }
        }

        /// <summary>
        /// The last entry on the back stack.
        /// </summary>
        public NavBackStackEntry currentBackStackEntry => m_BackStack.TryPeek(out var entry) ? entry : null;

        /// <summary>
        /// The current graph.
        /// </summary>
        /// <remarks> Can be null if there's no current destination. </remarks>
        public NavGraph graph => currentDestination ? currentDestination.parent : null;

        readonly Stack<NavBackStackEntry> m_BackStack = new Stack<NavBackStackEntry>();

        readonly NavHost m_NavHost;

        Handler m_Handler;

        NavDestination m_CurrentDestination;

        NavigationAnimation m_CurrentPopExitAnimation = NavigationAnimation.None;

        NavigationAnimation m_CurrentPopEnterAnimation = NavigationAnimation.None;

        readonly Dictionary<string, Stack<NavBackStackEntry>> m_SavedStates = new Dictionary<string, Stack<NavBackStackEntry>>();

        /// <summary>
        /// The message queue handler to navigate from the main thread.
        /// </summary>
        Handler handler
        {
            get
            {
                if (m_Handler == null)
                {
                    m_Handler = new Handler(AppUI.Core.AppUI.mainLooper, message =>
                    {
                        switch (message.what)
                        {
                            case k_ActionNavigate:
                                HandleNavigate((NavBackStackEntry)message.obj);
                                return true;
                            case k_ActionPopBack:
                                HandlePopBack((NavBackStackEntry)message.obj);
                                return true;
                            default:
                                return false;
                        }
                    });
                }

                return m_Handler;
            }
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="navHost"> The NavHost that this NavController is associated with. </param>
        public NavController(NavHost navHost)
        {
            m_NavHost = navHost;
        }

        /// <summary>
        /// Set the Navigation Graph asset that will be used as navigation definition.
        /// </summary>
        /// <param name="navGraphViewAsset"> The Navigation Graph asset. </param>
        public void SetGraph(NavGraphViewAsset navGraphViewAsset)
        {
            if (ReferenceEquals(m_GraphAsset, navGraphViewAsset))
                return;

            if (m_GraphAsset && m_GraphAsset.Equals(navGraphViewAsset))
                return;

            m_GraphAsset = navGraphViewAsset;
            RebuildFromGraph();
        }

        /// <summary>
        /// Clear the back stack entirely.
        /// </summary>
        /// <remarks> This will not clear the saved states, and the current destination will not be changed. </remarks>
        public void ClearBackStack()
        {
            m_BackStack.Clear();
        }

        void RebuildFromGraph()
        {
            ClearBackStack();
            // try to navigate
            if (m_GraphAsset != null && !m_GraphAsset.isEmpty)
            {
                NavigateInternal(m_GraphAsset.rootGraph.FindStartDestination(), null, null);
            }
        }

        /// <summary>
        /// Navigate to the destination with the given name.
        /// </summary>
        /// <param name="actionOrDestination"> The name of the destination or action. </param>
        /// <param name="arguments"> The arguments to pass to the destination. </param>
        /// <returns> True if the navigation was successful. </returns>
        public bool Navigate(string actionOrDestination, params Argument[] arguments)
        {
            if (m_GraphAsset.TryFindAction(actionOrDestination, out var action))
            {
                actionTriggered?.Invoke(this, action);
                return Navigate(action.destination.name, action.options, arguments);
            }
            return Navigate(actionOrDestination, null, arguments);
        }

        /// <summary>
        /// Navigate to the destination with the given name.
        /// </summary>
        /// <param name="route"> The route to the destination. </param>
        /// <param name="options"> The options to use for the navigation. </param>
        /// <param name="arguments"> The arguments to pass to the destination. </param>
        /// <returns> True if the navigation was successful. </returns>
        public bool Navigate(string route, NavOptions options, params Argument[] arguments)
        {
            options ??= new NavOptions();

            var destination = m_GraphAsset.FindDestinationByRoute(route);

            if (destination == null)
                return false;

            if (!m_GraphAsset.CanNavigate(currentDestination, destination, route))
                return false;

            if (currentDestination != null)
            {
                var canPush = !options.launchSingleTop ||
                    currentBackStackEntry == null ||
                    currentBackStackEntry.destination.name != currentDestination.name;
                if (canPush)
                    m_BackStack.Push(new NavBackStackEntry(currentDestination, options, m_CurrentArgs));
            }

            if (options.popUpToStrategy == PopupToStrategy.CurrentStartDestination)
                options.popUpToRoute = graph.FindStartDestination();

            if (options.popUpToStrategy != PopupToStrategy.None && options.popUpToRoute)
                PopUpTo(options.popUpToRoute.name, options.popUpToInclusive, options.popUpToSaveState);

            if (options.restoreState)
                RestoreState(route);

            if (destination.name == currentBackStackEntry?.destination.name && options.launchSingleTop)
                m_BackStack.Pop();

            NavigateInternal(destination, options, arguments);
            return true;
        }

        /// <summary>
        /// Navigate to the destination with the given DeepLink.
        /// </summary>
        /// <param name="deepLink"> The DeepLink to navigate to. </param>
        /// <param name="options"> The options to use for the navigation. </param>
        /// <returns> True if the navigation was successful. </returns>
        public bool Navigate(Uri deepLink, NavOptions options = null)
        {
            // clear current back stack
            ClearBackStack();

            if (deepLink.Segments.Length == 1)
            {
                // navigate to startDestination
                NavigateInternal(m_GraphAsset.rootGraph.FindStartDestination(), options, ParseDeepLinkQuery(deepLink.Query));
                return true;
            }

            // simulate navigation through each segment but not the last one
            for (var i = 1; i < deepLink.Segments.Length - 1; i++)
            {
                var segment = deepLink.Segments[i];
                var destination = m_GraphAsset.FindDestinationByRoute(segment);
                if (destination == null)
                {
                    // Unable to find the destination in the graph
                    ClearBackStack();
                    NavigateInternal(m_GraphAsset.rootGraph.FindStartDestination(), options, null);
                    return false;
                }
                m_BackStack.Push(new NavBackStackEntry(destination, new NavOptions(), new Argument[] {}));
            }

            // navigate to the last segment
            options = options ?? new NavOptions();
            options.enterAnim = NavigationAnimation.None;
            var lastSegment = deepLink.Segments[deepLink.Segments.Length - 1];
            var lastDestination = m_GraphAsset.FindDestinationByRoute(lastSegment);
            if (lastDestination == null)
            {
                // Unable to find the last segment in destinations routes
                ClearBackStack();
                NavigateInternal(m_GraphAsset.rootGraph.FindStartDestination(), options, null);
                return false;
            }
            NavigateInternal(lastDestination, options, ParseDeepLinkQuery(deepLink.Query));
            return true;
        }

        void NavigateInternal(NavDestination destination, NavOptions options, Argument[] arguments)
        {
            if (destination == null)
                return;

            options = options ?? new NavOptions();
            arguments = arguments ?? new Argument[] {};
            handler.SendMessage(Message.Obtain(handler, k_ActionNavigate, new NavBackStackEntry(destination, options, arguments)));
        }

        static Argument[] ParseDeepLinkQuery(string query)
        {
            var args = query.TrimStart('?').Split('&');
            var result = new Argument[args.Length];

            for (var i = 0; i < args.Length; i++)
            {
                var arg = args[i];
                var split = arg.Split('=');
                if (split.Length == 2)
                {
                    result[i] = new Argument(split[0], split[1]);
                }
                else
                {
                    result[i] = new Argument(split[0], "1");
                }
            }

            return result;
        }

        void RestoreState(string route)
        {
            if (m_SavedStates.TryGetValue(route, out var stack))
            {
                while (stack.TryPop(out var entry))
                {
                    m_BackStack.Push(entry);
                }

                m_SavedStates.Remove(route);
            }
        }

        NavBackStackEntry PopUpTo(string route, bool inclusive, bool saveState)
        {
            var result = currentBackStackEntry;
            if (saveState)
            {
                if (m_SavedStates.TryGetValue(route, out var state))
                    state.Clear();
                else
                    m_SavedStates[route] = new Stack<NavBackStackEntry>();
            }
            while (m_BackStack.TryPeek(out var entry))
            {
                if (entry.destination.name == route)
                {
                    if (inclusive)
                    {
                        result = m_BackStack.Pop();
                        if (saveState)
                            m_SavedStates[route].Push(result);
                    }
                    break;
                }
                result = m_BackStack.Pop();
                if (saveState)
                    m_SavedStates[route].Push(result);
            }

            return result;
        }

        /// <summary>
        /// Pop the current destination from the back stack and navigate to the previous destination.
        /// </summary>
        /// <returns> True if the back stack was popped, false otherwise. </returns>
        public bool PopBackStack()
        {
            if (m_BackStack.Count > 0)
            {
                var entry = m_BackStack.Pop();
                handler.SendMessage(Message.Obtain(handler, k_ActionPopBack, entry));
                return true;
            }

            return false;
        }

        /// <summary>
        /// Pop the current destination from the back stack and navigate to the given destination.
        /// </summary>
        /// <param name="route"> The route of the destination to navigate to. </param>
        /// <param name="inclusive"> Whether to pop the destination with the given route or not. </param>
        /// <param name="saveState"> Whether to save the state of the popped destinations or not before popping them. </param>
        /// <returns> True if the back stack was popped, false otherwise. </returns>
        public bool PopBackStack(string route, bool inclusive = false, bool saveState = false)
        {
            var canPop = false;
            foreach (var entry in m_BackStack)
            {
                if (entry.destination.name == route)
                {
                    canPop = true;
                    break;
                }
            }

            if (canPop)
            {
                var entry = PopUpTo(route, inclusive, saveState);
                handler.SendMessage(Message.Obtain(handler, k_ActionPopBack, entry));
                return true;
            }

            return false;
        }

        void HandleNavigate(NavBackStackEntry entry)
        {
            m_CurrentPopEnterAnimation = entry.options.popEnterAnim;
            m_CurrentPopExitAnimation = entry.options.popExitAnim;
            m_NavHost.SwitchTo(entry.destination, entry.options.exitAnim, entry.options.enterAnim, entry.arguments, false, success =>
            {
                if (success)
                {
                    m_CurrentArgs = entry.arguments;
                    currentDestination = entry.destination;
                }
            });
        }

        void HandlePopBack(NavBackStackEntry entry)
        {
            var dest = entry.destination;
            var exitAnim = m_CurrentPopExitAnimation;
            var enterAnim = m_CurrentPopEnterAnimation;
            m_NavHost.SwitchTo(dest, exitAnim, enterAnim, entry.arguments,true, success =>
            {
                if (success)
                {
                    m_CurrentArgs = entry.arguments;
                    currentDestination = dest;
                }
            });
        }
    }
}
