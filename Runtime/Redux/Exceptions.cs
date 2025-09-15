using System;

namespace Unity.AppUI.Redux
{
    /// <summary>
    /// An action dispatched when the condition set in
    /// the options of the Async Thunk creation is not met.
    /// </summary>
    /// <seealso cref="AsyncThunkOptions{TArg}.ConditionHandler"/>
    /// <seealso cref="AsyncThunkOptions{TArg}.ConditionHandlerAsync"/>
    public class ConditionUnmetException : Exception { }

    /// <summary>
    /// An exception thrown when a Thunk is aborted.
    /// </summary>
    public class AbortedWithReasonException : OperationCanceledException
    {
        /// <summary>
        /// The reason for the abortion.
        /// </summary>
        public object Reason { get; }

        /// <summary>
        /// Create a new instance of the <see cref="AbortedWithReasonException"/> class.
        /// </summary>
        /// <param name="reason"> The reason for the abortion. </param>
        public AbortedWithReasonException(object reason)
            : base("The operation was canceled.")
        {
            Reason = reason;
        }
    }

    /// <summary>
    /// An exception thrown when a Thunk is rejected.
    /// </summary>
    /// <typeparam name="TPayload"> The type of the payload. </typeparam>
    public class RejectedWithValueException<TPayload> : Exception
    {
        /// <summary>
        /// The value of the rejection.
        /// </summary>
        public TPayload Value { get; }

        /// <summary>
        /// Create a new instance of the <see cref="RejectedWithValueException{TPayload}"/> class.
        /// </summary>
        /// <param name="value"> The value of the rejection. </param>
        public RejectedWithValueException(TPayload value)
            : base("The Thunk was rejected with a value.")
        {
            Value = value;
        }
    }
}
