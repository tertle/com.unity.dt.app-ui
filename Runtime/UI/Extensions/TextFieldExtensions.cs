using System;
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
    }
}
