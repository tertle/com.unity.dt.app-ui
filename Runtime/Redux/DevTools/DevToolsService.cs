using System;
using System.Collections.Generic;
using UnityEngine;

namespace Unity.AppUI.Redux.DevTools
{
    /// <summary>
    /// The configuration for the DevTools.
    /// </summary>
    public class DevToolsConfiguration
    {
        /// <summary>
        /// Whether the DevTools are enabled.
        /// </summary>
        public bool enabled { get; set; }

        /// <summary>
        /// The name of the store.
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// Whether the DevTools should automatically record actions.
        /// </summary>
        public bool shouldRecordChanges { get; set; } = true;

        /// <summary>
        /// Whether the DevTools should start locked.
        /// </summary>
        public bool shouldStartLocked { get; set; } = false;

        /// <summary>
        /// The maximum number of actions to record.
        /// </summary>
        public int maxAge { get; set; } = 100;

        /// <summary>
        /// Whether the DevTools should catch exceptions.
        /// </summary>
        public bool shouldCatchExceptions { get; set; } = true;
    }

    /// <summary>
    /// The service that provides a set of tools for debugging and inspecting the state of the store.
    /// </summary>
    public interface IDevToolsService
    {
        /// <summary>
        /// Event that is invoked when the list of connected stores has changed.
        /// </summary>
        event System.Action connectedStoresChanged;

        /// <summary>
        /// Connect the DevTools to a store.
        /// </summary>
        /// <param name="store"> The store to connect to. </param>
        void Connect(IInstrumentedStore store);

        /// <summary>
        /// Disconnect the DevTools from a store.
        /// </summary>
        /// <param name="store"> The store to disconnect from. </param>
        void Disconnect(IInstrumentedStore store);

        /// <summary>
        /// Get the stores that the DevTools are connected to.
        /// </summary>
        /// <returns> The stores that the DevTools are connected to. </returns>
        IInstrumentedStore[] GetConnectedStores();

        /// <summary>
        /// Get a store by its ID if it is connected to the DevTools.
        /// </summary>
        /// <param name="id"> The ID of the store. </param>
        /// <returns> The store if it is connected to the DevTools; otherwise, null. </returns>
        IInstrumentedStore GetStoreById(string id);
    }

    /// <summary>
    /// A class that provides a set of tools for debugging and inspecting the state of the store.
    /// </summary>
    public class DevToolsService : IDevToolsService
    {
        static readonly Lazy<DevToolsService> k_Instance = new Lazy<DevToolsService>(() => new DevToolsService());

        /// <summary>
        /// The singleton instance of the DevTools service.
        /// </summary>
        internal static IDevToolsService Instance => k_Instance.Value;

        readonly object m_ConnectedStoresLock = new object();

        readonly List<WeakReference<IInstrumentedStore>> m_ConnectedStores = new List<WeakReference<IInstrumentedStore>>();

        /// <summary>
        /// Event that is invoked when the connected stores change.
        /// </summary>
        public event System.Action connectedStoresChanged;

        /// <summary>
        /// Create a new instance of the DevTools service.
        /// </summary>
        DevToolsService() { }

        /// <summary>
        /// Connect the DevTools to a store.
        /// </summary>
        /// <param name="store"> The store to connect to. </param>
        /// <exception cref="ArgumentNullException"> Thrown if <paramref name="store"/> is null. </exception>
        public void Connect(IInstrumentedStore store)
        {
            if (store == null)
                throw new ArgumentNullException(nameof(store));

            lock (m_ConnectedStoresLock)
            {
                foreach (var s in m_ConnectedStores)
                {
                    if (s.TryGetTarget(out var target) && target == store)
                        return;
                }

                m_ConnectedStores.Add(new WeakReference<IInstrumentedStore>(store));
            }

            if (EnsureConnectedStores())
                connectedStoresChanged?.Invoke();
        }

        /// <summary>
        /// Disconnect the DevTools from a store.
        /// </summary>
        /// <param name="store"> The store to disconnect from. </param>
        /// <exception cref="ArgumentNullException"> Thrown if <paramref name="store"/> is null. </exception>
        public void Disconnect(IInstrumentedStore store)
        {
            if (store == null)
                throw new ArgumentNullException(nameof(store));

            var found = false;
            lock (m_ConnectedStoresLock)
            {
                for (var i = m_ConnectedStores.Count - 1; i >= 0; i--)
                {
                    if (m_ConnectedStores[i].TryGetTarget(out var target) && target == store)
                    {
                        m_ConnectedStores.RemoveAt(i);
                        found = true;
                        break;
                    }
                }
            }

            if (EnsureConnectedStores() && found)
                connectedStoresChanged?.Invoke();
        }

        /// <summary>
        /// Get the stores that the DevTools are connected to.
        /// </summary>
        /// <returns> The stores that the DevTools are connected to. </returns>
        public IInstrumentedStore[] GetConnectedStores()
        {
            EnsureConnectedStores();
            IInstrumentedStore[] stores;
            lock (m_ConnectedStoresLock)
            {
                stores = new IInstrumentedStore[m_ConnectedStores.Count];
                for (var i = 0; i < m_ConnectedStores.Count; i++)
                {
                    if (m_ConnectedStores[i].TryGetTarget(out var store))
                        stores[i] = store;
                }
            }
            return stores;
        }

        /// <inheritdoc />
        public IInstrumentedStore GetStoreById(string id)
        {
            if (string.IsNullOrEmpty(id))
                return null;

            EnsureConnectedStores();
            IInstrumentedStore ret = null;
            lock (m_ConnectedStoresLock)
            {
                foreach (var store in m_ConnectedStores)
                {
                    if (store.TryGetTarget(out var target) && target.id == id)
                    {
                        ret = target;
                        break;
                    }
                }
            }

            return ret;
        }

        bool EnsureConnectedStores()
        {
            // remove all stores that have been garbage collected
            var removed = false;
            lock (m_ConnectedStoresLock)
            {
                for (var i = m_ConnectedStores.Count - 1; i >= 0; i--)
                {
                    if (!m_ConnectedStores[i].TryGetTarget(out _))
                    {
                        m_ConnectedStores.RemoveAt(i);
                        removed = true;
                    }
                }
            }

            if (removed)
                connectedStoresChanged?.Invoke();

            return !removed;
        }
    }
}
