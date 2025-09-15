using System;
using Unity.AppUI.Bridge;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.AppUI.UI
{
    /// <summary>
    /// Manipulator which monitors Press, Hold and Release events in order to drag visuals.
    /// </summary>
    public class Draggable : Pressable
    {
        /// <summary>
        /// The direction of the drag.
        /// </summary>
        [Flags]
        public enum DragDirection
        {
            /// <summary>
            /// Horizontal drag.
            /// </summary>
            Horizontal = 1 << 0,

            /// <summary>
            /// Vertical drag.
            /// </summary>
            Vertical = 1 << 1,

            /// <summary>
            /// Free drag (both horizontal and vertical).
            /// </summary>
            Free = Horizontal | Vertical,
        }

        readonly Action<Draggable> m_DownHandler;

        readonly Action<Draggable> m_DragHandler;

        readonly Action<Draggable> m_UpHandler;

        Vector2 m_LastPos = Vector2.zero;

        /// <summary>
        /// Construct a Draggable manipulator.
        /// </summary>
        /// <param name="clickHandler">A callback invoked when a <see cref="ClickEvent"/> has been received.</param>
        /// <param name="dragHandler">A callback invoked during dragging state.</param>
        /// <param name="upHandler">A callback invoked when a <see cref="PointerUpEvent"/> has been received.</param>
        /// <param name="downHandler">A callback invoked when a <see cref="PointerDownEvent"/> has been received.</param>
        public Draggable(Action clickHandler, Action<Draggable> dragHandler, Action<Draggable> upHandler, Action<Draggable> downHandler = null)
            : base(clickHandler)
        {
            m_DragHandler = dragHandler;
            m_UpHandler = upHandler;
            m_DownHandler = downHandler;

            longPressDuration = -1;
            keepEventPropagation = false;
        }

        /// <summary>
        /// The direction of the drag.
        /// </summary>
        public DragDirection dragDirection { get; set; } = DragDirection.Free;

        /// <summary>
        /// The delta position between the last frame and the current one.
        /// </summary>
        public Vector2 deltaPos { get; internal set; } = Vector2.zero;

        /// <summary>
        /// The delta position between the start of the drag and the current frame.
        /// </summary>
        public Vector2 deltaStartPos { get; internal set; } = Vector2.zero;

        /// <summary>
        /// The local position received from the imGui native event.
        /// </summary>
        public Vector2 localPosition { get; internal set; }

        /// <summary>
        /// The world position received from the imGui native event.
        /// </summary>
        public Vector2 position { get; internal set; }

        /// <summary>
        /// The start position of the drag, based on the world position received from the imGui native event.
        /// </summary>
        public Vector2 startPosition { get; internal set; }

        /// <summary>
        /// Whether the pointer is currently down.
        /// </summary>
        public bool isDown { get; private set; }

        /// <summary>
        /// Has the pointer moved since the last <see cref="PointerDownEvent"/>.
        /// </summary>
        public bool hasMoved { get; internal set; }

        /// <summary>
        /// Whether the Ctrl Key is currently pressed.
        /// </summary>
        public bool ctrlKey { get; internal set; }

        /// <summary>
        /// Whether the Shift Key is currently pressed.
        /// </summary>
        public bool shiftKey { get; internal set; }

        /// <summary>
        /// Whether the Alt Key is currently pressed.
        /// </summary>
        public bool altKey { get; internal set; }

        /// <summary>
        /// Whether the Action Key is currently pressed (Cmd on macOS, Ctrl on Windows).
        /// </summary>
        public bool actionKey { get; internal set; }

        /// <summary>
        /// The threshold in pixels to start the drag operation.
        /// </summary>
        public float threshold { get; set; } = 4.0f;

        /// <summary>
        /// Cancel the drag operation.
        /// </summary>
        public void Cancel()
        {
            if (active)
                target?.Blur();
        }

        /// <summary>
        /// Accept the drag operation.
        /// </summary>
        public Func<Draggable, bool> acceptDrag { get; set; }

        /// <summary>
        /// This method processes the down event sent to the target Element.
        /// </summary>
        /// <param name="evt"> The event to process.</param>
        /// <param name="localPosition"> The local position of the pointer.</param>
        /// <param name="pointerId"> The pointer id of the pointer.</param>
        protected override void ProcessDownEvent(EventBase evt, Vector2 localPosition, int pointerId)
        {
            var canDrag = acceptDrag?.Invoke(this) ?? true;
            if (!canDrag)
                return;

            deltaPos = Vector2.zero;
            deltaStartPos = Vector2.zero;
            this.localPosition = localPosition;
            switch (evt)
            {
                case PointerDownEvent pde:
                    position = pde.position;
                    ctrlKey = pde.ctrlKey;
                    shiftKey = pde.shiftKey;
                    altKey = pde.altKey;
                    actionKey = pde.actionKey;
                    break;
                case MouseDownEvent mde:
                    position = mde.mousePosition;
                    ctrlKey = mde.ctrlKey;
                    shiftKey = mde.shiftKey;
                    altKey = mde.altKey;
                    actionKey = mde.actionKey;
                    break;
            }
            m_LastPos = position;
            isDown = true;
            hasMoved = false;
            startPosition = position;

            m_DownHandler?.Invoke(this);
            base.ProcessDownEvent(evt, localPosition, pointerId);
        }

        /// <summary>
        /// This method processes the up event sent to the target Element.
        /// </summary>
        /// <param name="evt"> The event to process.</param>
        /// <param name="localPosition"> The local position of the pointer.</param>
        /// <param name="pointerId"> The pointer id of the pointer.</param>
        protected override void ProcessUpEvent(EventBase evt, Vector2 localPosition, int pointerId)
        {
            isDown = false;
            deltaPos = Vector2.zero;
            deltaStartPos = Vector2.zero;
            this.localPosition = localPosition;
            switch (evt)
            {
                case PointerUpEvent pue:
                    position = pue.position;
                    ctrlKey = pue.ctrlKey;
                    shiftKey = pue.shiftKey;
                    altKey = pue.altKey;
                    actionKey = pue.actionKey;
                    break;
                case MouseUpEvent mue:
                    position = mue.mousePosition;
                    ctrlKey = mue.ctrlKey;
                    shiftKey = mue.shiftKey;
                    altKey = mue.altKey;
                    actionKey = mue.actionKey;
                    break;
            }

            m_UpHandler?.Invoke(this);
            base.ProcessUpEvent(evt, localPosition, pointerId);
        }

        /// <summary>
        /// This method processes the move event sent to the target Element.
        /// </summary>
        /// <param name="evt"> The event to process.</param>
        /// <param name="localPosition"> The local position of the pointer.</param>
        protected override void ProcessMoveEvent(EventBase evt, Vector2 localPosition)
        {
            if (isDown)
            {
                var isValidPointerType = evt is MouseMoveEvent ||
                    (evt is PointerMoveEvent pe && IsValidPointerType(pe.pointerId));

                if (!isValidPointerType)
                {
                    // If the pointer type is not valid, we ignore the event.
                    return;
                }

                this.localPosition = localPosition;
                switch (evt)
                {
                    case PointerMoveEvent pme:
                        position = pme.position;
                        ctrlKey = pme.ctrlKey;
                        shiftKey = pme.shiftKey;
                        altKey = pme.altKey;
                        actionKey = pme.actionKey;
                        break;
                    case MouseMoveEvent mme:
                        position = mme.mousePosition;
                        ctrlKey = mme.ctrlKey;
                        shiftKey = mme.shiftKey;
                        altKey = mme.altKey;
                        actionKey = mme.actionKey;
                        break;
                }
                deltaPos = position - m_LastPos;
                deltaStartPos = position - startPosition;
                m_LastPos = position;

                var canDrag = hasMoved || IsDraggingInDirection();
                if (canDrag || !hasMoved)
                {
                    if (evt is PointerMoveEvent pointerMoveEvent)
                    {
                        if (pointerMoveEvent.pointerId != PointerId.mousePointerId)
                            pointerMoveEvent.SetIsHandledByDraggable(true);
                    }
                }

                if (canDrag)
                {
                    m_DragHandler?.Invoke(this);
                    hasMoved = true;
                }
            }

            base.ProcessMoveEvent(evt, localPosition);
        }

        bool IsDraggingInDirection()
        {
            var r = dragDirection switch
            {
                DragDirection.Horizontal => Mathf.Abs(deltaStartPos.x) >= threshold,
                DragDirection.Vertical => Mathf.Abs(deltaStartPos.y) >= threshold,
                DragDirection.Free => deltaStartPos.magnitude >= threshold,
                _ => false
            };

            if (!r)
            {
                var isCrossDirection = dragDirection switch
                {
                    DragDirection.Horizontal => Mathf.Abs(deltaStartPos.y) >= threshold,
                    DragDirection.Vertical => Mathf.Abs(deltaStartPos.x) >= threshold,
                    _ => false
                };

                // if we are dragging in a cross direction, we cancel the drag
                if (isCrossDirection)
                    isDown = false;
            }

            return r;
        }

        static bool IsValidPointerType(int pointerId)
        {
            return pointerId == PointerId.mousePointerId
#if UNITY_6000_2_OR_NEWER
                || pointerId >= PointerId.trackedPointerIdBase
#endif
                ;
        }
    }
}
