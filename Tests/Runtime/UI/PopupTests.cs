using System.Collections;
using NUnit.Framework;
using Unity.AppUI.Core;
using Unity.AppUI.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UIElements;

namespace Unity.AppUI.Tests.UI
{
    class PopupTests<T> where T : Popup
    {
        Popup m_Popup;

        bool m_SetupDone;

        UIDocument m_TestUI;

        Panel m_Panel;

        ActionButton m_Trigger;

        protected T popup => m_Popup as T;

        protected virtual bool shouldContainView => true;

        protected VisualElement GetReferenceElement()
        {
            return m_Trigger;
        }

        protected VisualElement GetContentElement()
        {
            var element = new VisualElement();
            var text = new Text("Hello");
            element.Add(text);
            return element;
        }

        protected virtual T CreatePopup() => null;

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            if (!m_SetupDone)
            {
                // Load new scene
                var scene = SceneManager.CreateScene("ComponentTestScene-" + Random.Range(1, 1000000));
                while (!SceneManager.SetActiveScene(scene))
                {
                    yield return null;
                }
                m_TestUI = Utils.ConstructTestUI();
                m_Panel = new Panel();
                m_Trigger = new ActionButton {label = "Trigger"};
                m_Panel.Add(m_Trigger);
                m_TestUI.rootVisualElement.Add(m_Panel);
            }
            m_Panel.DismissAllPopups();
            m_SetupDone = true;
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            if (m_TestUI)
                Object.Destroy(m_TestUI.gameObject);

            m_TestUI = null;
            m_SetupDone = false;
#pragma warning disable CS0618
            SceneManager.UnloadScene(SceneManager.GetActiveScene());
#pragma warning restore CS0618
        }

        protected virtual void OnCanBuildPopupTested()
        {

        }

        [Test, Order(1)]
        public void CanBuildPopup()
        {
            m_Popup = CreatePopup();
            Assert.IsNotNull(m_Popup);

            Assert.IsNotNull(m_Popup.view);

            if (shouldContainView)
                Assert.IsNotNull(m_Popup.contentView);
            else
                Assert.IsNull(m_Popup.contentView);

            Assert.IsNull(m_Popup.containerView,
                "The container view should be null until the popup is requested to be shown for the first time.");

            OnCanBuildPopupTested();
        }
    }
}
