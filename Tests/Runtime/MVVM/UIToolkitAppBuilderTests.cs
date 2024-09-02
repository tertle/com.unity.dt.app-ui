using System.Collections;
using NUnit.Framework;
using Unity.AppUI.MVVM;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UIElements;

namespace Unity.AppUI.Tests.MVVM
{
    [TestFixture]
    [TestOf(typeof(UIToolkitAppBuilder<>))]
    public class UIToolkitAppBuilderTests
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
        public IEnumerator CanSetUpUITKApp()
        {
            yield return null;

            m_Document.gameObject.SetActive(false);
            var builder = m_Document.gameObject.AddComponent<TestableUITKAppMonoBehaviour>();
            builder.uiDocument = m_Document;

            Assert.IsNotNull(builder);
            m_Document.gameObject.SetActive(true);

            yield return null;

            builder.enabled = false;

            yield return null;
        }
    }
}
