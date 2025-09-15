using System;

namespace Unity.AppUI.MVVM
{
    /// <summary>
    /// Attribute to mark a field as an observable property.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public sealed class ObservablePropertyAttribute : Attribute { }

    /// <summary>
    /// An attribute that can be used to support notifying multiple properties when a property changes.
    /// </summary>
    /// <remarks>
    /// This attribute can only be used in conjunction with the <see cref="ObservablePropertyAttribute"/>.
    /// In others cases, it will be ignored.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = true)]
    public sealed class AlsoNotifyChangeForAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AlsoNotifyChangeForAttribute"/> class.
        /// </summary>
        /// <param name="propertyName"> The name of the property to also notify when the annotated property changes. </param>
        public AlsoNotifyChangeForAttribute(string propertyName) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="AlsoNotifyChangeForAttribute"/> class.
        /// </summary>
        /// <param name="propertyName"> The name of the property to also notify when the annotated property changes. </param>
        /// <param name="propertyNames"> The names of others properties to also notify when the annotated property changes. </param>
        public AlsoNotifyChangeForAttribute(string propertyName, params string[] propertyNames) { }
    }
}
