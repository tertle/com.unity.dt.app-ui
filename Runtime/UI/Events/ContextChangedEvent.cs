using Unity.AppUI.Bridge;
using Unity.AppUI.Core;
using UnityEngine.UIElements;
using EventPropagation = Unity.AppUI.Bridge.EventBaseExtensionsBridge.EventPropagation;

namespace Unity.AppUI.UI
{
    /// <summary>
    /// Event raised when the context has changed.
    /// </summary>
    /// <typeparam name="T"> The type of the context. </typeparam>
    public class ContextChangedEvent<T> : EventBase<ContextChangedEvent<T>>
        where T : IContext
    {
        /// <summary>
        /// The new context.
        /// </summary>
        public T context { get; private set; }

        /// <summary>
        /// Resets all event members to their initial values.
        /// </summary>
        protected override void Init()
        {
            base.Init();
            LocalInit();
        }

        void LocalInit()
        {
            this.SetPropagation(EventPropagation.TricklesDown
#if !UNITY_2023_2_OR_NEWER
                | EventPropagation.Cancellable
#endif
            );
            context = default;
        }

        /// <summary>
        /// Gets a pooled event instance.
        /// </summary>
        /// <param name="context"> The new context. </param>
        /// <returns> A pooled event instance. </returns>
        public static ContextChangedEvent<T> GetPooled(T context)
        {
            var pooled = GetPooled();
            pooled.context = context;
            return pooled;
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ContextChangedEvent() => LocalInit();
    }
}
