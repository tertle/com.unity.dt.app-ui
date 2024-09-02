using System;
using UnityEngine;

namespace Unity.AppUI.Core
{
    /// <summary>
    /// The Theme context of the application.
    /// </summary>
    /// <param name="theme"> The theme. </param>
    public record ThemeContext(string theme) : IContext
    {
        /// <summary>
        /// The current theme.
        /// </summary>
        public string theme { get; } = theme;
    }
}
