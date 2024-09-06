using System;

namespace Unity.AppUI.UI
{
    /// <summary>
    /// The sizing of UI components.
    /// </summary>
    public enum Size
    {
        /// <summary>
        /// Small
        /// </summary>
        S = 1,

        /// <summary>
        /// Medium
        /// </summary>
        M,

        /// <summary>
        /// Large
        /// </summary>
        L
    }

    /// <summary>
    /// The spacing of UI components.
    /// </summary>
    public enum Spacing
    {
        /// <summary>
        /// None
        /// </summary>
        None = 0,

        /// <summary>
        /// Small
        /// </summary>
        S,

        /// <summary>
        /// Medium
        /// </summary>
        M,

        /// <summary>
        /// Large
        /// </summary>
        L
    }

    /// <summary>
    /// General usage styling classes for UI components.
    /// </summary>
    public static class Styles
    {
        /// <summary>
        /// The delay between Animation Refresh calls.
        /// </summary>
        public const uint animationRefreshDelayMs = 16;

        /// <summary>
        /// The styling class used to hide an element completely.
        /// </summary>
        public const string hiddenUssClassName = "unity-hidden";

        /// <summary>
        /// The styling class used to set an "invalid" pseudo-state on a element.
        /// </summary>
        public const string invalidUssClassName = "is-invalid";

        /// <summary>
        /// The styling class used to set a "checked" pseudo-state on a element.
        /// </summary>
        public const string checkedUssClassName = "is-checked";

        /// <summary>
        /// The styling class used to set an "intermediate" pseudo-state on a element.
        /// </summary>
        public const string intermediateUssClassName = "is-intermediate";

        /// <summary>
        /// The styling class used to set an "open" pseudo-state on a element.
        /// </summary>
        public const string openUssClassName = "is-open";

        /// <summary>
        /// The styling class used to start animating in an element.
        /// </summary>
        public const string animateInUssClassName = "animate-in";

        /// <summary>
        /// The styling class used to set a "selected" pseudo-state on a element.
        /// </summary>
        public const string selectedUssClassName = "is-selected";

        /// <summary>
        /// The styling class used to set an "active" pseudo-state on a element.
        /// </summary>
        public const string activeUssClassName = "is-active";

        /// <summary>
        /// The styling class used to set a "hover" pseudo-state on a element.
        /// </summary>
        public const string hoveredUssClassName = "is-hovered";

        /// <summary>
        /// The styling class used to set an element's elevation.
        /// </summary>
        public const string elevationUssClassName = "appui-elevation-";

        /// <summary>
        /// The styling class used to set a ":first-child" pseudo-state on a element.
        /// </summary>
        public const string firstChildUssClassName = "unity-first-child";

        /// <summary>
        /// The styling class used to set a ":last-child" pseudo-state on a element.
        /// </summary>
        public const string lastChildUssClassName = "unity-last-child";

        /// <summary>
        /// The styling class used to set a "focus" pseudo-state on a element.
        /// </summary>
        public const string focusedUssClassName = "is-focused";

        /// <summary>
        /// The styling class used to set a "required" pseudo-state on a element.
        /// </summary>
        public const string requiredUssClassName = "is-required";

        /// <summary>
        /// Used in popups to hide the arrow/tip.
        /// </summary>
        public const string noArrowUssClassName = "no-arrow";

        /// <summary>
        /// The styling class prefix used to set a cursor.
        /// </summary>
        public const string cursorUsClassNamePrefix = "cursor--";

        /// <summary>
        /// The styling class used to set a "keyboard-focus" pseudo-state on a element.
        /// </summary>
        public const string keyboardFocusUssClassName = "keyboard-focus";

        /// <summary>
        /// Converts a <see cref="Size"/> to a <see cref="IconSize"/>.
        /// </summary>
        /// <param name="size"> The size to convert. </param>
        /// <returns> The converted <see cref="IconSize"/>. </returns>
        /// <exception cref="ArgumentOutOfRangeException"> Thrown when an unknown <see cref="Size"/> is provided. </exception>
        public static IconSize ToIconSize(this Size size)
        {
            switch (size)
            {
                case Size.S:
                    return IconSize.S;
                case Size.M:
                    return IconSize.M;
                case Size.L:
                    return IconSize.L;
                default:
                    throw new ArgumentOutOfRangeException(nameof(size), size, null);
            }
        }

        /// <summary>
        /// Converts a <see cref="IconSize"/> to a <see cref="Size"/>.
        /// </summary>
        /// <param name="size"> The size to convert. </param>
        /// <returns> The converted <see cref="Size"/>. </returns>
        /// <exception cref="ArgumentOutOfRangeException"> Thrown when an unknown <see cref="IconSize"/> is provided. </exception>
        public static Size ToSize(this IconSize size)
        {
            switch (size)
            {
                case IconSize.S:
                    return Size.S;
                case IconSize.M:
                    return Size.M;
                case IconSize.L:
                    return Size.L;
                default:
                    throw new ArgumentOutOfRangeException(nameof(size), size, null);
            }
        }
    }
}
