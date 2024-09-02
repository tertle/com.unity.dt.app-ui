using System;
using Unity.AppUI.Bridge;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.AppUI.UI
{
    /// <summary>
    /// Clickable Manipulator, used on <see cref="Button"/> elements.
    /// </summary>
    public class Clickable : UnityEngine.UIElements.Clickable
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="handler">The handler to invoke when the click event is triggered.</param>
        /// <param name="delay"> The delay before the first click event is triggered.</param>
        /// <param name="interval"> The interval between two click events.</param>
        public Clickable(Action handler, long delay, long interval)
            : base(handler, delay, interval)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="handler"> The handler to invoke when the click event is triggered.</param>
        public Clickable(Action<EventBase> handler)
            : base(handler)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="handler"> The handler to invoke when the click event is triggered.</param>
        public Clickable(Action handler)
            : base(handler)
        {
        }

        /// <summary>
        /// This method processes the move event sent to the target Element.
        /// </summary>
        /// <param name="evt"> The base event to process.</param>
        /// <param name="localPosition"> The local position of the pointer.</param>
        protected override void ProcessMoveEvent(EventBase evt, Vector2 localPosition)
        {
            evt.StopPropagation();
        }

        /// <summary>
        /// Invoke the click event.
        /// </summary>
        /// <param name="evt">The base event to use to invoke the click.</param>
        internal void InvokeClick(EventBase evt) => Invoke(evt);

        /// <summary>
        /// Simulate a single click on the target element.
        /// </summary>
        /// <param name="evt">The base event to use to invoke the click.</param>
        internal void SimulateSingleClickInternal(EventBase evt)
        {
            if (target != null)
            {
                var pseudoStates = target.GetPseudoStates();
                target.SetPseudoStates(pseudoStates | PseudoStates.Active);
                target.schedule
                    .Execute(() =>
                        target.SetPseudoStates(target.GetPseudoStates() & ~PseudoStates.Active))
                    .ExecuteLater(16L);
            }
            InvokeClick(evt);
        }

        /// <summary>
        /// Force the active pseudo state on the target element.
        /// </summary>
        public void ForceActivePseudoState()
        {
            if (target != null)
            {
                var pseudoStates = target.GetPseudoStates();
                target.SetPseudoStates(pseudoStates | PseudoStates.Active);
            }
        }

        /// <summary>
        /// Force the remove of active pseudo state on the target element.
        /// </summary>
        public void ForceRemoveActivePseudoState()
        {
            if (target != null)
            {
                var pseudoStates = target.GetPseudoStates();
                target.SetPseudoStates(pseudoStates & ~PseudoStates.Active);
            }
        }
    }
}
