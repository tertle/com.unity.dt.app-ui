using System;

namespace Unity.AppUI.MVVM
{
    /// <summary>
    /// Attribute to mark a class as implementing INotifyPropertyChanged.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class INotifyPropertyChangedAttribute : Attribute
    {
        /// <summary>
        /// Whether to include additional helper methods in the generated code,
        /// such as SetProperty().
        /// </summary>
        public bool IncludeAdditionalHelperMethods { get; set; }
    }
}
