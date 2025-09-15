using System;
using System.Collections;
using NUnit.Framework;
using Unity.AppUI.Core;
using Unity.AppUI.UI;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;
using VisualElementExtensions = Unity.AppUI.UI.VisualElementExtensions;

namespace Unity.AppUI.Tests.Core
{
    [TestFixture]
    [TestOf(typeof(IContext))]
    class ContextTests
    {
        UIDocument m_TestUI;

        [Test]
        public void CanCreateContextOnVisualElement()
        {
            var element = new VisualElement();
            var context = new TestContext();
            element.ProvideContext(context);

            Assert.AreEqual(element.GetSelfContext<TestContext>(), context);

            Assert.IsTrue(element.IsContextProvider<TestContext>());

            Assert.AreEqual(context, element.GetContext<TestContext>());
        }

        [Test]
        public void CreateContextWithNullElementThrows()
        {
            VisualElement el = null;
            // ReSharper disable once ExpressionIsAlwaysNull
            Assert.Throws<ArgumentNullException>(() => el.ProvideContext(new TestContext()));
            // ReSharper disable once ExpressionIsAlwaysNull
            Assert.Throws<ArgumentNullException>(() => el.IsContextProvider<TestContext>());
            // ReSharper disable once ExpressionIsAlwaysNull
            Assert.Throws<ArgumentNullException>(() => el.GetContextProvider<TestContext>());
            // ReSharper disable once ExpressionIsAlwaysNull
            Assert.Throws<ArgumentNullException>(() => el.GetSelfContext<TestContext>());
        }

        [Test]
        public void GetContextWithNullElementThrows()
        {
            VisualElement el = null;

            // ReSharper disable once ExpressionIsAlwaysNull
            Assert.Throws<ArgumentNullException>(() => el.GetContext<TestContext>());
        }

        [Test]
        public void GetContextWithNullContextSucceed()
        {
            var el = new VisualElement();
            Assert.IsNull(el.GetContext<TestContext>());
            Assert.IsFalse(el.IsContextProvider<TestContext>());
            Assert.IsNull(el.GetSelfContext<TestContext>());
        }

        [Test]
        public void UseContextWithNullElementThrows()
        {
            VisualElement el = null;

            // ReSharper disable once ExpressionIsAlwaysNull
            Assert.Throws<ArgumentNullException>(() => el.RegisterContextChangedCallback<TestContext>(null));
            Assert.Throws<ArgumentNullException>(() => el.UnregisterContextChangedCallback<TestContext>(null));
        }

        [Test]
        public void UseContextWithNullCallbackThrows()
        {
            var el = new VisualElement();
            Assert.Throws<ArgumentNullException>(() => el.RegisterContextChangedCallback<TestContext>(null));
            Assert.Throws<ArgumentNullException>(() => el.UnregisterContextChangedCallback<TestContext>(null));
        }

        [Test]
        public void SendContextChangedEventWithNullElementThrows()
        {
            // ReSharper disable once ExpressionIsAlwaysNull
            Assert.Throws<ArgumentNullException>(() => VisualElementExtensions.SendContextChangedEvent<TestContext>(null, null));
        }

        [Test]
        public void UnregisterToNotRegisteredContextSucceed()
        {
            var el = new VisualElement();

            void OnContextChanged(ContextChangedEvent<TestContext> evt) { }

            Assert.DoesNotThrow(() => el.UnregisterContextChangedCallback<TestContext>(OnContextChanged));
        }

        [Test]
        public void CanGetContextFromAncestor()
        {
            var parent = new VisualElement();
            var child = new VisualElement();
            var context = new TestContext();

            parent.ProvideContext(context);
            parent.Add(child);

            Assert.AreEqual(context, child.GetContext<TestContext>());
        }

        [Test]
        public void RecordContextTypeEqualityTest()
        {
            var parent = new VisualElement();
            var child = new VisualElement();
            var context = new TestRecordContext(1);

            parent.ProvideContext(context);
            parent.Add(child);

            Assert.AreEqual(context, child.GetContext<TestRecordContext>());

            var context2 = new TestRecordContext(1);
            Assert.AreEqual(context2, child.GetContext<TestRecordContext>());

            parent.ProvideContext(context2);

            Assert.AreEqual(context2, child.GetContext<TestRecordContext>());

            Assert.AreEqual(context, context2);
        }

