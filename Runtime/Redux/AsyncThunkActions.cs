using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Unity.AppUI.Core;
using UnityEngine;

namespace Unity.AppUI.Redux
{
    /// <summary>
    /// Metadata for pending actions dispatched by async thunks.
    /// </summary>
    public interface IAsyncThunkAction
    {
        /// <summary>
        /// Executes the async thunk action.
        /// </summary>
        /// <param name="store"> The store to dispatch the action to. </param>
        /// <param name="cancellationToken"> The cancellation token. </param>
        /// <returns> A task that represents the asynchronous operation. </returns>
        Task ExecuteAsync(IDispatchable store, CancellationToken cancellationToken = default);

        /// <summary>
        /// Executes the async thunk action as a coroutine.
        /// </summary>
        /// <param name="store"> The store to dispatch the action to. </param>
        /// <param name="cancellationToken"> The cancellation token. </param>
        /// <returns> The coroutine. </returns>
        Coroutine ExecuteCoroutine(IDispatchable store, CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// An action dispatched when the thunk is pending.
    /// </summary>
    /// <typeparam name="TPayload"> The type of the payload. </typeparam>
    /// <typeparam name="TArg"> The type of the argument to pass to the thunk. </typeparam>
    public class PendingAction<TArg,TPayload> : Action<TPayload>, IEquatable<PendingAction<TArg,TPayload>>
    {
        /// <summary>
        /// The metadata of the action.
        /// </summary>
        public PendingMeta<TArg> meta { get; }

        internal PendingAction(string type, PendingMeta<TArg> meta)
            : base(type, default)
        {
            this.meta = meta;
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return Equals(obj as PendingAction<TArg,TPayload>);
        }

        /// <inheritdoc/>
        public bool Equals(PendingAction<TArg,TPayload> other)
        {
            return base.Equals(other) && EqualityComparer<PendingMeta<TArg>>.Default.Equals(meta, other!.meta);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            var hash = base.GetHashCode();
            hash = (hash * 31) + (meta?.GetHashCode() ?? 0);
            return hash;
        }
    }

    /// <summary>
    /// An action dispatched when the thunk is fulfilled.
    /// </summary>
    /// <typeparam name="TPayload"> The type of the payload. </typeparam>
    /// <typeparam name="TArg"> The type of the argument to pass to the thunk. </typeparam>
    public class FulfilledAction<TArg,TPayload> : Action<TPayload>, IEquatable<FulfilledAction<TArg,TPayload>>
    {
        /// <summary>
        /// The metadata of the action.
        /// </summary>
        public FulfilledMeta<TArg> meta { get; }

        internal FulfilledAction(string type, TPayload payload, FulfilledMeta<TArg> meta)
            : base(type, payload)
        {
            this.meta = meta;
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return Equals(obj as FulfilledAction<TArg,TPayload>);
        }

        /// <inheritdoc/>
        public bool Equals(FulfilledAction<TArg,TPayload> other)
        {
            return base.Equals(other) && EqualityComparer<FulfilledMeta<TArg>>.Default.Equals(meta, other!.meta);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            var hash = base.GetHashCode();
            hash = (hash * 31) + (meta?.GetHashCode() ?? 0);
            return hash;
        }
    }

    /// <summary>
    /// An action dispatched when the thunk is rejected with an optional payload.
    /// </summary>
    /// <typeparam name="TPayload"> The type of the payload. </typeparam>
    /// <typeparam name="TArg"> The type of the argument to pass to the thunk. </typeparam>
    public class RejectedAction<TArg,TPayload> : Action<TPayload>, IEquatable<RejectedAction<TArg,TPayload>>
    {
        /// <summary>
        /// The metadata of the action.
        /// </summary>
        public RejectedMeta<TArg> meta { get; }

        internal RejectedAction(string type, TPayload payload, RejectedMeta<TArg> meta)
            : base(type, payload)
        {
            this.meta = meta;
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return Equals(obj as RejectedAction<TArg,TPayload>);
        }

        /// <inheritdoc/>
        public bool Equals(RejectedAction<TArg,TPayload> other)
        {
            return base.Equals(other) && EqualityComparer<RejectedMeta<TArg>>.Default.Equals(meta, other!.meta);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            var hash = base.GetHashCode();
            hash = (hash * 31) + (meta?.GetHashCode() ?? 0);
            return hash;
        }
    }

    /// <summary>
    /// An action created by an async thunk action creator.
    /// </summary>
    /// <typeparam name="TPayload"> The type of the payload. </typeparam>
    /// <typeparam name="TArg"> The type of the argument to pass to the thunk. </typeparam>
    public class AsyncThunkAction<TArg,TPayload>
        : Action<TArg>, IAsyncThunkAction, IEquatable<AsyncThunkAction<TArg,TPayload>>
    {
        internal AsyncThunkCreator<TArg, TPayload> creator { get; }

        /// <summary>
        /// Creates a new async thunk action.
        /// </summary>
        /// <param name="creator"> The creator of the async thunk. </param>
        /// <param name="arg"> The argument to pass to the thunk. </param>
        internal AsyncThunkAction(AsyncThunkCreator<TArg, TPayload> creator, TArg arg)
            : base(creator.type, arg)
        {
            this.creator = creator;
        }

        /// <inheritdoc/>
        public async Task ExecuteAsync(IDispatchable store, CancellationToken cancellationToken = default)
        {
            var api = new ThunkAPI<TArg, TPayload>(store, this);
            await api.DispatchThunk(cancellationToken);
        }

        /// <inheritdoc/>
        public Coroutine ExecuteCoroutine(IDispatchable store, CancellationToken cancellationToken = default)
        {
            if (!AppUI.Core.AppUI.gameObject)
            {
                Debug.LogError("App UI main GameObject does not exist. " +
                    "This GameObject exists only in Play Mode. " +
                    "Please enter Play Mode to use this method.");
            }

            return AppUI.Core.AppUI.gameObject.StartCoroutine(
                AsyncThunkCoroutine(store, this, cancellationToken));
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return Equals(obj as AsyncThunkAction<TArg,TPayload>);
        }

        /// <inheritdoc/>
        public bool Equals(AsyncThunkAction<TArg, TPayload> other)
        {
            return base.Equals(other) &&
                EqualityComparer<AsyncThunkCreator<TArg,TPayload>>.Default.Equals(creator, other!.creator);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            var hash = base.GetHashCode();
            hash = (hash * 31) + (creator?.GetHashCode() ?? 0);
            return hash;
        }

        static IEnumerator AsyncThunkCoroutine(
            IDispatchable store,
            AsyncThunkAction<TArg, TPayload> asyncThunkAction,
            CancellationToken cancellationToken)
        {
            if (store != null && asyncThunkAction != null)
            {
                var api = new ThunkAPI<TArg,TPayload>(store, asyncThunkAction);
                var task = api.DispatchThunk(cancellationToken);
                yield return task.AsCoroutine();
            }

            yield return null;
        }
    }

    /// <summary>
    /// An Action Creator to create pending actions for async thunks.
    /// </summary>
    /// <typeparam name="TPayload"> The type of the payload. </typeparam>
    public class PendingActionCreator<TPayload> : ActionCreator<TPayload>
    {
        internal PendingActionCreator(string type)
            : base(type)
        { }

        internal PendingAction<TArg,TPayload> Invoke<TArg>(PendingMeta<TArg> meta)
        {
            return new PendingAction<TArg,TPayload>(type, meta);
        }
    }

    /// <summary>
    /// An Action Creator to create fulfilled actions for async thunks.
    /// </summary>
    /// <typeparam name="TPayload"> The type of the payload. </typeparam>
    public class FulfilledActionCreator<TPayload> : ActionCreator<TPayload>
    {
        internal FulfilledActionCreator(string type)
            : base(type)
        { }

        internal FulfilledAction<TArg,TPayload> Invoke<TArg>(TPayload payload, FulfilledMeta<TArg> meta)
        {
            return new FulfilledAction<TArg,TPayload>(type, payload, meta);
        }
    }

    /// <summary>
    /// An Action Creator to create rejected actions for async thunks.
    /// </summary>
    /// <typeparam name="TPayload"> The type of the payload. </typeparam>
    public class RejectedActionCreator<TPayload> : ActionCreator<TPayload>
    {
        internal RejectedActionCreator(string type)
            : base(type)
        { }

        internal RejectedAction<TArg,TPayload> Invoke<TArg>(RejectedMeta<TArg> meta, TPayload payload = default)
        {
            return new RejectedAction<TArg,TPayload>(type, payload, meta);
        }
    }

    /// <summary>
    /// An Asybc Thunk Action that does not receive a payload.
    /// </summary>
    /// <typeparam name="TPayload"> The type of the payload. </typeparam>
    public class AsyncThunkAction<TPayload> : AsyncThunkAction<bool, TPayload>
    {
        /// <inheritdoc/>
        internal AsyncThunkAction(AsyncThunkCreator<bool,TPayload> creator)
            : base(creator, true) { }
    }
}
