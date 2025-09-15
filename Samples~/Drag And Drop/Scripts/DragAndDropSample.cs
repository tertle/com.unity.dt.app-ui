using System;
using System.Collections.Generic;
using System.Linq;
using Unity.AppUI.UI;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.AppUI.Samples
{
    public class DragAndDropSample : MonoBehaviour
    {
        public UIDocument uiDocument;

        void Start()
        {
            var root = uiDocument.rootVisualElement;
            var script = new DragAndDropSampleScript();
            script.Start(root);
        }

        public class DragAndDropSampleScript
        {
            GridView m_SrcList;

            GridView m_DstList;

            DropZone m_Dropzone;

            static List<string> s_DraggedObjects;

            public void Start(VisualElement root)
            {
                m_SrcList = root.Q<GridView>("dnd-gridview-src");
                m_DstList = root.Q<GridView>("dnd-gridview-dst");
                m_Dropzone = root.Q<DropZone>("dnd-dropzone");

                m_SrcList.columnCount = 1;
                m_DstList.columnCount = 1;

                m_SrcList.makeItem = MakeItem;
                m_DstList.makeItem = MakeItem;

                m_SrcList.bindItem = BindSrcItem;
                m_DstList.bindItem = BindDstItem;

                // Populate source list
                var srcItems = new List<string>();
                for (var i = 0; i < 10; ++i)
                {
                    srcItems.Add($"Item {i}");
                }
                m_SrcList.itemsSource = srcItems;
                m_SrcList.selectionType = SelectionType.Multiple;

                // Subscribe to the drag event from the source list
                m_SrcList.dragStarted += OnDragListItemStarted;

                // Handle drop in the dropzone
                var dropController = m_Dropzone.controller;
                dropController.dropped += OnDropped;
                dropController.acceptDrag = ShouldAcceptDrag;
                dropController.dragEnded += () => UpdateView();

#if UNITY_EDITOR
                root.RegisterCallback<DragUpdatedEvent>(_ => UpdateView(true));
                root.RegisterCallback<DragLeaveEvent>(_ => UpdateView(false, true));
                root.RegisterCallback<DragExitedEvent>(_ => UpdateView());
#endif
                UpdateView();
            }

            void UpdateView(bool forceShowDropZone = false, bool forceHideDropZone = false)
            {
                m_Dropzone.visibleIndicator = forceShowDropZone || m_DstList.itemsSource == null || m_DstList.itemsSource.Count == 0;
                if (forceHideDropZone)
                    m_Dropzone.visibleIndicator = false;
            }

            void BindDstItem(VisualElement el, int idx)
            {
                var item = (Text)el.ElementAt(0);
                item.text = (string)m_DstList.itemsSource[idx];
            }

            void BindSrcItem(VisualElement el, int idx)
            {
                var item = (Text)el.ElementAt(0);
                item.text = (string)m_SrcList.itemsSource[idx];
            }

            static VisualElement MakeItem()
            {
                var item = new VisualElement();
                item.AddToClassList("dnd-item");
                var text = new Text();
                text.AddToClassList("dnd-item__label");
                item.Add(text);
                return item;
            }

            void OnDragListItemStarted(PointerMoveEvent evt)
            {
                // store the dragged items
                var draggedItems = m_SrcList.selectedIndices.Select(i => m_SrcList.itemsSource[i]).ToList();

                // We will handle drag and drop with App UI drag and drop system, so we can release the pointer capture
                var capturingElement = m_SrcList.panel.GetCapturingElement(evt.pointerId);
                capturingElement?.ReleasePointer(evt.pointerId);

                // Start the drag operation
                Core.DragAndDrop.PrepareStartDrag();
                Core.DragAndDrop.objects = draggedItems;
                Core.DragAndDrop.StartDrag($"{draggedItems.Count} list item{(draggedItems.Count > 1 ? "s" : "")}");
            }

            static bool ShouldAcceptDrag(IEnumerable<object> draggedObjects)
            {
                var ret = false;
                foreach (var obj in draggedObjects)
                {
                    switch (obj)
                    {
                        case "Item 0":
                            return false;
                        case string:
                            ret = true;
                            break;
                    }
                }
                return ret;
            }

            void OnDropped(IEnumerable<object> objects)
            {
                var selection = objects.Where(o => o is string).Cast<string>().ToList();

                if (selection.Count == 0)
                    return;

                // Add dropped items to the destination list
                var dstItems = new List<string>(m_DstList.itemsSource?.Cast<string>() ?? Enumerable.Empty<string>());
                dstItems.AddRange(selection);
                m_DstList.itemsSource = dstItems;

                // Remove dropped items from the source list
                var srcItems = new List<string>(m_SrcList.itemsSource?.Cast<string>() ?? Enumerable.Empty<string>());
                foreach (var item in selection)
                {
                    srcItems.Remove(item);
                }
                m_SrcList.itemsSource = srcItems;

                UpdateView();
            }
        }
    }
}