        [UnityTest, Order(10)]
        public IEnumerator CanUseContextWhenAttachedToPanel()
        {
            var scene = SceneManager.CreateScene("ContextTestScene-" + Random.Range(1, 1000000));
            while (!SceneManager.SetActiveScene(scene))
            {
                yield return null;
            }
            m_TestUI = Utils.ConstructTestUI();

            var parent = new VisualElement();
            var context = new TestContext();
            parent.ProvideContext(context);
            m_TestUI.rootVisualElement.Add(parent);

            var child = new TestElement
            {
                referenceContext = context
            };

            var subChild = new VisualElement();

            parent.Add(child);

            child.Add(subChild);

            var subContext = new TestContext();
            subChild.ProvideContext(subContext);

            yield return new WaitUntilOrTimeOut(() => child.contextReceived);

            Assert.IsTrue(child.contextReceived);

            var received = 0;
            void OnContextReceived(ContextChangedEvent<TestContext> evt)
            {
                received++;
            }

            child.RegisterContextChangedCallback<TestContext>(OnContextReceived);

            // try a second time to make sure the callback is not called twice
            child.RegisterContextChangedCallback<TestContext>(OnContextReceived);

            yield return new WaitUntilOrTimeOut(() => received > 0);

            Assert.AreEqual(1, received);

            // try to change the context
            var newContext = new TestContext();
            parent.ProvideContext(newContext);

            yield return new WaitUntilOrTimeOut(() => received > 1);

            Assert.AreEqual(2, received);

            // try to change the context again but this time unregister the callback
            child.UnregisterContextChangedCallback<TestContext>(OnContextReceived);

            newContext = new TestContext();
            parent.ProvideContext(newContext);

            yield return new WaitUntilOrTimeOut(() => received > 2, false, TimeSpan.FromMilliseconds(500));

            Assert.AreEqual(2, received);

            parent.ProvideContext<TestContext>(null);

            // register to a context change on an element that is already attached to a panel
            var otherElement = new VisualElement();
            subChild.Add(otherElement);

            yield return new WaitUntilOrTimeOut(() => otherElement.panel != null);

            otherElement.RegisterContextChangedCallback<TestContext>(OnContextReceived);

            yield return new WaitUntilOrTimeOut(() => received > 2);

            Assert.AreEqual(3, received);
        }

        [UnityTest, Order(11)]
        public IEnumerator CanProvideAndListenToContextChanges()
        {
            m_TestUI.rootVisualElement.Clear();

            yield return null;

            var element = new ProviderTestElement();
            Assert.AreEqual(1, element.received, "The element should have received one context change event");

            m_TestUI.rootVisualElement.Add(element);

            yield return null;

            Assert.AreEqual(1, element.received, "The element should have received one context change events");
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            if (m_TestUI)
            {
                Object.Destroy(m_TestUI.gameObject);
                m_TestUI = null;
#pragma warning disable CS0618
                var scene = SceneManager.GetActiveScene();
                if (scene.IsValid())
                    SceneManager.UnloadScene(scene);
#pragma warning restore CS0618
            }
        }

        class TestContext : IContext {}

        record TestRecordContext(int value) : IContext
        {
            public int value { get; } = value;
        }

        class TestElement : VisualElement
        {
            internal TestContext referenceContext { get; set; }

            internal bool contextReceived { get; private set; }

            public TestElement()
            {
                this.RegisterContextChangedCallback<TestContext>(OnContextReceived);
            }

            void OnContextReceived(ContextChangedEvent<TestContext> evt)
            {
                contextReceived = evt.context == referenceContext;
            }
        }

        class ProviderTestElement : VisualElement
        {
            public int received { get; private set; }

            public ProviderTestElement()
            {
                this.RegisterContextChangedCallback<TestContext>(OnContextReceived);

                this.ProvideContext(new TestContext());
            }

            void OnContextReceived(ContextChangedEvent<TestContext> evt)
            {
                received++;
            }
        }
    }
}
