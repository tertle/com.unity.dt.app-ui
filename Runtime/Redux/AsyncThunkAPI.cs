using System;
using System.Threading;
using System.Threading.Tasks;

namespace Unity.AppUI.Redux
{
    /// <summary>
    /// An interface to interact with the thunk during the execution.
    /// You can use this interface to abort the thunk, reject the thunk with a value, or fulfill the thunk with a value.
    /// You also have access to the store to dispatch new actions.
    /// </summary>
    /// <typeparam name="TArg"> The type of the argument to pass to the thunk. </typeparam>
    /// <typeparam name="TPayload"> The type of the payload. </typeparam>
    public class ThunkAPI<TArg,TPayload> : IThunkAPI<TArg,TPayload>
    {
        readonly AsyncThunkAction<TArg,TPayload> m_Action;

        CancellationTokenSource m_CancellationTokenSource;

        TaskCompletionSource<TPayload> m_TaskCompletionSource;

        /// <summary>
        /// The request ID of the thunk.
        /// </summary>
        public string requestId { get; }

        /// <summary>
        /// The argument passed to the thunk when it was dispatched.
        /// </summary>
        public TArg arg => m_Action.payload;

        /// <summary>
        /// Whether the thunk has been canceled (ether by the API or by an external cancellation token).
        /// </summary>
        public bool isCancellationRequested => m_CancellationTokenSource.IsCancellationRequested;

        /// <summary>
        /// The cancellation token of the thunk.
        /// </summary>
        public CancellationToken cancellationToken => m_CancellationTokenSource.Token;

        /// <summary>
        /// The store to dispatch new actions or access the state.
        /// </summary>
        public IDispatchable store { get; }

        internal ThunkAPI(IDispatchable store, AsyncThunkAction<TArg,TPayload> action)
        {
            this.store = store ?? throw new ArgumentNullException(nameof(store));
            m_Action = action ?? throw new ArgumentNullException(nameof(action));
            requestId = GenerateRequestId();
        }

        /// <summary>
        /// Aborts the thunk.
        /// </summary>
        /// <param name="reason"> The reason for aborting the thunk. It can be null. </param>
        public void Abort(object reason)
        {
            if (m_TaskCompletionSource.Task.IsCompleted)
                return;

            // We cant use SetCanceled() here because we need to pass a reason.
            m_TaskCompletionSource.SetException(new AbortedWithReasonException(reason));
            m_CancellationTokenSource.Cancel();
        }

        /// <summary>
        /// Rejects the thunk with a value.
        /// </summary>
        /// <param name="value"> The value to reject the thunk with. </param>
        public void RejectWithValue(TPayload value)
        {
            if (m_TaskCompletionSource.Task.IsCompleted)
                return;

            m_TaskCompletionSource.SetException(new RejectedWithValueException<TPayload>(value));
            m_CancellationTokenSource.Cancel();
        }

        /// <summary>
        /// Fulfills the thunk with a value.
        /// </summary>
        /// <param name="value"> The value to fulfill the thunk with. </param>
        public void FulFillWithValue(TPayload value)
        {
            if (m_TaskCompletionSource.Task.IsCompleted)
                return;

            m_TaskCompletionSource.SetResult(value);
            m_CancellationTokenSource.Cancel();
        }

        internal string GenerateRequestId()
        {
            return m_Action.creator.options.idGenerator?.Invoke(m_Action.payload) ?? Guid.NewGuid().ToString();
        }

        internal async Task DispatchThunk(CancellationToken cToken = default)
        {
            m_TaskCompletionSource = new TaskCompletionSource<TPayload>();
            m_CancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cToken);

            var creator = m_Action.creator;
            var options = m_Action.creator.options;

            var conditionMet = options.conditionAsync != null
                ? await options.conditionAsync.Invoke(arg, store)
                : options.condition?.Invoke(arg, store) ?? true;

            if (!conditionMet)
            {
                if (options.dispatchConditionRejection)
                    HandleNotMetCondition();
                return;
            }

            store.Dispatch(creator.pending.Invoke(new PendingMeta<TArg>
            {
                requestId = requestId,
                arg = arg
            }));

            try
            {
                var runnerTask = creator.runner.Invoke(this);
                _ = await Unity.AppUI.Core.TaskExtensions.WhenAny(m_TaskCompletionSource.Task, runnerTask);

                if (m_TaskCompletionSource.Task.IsCompleted)
                {
                    if (m_TaskCompletionSource.Task.IsFaulted)
                    {
                        var exception = m_TaskCompletionSource.Task.Exception!.InnerException;
                        if (exception is OperationCanceledException operationCanceledException)
                            HandleCancellation(operationCanceledException);
                        else
                            HandleFaultedTask(exception);
                    }
                    else
                    {
                        HandleFulfillment(m_TaskCompletionSource.Task.Result);
                    }
                }
                else // runnerTask is finished
                {
                    var payload = await runnerTask;
                    HandleFulfillment(payload);
                }
            }
            catch (Exception e)
            {
                // An unknown exception occurred, reject the action.
                HandleException(e);
            }

            m_CancellationTokenSource.Dispose();
            m_TaskCompletionSource = null;
            m_CancellationTokenSource = null;
        }

        void HandleNotMetCondition()
        {
            store.Dispatch(m_Action.creator.rejected.Invoke(new RejectedMeta<TArg>
            {
                requestId = requestId,
                arg = arg,
                reason = "The condition is not met.",
                condition = true,
                exception = new ConditionUnmetException()
            }));
        }

