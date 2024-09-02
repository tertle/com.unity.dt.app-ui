#if !UNITY_EDITOR && ENABLE_IL2CPP && !CONDITIONAL_WEAK_TABLE_IL2CPP
using System;
using System.Collections.Generic;

namespace Unity.AppUI.Core
{
    /// <summary>
    /// Implementation of a weak reference table.
    /// </summary>
    class WeakReferenceTable<TKey,TValue>
        where TKey : class
        where TValue : new()
    {
        readonly Dictionary<WeakReference, TValue> m_WeakReferenceTable = new Dictionary<WeakReference, TValue>();

        /// <summary>
        /// Default constructor.
        /// </summary>
        public WeakReferenceTable() { }

        /// <summary>
        /// Try to get the value associated with the given key.
        /// </summary>
        /// <param name="key"> The key to look for.</param>
        /// <param name="value"> The value associated with the key.</param>
        /// <returns> True if the key was found, false otherwise.</returns>
        public bool TryGetValue(TKey key, out TValue value)
        {
            var keysToRemove = new List<WeakReference>();

            foreach (var kvp in m_WeakReferenceTable)
            {
                if (!kvp.Key.IsAlive)
                    keysToRemove.Add(kvp.Key);
            }

            foreach (var keyToRemove in keysToRemove)
            {
                m_WeakReferenceTable.Remove(keyToRemove);
            }

            foreach (var kvp in m_WeakReferenceTable)
            {
                if (kvp.Key.Target is TKey target && target == key)
                {
                    value = kvp.Value;
                    return true;
                }
            }

            value = default;
            return false;
        }

        /// <summary>
        /// Get the value associated with the given key or create a new one if the key is not found.
        /// </summary>
        /// <param name="key"> The key to look for.</param>
        /// <returns> The value associated with the key.</returns>
        public TValue GetOrCreateValue(TKey key)
        {
            TValue value;
            if (TryGetValue(key, out value))
                return value;

            value = new TValue();
            var weakReference = new WeakReference(key);
            m_WeakReferenceTable.Add(weakReference, value);
            return value;
        }
    }
}

#endif // !UNITY_EDITOR && ENABLE_IL2CPP && !CONDITIONAL_WEAK_TABLE_IL2CPP