using System;
using UnityEngine;

namespace Unity.AppUI.Navigation
{
    /// <summary>
    /// Argument to pass to a navigation request.
    /// </summary>
    [Serializable]
    public class NavigationArgs
    {
        [Tooltip("The name of the navigation argument.")]
        [SerializeField]
        string m_Name;

        /// <summary>
        /// The name of the navigation argument.
        /// </summary>
        public string name
        {
            get => m_Name;
            set => m_Name = value;
        }
    }
}
