using System;
using UnityEngine;

namespace Unity.AppUI.Navigation
{
    /// <summary>
    /// Representation of a node in a <see cref="NavGraph"/>.
    /// </summary>
    public class NavGraphViewNode : ScriptableObject
    {
        [SerializeField]
        [HideInInspector]
        Vector2 m_Position;

        [SerializeField]
        [HideInInspector]
        string m_Guid;

        /// <summary>
        /// The GUID of this NavGraphViewNode.
        /// </summary>
        public string guid
        {
            get => m_Guid;
            set => m_Guid = value;
        }

        /// <summary>
        /// The position of this NavGraphViewNode in the graph view.
        /// </summary>
        public Vector2 position
        {
            get => m_Position;
            set => m_Position = value;
        }

        void OnEnable()
        {
            if (string.IsNullOrEmpty(m_Guid))
                m_Guid = Guid.NewGuid().ToString();
        }
    }
}
