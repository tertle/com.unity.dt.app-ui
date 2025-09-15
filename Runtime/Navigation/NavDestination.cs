using System;
using System.Collections.Generic;
using UnityEngine;

namespace Unity.AppUI.Navigation
{
    /// <summary>
    /// <para>NavDestination represents one node within an overall navigation graph.</para>
    /// <para>
    /// Destinations declare a set of actions that they support. These actions form a navigation API for the destination;
    /// the same actions declared on different destinations that fill similar roles
    /// allow application code to navigate based on semantic intent.
    /// </para>
    /// <para>
    /// Each destination has a set of arguments that will be applied when navigating to that destination.
    /// Any default values for those arguments can be overridden at the time of navigation.
    /// </para>
    /// </summary>
    [Serializable]
    public class NavDestination : NavGraphViewHierarchicalNode
    {
        [SerializeReference]
        [Tooltip("The template that defines the screen to use when navigating to this destination.")]
        NavDestinationTemplate m_TemplateSettings;

        [SerializeField]
        [Tooltip("A dictionary of arguments to apply when navigating to this destination." +
                 "The key is the name of the argument and the value is the default value of the argument.")]
        List<Argument> m_Arguments;

        /// <summary>
        /// The template that defines the screen to use when navigating to this destination.
        /// </summary>
        public NavDestinationTemplate destinationTemplate
        {
            get => m_TemplateSettings;
            set => m_TemplateSettings = value;
        }

        /// <summary>
        /// The arguments supported by this destination.
        /// </summary>
        public List<Argument> arguments
        {
            get => m_Arguments;
            set => m_Arguments = value;
        }

        /// <summary>
        /// Merge the default arguments with the provided arguments.
        /// </summary>
        /// <param name="args"> The arguments to merge with the default arguments.</param>
        /// <returns> The merged arguments.</returns>
        public Argument[] MergeArguments(params Argument[] args)
        {
            var mergedArgs = new List<Argument>();

            // Add the default arguments
            foreach (var argument in arguments)
            {
                mergedArgs.Add(new Argument(argument.name, argument.value));
            }

            if (args != null)
            {
                // Add the new arguments
                foreach (var argument in args)
                {
                    mergedArgs.Add(new Argument(argument.name, argument.value));
                }
            }

            return mergedArgs.ToArray();
        }
    }
}
