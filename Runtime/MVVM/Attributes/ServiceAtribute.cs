using System;

namespace Unity.AppUI.MVVM
{
    /// <summary>
    /// The ServiceAttribute is used to mark a field or a property as a service instance.
    /// This attribute is used by the dependency injection system from the MVVM framework to inject services into
    /// the ViewModel (or any others kinds of service).
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class ServiceAttribute : Attribute { }
}
