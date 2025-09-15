using System;

namespace Unity.AppUI.MVVM
{
    /// <summary>
    /// Attribute to mark a class as implementing INotifyPropertyChanged and INotifyPropertyChanging.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class ObservableObjectAttribute : Attribute
    {

    }
}
