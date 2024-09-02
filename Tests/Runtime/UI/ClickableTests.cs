using System.Collections;
using NUnit.Framework;
using UnityEngine.TestTools;
using UnityEngine.UIElements;
using Clickable = Unity.AppUI.UI.Clickable;

namespace Unity.AppUI.Tests.UI
{
    [TestFixture]
    class ClickableTests
    {
        [UnityTest]
        public IEnumerator Clickable_SimulateSingleClickInternal_ShouldInvokeClickedEvent()
        {
            var clicked = false;
            var clickable = new Clickable(() => clicked = true);
            Assert.IsNotNull(clickable);
            clickable.SimulateSingleClickInternal(new ClickEvent());
            yield return new WaitUntilOrTimeOut(() => clicked);
            Assert.IsTrue(clicked);
        }
    }
}
