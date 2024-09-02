using System;
using NUnit.Framework;
using Unity.AppUI.Bridge;
using UnityEngine.UIElements;

namespace Unity.AppUI.Tests.Bridge
{
    [TestFixture]
    [TestOf(typeof(EventBaseExtensionsBridge))]
    class EventBaseExtensionsBridgeTests
    {
        class CustomTestEvent : EventBase<CustomTestEvent>
        {
            protected override void Init()
            {
                base.Init();
                LocalInit();
            }

            void LocalInit()
            {
                this.SetPropagation(EventBaseExtensionsBridge.EventPropagation.TricklesDown);
            }
        }

        [Test]
        public void CanGetAndSetPropagation()
        {
            var evt = CustomTestEvent.GetPooled();
            Assert.AreEqual(EventBaseExtensionsBridge.EventPropagation.TricklesDown, evt.GetPropagation());
        }
    }
}
