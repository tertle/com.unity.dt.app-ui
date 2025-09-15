using System.Collections.Generic;
using System.Linq;
using Unity.AppUI.UI;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.AppUI.Samples
{
    public class DragAndDropOverElementsSample : MonoBehaviour
    {
        public UIDocument uiDocument;

        void Start()
        {
            var root = uiDocument.rootVisualElement;
            var script = new DragAndDropOverElementsSampleScript();
            script.Start(root);
        }

        public class DragAndDropOverElementsSampleScript
        {
            DropZone m_Dropzone;

            VisualElement m_ItemsContainer;

            VisualElement m_Form;

            public void Start(VisualElement root)
            {
                m_Dropzone = root.Q<DropZone>("dnd-dropzone");
                m_ItemsContainer = root.Q<VisualElement>("dnd-items-container");
                m_Form = root.Q<VisualElement>("dnd-form");

                var dropController = m_Dropzone.controller;
                dropController.acceptDrag = ShouldAcceptDrag;
                dropController.dropped += OnDropped;
                dropController.dragEnded += () => UpdateView(false);

#if UNITY_EDITOR
                root.RegisterCallback<DragUpdatedEvent>(_ => UpdateView(true));
                root.RegisterCallback<DragLeaveEvent>(_ => UpdateView(false));
                root.RegisterCallback<DragExitedEvent>(_ => UpdateView(false));
#endif

                UpdateView(false);
            }

            void UpdateView(bool showDropZone)
            {
                m_Form.EnableInClassList("dnd-form--with-attachments", m_ItemsContainer.childCount > 0);
                m_Dropzone.visibleIndicator = showDropZone;
            }

            static bool ShouldAcceptDrag(IEnumerable<object> draggedObjects) =>
                draggedObjects.Any(o => o is string);

            void OnDropped(IEnumerable<object> droppedItems)
            {
                var droppedStrings = droppedItems.Where(o => o is string).Cast<string>().ToList();
                foreach (var path in droppedStrings)
                {
                    // Add dropped items to the destination list
                    var chip = new Chip
                    {
                        deletable = true,
                        label = System.IO.Path.GetFileName(path),
                        variant = Chip.Variant.Outlined
                    };
                    chip.delete.clickedWithEventInfo += OnChipDeleted;
                    m_ItemsContainer.Add(chip);
                }
            }

            void OnChipDeleted(EventBase evt)
            {
                if (evt.target is VisualElement element && element.GetFirstAncestorOfType<Chip>() is {} chip)
                {
                    m_ItemsContainer.Remove(chip);
                    chip.delete.clickedWithEventInfo -= OnChipDeleted;
                    UpdateView(false);
                }
            }
        }
    }
}
