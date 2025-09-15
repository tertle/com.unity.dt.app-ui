using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Unity.AppUI.Core;
using System.Linq;

namespace Unity.AppUI.UI
{

    /// <summary>
    /// A droppable is a container that can be used to drop content into.
    /// </summary>
    public class DropZoneController : Manipulator
    {
#pragma warning disable 67
        /// <summary>
        /// Method called to determine if the target can accept the drag.
        /// </summary>
        public Func<IEnumerable<object>, bool> acceptDrag;

        /// <summary>
        /// Event fired either when objecs have been dropped on the target or when the user cancels the drag operation or when the user exits the target.
        /// </summary>
        /// <remarks>
        /// Use this event to clean up any state that was set up when the drag operation started.
        /// </remarks>
        public event Action dragEnded;

        /// <summary>
        /// Event fired when the user drops droppable object(s) on the target.
        /// </summary>
        /// <remarks>
        /// This event is fired only if the target is currently accepting the drag operation.
        /// </remarks>
        public event Action<IEnumerable<object>> dropped;
#pragma warning restore 67

        readonly Action<DragAndDropState> m_StateChanged;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public DropZoneController() : this(null) { }

        /// <summary>
        /// Constructor with a state changed callback.
        /// </summary>
        /// <param name="stateChanged"> The callback to be called when the drag and drop state changes. </param>
        public DropZoneController(Action<DragAndDropState> stateChanged)
        {
            m_StateChanged = stateChanged;
        }

        /// <summary>
        /// Called to register event callbacks on the target element.
        /// </summary>
        protected override void RegisterCallbacksOnTarget()
        {
#if UNITY_EDITOR
            target.RegisterCallback<DragEnterEvent>(OnDragEnter);
            target.RegisterCallback<DragLeaveEvent>(OnDragLeave);
            target.RegisterCallback<DragPerformEvent>(OnDragPerform);
            target.RegisterCallback<DragUpdatedEvent>(OnDragUpdate);
            target.RegisterCallback<DragExitedEvent>(OnDragExit);
#endif
        }

        /// <summary>
        /// Called to unregister event callbacks from the target element.
        /// </summary>
        protected override void UnregisterCallbacksFromTarget()
        {
#if UNITY_EDITOR
            target.UnregisterCallback<DragEnterEvent>(OnDragEnter);
            target.UnregisterCallback<DragLeaveEvent>(OnDragLeave);
            target.UnregisterCallback<DragPerformEvent>(OnDragPerform);
            target.UnregisterCallback<DragUpdatedEvent>(OnDragUpdate);
            target.UnregisterCallback<DragExitedEvent>(OnDragExit);
#endif
        }

#if UNITY_EDITOR
        void OnDragEnter(DragEnterEvent evt) => TryStartDragOperation();

        void OnDragUpdate(DragUpdatedEvent evt) => TryStartDragOperation();

        void OnDragPerform(DragPerformEvent evt)
        {
            if (acceptDrag == null)
            {
                EndDragOperation();
                return;
            }

            if (acceptDrag(DragAndDrop.objects))
            {
                var objects = DragAndDrop.Drop();
                dropped?.Invoke(objects);
            }

            EndDragOperation();
        }

        void OnDragLeave(DragLeaveEvent evt) => EndDragOperation();

        void OnDragExit(DragExitedEvent evt) => EndDragOperation();
#endif

        void TryStartDragOperation()
        {
            // If there are no objects being dragged, we don't need to do anything.
            var objects = DragAndDrop.objects.ToList();
            if (objects.Count == 0)
                return;

            DragAndDrop.activeDropTarget = this;
            if (acceptDrag == null)
            {
                m_StateChanged?.Invoke(DragAndDropState.Default);
                return;
            }

            var state = acceptDrag(DragAndDrop.objects) ? DragAndDropState.AcceptDrag : DragAndDropState.RejectDrag;
            DragAndDrop.state = state;
            m_StateChanged?.Invoke(state);
        }

        void EndDragOperation()
        {
            m_StateChanged?.Invoke(DragAndDropState.Default);
            if (DragAndDrop.activeDropTarget == this)
                DragAndDrop.activeDropTarget = null;
            dragEnded?.Invoke();
        }
    }
}
