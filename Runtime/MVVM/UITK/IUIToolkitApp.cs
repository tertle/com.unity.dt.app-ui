using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace Unity.AppUI.MVVM
{
    /// <summary>
    /// Interface for the application using UI Toolkit.
    /// </summary>
    public interface IUIToolkitApp : IApp<UIToolkitHost>
    {
        /// <summary>
        /// The main page of the application.
        /// </summary>
        VisualElement rootVisualElement { get; set; }
    }
}
