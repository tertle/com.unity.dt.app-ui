using System;
using Unity.AppUI.UI;
using Button = Unity.AppUI.UI.Button;

namespace Unity.AppUI.Editor
{
    public class ButtonPage : StoryBookPage
    {
        public override string displayName => "Button";

        public override Type componentType => typeof(ButtonComponent);

        public ButtonPage()
        {
            m_Stories.Add(new StoryBookStory("Primary", () => new Button
            {
                variant = ButtonVariant.Accent,
                title = "Primary Style Button"
            }));
        }
    }

    public class ButtonComponent : StoryBookComponent
    {
        public override Type uiElementType => typeof(Button);

        public ButtonComponent()
        {
            m_Properties.Add(new StoryBookEnumProperty<ButtonVariant>(
                nameof(Button.variant),
                (btn) => ((Button)btn).variant,
                (btn, val) => ((Button)btn).variant = val));

            m_Properties.Add(new StoryBookStringProperty(
                nameof(Button.title),
                (btn) => ((Button)btn).title,
                (btn, val) => ((Button)btn).title = val));
        }
    }
}
