using System;
using System.Collections.Generic;
using UnityEngine;

namespace Unity.AppUI.Redux
{
    /// <summary>
    /// A partitioned state. This is the specialization of a state that is partitioned into multiple slices.
    /// </summary>
    [Serializable]
    public class PartitionedState :
        Dictionary<string, object>,
        IPartionableState<PartitionedState>,
        ISerializationCallbackReceiver
    {
        [Serializable]
        struct SliceKeyValuePair
        {
            public string sliceName;
            public object sliceState;
            public SliceKeyValuePair(string sliceName, object sliceState)
            {
                this.sliceName = sliceName;
                this.sliceState = sliceState;
            }
        }

        [SerializeField]
        List<SliceKeyValuePair> m_Data = new();

        /// <summary>
        /// Default constructor.
        /// </summary>
        public PartitionedState() { }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="partitionedState"> The partitioned state to copy. </param>
        public PartitionedState(PartitionedState partitionedState) : base(partitionedState) { }

        /// <inheritdoc/>
        public TSliceState Get<TSliceState>(string sliceName)
        {
            if (string.IsNullOrEmpty(sliceName))
                throw new ArgumentException("Slice name cannot be null or empty.", nameof(sliceName));

            if (!TryGetValue(sliceName, out var sliceState))
                throw new KeyNotFoundException($"Slice with name '{sliceName}' not found.");

            return (TSliceState)sliceState;
        }

        /// <inheritdoc/>
        public PartitionedState Set<TSliceState>(string sliceName, TSliceState sliceState)
        {
            if (string.IsNullOrEmpty(sliceName))
                throw new ArgumentException("Slice name cannot be null or empty.", nameof(sliceName));
            return new PartitionedState(this) { [sliceName] = sliceState };
        }

        /// <inheritdoc/>
        public void OnBeforeSerialize()
        {
            m_Data.Clear();
            m_Data.Capacity = Count;
            foreach (var (key, value) in this)
            {
                m_Data.Add(new SliceKeyValuePair(key, value));
            }
        }

        /// <inheritdoc/>
        public void OnAfterDeserialize()
        {
            Clear();
            foreach (var pair in m_Data)
            {
                Add(pair.sliceName, pair.sliceState);
            }
        }
    }
}