        void HandleCancellation(OperationCanceledException exception)
        {
            store.Dispatch(m_Action.creator.rejected.Invoke(new RejectedMeta<TArg>
            {
                requestId = requestId,
                arg = arg,
                aborted = true,
                reason = (exception as AbortedWithReasonException)?.Reason ?? exception.Message,
                exception = exception
            }));
        }

        void HandleFaultedTask(Exception exception)
        {
            if (exception is RejectedWithValueException<TPayload> rejectedEx)
            {
                // Rejected with value
                store.Dispatch(m_Action.creator.rejected.Invoke(new RejectedMeta<TArg>
                {
                    requestId = requestId,
                    arg = arg,
                    rejectedWithValue = true,
                    exception = rejectedEx
                }, rejectedEx.Value));
            }
            else
            {
                HandleException(exception);
            }
        }

        void HandleException(Exception exception)
        {
            store.Dispatch(m_Action.creator.rejected.Invoke(new RejectedMeta<TArg>
            {
                requestId = requestId,
                arg = arg,
                exception = exception
            }));
        }

        void HandleFulfillment(TPayload result)
        {
            store.Dispatch(m_Action.creator.fulfilled.Invoke(result, new FulfilledMeta<TArg>
            {
                requestId = requestId,
                arg = arg
            }));
        }
    }

    /// <summary>
    /// Extensions for the async thunk API.
    /// </summary>
    public static class AsyncThunkAPIExtensions
    {
        /// <summary>
        /// Dispatches an action to the store.
        /// </summary>
        /// <param name="api"> The thunk API. </param>
        /// <param name="action"> The action to dispatch. </param>
        /// <exception cref="ArgumentNullException"> Thrown if the action or thunk API is null. </exception>
        public static void Dispatch(this IThunkAPI api, IAction action)
        {
            if (api == null)
                throw new ArgumentNullException(nameof(api));
            if (action == null)
                throw new ArgumentNullException(nameof(action));
            api.store.Dispatch(action);
        }

        /// <summary>
        /// Dispatches an action creator to the store.
        /// </summary>
        /// <param name="api"> The thunk API. </param>
        /// <param name="creator"> The action creator to dispatch. </param>
        /// <exception cref="ArgumentNullException"> Thrown if the action creator or thunk API is null. </exception>
        public static void Dispatch(this IThunkAPI api, ActionCreator creator)
        {
            if (api == null)
                throw new ArgumentNullException(nameof(api));
            if (creator == null)
                throw new ArgumentNullException(nameof(creator));
            api.store.Dispatch(creator.Invoke());
        }

        /// <summary>
        /// Dispatches an action creator with a payload to the store.
        /// </summary>
        /// <param name="api"> The thunk API. </param>
        /// <param name="creator"> The action creator to dispatch. </param>
        /// <param name="payload"> The payload to pass to the action creator. </param>
        /// <typeparam name="TPayload"> The type of the payload for the action to dispatch. </typeparam>
        /// <exception cref="ArgumentNullException"> Thrown if the action creator or thunk API is null. </exception>
        public static void Dispatch<TPayload>(this IThunkAPI api, ActionCreator<TPayload> creator, TPayload payload)
        {
            if (api == null)
                throw new ArgumentNullException(nameof(api));
            if (creator == null)
                throw new ArgumentNullException(nameof(creator));
            api.store.Dispatch(creator, payload);
        }

        /// <summary>
        /// Dispatches an action type to the store.
        /// </summary>
        /// <param name="api"> The thunk API. </param>
        /// <param name="actionType"> The action type to dispatch. </param>
        /// <exception cref="ArgumentNullException"> Thrown if the thunk API is null. </exception>
        public static void Dispatch(this IThunkAPI api, string actionType)
        {
            if (api == null)
                throw new ArgumentNullException(nameof(api));
            if (string.IsNullOrEmpty(actionType))
                throw new ArgumentNullException(nameof(actionType));
            api.store.Dispatch(actionType);
        }

        /// <summary>
        /// Dispatches an action type to the store.
        /// </summary>
        /// <param name="api"> The thunk API. </param>
        /// <param name="actionType"> The action type to dispatch. </param>
        /// <param name="payload"> The payload to pass to the action creator. </param>
        /// <typeparam name="TPayload"> The type of the payload for the action to dispatch. </typeparam>
        /// <exception cref="ArgumentNullException"> Thrown if the thunk API is null. </exception>
        public static void Dispatch<TPayload>(this IThunkAPI api, string actionType, TPayload payload)
        {
            if (api == null)
                throw new ArgumentNullException(nameof(api));
            if (string.IsNullOrEmpty(actionType))
                throw new ArgumentNullException(nameof(actionType));
            api.store.Dispatch(actionType, payload);
        }

        /// <summary>
        /// Gets the state of the store.
        /// </summary>
        /// <param name="api"> The thunk API. </param>
        /// <typeparam name="TState"> The type of the store state. </typeparam>
        /// <returns> The state of the store. </returns>
        /// <exception cref="ArgumentNullException"> Thrown if the thunk API is null. </exception>
        /// <exception cref="InvalidOperationException"> Thrown if the store does not implement IStore{T}. </exception>
        public static TState GetState<TState>(this IThunkAPI api)
        {
            if (api == null)
                throw new ArgumentNullException(nameof(api));
            if (api.store is IStore<TState> s)
                return s.GetState();
            throw new InvalidOperationException("The store does not implement IStore<T>.");
        }
    }
}
