using System;
using Unity.AppUI.Bridge;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.AppUI.UI
{
    /// <summary>
    /// A Manipulator that allows the user to scroll a target element using your finger or mouse click and drag.
    /// </summary>
    public class Scrollable : Manipulator
    {
        readonly Action<Scrollable> m_DownHandler;

        readonly Action<Scrollable> m_DragHandler;

        readonly Action<Scrollable> m_UpHandler;

        readonly Action<Scrollable> m_CancelHandler;

        bool m_IsDown;

        Vector2 m_LastPos = Vector2.zero;

        int m_PointerId = PointerId.invalidPointerId;

        /// <summary>
        /// The direction of the scroll.
        /// </summary>
        public ScrollViewMode direction { get; set; } = ScrollViewMode.Vertical;

        /// <summary>
        /// Construct a Scrollable manipulator.
        /// </summary>
        /// <param name="dragHandler">A callback invoked during dragging state.</param>
        /// <param name="upHandler">A callback invoked when a <see cref="PointerUpEvent"/> has been received.</param>
        /// <param name="downHandler">A callback invoked when a <see cref="PointerDownEvent"/> has been received.</param>
        /// <param name="cancelHandler">A callback invoked when a <see cref="PointerCancelEvent"/> has been received.</param>
        public Scrollable(
            Action<Scrollable> dragHandler,
            Action<Scrollable> upHandler,
            Action<Scrollable> downHandler = null,
            Action<Scrollable> cancelHandler = null)
        {
            m_DragHandler = dragHandler;
            m_UpHandler = upHandler;
            m_DownHandler = downHandler;
            m_CancelHandler = cancelHandler;
        }

        /// <summary>
        /// The delta position between the last frame and the current one.
        /// </summary>
        internal Vector2 deltaPos { get; set; } = Vector2.zero;

        /// <summary>
        /// The local position received from the imGui native event.
        /// </summary>
        internal Vector2 localPosition { get; set; }

        /// <summary>
        /// The world position received from the imGui native event.
        /// </summary>
        internal Vector2 position { get; set; }

        /// <summary>
        /// Has the pointer moved.
        /// </summary>
        internal bool hasMoved { get; set; }

        /// <summary>
        /// The threshold to consider a drag operation. Default is 8f.
        /// </summary>
        public float threshold { get; set; } = 8f;

        /// <summary>
        /// Called to register event callbacks on the target element.
        /// </summary>
        protected override void RegisterCallbacksOnTarget()
        {
            target.RegisterCallback<PointerDownEvent>(OnPointerDown, TrickleDown.TrickleDown);
            target.RegisterCallback<PointerUpEvent>(OnPointerUp);
            target.RegisterCallback<PointerMoveEvent>(OnPointerMove);
            target.RegisterCallback<PointerCancelEvent>(OnPointerCancel);
            target.RegisterCallback<PointerCaptureOutEvent>(OnPointerCaptureOut);
            target.RegisterCallback<WheelEvent>(OnWheel);
        }

        void OnWheel(WheelEvent evt)
        {
            evt.StopPropagation();
        }

        void OnPointerDown(PointerDownEvent evt)
        {
            deltaPos = Vector2.zero;
            localPosition = evt.localPosition;
            position = evt.position;
            m_LastPos = evt.position;
            m_IsDown = true;
            m_PointerId = PointerId.invalidPointerId;
            hasMoved = false;

            if (Mathf.Approximately(threshold, 0))
            {
                m_PointerId = evt.pointerId;
                m_LastPos = evt.position;
                target.CapturePointer(evt.pointerId);
#if !UNITY_2023_1_OR_NEWER
                if (evt.pointerId == PointerId.mousePointerId)
                    target.CaptureMouse();
#endif
                var pseudoStates = target.GetPseudoStates();
                target.SetPseudoStates(pseudoStates | PseudoStates.Active);
            }

            m_DownHandler?.Invoke(this);
        }

        void OnPointerUp(PointerUpEvent evt)
        {
            if (m_PointerId == evt.pointerId)
                m_UpHandler?.Invoke(this);

            target.ReleasePointer(evt.pointerId);
            m_PointerId = PointerId.invalidPointerId;

            var pseudoStates = target.GetPseudoStates();
            target.SetPseudoStates(pseudoStates & ~PseudoStates.Active);

            m_IsDown = false;
            deltaPos = Vector2.zero;
            localPosition = evt.localPosition;
            position = evt.position;
            m_LastPos = Vector3.zero;

            evt.StopPropagation();
        }

        void OnPointerMove(PointerMoveEvent evt)
        {
            if (!m_IsDown)
                return;

            if (m_PointerId != evt.pointerId && HasMovedEnoughInRightDirection(evt.position))
            {
                m_PointerId = evt.pointerId;
                m_LastPos = evt.position;
                if (!target.HasPointerCapture(evt.pointerId))
                {
                    target.CapturePointer(evt.pointerId);
#if !UNITY_2023_1_OR_NEWER
                    if (evt.pointerId == PointerId.mousePointerId)
                        target.CaptureMouse();
#endif
                }
                var pseudoStates = target.GetPseudoStates();
                target.SetPseudoStates(pseudoStates | PseudoStates.Active);
            }

            if (m_PointerId != evt.pointerId)
                return;

            localPosition = evt.localPosition;
            position = evt.position;
            deltaPos = position - m_LastPos;
            m_LastPos = position;

            evt.SetIsHandledByDraggable(true);

            m_DragHandler?.Invoke(this);
            hasMoved = true;

            evt.StopPropagation();
        }

        void OnPointerCancel(PointerCancelEvent evt)
        {
            var inside = evt.target != target && target.FindCommonAncestor(evt.target as VisualElement) == target;
            if (inside || evt.pointerId != m_PointerId)
                return;

            Cancel(evt, evt.pointerId);
        }

        void OnPointerCaptureOut(PointerCaptureOutEvent evt)
        {
            var inside = evt.target != target && target.FindCommonAncestor(evt.target as VisualElement) == target;
            if (inside || evt.pointerId != m_PointerId)
                return;

            Cancel(evt, evt.pointerId);
        }

        void Cancel(EventBase evt, int pointerId)
        {
            target.ReleasePointer(pointerId);
            Cancel();
            evt.StopPropagation();
        }

        /// <summary>
        /// Cancel the current drag operation.
        /// </summary>
        public void Cancel()
        {
            m_CancelHandler?.Invoke(this);
            m_PointerId = PointerId.invalidPointerId;

            var pseudoStates = target.GetPseudoStates();
            target.SetPseudoStates(pseudoStates & ~PseudoStates.Active);

            m_IsDown = false;
            deltaPos = Vector2.zero;
            m_LastPos = Vector3.zero;
            position = Vector2.zero;
            localPosition = Vector2.zero;
        }

        bool HasMovedEnoughInRightDirection(Vector2 pos)
        {
            switch (direction)
            {
                case ScrollViewMode.Vertical:
                    return Mathf.Abs(m_LastPos.y - pos.y) > threshold;
                case ScrollViewMode.Horizontal:
                    return Mathf.Abs(m_LastPos.x - pos.x) > threshold;
                case ScrollViewMode.VerticalAndHorizontal:
                    return Vector3.Distance(m_LastPos, pos) > threshold;
            }
            return false;
        }

        /// <summary>
        /// Called to unregister event callbacks from the target element.
        /// </summary>
        protected override void UnregisterCallbacksFromTarget()
        {
            target.UnregisterCallback<PointerDownEvent>(OnPointerDown, TrickleDown.TrickleDown);
            target.UnregisterCallback<PointerUpEvent>(OnPointerUp);
            target.UnregisterCallback<PointerMoveEvent>(OnPointerMove);
            target.UnregisterCallback<PointerCancelEvent>(OnPointerCancel);
            target.UnregisterCallback<PointerCaptureOutEvent>(OnPointerCaptureOut);
            target.UnregisterCallback<WheelEvent>(OnWheel);
        }
    }
}
