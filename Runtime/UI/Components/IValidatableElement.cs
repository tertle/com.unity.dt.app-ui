using System;
using UnityEngine.UIElements;

namespace Unity.AppUI.UI
{
    /// <summary>
    /// Interface used on UI elements which handle value validation.
    /// </summary>
    /// <remarks>
    /// Value validation implies the UI element has a `value` property,
    /// hence this interface inherits from <see cref="INotifyValueChanged{T}"/>.
    /// </remarks>
    /// <typeparam name="TValue">The type of the `value`.</typeparam>
    public interface IValidatableElement<TValue> : INotifyValueChanged<TValue>
    {
        /// <summary>
        /// **True** if the current value set on the UI element is invalid, **False** otherwise.
        /// </summary>
        /// <remarks>
        /// The invalid state is handled by the returned result of the <see cref="validateValue"/> function.
        /// </remarks>
        bool invalid { get; set; }

        /// <summary>
        /// Set this property to a reference of your custom function which will validate the current `value` of a UI element.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This function will be invoked automatically by the UI element implementation in order
        /// to update the <see cref="invalid"/> state property.
        /// </para>
        /// <para>
        /// If the property is `null`, there wont be any validation process so by convention the `value` will be always valid.
        /// </para>
        /// </remarks>
        Func<TValue, bool> validateValue { get; set; }
    }
}
