using System;
using UnityEngine;

namespace Unity.AppUI.Navigation
{
    /// <summary>
    /// NavArgument stores information about a navigation argument.
    /// </summary>
    [Serializable]
    public class NavArgument
    {
        /// <summary>
        /// The default value for the argument.
        /// </summary>
        [SerializeField]
        public string defaultValue;
    }

    /// <summary>
    /// NavArgumentKeyValuePair stores a key value pair of a navigation argument.
    /// </summary>
    [Serializable]
    public class NavArgumentKeyValuePair
    {
        /// <summary>
        /// The key of the argument.
        /// </summary>
        [SerializeField]
        public string key;

        /// <summary>
        /// The value of the argument.
        /// </summary>
        [SerializeField]
        public NavArgument value;
    }

    /// <summary>
    /// Argument stores information about a navigation argument.
    /// </summary>
    /// <param name="name"> The name of the argument. </param>
    /// <param name="value"> The value of the argument. </param>
    [Serializable]
    public record Argument(string name, string value)
    {
        [Tooltip("The name of the navigation argument.")]
        [SerializeField]
        string m_Name = name;

        [Tooltip("The value of the navigation argument.")]
        [SerializeField]
        string m_Value = value;

        /// <summary>
        /// The name of the navigation argument.
        /// </summary>
        public string name => m_Name;

        /// <summary>
        /// The value of the navigation argument.
        /// </summary>
        public string value => m_Value;
    }
}
