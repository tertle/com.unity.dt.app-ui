using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.AppUI.UI
{
    /// <summary>
    /// Extensions for the <see cref="TextField"/> class.
    /// </summary>
    public static class TextFieldExtensions
    {
        /// <summary>
        /// Make the cursor blink.
        /// </summary>
        /// <param name="tf">The <see cref="TextField"/> object.</param>
        [Obsolete("Use Unity.AppUI.UI.BlinkingCursor manipulator instead.")]
        public static void BlinkingCursor(this UnityEngine.UIElements.TextField tf)
        {
            tf.AddManipulator(new BlinkingCursor());
        }

        /// <summary>
        /// Adds a runtime context menu to the text field.
        /// </summary>
        /// <param name="tf"></param>
        internal static void RuntimeContextMenu(this UnityEngine.UIElements.TextField tf)
        {
#if ENABLE_UITK_TEXT_SELECTION
            if (tf == null)
                throw new ArgumentNullException(nameof(tf));
            tf.AddManipulator(new RuntimeContextMenuManipulator(RuntimeContextMenuBuilder));
            tf.textSelection.selectAllOnFocus = false;
#endif
        }

#if ENABLE_UITK_TEXT_SELECTION
        struct Selection
        {
            public readonly int cursorIndex;
            public readonly int selectionIndex;
            public readonly UnityEngine.UIElements.TextField textField;

            public Selection(UnityEngine.UIElements.TextField textField, int cursorIndex, int selectionIndex)
            {
                this.textField = textField;
                this.cursorIndex = cursorIndex;
                this.selectionIndex = selectionIndex;
            }
        }

        static Selection s_Selection;

        static void RuntimeContextMenuBuilder(RuntimeContextMenuEvent evt)
        {
            var textField = evt.target as UnityEngine.UIElements.TextField;
            if (textField == null)
            {
                Debug.LogError("RuntimeContextMenuBuilder: target is not a TextField.");
                return;
            }

            var textSelection = textField.textSelection;
            var hasSelection = textSelection.HasSelection();
            var readOnly = textField.isReadOnly;
            var cursorIndex = textSelection.cursorIndex;
            var selectIndex = textSelection.selectIndex;
            var text = textField.value;

            evt.menuBuilder.SetPlacement(PopoverPlacement.InsideTopStart);
            evt.menuBuilder.SetOffset((int)evt.localPosition.y);
            evt.menuBuilder.SetCrossOffset((int)evt.localPosition.x);

            // Store the selection to restore it later
            s_Selection = new Selection(textField, cursorIndex, selectIndex);

            evt.menuBuilder.shown += OnMenuShown;

            evt.menuBuilder.AddAction(TextActions.Cut, menuItem =>
            {
                menuItem.label = TextActions.CutLabel;
                menuItem.SetEnabled(hasSelection);
                menuItem.clickable.clicked += () =>
                {
                    var clipboardContent = text.Substring(Math.Min(cursorIndex, selectIndex), Math.Abs(cursorIndex - selectIndex));
                    GUIUtility.systemCopyBuffer = clipboardContent;
                    // remove the text in the input field
                    var start = Math.Min(cursorIndex, selectIndex);
                    textField.value = text.Remove(start, Math.Abs(cursorIndex - selectIndex));
                    textSelection.cursorIndex = start;
                    textSelection.selectIndex = start;
                    textField.Focus();
                };
            });

            evt.menuBuilder.AddAction(TextActions.Copy, menuItem =>
            {
                menuItem.label = TextActions.CopyLabel;
                menuItem.SetEnabled(hasSelection);
                menuItem.clickable.clicked += () =>
                {
                    var clipboardContent = text.Substring(Math.Min(cursorIndex, selectIndex), Math.Abs(cursorIndex - selectIndex));
                    GUIUtility.systemCopyBuffer = clipboardContent;
                    textSelection.cursorIndex = cursorIndex;
                    textSelection.selectIndex = selectIndex;
                    textField.Focus();
                };
            });

            evt.menuBuilder.AddAction(TextActions.Paste, menuItem =>
            {
                var clipboardContent = GUIUtility.systemCopyBuffer;
                menuItem.label = TextActions.PasteLabel;
                menuItem.SetEnabled(!readOnly && !string.IsNullOrEmpty(clipboardContent));
                menuItem.clickable.clicked += () =>
                {
                    // remove the selected text in the input field
                    if (hasSelection)
                    {
                        var startIndex = Math.Min(cursorIndex, selectIndex);
                        var length = Math.Abs(cursorIndex - selectIndex);
                        text = text.Remove(startIndex, length);
                    }
                    // insert the text in the input field
                    var insertIndex = Math.Min(cursorIndex, selectIndex);
                    text = text.Insert(insertIndex, clipboardContent);
                    var end = insertIndex + clipboardContent.Length;
                    if (textField.maxLength > 0 && text.Length > textField.maxLength)
                    {
                        // If the new text exceeds the max length, truncate it
                        text = text[..textField.maxLength];
                        end = text.Length;
                    }
                    textField.value = text;
                    textSelection.cursorIndex = end;
                    textSelection.selectIndex = end;
                    textField.Focus();
                };
            });
        }

        static void OnMenuShown(MenuBuilder builder)
        {
            builder.shown -= OnMenuShown;
            // restore the selection
            s_Selection.textField.Focus();
            s_Selection.textField.schedule.Execute(static () =>
            {
                s_Selection.textField.textSelection.SelectRange(s_Selection.cursorIndex, s_Selection.selectionIndex);
            });
        }
#endif
    }
}
