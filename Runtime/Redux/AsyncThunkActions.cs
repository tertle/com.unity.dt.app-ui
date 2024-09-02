using System;

namespace Unity.AppUI.Redux
{
    /// <summary>
    /// An action dispatched when the thunk is pending.
    /// </summary>
    /// <param name="type"> The type of the action. </param>
    /// <param name="meta"> The metadata of the action. </param>
    /// <typeparam name="TPayload"> The type of the payload. </typeparam>
    /// <typeparam name="TThunkArg"> The type of the argument to pass to the thunk. </typeparam>
    public record PendingAction<TPayload, TThunkArg>(string type, PendingMeta<TThunkArg> meta)
        : Action<TPayload>(type, default)
    {
        public PendingMeta<TThunkArg> meta { get; } = meta;
    }

    /// <summary>
    /// An action dispatched when the thunk is fulfilled.
    /// </summary>
    /// <param name="type"> The type of the action. </param>
    /// <param name="payload"> The payload of the action. </param>
    /// <param name="meta"> The metadata of the action. </param>
    /// <typeparam name="TPayload"> The type of the payload. </typeparam>
    /// <typeparam name="TThunkArg"> The type of the argument to pass to the thunk. </typeparam>
    public record FulfilledAction<TPayload, TThunkArg>(string type, TPayload payload, FulfilledMeta<TThunkArg> meta)
        : Action<TPayload>(type, payload)
    {
        public FulfilledMeta<TThunkArg> meta { get; } = meta;
    }

    /// <summary>
    /// An action dispatched when the thunk is rejected with an optional payload.
    /// </summary>
    /// <param name="type"> The type of the action. </param>
    /// <param name="payload"> The payload of the action. </param>
    /// <param name="meta"> The metadata of the action. </param>
    /// <typeparam name="TPayload"> The type of the payload. </typeparam>
    /// <typeparam name="TThunkArg"> The type of the argument to pass to the thunk. </typeparam>
    public record RejectedAction<TPayload, TThunkArg>(string type, TPayload payload, RejectedMeta<TThunkArg> meta)
        : Action<TPayload>(type, payload)
    {
        public RejectedMeta<TThunkArg> meta { get; } = meta;
    }

    /// <summary>
    /// An action created by an async thunk action creator.
    /// </summary>
    /// <param name="creator"> The creator of the action. </param>
    /// <param name="type"> The type of the action. </param>
    /// <param name="payload"> The payload of the action. </param>
    /// <typeparam name="TPayload"> The type of the payload. </typeparam>
    /// <typeparam name="TThunkArg"> The type of the argument to pass to the thunk. </typeparam>
    public record AsyncThunkAction<TPayload, TThunkArg>(
        AsyncThunkActionCreator<TPayload, TThunkArg> creator,
        string type,
        TThunkArg payload) : Action<TThunkArg>(type, payload)
    {
        internal AsyncThunkActionCreator<TPayload, TThunkArg> creator { get; } = creator;
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

        internal PendingAction<TPayload, TThunkArg> Invoke<TThunkArg>(PendingMeta<TThunkArg> meta)
        {
            return new PendingAction<TPayload, TThunkArg>(type, meta);
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

        internal FulfilledAction<TPayload, TThunkArg> Invoke<TThunkArg>(TPayload payload, FulfilledMeta<TThunkArg> meta)
        {
            return new FulfilledAction<TPayload, TThunkArg>(type, payload, meta);
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

        internal RejectedAction<TPayload, TThunkArg> Invoke<TThunkArg>(RejectedMeta<TThunkArg> meta, TPayload payload = default)
        {
            return new RejectedAction<TPayload, TThunkArg>(type, payload, meta);
        }
    }
}
