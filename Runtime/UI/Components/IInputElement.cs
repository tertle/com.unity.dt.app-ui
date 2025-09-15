namespace Unity.AppUI.UI
{
    /// <summary>
    /// Interface used on UI elements which handle user input.
    /// </summary>
    /// <typeparam name="TValue">The type of the `value`.</typeparam>
    public interface IInputElement<TValue> : IValidatableElement<TValue>
    { }

    /// <summary>
    /// A function to format the value of a slider thumb or mark.
    /// </summary>
    /// <typeparam name="TValue"> The type of the `value`.</typeparam>
    /// <param name="value"> The value to format.</param>
    /// <returns> The formatted value.</returns>
    public delegate string FormatFunction<in TValue>(TValue value);

    /// <summary>
    /// Interface used on UI elements which can be formatted.
    /// </summary>
    /// <typeparam name="TValue"> The type of the `value`.</typeparam>
    public interface IFormattable<TValue>
    {
        /// <summary>
        /// The format string used to format the value.
        /// </summary>
        string formatString { get; set; }

        /// <summary>
        /// The function used to format the value.
        /// </summary>
        FormatFunction<TValue> formatFunction { get; set; }
    }
}
