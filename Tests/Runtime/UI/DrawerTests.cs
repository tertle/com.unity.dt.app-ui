using System.Collections;
using NUnit.Framework;
using Unity.AppUI.UI;
using UnityEngine;
using UnityEngine.TestTools;

namespace Unity.AppUI.Tests.UI
{
    [TestFixture]
    [TestOf(typeof(Drawer))]
    class DrawerTests : VisualElementTests<Drawer>
    {
        protected override string mainUssClassName => Drawer.ussClassName;

        [Test]
        [TestCase(DrawerAnchor.Left, true, DrawerVariant.Temporary)]
        [TestCase(DrawerAnchor.Left, false, DrawerVariant.Temporary)]
        [TestCase(DrawerAnchor.Right, true, DrawerVariant.Temporary)]
        [TestCase(DrawerAnchor.Right, false, DrawerVariant.Temporary)]
        public void CanGetAndSetIsOpenProp(DrawerAnchor anchor, bool hideBackdrop, DrawerVariant variant)
        {
            var drawer = new Drawer { anchor = anchor, hideBackdrop = hideBackdrop, variant = variant };

            Assert.That(drawer.isOpen, Is.False);

            drawer.isOpen = true;

            Assert.That(drawer.isOpen, Is.True);

            drawer.isOpen = false;

            Assert.That(drawer.isOpen, Is.False);
        }

        [Test]
        [TestCase(DrawerAnchor.Left, true, DrawerVariant.Permanent)]
        [TestCase(DrawerAnchor.Left, false, DrawerVariant.Permanent)]
        [TestCase(DrawerAnchor.Right, true, DrawerVariant.Permanent)]
        [TestCase(DrawerAnchor.Right, false, DrawerVariant.Permanent)]
        public void CannotGetAndSetIsOpenProp(DrawerAnchor anchor, bool hideBackdrop, DrawerVariant variant)
        {
            var drawer = new Drawer { anchor = anchor, hideBackdrop = hideBackdrop, variant = variant };

            Assert.That(drawer.isOpen, Is.True);

            drawer.isOpen = true;

            Assert.That(drawer.isOpen, Is.True);

            drawer.isOpen = false;

            Assert.That(drawer.isOpen, Is.True);
        }

        static readonly (DrawerAnchor anchor, bool hideBackdrop)[] k_Anchors = new []
        {
            (DrawerAnchor.Left, true),
            (DrawerAnchor.Left, false),
            (DrawerAnchor.Right, true),
            (DrawerAnchor.Right, false),
        };

        [UnityTest]
        public IEnumerator CanOpenAndClose([ValueSource(nameof(k_Anchors))](DrawerAnchor anchor, bool hideBackdrop) args)
        {
            var doc = Utils.ConstructTestUI();

            var drawer = new Drawer { anchor = args.anchor, hideBackdrop = args.hideBackdrop };
            doc.rootVisualElement.Add(drawer);

            yield return null;

            Assert.That(drawer.isOpen, Is.False);

            drawer.Toggle();

            yield return new WaitForSeconds(0.5f);

            Assert.That(drawer.isOpen, Is.True);

            drawer.Open();

            yield return null;

            Assert.That(drawer.isOpen, Is.True);

            drawer.Toggle();

            yield return new WaitForSeconds(0.5f);

            Assert.That(drawer.isOpen, Is.False);

            Object.Destroy(doc);
        }
    }
}
