using UnityEngine.UIElements;

namespace Unity.AppUI.UI
{
    /// <summary>
    /// A manipulator that makes the cursor of a <see cref="TextField"/> blink.
    /// </summary>
    public class BlinkingCursor : Manipulator
    {
        IVisualElementScheduledItem m_ScheduledBlink;

        int m_Interval = 500;

        UnityEngine.UIElements.TextField textField => target as UnityEngine.UIElements.TextField;

        /// <summary>
        /// The interval in milliseconds between cursor blinks.
        /// </summary>
        public int interval
        {
            get => m_Interval;
            set
            {
                m_Interval = value;
                m_ScheduledBlink?.Pause();
                m_ScheduledBlink = target?.schedule.Execute(UpdateCursorColor).Every(interval);
                m_ScheduledBlink?.Pause();
            }
        }

        /// <summary>
        /// Called to register event callbacks on the target element.
        /// </summary>
        protected override void RegisterCallbacksOnTarget()
        {
#if UNITY_2022_1_OR_NEWER
            if (target is not UnityEngine.UIElements.TextField)
                return;

            target.RegisterCallback<FocusInEvent>(OnFocusIn);
            target.RegisterCallback<FocusOutEvent>(OnFocusOut);
            interval = m_Interval;
#endif
        }

        /// <summary>
        /// Called to unregister event callbacks from the target element.
        /// </summary>
        protected override void UnregisterCallbacksFromTarget()
        {
#if UNITY_2022_1_OR_NEWER
            target.UnregisterCallback<FocusInEvent>(OnFocusIn);
            target.UnregisterCallback<FocusOutEvent>(OnFocusOut);
            m_ScheduledBlink?.Pause();
            m_ScheduledBlink = null;
#endif
        }

        void OnFocusIn(FocusInEvent e)
        {
            m_ScheduledBlink?.Resume();
        }

        void OnFocusOut(FocusOutEvent e)
        {
            m_ScheduledBlink?.Pause();
        }

        void UpdateCursorColor()
        {
#if UNITY_2022_1_OR_NEWER
            var color = textField.textSelection.cursorColor;
            textField.textSelection.cursorColor = color.a == 0
                ? new UnityEngine.Color(color.r, color.g, color.b, 1)
                : new UnityEngine.Color(color.r, color.g, color.b, 0);
#endif
        }
    }
}
