using UnityEngine.UIElements;

namespace Unity.AppUI.UI
{
    /// <summary>
    /// Event for when an accordion item value has changed.
    /// </summary>
    public class AccordionItemValueChangedEvent : EventBase<AccordionItemValueChangedEvent>
    {
        /// <summary>
        /// Resets all event members to their initial values.
        /// </summary>
        protected override void Init()
        {
            base.Init();
            LocalInit();
        }

        void LocalInit()
        {
            bubbles = true;
            tricklesDown = true;
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public AccordionItemValueChangedEvent() => LocalInit();
    }
}
