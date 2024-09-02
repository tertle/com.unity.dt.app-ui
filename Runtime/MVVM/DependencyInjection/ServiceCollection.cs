// This file draws upon the concepts found in the ServiceCollection implementation from the .NET Runtime library (dotnet/runtime),
// more information in Third Party Notices.md

using System.Collections;
using System.Collections.Generic;

namespace Unity.AppUI.MVVM
{
    /// <summary>
    /// Default implementation of <see cref="IServiceCollection"/>.
    /// </summary>
    public class ServiceCollection : IServiceCollection
    {
        readonly List<ServiceDescriptor> m_Descriptors = new List<ServiceDescriptor>();

        /// <summary>
        /// Get an enumerator for the services.
        /// </summary>
        /// <returns> The enumerator. </returns>
        public IEnumerator<ServiceDescriptor> GetEnumerator()
        {
            return m_Descriptors.GetEnumerator();
        }

        /// <summary>
        /// Get an enumerator for the services.
        /// </summary>
        /// <returns> The enumerator. </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Add a service to the collection.
        /// </summary>
        /// <param name="item"> The service to add. </param>
        public void Add(ServiceDescriptor item)
        {
            m_Descriptors.Add(item);
        }

        /// <summary>
        /// Clear all services from the collection.
        /// </summary>
        public void Clear()
        {
            m_Descriptors.Clear();
        }

        /// <summary>
        /// Check if the collection contains a service.
        /// </summary>
        /// <param name="item"> The service to check for. </param>
        /// <returns> True if the collection contains the service. </returns>
        public bool Contains(ServiceDescriptor item)
        {
            return m_Descriptors.Contains(item);
        }

        /// <summary>
        /// Copy the services to an array.
        /// </summary>
        /// <param name="array"> The array to copy to. </param>
        /// <param name="arrayIndex"> The index to start copying at. </param>
        public void CopyTo(ServiceDescriptor[] array, int arrayIndex)
        {
            m_Descriptors.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Remove a service from the collection.
        /// </summary>
        /// <param name="item"> The service to remove. </param>
        /// <returns> True if the service was removed. </returns>
        public bool Remove(ServiceDescriptor item)
        {
            return m_Descriptors.Remove(item);
        }

        /// <summary>
        /// The number of services in the collection.
        /// </summary>
        public int Count => m_Descriptors.Count;

        /// <summary>
        /// The collection is not read-only.
        /// </summary>
        public bool IsReadOnly => false;

        /// <summary>
        /// Get the index of a service in the collection.
        /// </summary>
        /// <param name="item"> The service to get the index of. </param>
        /// <returns> The index of the service. </returns>
        public int IndexOf(ServiceDescriptor item)
        {
            return m_Descriptors.IndexOf(item);
        }

        /// <summary>
        /// Insert a service into the collection.
        /// </summary>
        /// <param name="index"> The index to insert the service at. </param>
        /// <param name="item"> The service to insert. </param>
        public void Insert(int index, ServiceDescriptor item)
        {
            m_Descriptors.Insert(index, item);
        }

        /// <summary>
        /// Remove a service from the collection.
        /// </summary>
        /// <param name="index"> The index of the service to remove. </param>
        public void RemoveAt(int index)
        {
            m_Descriptors.RemoveAt(index);
        }

        /// <summary>
        /// Get or set a service in the collection.
        /// </summary>
        /// <param name="index"> The index of the service to get or set. </param>
        public ServiceDescriptor this[int index]
        {
            get => m_Descriptors[index];
            set => m_Descriptors[index] = value;
        }
    }
}
