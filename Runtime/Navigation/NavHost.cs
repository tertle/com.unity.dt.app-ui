using System;
using Unity.AppUI.UI;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UIElements.Experimental;

namespace Unity.AppUI.Navigation
{
    /// <summary>
    /// The NavHost is the UI element that manages the navigation stack.
    /// It will manage the display of <see cref="NavigationScreen"/> objects through its <see cref="NavController"/>.
    /// </summary>
#if ENABLE_UXML_SERIALIZED_DATA
    [UxmlElement]
#endif
    public partial class NavHost : VisualElement
    {
        /// <summary>
        /// The NavHost main styling class.
        /// </summary>
        public const string ussClassName = "appui-navhost";

        /// <summary>
        /// The NavHost container styling class.
        /// </summary>
        public const string containerUssClassName = ussClassName + "__container";

        /// <summary>
        /// The NavHost item styling class.
        /// </summary>
        public const string itemUssClassName = ussClassName + "__item";

        /// <summary>
        /// The controller that manages the navigation stack.
        /// </summary>
        public NavController navController { get; }

        /// <summary>
        /// The visual controller that will be used to handle modification of Navigation visual elements, such as BottomNavBar.
        /// </summary>
        public INavVisualController visualController { get; set; }

        readonly VisualElement m_Container;

        ValueAnimation<float> m_RemoveAnim;

        ValueAnimation<float> m_AddAnim;

        /// <summary>
        /// The container that will hold the current <see cref="NavigationScreen"/>.
        /// </summary>
        public override VisualElement contentContainer => m_Container.contentContainer;

