using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace Unity.AppUI.MVVM
{
    /// <summary>
    /// Interface for the application using UI Toolkit.
    /// </summary>
    public interface IUIToolkitApp : IApp
    {
        /// <summary>
        /// The main page of the application.
        /// </summary>
        VisualElement rootVisualElement { get; set; }

        /// <summary>
        /// The hosts of the application.
        /// </summary>
        IEnumerable<IUIToolkitHost> hosts { get; }
    }
}
