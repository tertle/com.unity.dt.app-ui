using System.Collections;
using NUnit.Framework;
using Unity.AppUI.Bridge;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UIElements;

namespace Unity.AppUI.Tests.Bridge
{
    [TestFixture]
    [TestOf(typeof(PanelExtensionsBridge))]
    class PanelExtensionsBridgeTests
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
        public IEnumerator CanGetPanelSettings()
        {
            var element = new UnityEngine.UIElements.VisualElement();
            m_Document.rootVisualElement.Add(element);

            yield return null;

            PanelSettings settings = null;

            Assert.DoesNotThrow(() =>
            {
                settings = element.panel.GetPanelSettings();
            });

            Assert.IsNotNull(settings);
        }
    }
}
