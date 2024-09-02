using System;
using Unity.AppUI.Bridge;
using UnityEngine.UIElements;
using EventPropagation = Unity.AppUI.Bridge.EventBaseExtensionsBridge.EventPropagation;

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
            this.SetPropagation(EventPropagation.Bubbles | EventPropagation.TricklesDown
#if !UNITY_2023_2_OR_NEWER
                | EventPropagation.Cancellable
#endif
                );
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ActionTriggeredEvent() => LocalInit();
    }
}
