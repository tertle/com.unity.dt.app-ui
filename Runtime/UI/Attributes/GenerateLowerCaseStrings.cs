using System;

namespace Unity.AppUI.UI
{
    /// <summary>
    /// Place this attribute on an enum to generate a method to return a lowercase string representation of the enum value.
    /// </summary>
    [AttributeUsage(AttributeTargets.Enum, AllowMultiple = false, Inherited = false)]
    class GenerateLowerCaseStringsAttribute : Attribute
    {

    }
}
