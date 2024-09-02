#if UNITY_EDITOR
using System.Collections;
using NUnit.Framework;
using Unity.AppUI.Navigation;
using Unity.AppUI.UI;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UIElements;

namespace Unity.AppUI.Tests.Navigation
{
    [TestFixture]
    [TestOf(typeof(NavHost))]
    class NavHostTests
    {
        class TestableVisualController : INavVisualController
        {
            public void SetupBottomNavBar(BottomNavBar bottomNavBar, NavDestination destination, NavController navController)
            {
                if (destination.showBottomNavBar)
                {

                }
            }

            public void SetupAppBar(AppBar appBar, NavDestination destination, NavController navController)
            {
                if (destination.showAppBar)
                {

                }
            }

            public void SetupDrawer(Drawer drawer, NavDestination destination, NavController navController)
            {
                if (destination.showDrawer)
                {

                }
            }
        }

        UIDocument m_Document;

        [SetUp]
        public void SetUp()
        {
            TearDown();
            m_Document = Utils.ConstructTestUI();
        }

        [TearDown]
        public void TearDown()
        {
            if (m_Document && m_Document.gameObject)
                Object.Destroy(m_Document.gameObject);
        }

        [UnityTest]
        public IEnumerator NavHost_CanBePopulatedViaNavController()
        {
            var panel = new Panel();
            m_Document.rootVisualElement.Add(panel);
            panel.StretchToParentSize();

            var host = new NavHost();
            host.navController.SetGraph(Utils.navGraphTestAsset);
            host.visualController = new TestableVisualController();
            host.StretchToParentSize();
            panel.Add(host);

            yield return null;

            var currentScreen = m_Document.rootVisualElement.Q<NavigationScreen>();

            Assert.IsNotNull(currentScreen);

            var controller = currentScreen.FindNavController();

            Assert.IsNotNull(controller);

            Assert.AreEqual("start", controller.currentDestination.name);

            Assert.AreEqual(Utils.navGraphTestAsset, controller.graphAsset);

            Assert.DoesNotThrow(() =>
            {
                controller.Navigate("start_to_other", new[]
                {
                    new Argument("username", "test")
                });
            });

            yield return new WaitForSeconds(0.5f);

            Assert.AreEqual("other", controller.currentDestination.name);

            Assert.DoesNotThrow(() =>
            {
                controller.PopBackStack();
            });

            yield return new WaitForSeconds(0.5f);

            Assert.AreEqual("start", controller.currentDestination.name);

            Assert.DoesNotThrow(() =>
            {
                controller.Navigate("other", new[]
                {
                    new Argument("username", "test")
                });
            });

            yield return new WaitForSeconds(0.5f);

            Assert.AreEqual("other", controller.currentDestination.name);

            Assert.IsFalse(controller.Navigate(null, new NavOptions(), null));

            Assert.DoesNotThrow(() =>
            {
                controller.PopBackStack("start", false, true);
            });

            yield return new WaitForSeconds(0.5f);

            Assert.AreEqual("start", controller.currentDestination.name);
        }
    }
}

#endif