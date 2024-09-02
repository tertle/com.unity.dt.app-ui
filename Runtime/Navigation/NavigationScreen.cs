using System;
using Unity.AppUI.UI;
using UnityEngine.UIElements;

namespace Unity.AppUI.Navigation
{
    /// <summary>
    /// Base class for all navigation screens.
    /// A navigation screen is a <see cref="VisualElement"/> that can be pushed to a <see cref="NavHost"/>.
    /// Classes that inherit from this class can be used as <see cref="NavDestination.template"/> in a <see cref="NavDestination"/>
    /// with the value of `typeof(YourScreenType).AssemblyQualifiedName`.
    /// </summary>
    public class NavigationScreen : VisualElement
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
        /// The NavigationScreen with app bar styling class.
        /// </summary>
        public const string withAppBarUssClassName = ussClassName + "--with-appbar";

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
        /// Base Constructor.
        /// </summary>
        public NavigationScreen()
        {
            AddToClassList(ussClassName);

            scrollView = new ScrollView { name = containerUssClassName };
            scrollView.AddToClassList(containerUssClassName);
            scrollView.StretchToParentSize();
            hierarchy.Add(scrollView);
        }

        /// <summary>
        /// Called when the screen is pushed to a <see cref="NavHost"/>.
        /// </summary>
        /// <param name="controller"> The <see cref="NavController"/> that manages the navigation stack. </param>
        /// <param name="destination"> The <see cref="NavDestination"/> associated with this screen. </param>
        /// <param name="args"> The arguments associated with this screen. </param>
        protected virtual void OnEnter(NavController controller, NavDestination destination, Argument[] args)
        {

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
        protected virtual void OnExit(NavController controller, NavDestination destination, Argument[] args)
        {

        }

        /// <summary>
        /// Implement this method to setup the <see cref="BottomNavBar"/> of this screen specifically.
        /// </summary>
        /// <param name="bottomNavBar"> The <see cref="BottomNavBar"/> to setup. </param>
        /// <remarks>
        /// To setup the <see cref="BottomNavBar"/> globally, use <see cref="INavVisualController.SetupBottomNavBar"/>
        /// in your implementation of <see cref="INavVisualController"/>.
        /// </remarks>
        protected internal virtual void SetupBottomNavBar(BottomNavBar bottomNavBar)
        {

        }

        /// <summary>
        /// Implement this method to setup the <see cref="AppBar"/> of this screen specifically.
        /// </summary>
        /// <param name="appBar"> The <see cref="AppBar"/> to setup. </param>
        /// <remarks>
        /// To setup the <see cref="AppBar"/> globally, use <see cref="INavVisualController.SetupAppBar"/>
        /// in your implementation of <see cref="INavVisualController"/>.
        /// </remarks>
        protected internal virtual void SetupAppBar(AppBar appBar)
        {

        }

        /// <summary>
        /// Implement this method to setup the <see cref="Drawer"/> of this screen specifically.
        /// </summary>
        /// <param name="drawer"> The <see cref="Drawer"/> to setup. </param>
        /// <remarks>
        /// To setup the <see cref="Drawer"/> globally, use <see cref="INavVisualController.SetupDrawer"/>
        /// in your implementation of <see cref="INavVisualController"/>.
        /// </remarks>
        protected internal virtual void SetupDrawer(Drawer drawer)
        {

        }

        internal void InvokeOnEnter(NavController controller, NavDestination destination, Argument[] args)
        {
            OnEnter(controller, destination, args);
        }

        internal void InvokeOnExit(NavController controller, NavDestination destination, Argument[] args)
        {
            OnExit(controller, destination, args);
        }
    }
}
