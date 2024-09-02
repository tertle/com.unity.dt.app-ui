using System.Collections;
using NUnit.Framework;
using Unity.AppUI.Bridge;
using Unity.AppUI.UI;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UIElements;
using Button = Unity.AppUI.UI.Button;

namespace Unity.AppUI.Tests.Bridge
{
    [TestFixture]
    [TestOf(typeof(FocusControllerExtensionsBridge))]
    class FocusControllerExtensionsBridgeTests
    {
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
        public IEnumerator CanFocusNextInDirection()
        {
            var root = new Panel();
            var focusable1 = new Button();
            var focusable2 = new Button();

            root.Add(focusable1);
            root.Add(focusable2);

            m_Document.rootVisualElement.Add(root);

            yield return null;

            focusable1.Focus();

            yield return null;

            var controller = root.panel.focusController;

            Assert.IsNotNull(controller);

#if UNITY_2022_3_OR_NEWER

            Assert.DoesNotThrow(() =>
            {
                controller.FocusNextInDirectionEx(VisualElementFocusChangeDirection.right);
            });

            yield return null;

            Assert.AreEqual(focusable2, controller.focusedElement);

            Assert.DoesNotThrow(() =>
            {
                controller.FocusNextInDirectionEx(focusable2, VisualElementFocusChangeDirection.left);
            });

            yield return null;

            Assert.AreEqual(focusable1, controller.focusedElement);

#endif
        }
    }
}
