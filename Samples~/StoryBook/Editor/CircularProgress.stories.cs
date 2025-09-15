using System;
using Unity.AppUI.UI;
using UnityEngine.UIElements;

namespace Unity.AppUI.Editor
{
    public class CircularProgressPage : StoryBookPage
    {
        public override string displayName => "CircularProgress";

        public override Type componentType => typeof(CircularProgressComponent);
    }

    public class CircularProgressComponent : StoryBookComponent
    {
        public override Type uiElementType => typeof(CircularProgress);

        public override void Setup(VisualElement element)
        {
            if (element is not CircularProgress circularProgress)
                return;

            circularProgress.variant = Progress.Variant.Determinate;
            circularProgress.value = 0.15f;
            circularProgress.bufferValue = 0.65f;
        }

        public CircularProgressComponent()
        {
            m_Properties.Add(new StoryBookEnumProperty<Progress.Variant>(nameof(CircularProgress.variant),
                (circularProgress) => ((CircularProgress)circularProgress).variant,
                (circularProgress, val) => ((CircularProgress)circularProgress).variant = val));

            m_Properties.Add(new StoryBookFloatProperty(nameof(CircularProgress.value),
                (circularProgress) => ((CircularProgress)circularProgress).value,
                (circularProgress, val) => ((CircularProgress)circularProgress).value = val));

            m_Properties.Add(new StoryBookFloatProperty(nameof(CircularProgress.bufferValue),
                (circularProgress) => ((CircularProgress)circularProgress).bufferValue,
                (circularProgress, val) => ((CircularProgress)circularProgress).bufferValue = val));

            m_Properties.Add(new StoryBookFloatProperty(nameof(CircularProgress.innerRadius),
                (circularProgress) => ((CircularProgress)circularProgress).innerRadius,
                (circularProgress, val) => ((CircularProgress)circularProgress).innerRadius = val));
        }
    }
}
