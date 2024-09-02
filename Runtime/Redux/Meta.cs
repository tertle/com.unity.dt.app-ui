using System;

namespace Unity.AppUI.Redux
{
    /// <summary>
    /// The metadata of the thunk.
    /// </summary>
    /// <param name="thunkStatus"> The status of the thunk. </param>
    /// <typeparam name="TThunkArg"> The type of the argument to pass to the thunk. </typeparam>
    public record Meta<TThunkArg>(ThunkStatus thunkStatus)
    {
        /// <summary>
        /// The request ID of the thunk.
        /// </summary>
        public string requestId { get; internal set; }

        /// <summary>
        /// The argument passed to the thunk.
        /// </summary>
        public TThunkArg arg { get; internal set; }

        /// <summary>
        /// The status of the thunk.
        /// </summary>
        public ThunkStatus thunkStatus { get; } = thunkStatus;
    }

    /// <summary>
    /// The metadata of the thunk when it is pending.
    /// </summary>
    /// <typeparam name="TThunkArg"> The type of the argument to pass to the thunk. </typeparam>
    public record PendingMeta<TThunkArg>() : Meta<TThunkArg>(ThunkStatus.Pending);

    /// <summary>
    /// The metadata of the thunk when it is fulfilled.
    /// </summary>
    /// <typeparam name="TThunkArg"> The type of the argument to pass to the thunk. </typeparam>
    public record FulfilledMeta<TThunkArg>() : Meta<TThunkArg>(ThunkStatus.Fulfilled);

    /// <summary>
    /// The metadata of the thunk when it is rejected.
    /// </summary>
    /// <typeparam name="TThunkArg"> The type of the argument to pass to the thunk. </typeparam>
    public record RejectedMeta<TThunkArg>() : Meta<TThunkArg>(ThunkStatus.Rejected)
    {
        /// <summary>
        /// Whether the thunk has been aborted.
        /// </summary>
        public bool aborted { get; internal set; }

        /// <summary>
        /// The reason for aborting the thunk.
        /// </summary>
        public object reason { get; internal set; }

        /// <summary>
        /// Whether the thunk has been rejected because of the condition set in the options.
        /// </summary>
        public bool condition { get; internal set; }

        /// <summary>
        /// Whether the thunk has been rejected with a value.
        /// </summary>
        public bool rejectedWithValue { get; internal set; }

        /// <summary>
        /// The exception thrown by the thunk.
        /// </summary>
        public Exception exception { get; internal set; }
    }
}
