using System;
using System.Collections.Generic;
using UnityEngine;

namespace Unity.AppUI.Navigation
{
    /// <summary>
    /// <para>
    /// Navigation actions provide a level of indirection between your navigation code and the underlying destinations.
    /// This allows you to define common actions that change their destination or <see cref="NavOptions"/> based
    /// on the current <see cref="NavDestination"/>.
    /// </para>
    /// <para>
    /// The <see cref="NavOptions"/> associated with a <see cref="NavAction"/> are used by default when navigating
    /// to this action via <see cref="NavController.Navigate(string, NavOptions, Argument[])"/>.
    /// </para>
    /// </summary>
    [Serializable]
    public class NavAction : NavGraphViewNode
    {
        [SerializeField]
        NavGraphViewHierarchicalNode m_Destination;

        [SerializeField]
        NavOptions m_Options;

        [SerializeField]
        List<Argument> m_DefaultArguments;

        /// <summary>
        /// The ID of the destination that should be navigated to when this action is used
        /// </summary>
        public NavGraphViewHierarchicalNode destination
        {
            get => m_Destination;
            set => m_Destination = value;
        }

        /// <summary>
        /// The NavOptions to be used by default when navigating to this action
        /// </summary>
        public NavOptions options
        {
            get => m_Options;
            set => m_Options = value;
        }

        /// <summary>
        /// The default arguments to be used when navigating to this action
        /// </summary>
        public List<Argument> defaultArguments
        {
            get => m_DefaultArguments;
            set => m_DefaultArguments = value;
        }

        /// <summary>
        /// Merge the default arguments with the provided arguments.
        /// </summary>
        /// <param name="arguments"> The arguments to merge with the default arguments.</param>
        /// <returns> The merged arguments.</returns>
        public Argument[] MergeArguments(params Argument[] arguments)
        {
            var mergedArguments = defaultArguments != null ? new List<Argument>(defaultArguments) : new List<Argument>();
            foreach (var arg in arguments)
            {
                if (arg == null)
                    continue;
                var existingArgIdx = mergedArguments.FindIndex(a => a.name == arg.name);
                if (existingArgIdx >= 0)
                    mergedArguments[existingArgIdx] = arg;
                else
                    mergedArguments.Add(arg);
            }
            return mergedArguments.ToArray();
        }
    }
}
