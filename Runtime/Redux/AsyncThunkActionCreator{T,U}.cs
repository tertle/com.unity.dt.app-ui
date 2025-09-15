using System;
using Unity.AppUI.Core;

namespace Unity.AppUI.Redux
{
    /// <summary>
    /// The async thunk action creator.
    /// </summary>
    /// <typeparam name="TPayload"> The type of the payload. </typeparam>
    /// <typeparam name="TArg"> The type of the thunk argument. </typeparam>
    public abstract class AsyncThunkCreatorBase<TArg, TPayload>
    {
        /// <summary>
        /// The type of the action.
        /// </summary>
        public string type { get; }

        /// <summary>
        /// The action creator for the pending action.
        /// </summary>
        public PendingActionCreator<TPayload> pending { get; private set; }

        /// <summary>
        /// The action creator for the fulfilled action.
        /// </summary>
        public FulfilledActionCreator<TPayload> fulfilled { get; private set; }

        /// <summary>
        /// The action creator for the rejected action.
        /// </summary>
        public RejectedActionCreator<TPayload> rejected { get; private set; }

        internal AsyncThunk<TArg,TPayload> runner { get; private set; }

        internal AsyncThunkOptions<TArg> options { get; private set; }

        /// <summary>
        /// Creates a new async thunk action creator.
        /// </summary>
        /// <param name="type"> The type of the action. </param>
        /// <param name="runner"> The async thunk runner. </param>
        /// <param name="options"> The options for the async thunk. </param>
        /// <exception cref="ArgumentNullException"> Thrown when the type or runner is null or empty. </exception>
        protected AsyncThunkCreatorBase(
            string type,
            AsyncThunk<TArg,TPayload> runner,
            AsyncThunkOptions<TArg> options = null)
        {
            this.type = !string.IsNullOrEmpty(type) ? type : throw new ArgumentNullException(nameof(type));
            this.runner = runner ?? throw new ArgumentNullException(nameof(runner));
            this.options = options ?? new AsyncThunkOptions<TArg>();

            pending = new PendingActionCreator<TPayload>(MemoryUtils.Concatenate(type, "/pending"));
            fulfilled = new FulfilledActionCreator<TPayload>(MemoryUtils.Concatenate(type, "/fulfilled"));
            rejected = new RejectedActionCreator<TPayload>(MemoryUtils.Concatenate(type, "/rejected"));
        }
    }

    /// <summary>
    /// The async thunk action creator.
    /// </summary>
    /// <typeparam name="TPayload"> The type of the payload. </typeparam>
    /// <typeparam name="TArg"> The type of the thunk argument. </typeparam>
    public class AsyncThunkCreator<TArg, TPayload> :
        AsyncThunkCreatorBase<TArg,TPayload>,
        IAsyncThunkCreator<TArg,TPayload>
    {
        /// <summary>
        /// Creates a new async thunk action creator.
        /// </summary>
        /// <param name="type"> The type of the action. </param>
        /// <param name="runner"> The async thunk runner. </param>
        /// <param name="options"> The options for the async thunk. </param>
        public AsyncThunkCreator(
            string type,
            AsyncThunk<TArg,TPayload> runner,
            AsyncThunkOptions<TArg> options = null)
            : base(type, runner, options) { }

        /// <summary>
        /// Create the action to dispatch.
        /// </summary>
        /// <param name="arg"> The argument to pass to the thunk. </param>
        /// <returns> The async thunk action. </returns>
        public AsyncThunkAction<TArg,TPayload> Invoke(TArg arg)
        {
            return new AsyncThunkAction<TArg, TPayload>(this, arg);
        }
    }

    /// <summary>
    /// The async thunk action creator for a thunk without arguments.
    /// </summary>
    /// <typeparam name="TPayload"> The type of the payload. </typeparam>
    public class AsyncThunkCreator<TPayload> :
        AsyncThunkCreator<bool, TPayload>,
        IAsyncThunkCreator<TPayload>
    {
        /// <summary>
        /// Creates a new async thunk action creator.
        /// </summary>
        /// <param name="type"> The type of the action. </param>
        /// <param name="runner"> The async thunk runner. </param>
        /// <param name="options"> The options for the async thunk. </param>
        public AsyncThunkCreator(
            string type,
            AsyncThunk<TPayload> runner,
            AsyncThunkOptions<bool> options = null)
            : base(type, api => runner(api), options) { }

        /// <inheritdoc />
        public AsyncThunkAction<TPayload> Invoke()
        {
            return new AsyncThunkAction<TPayload>(this);
        }
    }
}
