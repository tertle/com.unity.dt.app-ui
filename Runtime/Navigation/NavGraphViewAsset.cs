using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Unity.AppUI.Navigation
{
    /// <summary>
    /// Representation of a navigation graph as a Unity asset.
    /// This asset contain a hierarchy of <see cref="NavGraph"/> instances.
    /// </summary>
    [CreateAssetMenu(fileName = "New Navigation Graph.asset", menuName = "App UI/Navigation Graph")]
    public class NavGraphViewAsset : ScriptableObject
    {
        [Tooltip("The nodes of this navigation graph.")]
        [SerializeField]
        List<NavGraphViewNode> m_Nodes;

        /// <summary>
        /// The nodes of this navigation graph.
        /// </summary>
        public IEnumerable<NavGraphViewNode> nodes => m_Nodes;

        /// <summary>
        /// Add a node to this navigation graph. The component will be saved as a sub-asset of this graph.
        /// </summary>
        /// <param name="node"> The component to add. </param>
        public void AddNode(NavGraphViewNode node)
        {
            m_Nodes.Add(node);
#if UNITY_EDITOR
            if (EditorUtility.IsPersistent(this))
            {
                AssetDatabase.AddObjectToAsset(node, this);
                EditorUtility.SetDirty(this);
                AssetDatabase.SaveAssets();
            }
#endif
        }

        /// <summary>
        /// Remove a component from this navigation graph. The component will be destroyed.
        /// </summary>
        /// <param name="node"> The node to remove. </param>
        public void RemoveNode(NavGraphViewNode node)
        {
            if (node is NavGraph)
            {
                var children = m_Nodes
                    .Where(n => n is NavGraphViewHierarchicalNode hn && hn.parent == node)
                    .ToList();
                foreach (var child in children)
                {
                    RemoveNode(child);
                }
            }

            if (node is NavAction action)
            {
                var destinations = m_Nodes
                    .Where(n => n is NavDestination dest && dest.actions
                        .Contains(action))
                    .Cast<NavDestination>().ToList();

                foreach (var destination in destinations)
                {
                    destination.actions.Remove(action);
                }
            }

            if (node is NavGraphViewHierarchicalNode)
            {
                var actions = m_Nodes
                    .Where(n => n is NavAction a && a.destination == node)
                    .Cast<NavAction>()
                    .ToList();

                foreach (var act in actions)
                {
                    act.destination = null;
                }
            }

            m_Nodes.Remove(node);
#if UNITY_EDITOR
            if (EditorUtility.IsPersistent(this))
            {
                AssetDatabase.RemoveObjectFromAsset(node);
                EditorUtility.SetDirty(this);
                AssetDatabase.SaveAssets();
            }
#endif
        }

        void OnEnable()
        {
            if (m_Nodes != null)
                return;

            m_Nodes = new List<NavGraphViewNode>();
        }

#if UNITY_EDITOR

        void Awake()
        {
            Init();
        }

        void OnValidate()
        {
            Init();
        }

        void Reset()
        {
            Init();
        }

        void OnDestroy()
        {
            EditorApplication.update -= DelayedInit;
        }

        void Init()
        {
            if (rootGraph != null)
                return;

            if (AssetDatabase.Contains(this))
            {
                DelayedInit();
            }
            else
            {
                EditorApplication.update -= DelayedInit;
                EditorApplication.update += DelayedInit;
            }
        }

        void DelayedInit()
        {
            if (!AssetDatabase.Contains(this))
                return;

            EditorApplication.update -= DelayedInit;

            var g = CreateInstance<NavGraph>();
            g.name = "nav_graph";
            AddNode(g);
        }

#endif
        /// <summary>
        /// The root graph of this navigation graph.
        /// </summary>
        public NavGraph rootGraph
        {
            get
            {
                if (m_Nodes == null)
                    return null;

                foreach (var component in m_Nodes)
                {
                    if (component is NavGraph graph && graph.parent == null)
                        return graph;
                }

                return null;
            }
        }

        /// <summary>
        /// Whether this navigation graph is empty.
        /// </summary>
        public bool isEmpty => m_Nodes == null || m_Nodes.Count == 0;

        /// <summary>
        /// Find an action by its id.
        /// </summary>
        /// <param name="actionId"> The id of the action to find. </param>
        /// <param name="action"> The action if found, null otherwise. </param>
        /// <returns> True if the action was found, false otherwise. </returns>
        public bool TryFindAction(string actionId, out NavAction action)
        {
            foreach (var component in m_Nodes
                         .Where(n => n is NavGraphViewHierarchicalNode)
                         .Cast<NavGraphViewHierarchicalNode>())
            {
                NavAction foundAction = null;
                foreach (var a in component.actions)
                {
                    if (a.name == actionId)
                    {
                        foundAction = a;
                        break;
                    }
                }

                if (foundAction != null)
                {
                    action = foundAction;
                    return true;
                }
            }

            action = null;
            return false;
        }

        /// <summary>
        /// Find a destination by its id.
        /// </summary>
        /// <param name="route"> The id of the destination to find. </param>
        /// <returns> The destination if found, null otherwise. </returns>
        public NavDestination FindDestinationByRoute(string route)
        {
            if (string.IsNullOrEmpty(route))
                return null;

            foreach (var component in m_Nodes)
            {
                if (component.name == route)
                {
                    if (component is NavGraph graph)
                        return graph.FindStartDestination();
                    return (NavDestination)component;
                }
            }

            return null;
        }

        /// <summary>
        /// Find a destination by its id.
        /// </summary>
        /// <param name="from"> The source destination. </param>
        /// <param name="to"> The destination to find. </param>
        /// <param name="route"> The route to take to reach the destination. </param>
        /// <returns> True if the destination can be reached from the source destination, false otherwise. </returns>
        /// <exception cref="ArgumentNullException"> Thrown if the destination is null. </exception>
        public bool CanNavigate(NavDestination from, NavDestination to, string route)
        {
            if (!to)
                throw new ArgumentNullException(nameof(to));

            if (!from)
                return rootGraph.FindStartDestination().name == to.name;

            if (from == to)
                return false;

            foreach (var a in from.actions)
            {
                if (a.destination == to)
                    return true;
            }

            var graph = to.parent;
            while (graph != null)
            {
                if (route == graph.name && graph.FindStartDestination().name == to.name)
                    return true;
                foreach (var a in graph.actions)
                {
                    if (a.destination == to)
                        return true;
                }
                graph = graph.parent;
            }

            return false;
        }
    }
}
