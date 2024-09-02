using System;
using System.Collections.Generic;
using UnityEngine;

namespace Unity.AppUI.Navigation
{
    /// <summary>
    /// Representation of a hierarchical node in a <see cref="NavGraph"/>.
    /// </summary>
    public class NavGraphViewHierarchicalNode : NavGraphViewNode
    {
        [SerializeField]
        NavGraph m_Parent;

        [SerializeField]
        List<NavAction> m_Actions = new List<NavAction>();

        [SerializeField]
        string m_Label;

        /// <summary>
        /// The parent of this NavGraphViewHierarchicalNode.
        /// </summary>
        public NavGraph parent
        {
            get => m_Parent;
            set => m_Parent = value;
        }

        /// <summary>
        /// The actions associated with this NavGraphViewHierarchicalNode.
        /// </summary>
        public List<NavAction> actions
        {
            get => m_Actions;
            set => m_Actions = value;
        }

        /// <summary>
        /// The label of this NavGraphViewHierarchicalNode.
        /// </summary>
        public string label
        {
            get => m_Label;
            set => m_Label = value;
        }
    }
}
