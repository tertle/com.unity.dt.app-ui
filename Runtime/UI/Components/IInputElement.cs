namespace Unity.AppUI.UI
{
    /// <summary>
    /// Interface used on UI elements which handle user input.
    /// </summary>
    /// <typeparam name="TValueType">The type of the `value`.</typeparam>
    public interface IInputElement<TValueType> : IValidatableElement<TValueType>
    { }
}
