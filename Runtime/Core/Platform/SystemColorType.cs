namespace Unity.AppUI.Core
{
    /// <summary>
    /// The color types that can be retrieved from the system.
    /// </summary>
    /// <remarks>
    /// This is used to support high contrast and theme changes.
    /// </remarks>
    public enum SystemColorType
    {
        /// <summary>
        /// The text color.
        /// </summary>
        Text,

        /// <summary>
        /// The text color for a hyperlink.
        /// </summary>
        Hyperlink,

        /// <summary>
        /// The text color for a disabled element.
        /// </summary>
        DisabledText,

        /// <summary>
        /// The text color for a selected element.
        /// </summary>
        SelectedText,

        /// <summary>
        /// The background color of a selected element.
        /// </summary>
        SelectedBackground,

        /// <summary>
        /// The text color for a interactable element.
        /// </summary>
        ButtonText,

        /// <summary>
        /// The background color of a interactable element.
        /// </summary>
        ButtonBackground,

        /// <summary>
        /// The overall background color.
        /// </summary>
        Background,

        /// <summary>
        /// The system accent color.
        /// </summary>
        Accent,
    }
}