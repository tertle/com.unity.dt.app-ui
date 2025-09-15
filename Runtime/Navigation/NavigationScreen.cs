using System;
using Unity.AppUI.Core;
using Unity.AppUI.UI;
using UnityEngine.UIElements;

namespace Unity.AppUI.Navigation
{
    /// <summary>
    /// Base class for all navigation screens that implements the <see cref="INavigationScreen"/> interface.
    /// A navigation screen is a <see cref="VisualElement"/> that can be pushed to a <see cref="NavHost"/>.
    /// </summary>
    /// <seealso cref="INavigationScreen"/>
    public class NavigationScreen : VisualElement, INavigationScreen
    {
        /// <summary>
        /// The NavigationScreen main styling class.
        /// </summary>
        public const string ussClassName = "appui-navigation-screen";

        /// <summary>
        /// The NavigationScreen container styling class.
        /// </summary>
        public const string containerUssClassName = ussClassName + "__container";

        /// <summary>
        /// The NavigationScreen left container styling class.
        /// </summary>
        public const string leftContainerUssClassName = ussClassName + "__left-container";

        /// <summary>
        /// The NavigationScreen right container styling class.
        /// </summary>
        public const string rightContainerUssClassName = ussClassName + "__right-container";

        /// <summary>
        /// The NavigationScreen navigation rail styling class.
        /// </summary>
        public const string navigationRailUssClassName = ussClassName + "__navigation-rail";

        /// <summary>
        /// The NavigationScreen bottom nav bar styling class.
        /// </summary>
        public const string bottomNavBarUssClassName = ussClassName + "__bottom-nav-bar";

        /// <summary>
        /// The NavigationScreen app bar styling class.
        /// </summary>
        public const string appBarUssClassName = ussClassName + "__app-bar";

        /// <summary>
        /// The NavigationScreen drawer styling class.
        /// </summary>
        public const string drawerUssClassName = ussClassName + "__drawer";

        /// <summary>
        /// The NavigationScreen with app bar styling class.
        /// </summary>
        public const string withAppBarUssClassName = ussClassName + "--with-appbar";

        /// <summary>
        /// The NavigationScreen with drawer styling class.
        /// </summary>
        public const string withDrawerUssClassName = ussClassName + "--with-drawer--";

        /// <summary>
        /// The NavigationScreen with rail styling class.
        /// </summary>
        public const string withRailUssClassName = ussClassName + "--with-rail--";

        /// <summary>
        /// The NavigationScreen with bottom nav bar styling class.
        /// </summary>
        public const string withBottomNavBarUssClassName = ussClassName + "--with-bottom-nav-bar";

        /// <summary>
        /// The NavigationScreen with compact app bar styling class.
        /// </summary>
        public const string withCompactAppBarUssClassName = withAppBarUssClassName + "--compact";

        /// <summary>
        /// Child elements are added to this element.
        /// </summary>
        public override VisualElement contentContainer => scrollView.contentContainer;

        /// <summary>
        /// The <see cref="ScrollView"/> that will be used to display the content of the screen.
        /// </summary>
        public ScrollView scrollView { get; }

        /// <summary>
        /// The <see cref="NavigationRail"/> that will be used to display the navigation items of the screen.
        /// </summary>
        public NavigationRail navigationRail { get; }

        /// <summary>
        /// The <see cref="BottomNavBar"/> that will be used to display the bottom navigation items of the screen.
        /// </summary>
        public BottomNavBar bottomNavBar { get; }

        /// <summary>
        /// The <see cref="AppBar"/> that will be used to display the app bar of the screen.
        /// </summary>
        public AppBar appBar { get; }

        /// <summary>
        /// The <see cref="Drawer"/> that will be used to display the drawer of the screen.
        /// </summary>
        public Drawer drawer { get; }

        /// <summary>
        /// Display the navigation rail on this screen.
        /// </summary>
        public bool showBottomNavBar { get; set; }

        /// <summary>
        /// Display the bottom navigation bar on this screen.
        /// </summary>
        public bool showAppBar { get; set; }

        /// <summary>
        /// Display the drawer on this screen.
        /// </summary>
        public bool showBackButton { get; set; }

        /// <summary>
        /// Display the drawer on this screen.
        /// </summary>
        public bool showDrawer { get; set; }

        /// <summary>
        /// Display the navigation rail on this screen.
        /// </summary>
        public bool showNavigationRail { get; set; }

        /// <summary>
        /// Base Constructor.
        /// </summary>
        public NavigationScreen()
        {
            AddToClassList(ussClassName);

            var leftContainer = new VisualElement { name = leftContainerUssClassName };
            leftContainer.AddToClassList(leftContainerUssClassName);
            hierarchy.Add(leftContainer);

            var rightContainer = new VisualElement { name = rightContainerUssClassName };
            rightContainer.AddToClassList(rightContainerUssClassName);
            hierarchy.Add(rightContainer);

            navigationRail = new NavigationRail { name = navigationRailUssClassName };
            navigationRail.AddToClassList(navigationRailUssClassName);
            leftContainer.hierarchy.Add(navigationRail);

            scrollView = new ScrollView { name = containerUssClassName };
            scrollView.AddToClassList(containerUssClassName);
            rightContainer.hierarchy.Add(scrollView);

            bottomNavBar = new BottomNavBar { name = bottomNavBarUssClassName };
            bottomNavBar.AddToClassList(bottomNavBarUssClassName);
            rightContainer.hierarchy.Add(bottomNavBar);

            appBar = new AppBar { name = appBarUssClassName };
            appBar.AddToClassList(appBarUssClassName);
            rightContainer.hierarchy.Add(appBar);

            drawer = new Drawer { name = drawerUssClassName };
            drawer.AddToClassList(drawerUssClassName);
            rightContainer.hierarchy.Add(drawer);
        }

