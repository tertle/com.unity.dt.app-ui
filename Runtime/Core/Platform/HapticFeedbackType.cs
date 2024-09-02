namespace Unity.AppUI.Core
{
    /// <summary>
    /// The type of haptic feedback to trigger.
    /// </summary>
    public enum HapticFeedbackType
    {
        /// <summary>
        /// No haptic feedback will be triggered with this value.
        /// </summary>
        UNDEFINED = 0,
        /// <summary>
        /// A light haptic feedback.
        /// </summary>
        LIGHT = 1,
        /// <summary>
        /// A medium haptic feedback.
        /// </summary>
        MEDIUM,
        /// <summary>
        /// A heavy haptic feedback.
        /// </summary>
        HEAVY,
        /// <summary>
        /// A success haptic feedback.
        /// </summary>
        SUCCESS,
        /// <summary>
        /// An error haptic feedback.
        /// </summary>
        ERROR,
        /// <summary>
        /// A warning haptic feedback.
        /// </summary>
        WARNING,
        /// <summary>
        /// A selection haptic feedback.
        /// </summary>
        SELECTION
    }
}
