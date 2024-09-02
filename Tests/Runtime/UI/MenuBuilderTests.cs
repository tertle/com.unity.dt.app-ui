using NUnit.Framework;
using Unity.AppUI.UI;

namespace Unity.AppUI.Tests.UI
{
    [TestFixture]
    [TestOf(typeof(MenuBuilder))]
    class MenuBuilderTests : AnchorPopupTests<MenuBuilder>
    {
        protected override AnchorPopup<MenuBuilder> CreatePopup()
        {
            return MenuBuilder.Build(GetReferenceElement(), CreateMenu());
        }

        Menu CreateMenu()
        {
            var menu = new Menu();
            menu.Add(new MenuItem()
            {
                label = "Menu Item 1",
            });
            menu.Add(new MenuDivider());
            menu.Add(new MenuItem()
            {
                label = "Menu Item 2",
                selectable = true,
                value = false
            });
            menu.Add(new MenuItem()
            {
                label = "Menu Item 3",
                selectable = true,
                value = true
            });

            return menu;
        }

        protected override void OnCanBuildPopupTested()
        {
            base.OnCanBuildPopupTested();

            var menuBuilder = (MenuBuilder)popup;

            Assert.IsNotNull(menuBuilder);
            Assert.IsNotNull(menuBuilder.anchor);
            Assert.AreEqual(0, menuBuilder.offset);
            Assert.IsTrue(menuBuilder.arrowVisible);
            Assert.AreEqual(0, menuBuilder.containerPadding);
            Assert.AreEqual(0, menuBuilder.crossOffset);
            Assert.AreEqual(PopoverPlacement.BottomStart, menuBuilder.currentPlacement);
            Assert.IsTrue(menuBuilder.shouldFlip);
            Assert.IsTrue(menuBuilder.outsideClickDismissEnabled);
            Assert.IsNotNull(menuBuilder.view.contentContainer);
            Assert.AreEqual(PopoverPlacement.BottomStart, menuBuilder.placement);
            Assert.AreEqual(((IPlaceableElement)menuBuilder.view).placement, menuBuilder.placement);
            menuBuilder.SetPlacement(PopoverPlacement.Left);
            Assert.AreEqual(PopoverPlacement.Left, menuBuilder.placement);
            menuBuilder.SetPlacement(PopoverPlacement.Right);
            Assert.AreEqual(PopoverPlacement.Right, menuBuilder.placement);
            Assert.Throws<ValueOutOfRangeException>(() => menuBuilder.SetPlacement((PopoverPlacement)100));
        }
    }
}
