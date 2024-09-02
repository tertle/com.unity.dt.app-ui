using System;
using UnityEngine;

namespace Unity.AppUI.Navigation
{
    /// <summary>
    /// <para>NavGraph is a collection of <see cref="NavDestination"/> nodes fetchable by ID.</para>
    /// <para>
    /// A <see cref="NavGraph"/> serves as a 'virtual' destination: while the NavGraph itself will not appear on the back stack,
    /// navigating to the NavGraph will cause the <see cref="startDestination"/> to be added to the back stack.
    /// </para>
    /// </summary>
    /// <remarks>
    /// A NavGraph is not valid until you add a destination and set the <see cref="startDestination"/>.
    /// </remarks>
    [Serializable]
    public class NavGraph : NavGraphViewHierarchicalNode
    {
        [SerializeField]
        NavDestination m_StartDestination;

        /// <summary>
        /// Sets the starting destination for this NavGraph.
        /// </summary>
        public NavDestination startDestination
        {
            get => m_StartDestination;
            set => m_StartDestination = value;
        }

        /// <summary>
        /// Find the start destination for this NavGraph.
        /// </summary>
        /// <returns> The start destination for this NavGraph. </returns>
        public NavDestination FindStartDestination()
        {
            return startDestination;
        }
    }
}
