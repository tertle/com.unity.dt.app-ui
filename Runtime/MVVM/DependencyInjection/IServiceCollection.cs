// This file draws upon the concepts found in the IServiceCollection implementation from the .NET Runtime library (dotnet/runtime),
// more information in Third Party Notices.md

using System.Collections.Generic;

namespace Unity.AppUI.MVVM
{
    /// <summary>
    /// Specifies the contract for a collection of service descriptors.
    /// </summary>
    public interface IServiceCollection : IList<ServiceDescriptor>
    {

    }
}
