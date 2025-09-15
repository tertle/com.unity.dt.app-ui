using System;
using UnityEngine.UIElements;

namespace Unity.AppUI.UI
{
    /// <summary>
    /// An Action has been triggered.
    /// </summary>
    public class ActionTriggeredEvent : EventBase<ActionTriggeredEvent>
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
            tricklesDown = true;
            bubbles = true;
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ActionTriggeredEvent() => LocalInit();
    }
}