        /// <summary>
        /// Method called to create a new instance of a <see cref="NavigationScreen"/> based on a given type.
        /// </summary>
        public Func<Type, NavigationScreen> makeScreen { get; set; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public NavHost()
        {
            AddToClassList(ussClassName);

            focusable = true;
            pickingMode = PickingMode.Position;
            navController = new NavController(this);
            m_Container = new StackView();
            m_Container.AddToClassList(containerUssClassName);
            m_Container.StretchToParentSize();
            hierarchy.Add(m_Container);

            makeScreen = MakeScreen;

            RegisterCallback<NavigationCancelEvent>(OnCancelNavigation);
        }

        void OnCancelNavigation(NavigationCancelEvent evt)
        {
            if (navController.canGoBack)
            {
                evt.StopPropagation();
                navController.PopBackStack();
            }
        }

        /// <summary>
        /// Switch to a new <see cref="NavDestination"/> using the provided <see cref="NavigationAnimation"/>.
        /// </summary>
        /// <param name="destination"> The destination to switch to. </param>
        /// <param name="exitAnim"> The animation to use when exiting the current destination. </param>
        /// <param name="enterAnim"> The animation to use when entering the new destination. </param>
        /// <param name="args"> The arguments to pass to the new destination. </param>
        /// <param name="isPop"> Whether the navigation is a pop operation. </param>
        /// <param name="callback"> A callback that will be called when the navigation is complete. </param>
        internal void SwitchTo(
            NavDestination destination,
            NavigationAnimation exitAnim,
            NavigationAnimation enterAnim,
            Argument[] args,
            bool isPop,
            Action<bool> callback = null)
        {
            var content = destination.template;
            if (content != null)
            {
                m_RemoveAnim?.Recycle();
                m_AddAnim?.Recycle();

                VisualElement item;
                try
                {
                    item = CreateItem(destination, content);
                }
                catch (Exception e)
                {
                    Debug.LogError($"The template for navigation " +
                        $"destination {destination.name} could not be created: {e.Message}");
                    callback?.Invoke(false);
                    return;
                }
                if (m_Container.childCount == 0)
                {
                    m_Container.Add(item);
                }
                else
                {
                    var exitAnimationFunc = GetAnimationFunc(exitAnim);
                    var enterAnimationFunc = GetAnimationFunc(enterAnim);
                    if (enterAnimationFunc.durationMs > 0 && exitAnimationFunc.durationMs == 0)
                        exitAnimationFunc.durationMs = enterAnimationFunc.durationMs;
                    var previousItem = m_Container[0];
                    (previousItem.ElementAt(0) as NavigationScreen)!.InvokeOnExit(navController, destination, args);
                    m_RemoveAnim = previousItem.experimental.animation.Start(0, 1,
                            exitAnimationFunc.durationMs,
                            exitAnimationFunc.callback)
                        .Ease(exitAnimationFunc.easing)
                        .OnCompleted(() => m_Container.Remove(previousItem))
                        .KeepAlive();
                    if (isPop)
                        m_Container.Insert(0, item);
                    else
                        m_Container.Add(item);
                    m_AddAnim = item.experimental.animation.Start(0, 1,
                            enterAnimationFunc.durationMs,
                            enterAnimationFunc.callback)
                        .Ease(enterAnimationFunc.easing)
                        .KeepAlive();
                }
                if (item.Q<NavigationScreen>() is { } screen)
                    screen.InvokeOnEnter(navController, destination, args);
                callback?.Invoke(true);
            }
            else
            {
                Debug.LogError($"The template for navigation " +
                    $"destination {destination.name} is not valid.");
                callback?.Invoke(false);
            }
        }

        /// <summary>
        /// Create a new <see cref="VisualElement"/> item based on the provided <see cref="NavDestination"/>.
        /// </summary>
        /// <param name="destination"> The destination to create the item for. </param>
        /// <param name="template"> The template to use for the item. </param>
        /// <returns> The created <see cref="VisualElement"/> item. </returns>
        /// <exception cref="InvalidOperationException"> Thrown when the screen type could not be created. </exception>
        VisualElement CreateItem(NavDestination destination, string template)
        {
            var item = new VisualElement { name = itemUssClassName, pickingMode = PickingMode.Ignore };
            item.AddToClassList(itemUssClassName);

            var screenType = (string.IsNullOrEmpty(template) || Type.GetType(template) is not {} t) ?
                typeof(NavigationScreen) : t;

            var screen = makeScreen != null ? makeScreen.Invoke(screenType) : MakeScreen(screenType);
            if (screen == null)
                throw new InvalidOperationException($"The screen type {screenType} could not be created.");

            item.Add(screen);

            if (destination.showBottomNavBar)
            {
                var bottomNavBar = new BottomNavBar();
                item.Add(bottomNavBar);
                visualController?.SetupBottomNavBar(bottomNavBar, destination, navController);
                screen.SetupBottomNavBar(bottomNavBar);
            }

            AppBar appBar = null;
            if (destination.showAppBar)
            {
                appBar = new AppBar();
                item.Add(appBar);
                visualController?.SetupAppBar(appBar, destination, navController);
                screen.SetupAppBar(appBar);

                if (appBar.stretch)
                {
                    screen.scrollView.verticalScroller.valueChanged += (f) =>
                    {
                        appBar.scrollOffset = f;
                    };
                    appBar.RegisterCallback<GeometryChangedEvent>(evt =>
                    {
                        var h = evt.newRect.height;
                        screen.style.marginTop = h;
                    });
                }
                appBar.scrollOffset = screen.scrollView.verticalScroller.value;
                screen.AddToClassList(NavigationScreen.withAppBarUssClassName);
                screen.EnableInClassList(NavigationScreen.withCompactAppBarUssClassName, appBar.compact);

                if (destination.showBackButton && navController.canGoBack)
                {
                    appBar.backButtonTriggered += () => navController.PopBackStack();
                    appBar.showBackButton = true;
                }
            }

            if (destination.showDrawer)
            {
                var drawer = new Drawer();
                item.Add(drawer);
                visualController?.SetupDrawer(drawer, destination, navController);
                screen.SetupDrawer(drawer);

                if (destination.showAppBar && !navController.canGoBack)
                {
                    appBar.showDrawerButton = true;
                    appBar.drawerButtonTriggered += () => drawer.Toggle();
                }
            }

            return item;
        }

        /// <summary>
        /// Default implementation of the <see cref="makeScreen"/> delegate.
        /// </summary>
        /// <param name="t"> The type of the <see cref="NavigationScreen"/> to create. </param>
        /// <returns> The created <see cref="NavigationScreen"/>. </returns>
        /// <exception cref="InvalidCastException"> Thrown when the provided type is not a <see cref="NavigationScreen"/>. </exception>
        internal static NavigationScreen MakeScreen(Type t)
        {
            return (NavigationScreen)Activator.CreateInstance(t);
        }

        /// <summary>
        /// No animation.
        /// </summary>
        internal static readonly AnimationDescription noneAnimation = new AnimationDescription
        {
            easing = Easing.Linear,
            durationMs = 0,
            callback = null
        };

        /// <summary>
        /// Scale down and fade in animation.
        /// </summary>
        internal static readonly AnimationDescription scaleDownFadeInAnimation = new AnimationDescription
        {
            easing = Easing.OutCubic,
            durationMs = 150,
            callback = (v, f) =>
            {
                // Scale from 1.2 to 1.0
                var delta = 1.2f - (f * 0.2f);
                v.style.scale = new Scale(new Vector3(delta, delta, 1));
                v.style.opacity = f;
            }
        };

        /// <summary>
        /// Scale up and fade out animation.
        /// </summary>
        internal static readonly AnimationDescription scaleUpFadeOutAnimation = new AnimationDescription
        {
            easing = Easing.OutCubic,
            durationMs = 150,
            callback = (v, f) =>
            {
                // Scale from 1.0 to 1.2
                var delta = 1.0f + (f * 0.2f);
                v.style.scale = new Scale(new Vector3(delta, delta, 1));
                v.style.opacity = 1.0f - f;
            }
        };

        /// <summary>
        /// Fade in animation.
        /// </summary>
        internal static readonly AnimationDescription fadeInAnimation = new AnimationDescription
        {
            easing = Easing.OutCubic,
            durationMs = 500,
            callback = (v, f) =>
            {
                // Opacity from 0.0 to 1.0
                v.style.opacity = f;
            }
        };

        /// <summary>
        /// Fade out animation.
        /// </summary>
        internal static readonly AnimationDescription fadeOutAnimation = new AnimationDescription
        {
            easing = Easing.OutCubic,
            durationMs = 500,
            callback = (v, f) =>
            {
                // Opacity from 1.0 to 0.0
                v.style.opacity = 1.0f - f;
            }
        };

        /// <summary>
        /// Get the <see cref="AnimationDescription"/> for the provided <see cref="NavigationAnimation"/>.
        /// </summary>
        /// <param name="anim"> The <see cref="NavigationAnimation"/> to get the <see cref="AnimationDescription"/> for. </param>
        /// <returns> The <see cref="AnimationDescription"/> for the provided <see cref="NavigationAnimation"/>. </returns>
        internal static AnimationDescription GetAnimationFunc(NavigationAnimation anim)
        {
            switch (anim)
            {
                case NavigationAnimation.ScaleDownFadeIn:
                    return scaleDownFadeInAnimation;
                case NavigationAnimation.ScaleUpFadeOut:
                    return scaleUpFadeOutAnimation;
                case NavigationAnimation.FadeIn:
                    return fadeInAnimation;
                case NavigationAnimation.FadeOut:
                    return fadeOutAnimation;
                case NavigationAnimation.None:
                default:
                    return noneAnimation;
            }
        }
#if ENABLE_UXML_TRAITS
        /// <summary>
        /// The UXML Factory for the <see cref="NavHost"/>.
        /// </summary>
        public new class UxmlFactory : UxmlFactory<NavHost, UxmlTraits> { }

        /// <summary>
        /// Class containing the UXML traits for the <see cref="NavHost"/>.
        /// </summary>
        public new class UxmlTraits : VisualElement.UxmlTraits
        {

        }
#endif
    }
}
