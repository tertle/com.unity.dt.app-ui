using System;
using Unity.AppUI.Core;
using UnityEngine.UIElements;

namespace Unity.AppUI.UI
{
    /// <summary>
    /// A manipulator that can be used to receive trackpad gestures.
    /// </summary>
    public class TrackpadGestureManipulator : Manipulator
    {
        bool m_Inside;

        /// <summary>
        /// The callback that will be invoked when a pinch gesture is recognized.
        /// </summary>
        public Action<PinchGesture> onPinch { get; set; }

        /// <summary>
        /// Creates a new instance of <see cref="TrackpadGestureManipulator"/>.
        /// </summary>
        /// <param name="onPinch"> The callback that will be invoked when a pinch gesture is recognized.</param>
        public TrackpadGestureManipulator(Action<PinchGesture> onPinch = null)
        {
            this.onPinch = onPinch;
        }

        /// <summary>
        /// Called to register event callbacks on the target element.
        /// </summary>
        protected override void RegisterCallbacksOnTarget()
        {
            target.RegisterCallback<PointerEnterEvent>(OnEnter);
            target.RegisterCallback<PointerLeaveEvent>(OnLeave);
            target.RegisterCallback<PinchGestureEvent>(OnPinch);
        }

        /// <summary>
        /// Called to unregister event callbacks from the target element.
        /// </summary>
        protected override void UnregisterCallbacksFromTarget()
        {
            target.UnregisterCallback<PointerEnterEvent>(OnEnter);
            target.UnregisterCallback<PointerLeaveEvent>(OnLeave);
            target.UnregisterCallback<PinchGestureEvent>(OnPinch);
        }

        void OnEnter(PointerEnterEvent evt)
        {
            m_Inside = true;
        }

        void OnLeave(PointerLeaveEvent evt)
        {
            m_Inside = false;
        }

        void OnPinch(PinchGestureEvent evt)
        {
            if (!m_Inside)
                return;

            onPinch?.Invoke(evt.gesture);
        }
    }
}
