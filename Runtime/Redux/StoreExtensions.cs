using System;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using Unity.AppUI.Core;
using UnityEngine;

namespace Unity.AppUI.Redux
{
    /// <summary>
    /// Extensions for the <see cref="Store"/>.
    /// </summary>
    public static class StoreExtensions
    {
        /// <summary>
        /// <para>Dispatches an async thunk action.</para>
        /// <para>
        /// This method will dispatch the pending action,
        /// then call the thunk, and finally dispatch the fulfilled or rejected action based on the result of the thunk.
        /// </para>
        /// </summary>
        /// <typeparam name="TPayload"> The type of the payload (the result of the thunk). </typeparam>
        /// <typeparam name="TThunkArg"> The type of the argument to pass to the thunk. </typeparam>
        /// <param name="store"> The store to dispatch the action to. </param>
        /// <param name="asyncThunkAction"> The async thunk action to dispatch. </param>
        /// <param name="cancellationToken"> The cancellation token. </param>
        /// <remarks>
        /// Dispatching an async thunk action with this method invloves the use of async/await.
        /// If your target platform doesn't support async/await, you can use
        /// <see cref="DispatchAsyncThunkCoroutine{TPayload,TThunkArg}"/> instead.
        /// </remarks>
        /// <returns> A task that represents the asynchronous operation. </returns>
        /// <exception cref="System.ArgumentNullException"> Thrown when the store is null. </exception>
        /// <exception cref="System.ArgumentNullException"> Thrown when the asyncThunkAction is null. </exception>
        public static async Task DispatchAsyncThunk<TPayload,TThunkArg>(
            this Store store,
            AsyncThunkAction<TPayload,TThunkArg> asyncThunkAction,
            CancellationToken cancellationToken = default)
        {
            if (store == null)
                throw new ArgumentNullException(nameof(store));

            if (asyncThunkAction == null)
                throw new ArgumentNullException(nameof(asyncThunkAction));

            var api = new ThunkAPI<TPayload,TThunkArg>(store);
            await api.DispatchThunk(asyncThunkAction, cancellationToken);
        }

        /// <summary>
        /// <para>Dispatches an async thunk action.</para>
        /// <para>
        /// This method will dispatch the pending action,
        /// then call the thunk, and finally dispatch the fulfilled or rejected action based on the result of the thunk.
        /// </para>
        /// </summary>
        /// <typeparam name="TPayload"> The type of the payload (the result of the thunk). </typeparam>
        /// <typeparam name="TThunkArg"> The type of the argument to pass to the thunk. </typeparam>
        /// <param name="store"> The store to dispatch the action to. </param>
        /// <param name="asyncThunkAction"> The async thunk action to dispatch. </param>
        /// <param name="cancellationToken"> The cancellation token. </param>
        /// <returns> The coroutine. </returns>
        /// <exception cref="System.ArgumentNullException"> Thrown when the store is null. </exception>
        /// <exception cref="System.ArgumentNullException"> Thrown when the asyncThunkAction is null. </exception>
        public static Coroutine DispatchAsyncThunkCoroutine<TPayload,TThunkArg>(
            this Store store,
            AsyncThunkAction<TPayload,TThunkArg> asyncThunkAction,
            CancellationToken cancellationToken = default)
        {
            if (store == null)
                throw new ArgumentNullException(nameof(store));

            if (asyncThunkAction == null)
                throw new ArgumentNullException(nameof(asyncThunkAction));

            if (!AppUI.Core.AppUI.gameObject)
            {
                Debug.LogError("App UI main GameObject does not exist. " +
                    "This GameObject exists only in Play Mode. " +
                    "Please enter Play Mode to use this method.");
            }

            return AppUI.Core.AppUI.gameObject.StartCoroutine(
                AsyncThunkCoroutine(store, asyncThunkAction, cancellationToken));
        }

        /// <summary>
        /// Coroutine to dispatch an async thunk action.
        /// </summary>
        /// <typeparam name="TPayload"> The type of the payload (the result of the thunk). </typeparam>
        /// <typeparam name="TThunkArg"> The type of the argument to pass to the thunk. </typeparam>
        /// <param name="store"> The store to dispatch the action to. </param>
        /// <param name="asyncThunkAction"> The async thunk action to dispatch. </param>
        /// <param name="cancellationToken"> The cancellation token. </param>
        /// <returns> The coroutine. </returns>
        public static IEnumerator AsyncThunkCoroutine<TPayload,TThunkArg>(
            this Store store,
            AsyncThunkAction<TPayload,TThunkArg> asyncThunkAction,
            CancellationToken cancellationToken = default)
        {
            if (store != null && asyncThunkAction != null)
            {
                var api = new ThunkAPI<TPayload,TThunkArg>(store);
                var task = api.DispatchThunk(asyncThunkAction, cancellationToken);
                yield return task.AsCoroutine();
            }

            yield return null;
        }
    }
}
