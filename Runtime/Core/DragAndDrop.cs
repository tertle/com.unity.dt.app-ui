using UnityEngine;
using UnityEngine.UIElements;
using System;
using System.Collections.Generic;
using System.Linq;
using Object = UnityEngine.Object;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Unity.AppUI.Core
{
    /// <summary>
    /// The state of the drag and drop operation.
    /// </summary>
    public enum DragAndDropState
    {
        /// <summary>
        /// The default state of the drag and drop operation.
        /// The hovered element does not accept the drag operation nor reject it.
        /// </summary>
        Default,

        /// <summary>
        /// The hovered element rejects the drag operation.
        /// </summary>
        RejectDrag,

        /// <summary>
        /// The hovered element accepts the drag operation.
        /// </summary>
        AcceptDrag
    }

#if UNITY_EDITOR
    /// <summary>
    /// Dummy class to store dragged objects in the Editor.
    /// </summary>
    [Serializable]
    class DraggedAppUIObjectContainer : ScriptableObject
    {
        /// <summary>
        /// The dragged objects.
        /// </summary>
        internal List<object> draggedObjects = new List<object>();
    }
#endif

    /// <summary>
    /// The DragAndDrop class provides a way to manage drag and drop operations in App UI.
    /// </summary>
    /// <remarks>
    /// You can use this class to handle drag and drop at Runtime or in the Editor.
    /// </remarks>
    /// <example>
    /// <code>
    /// using Unity.AppUI.Core;
    ///
    /// // be sure to call this method during a PointerDown or PointerMove event to work properly.
    /// public void StartDraggingItems()
    /// {
    ///    var draggedItems = GetItemsToDrag();
    ///    DragAndDrop.PrepareStartDrag();
    ///    DragAndDrop.objects = draggedItems;
    ///    DragAndDrop.StartDrag($"{draggedItems.Count} list item{(draggedItems.Count > 1 ? "s" : "")}");
    /// }
    /// </code>
    /// <para>
    /// If you want to handle dropping items, you can use the <see cref="Unity.AppUI.UI.DropZone"/> UI element,
    /// or the <see cref="Unity.AppUI.UI.DropZoneController"/> controller for a lower-level approach.
    /// </para>
    /// <code>
    /// using Unity.AppUI.Core;
    /// using Unity.AppUI.UI;
    ///
    /// public class MyMainView : VisualElement
    /// {
    ///     public MyMainView()
    ///     {
    ///         var dropZone = new DropZone();
    ///         dropZone.controller.acceptDrag = ShouldAcceptDrag;
    ///         dropZone.controller.dropped += OnDropped;
    ///         dropZone.controller.dragEnded += CleanUp;
    ///         Add(dropZone);
    ///     }
    ///
    ///     bool ShouldAcceptDrag(IEnumerable&lt;object&gt; draggedObjects)
    ///     {
    ///         // return true or false depending on if the drop zone should accept the dragged objects
    ///     }
    ///
    ///     void OnDropped(IEnumerable&lt;object&gt; droppedItems)
    ///     {
    ///         // handle the dropped items here...
    ///         CleanUp();
    ///     }
    ///
    ///     void CleanUp()
    ///     {
    ///         // clean up any state set up when the drag operation started
    ///     }
    /// }
    /// </code>
    /// </example>
    public static class DragAndDrop
    {
        static readonly HashSet<object> k_DraggedObjects = new HashSet<object>();

        static DragAndDropState s_State = DragAndDropState.Default;

        /// <summary>
        /// The active drop target.
        /// </summary>
        public static Manipulator activeDropTarget { get; internal set; }

        /// <summary>
        /// The number of objects being dragged.
        /// </summary>
        public static int count
        {
            get
            {
#if UNITY_EDITOR
                if (editorDnDContainsObjects)
                    return editorDnDObjects.Count;
#endif
                return k_DraggedObjects.Count;
            }
        }

        /// <summary>
        /// The objects being dragged.
        /// </summary>
        public static IEnumerable<object> objects
        {
            get
            {
#if UNITY_EDITOR
                if (editorDnDContainsObjects)
                    return editorDnDObjects;
#endif
                return k_DraggedObjects;
            }
            set => SetDraggedObjects(value);
        }

        /// <summary>
        /// The current state of the drag and drop operation.
        /// </summary>
        public static DragAndDropState state
        {
            get => s_State;
            set
            {
                if (s_State != value)
                {
                    s_State = value;
                    if (s_State == DragAndDropState.Default)
                        activeDropTarget = null;
                }
#if UNITY_EDITOR
                if (editorDnDContainsObjects)
                {
                    if (s_State == DragAndDropState.AcceptDrag)
                        UnityEditor.DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                    else if (s_State == DragAndDropState.RejectDrag)
                        UnityEditor.DragAndDrop.visualMode = DragAndDropVisualMode.Rejected;
                    else
                        UnityEditor.DragAndDrop.visualMode = DragAndDropVisualMode.None;
                }
#endif
            }
        }

        /// <summary>
        /// Drops the dragged objects.
        /// </summary>
        /// <remarks>
        /// This is the equivalent of <see cref="UnityEditor.DragAndDrop.AcceptDrag"/>.
        /// </remarks>
        /// <returns> The dropped objects. </returns>
        public static IEnumerable<object> Drop()
        {
            if (activeDropTarget == null)
            {
                Debug.LogWarning("An active drop target is required to drop objects. " +
                    "Before calling Drop, set an active drop target.");
            }

            if (state != DragAndDropState.AcceptDrag)
            {
                Clear();
                return Enumerable.Empty<object>();
            }

#if UNITY_EDITOR
            if (editorDnDContainsObjects)
            {
                var editorDraggedObjects = editorDnDObjects;
                UnityEditor.DragAndDrop.AcceptDrag();
                return editorDraggedObjects;
            }
#endif
            var result = k_DraggedObjects.ToList();
            Clear();
            return result;
        }

        /// <summary>
        /// Clears drag and drop data.
        /// </summary>
        public static void PrepareStartDrag()
        {
            Clear();
        }

        /// <summary>
        /// Start a drag operation.
        /// </summary>
        /// <remarks>
        /// This method must be called after setting the dragged objects, and during a PointerDown or PointerMove event.
        /// </remarks>
        /// <param name="title"> The title of the drag operation. </param>
        public static void StartDrag(string title)
        {
            if (count == 0)
            {
                Debug.LogWarning("Cannot start a drag operation with no objects.");
                return;
            }

#if UNITY_EDITOR
            UnityEditor.DragAndDrop.StartDrag(title);
#else
            //TODO: Use our own system to start the drag operation
#endif
        }

        static void SetDraggedObjects(IEnumerable<object> list)
        {
#if UNITY_EDITOR
            if (editorDnDContainsObjects)
            {
                Debug.LogError("Calling SetDraggedObjects while there are objects in the Editor DragAndDrop is not supported.");
                return;
            }
#endif
            k_DraggedObjects.Clear();
            k_DraggedObjects.UnionWith(list);

#if UNITY_EDITOR
            if (k_DraggedObjects.Count > 0)
            {
                var obj = ScriptableObject.CreateInstance<DraggedAppUIObjectContainer>();
                obj.draggedObjects = new List<object>(DragAndDrop.objects);
                UnityEditor.DragAndDrop.objectReferences = new [] { (Object)obj };
                UnityEditor.DragAndDrop.paths = new string[] { };
            }
#endif
        }

        static void Clear()
        {
            k_DraggedObjects.Clear();
#if UNITY_EDITOR
            UnityEditor.DragAndDrop.PrepareStartDrag();
#endif
        }

#if UNITY_EDITOR
        static bool editorDnDContainsObjects => UnityEditor.DragAndDrop.objectReferences.Length > 0 || UnityEditor.DragAndDrop.paths.Length > 0;

        static readonly List<object> k_EditorDnDObjects = new List<object>();

        static List<object> editorDnDObjects
        {
            get
            {
                k_EditorDnDObjects.Clear();
                if (UnityEditor.DragAndDrop.objectReferences.Length > 0)
                    k_EditorDnDObjects.AddRange(GetFlatObjects(UnityEditor.DragAndDrop.objectReferences));
                if (UnityEditor.DragAndDrop.paths.Length > 0)
                    k_EditorDnDObjects.AddRange(UnityEditor.DragAndDrop.paths);
                return k_EditorDnDObjects;
            }
        }

        static IEnumerable<object> GetFlatObjects(Object[] objectReferences)
        {
            foreach (var obj in objectReferences)
            {
                if (obj is DraggedAppUIObjectContainer draggedAppUIObjectContainer)
                {
                    foreach (var draggedObj in draggedAppUIObjectContainer.draggedObjects)
                    {
                        yield return draggedObj;
                    }
                }
                else
                {
                    yield return obj;
                }
            }
        }
#endif
    }
}
