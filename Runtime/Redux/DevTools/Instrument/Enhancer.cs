using System;
using Unity.AppUI.Core;

namespace Unity.AppUI.Redux.DevTools
{
    /// <summary>
    /// The enhancer for the store creation that adds the DevTools to the store.
    /// </summary>
    /// <typeparam name="TState"> The type of the store state. </typeparam>
    public static partial class Instrument<TState>
    {
        /// <summary>
        /// Enhancer for the store creation that adds the DevTools to the store.
        /// </summary>
        /// <param name="options"> The configuration for the DevTools. </param>
        /// <returns> The store enhancer. </returns>
        public static StoreEnhancer<TState> Enhancer(DevToolsConfiguration options = null)
        {
            return createStore => (reducer, initialState) =>
            {
                options ??= new DevToolsConfiguration();

                if (!options.enabled)
                    return createStore(reducer, initialState);

                if (string.IsNullOrEmpty(options.name))
                    options.name = MemoryUtils.Concatenate("Store for ", typeof(TState).Name);

                var store = new InstrumentedStore<TState>(createStore, reducer, initialState, options);
                DevToolsService.Instance.Connect(store);
                return store;
            };
        }
    }
}
