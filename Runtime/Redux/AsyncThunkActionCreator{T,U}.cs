using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Unity.AppUI.Core;

namespace Unity.AppUI.Redux
{
    /// <summary>
    /// The async thunk action creator.
    /// </summary>
    /// <typeparam name="TPayload"> The type of the payload. </typeparam>
    /// <typeparam name="TThunkArg"> The type of the thunk argument. </typeparam>
    public class AsyncThunkActionCreator<TPayload, TThunkArg>
    {
        /// <summary>
        /// The payload creator signature for the thunk.
        /// </summary>
        /// <param name="arg"> The argument to pass to the thunk. </param>
        /// <param name="thunkAPI"> The thunk API. </param>
        /// <param name="cancellationToken"> The cancellation token. </param>
        /// <returns> The payload. </returns>
        public delegate Task<TPayload> PayloadCreator(TThunkArg arg, ThunkAPI<TPayload,TThunkArg> thunkAPI, CancellationToken cancellationToken);

        /// <summary>
        /// The type of the action.
        /// </summary>
        public string type { get; }

        /// <summary>
        /// The action creator for the pending action.
        /// </summary>
        public PendingActionCreator<TPayload> pending { get; }

        /// <summary>
        /// The action creator for the fulfilled action.
        /// </summary>
        public FulfilledActionCreator<TPayload> fulfilled { get; }

        /// <summary>
        /// The action creator for the rejected action.
        /// </summary>
        public RejectedActionCreator<TPayload> rejected { get; }

        internal PayloadCreator payloadCreator { get; }

        internal AsyncThunkOptions<TThunkArg> options { get; }

        internal AsyncThunkActionCreator(string type, PayloadCreator payloadCreator, AsyncThunkOptions<TThunkArg> options = null)
        {
            this.type = !string.IsNullOrEmpty(type) ? type : throw new ArgumentNullException(nameof(type));
            this.payloadCreator = payloadCreator ?? throw new ArgumentNullException(nameof(payloadCreator));
            this.options = options ?? new AsyncThunkOptions<TThunkArg>();

            pending = new PendingActionCreator<TPayload>(MemoryUtils.Concatenate(type, "/pending"));
            fulfilled = new FulfilledActionCreator<TPayload>(MemoryUtils.Concatenate(type, "/fulfilled"));
            rejected = new RejectedActionCreator<TPayload>(MemoryUtils.Concatenate(type, "/rejected"));
        }

        /// <summary>
        /// Create the action to dispatch.
        /// </summary>
        /// <param name="arg"> The argument to pass to the thunk. </param>
        /// <returns> The action to dispatch. </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public AsyncThunkAction<TPayload,TThunkArg> Invoke(TThunkArg arg)
        {
            return new AsyncThunkAction<TPayload,TThunkArg>(this, type, arg);
        }
    }
}