        /// <summary>
        /// Called when the screen is pushed to a <see cref="NavHost"/>.
        /// </summary>
        /// <param name="controller"> The <see cref="NavController"/> that manages the navigation stack. </param>
        /// <param name="destination"> The <see cref="NavDestination"/> associated with this screen. </param>
        /// <param name="args"> The arguments associated with this screen. </param>
        public virtual void OnEnter(NavController controller, NavDestination destination, Argument[] args)
        {
            if (GetFirstAncestorOfType<NavHost>() is not { } navHost)
                return;

            if (showNavigationRail)
            {
                navHost.visualController?.SetupNavigationRail(navigationRail, destination, controller);
                SetupNavigationRail(navigationRail, controller);
            }

            if (showBottomNavBar)
            {
                navHost.visualController?.SetupBottomNavBar(bottomNavBar, destination, controller);
                SetupBottomNavBar(bottomNavBar, controller);
            }

            if (showAppBar)
            {
                navHost.visualController?.SetupAppBar(appBar, destination, controller);
                SetupAppBar(appBar, controller);
                if (showBackButton && controller.canGoBack)
                {
                    appBar.backButtonTriggered += () => controller.PopBackStack();
                    appBar.showBackButton = true;
                }
            }

            if (showDrawer)
            {
                navHost.visualController?.SetupDrawer(drawer, destination, controller);
                SetupDrawer(drawer, controller);
                if (showAppBar && (!controller.canGoBack || !showBackButton))
                {
                    appBar.showDrawerButton = true;
                    appBar.drawerButtonTriggered += () => drawer.Toggle();
                }
            }
        }

        /// <summary>
        /// Called when the screen is popped from a <see cref="NavHost"/>.
        /// </summary>
        /// <param name="controller"> The <see cref="NavController"/> that manages the navigation stack. </param>
        /// <param name="destination"> The <see cref="NavDestination"/> associated with this screen. </param>
        /// <param name="args"> The arguments associated with this screen. </param>
        /// <remarks>
        /// This method is called before the screen is removed from the <see cref="NavHost"/>.
        /// </remarks>
        public virtual void OnExit(NavController controller, NavDestination destination, Argument[] args)
        {

        }

        /// <summary>
        /// Implement this method to setup the <see cref="BottomNavBar"/> of this screen specifically.
        /// </summary>
        /// <param name="bottomNavBar"> The <see cref="BottomNavBar"/> to setup. </param>
        /// <param name="controller"> The <see cref="NavController"/> that manages the navigation stack. </param>
        /// <remarks>
        /// To setup the <see cref="BottomNavBar"/> globally, use <see cref="INavVisualController.SetupBottomNavBar"/>
        /// in your implementation of <see cref="INavVisualController"/>.
        /// </remarks>
        protected virtual void SetupBottomNavBar(BottomNavBar bottomNavBar, NavController controller)
        {
            AddToClassList(withBottomNavBarUssClassName);
        }

        /// <summary>
        /// Implement this method to setup the <see cref="AppBar"/> of this screen specifically.
        /// </summary>
        /// <param name="appBar"> The <see cref="AppBar"/> to setup. </param>
        /// <param name="controller"> The <see cref="NavController"/> that manages the navigation stack. </param>
        /// <remarks>
        /// To setup the <see cref="AppBar"/> globally, use <see cref="INavVisualController.SetupAppBar"/>
        /// in your implementation of <see cref="INavVisualController"/>.
        /// </remarks>
        protected virtual void SetupAppBar(AppBar appBar, NavController controller)
        {
            if (appBar.stretch)
            {
                scrollView.verticalScroller.valueChanged += (f) =>
                {
                    appBar.scrollOffset = f;
                };
                appBar.RegisterCallback<GeometryChangedEvent>(evt =>
                {
                    var h = evt.newRect.height;
                    scrollView.style.marginTop = h;
                });
            }
            appBar.scrollOffset = scrollView.verticalScroller.value;
            AddToClassList(withAppBarUssClassName);
            EnableInClassList(withCompactAppBarUssClassName, appBar.compact);
        }

        /// <summary>
        /// Implement this method to setup the <see cref="Drawer"/> of this screen specifically.
        /// </summary>
        /// <param name="drawer"> The <see cref="Drawer"/> to setup. </param>
        /// <param name="controller"> The <see cref="NavController"/> that manages the navigation stack. </param>
        /// <remarks>
        /// To setup the <see cref="Drawer"/> globally, use <see cref="INavVisualController.SetupDrawer"/>
        /// in your implementation of <see cref="INavVisualController"/>.
        /// </remarks>
        protected virtual void SetupDrawer(Drawer drawer, NavController controller)
        {
            AddToClassList(MemoryUtils.Concatenate(withDrawerUssClassName, drawer.anchor.ToLowerCase()));
        }

        /// <summary>
        /// Implement this method to setup the <see cref="NavigationRail"/> of this screen specifically.
        /// </summary>
        /// <param name="navigationRail"> The <see cref="NavigationRail"/> to setup. </param>
        /// <param name="controller"> The <see cref="NavController"/> that manages the navigation stack. </param>
        /// <remarks>
        /// To setup the <see cref="NavigationRail"/> globally, use <see cref="INavVisualController.SetupNavigationRail"/>
        /// in your implementation of <see cref="INavVisualController"/>.
        /// </remarks>
        protected virtual void SetupNavigationRail(NavigationRail navigationRail, NavController controller)
        {
            AddToClassList(MemoryUtils.Concatenate(withRailUssClassName, navigationRail.anchor.ToLowerCase()));
        }
    }
}
