using System;
using Unity.AppUI.UI;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.AppUI.Editor
{
    public class BottomNavBarPage : StoryBookPage
    {
        public override string displayName => "BottomNavBar";

        public override Type componentType => typeof(BottomNavBarComponent);
    }

    public class BottomNavBarComponent : StoryBookComponent
    {
        public override Type uiElementType => typeof(BottomNavBar);

        public override void Setup(VisualElement element)
        {
            element.parent.style.alignItems = Align.Stretch;

            element.Add(new BottomNavBarItem("info", "Info", () => { Debug.Log("Info"); } ));
            element.Add(new BottomNavBarItem("settings", "Settings", () => { Debug.Log("Settings"); }) { isSelected = true } );
        }

        public BottomNavBarComponent()
        {

        }
    }
}
