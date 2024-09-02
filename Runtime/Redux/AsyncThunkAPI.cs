using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine.Assertions;

namespace Unity.AppUI.Redux
{
    /// <summary>
    /// An interface to interact with the thunk during the execution.
    /// You can use this interface to abort the thunk, reject the thunk with a value, or fulfill the thunk with a value.
    /// You also have access to the store to dispatch new actions.
    /// </summary>
    /// <typeparam name="TPayload"> The type of the payload. </typeparam>
    /// <typeparam name="TThunkArg"> The type of the argument to pass to the thunk. </typeparam>
    public class ThunkAPI<TPayload,TThunkArg>
    {
        enum ThunkStatus
        {
            Initial,
            Aborted,
            RejectedWithValue,
            Fulfilled
        }

        ThunkStatus m_Status = ThunkStatus.Initial;

        object m_AbortReason;

        TPayload m_RejectedValue;

        TPayload m_FulfilledValue;

        readonly WeakReference<CancellationTokenSource> m_CancellationTokenSource = new (null);

        /// <summary>
        /// The request ID of the thunk.
        /// </summary>
        public string requestId { get; private set; }

        /// <summary>
        /// Whether the thunk has been canceled (ether by the API or by an external cancellation token).
        /// </summary>
        public bool isCancellationRequested =>
            m_CancellationTokenSource.TryGetTarget(out var tgt) && tgt.IsCancellationRequested;

        /// <summary>
        /// The store to dispatch new actions or access the state.
        /// </summary>
        public Store store { get; }

        internal ThunkAPI(Store store)
        {
            this.store = store ?? throw new ArgumentNullException(nameof(store));
        }

        /// <summary>
        /// Aborts the thunk.
        /// </summary>
        /// <param name="reason"> The reason for aborting the thunk. It can be null. </param>
        public void Abort(object reason)
        {
            var tokenSourceExists = m_CancellationTokenSource.TryGetTarget(out var cancellationTokenSource);
            Assert.IsTrue(tokenSourceExists);
            m_AbortReason = reason;
            m_Status = ThunkStatus.Aborted;
            cancellationTokenSource.Cancel();
        }

        /// <summary>
        /// Rejects the thunk with a value.
        /// </summary>
        /// <param name="value"> The value to reject the thunk with. </param>
        public void RejectWithValue(TPayload value)
        {
            var tokenSourceExists = m_CancellationTokenSource.TryGetTarget(out var cancellationTokenSource);
            Assert.IsTrue(tokenSourceExists);
            m_RejectedValue = value;
            m_Status = ThunkStatus.RejectedWithValue;
            cancellationTokenSource.Cancel();
        }

        /// <summary>
        /// Fulfills the thunk with a value.
        /// </summary>
        /// <param name="value"> The value to fulfill the thunk with. </param>
        public void FulFillWithValue(TPayload value)
        {
            var tokenSourceExists = m_CancellationTokenSource.TryGetTarget(out var cancellationTokenSource);
            Assert.IsTrue(tokenSourceExists);
            m_FulfilledValue = value;
            m_Status = ThunkStatus.Fulfilled;
            cancellationTokenSource.Cancel();
        }

        internal async Task DispatchThunk(
            AsyncThunkAction<TPayload, TThunkArg> asyncThunkAction,
            CancellationToken cToken = default)
        {
            if (asyncThunkAction == null)
                throw new ArgumentNullException(nameof(asyncThunkAction));

            m_AbortReason = null;
            m_RejectedValue = default;
            m_FulfilledValue = default;
            m_Status = ThunkStatus.Initial;

            using var cts = CancellationTokenSource.CreateLinkedTokenSource(cToken);
            m_CancellationTokenSource.SetTarget(cts);

            var thunkArg = asyncThunkAction.payload;
            var actionCreator = asyncThunkAction.creator;

            var conditionMet = actionCreator.options.conditionAsync != null
                ? await actionCreator.options.conditionAsync.Invoke(thunkArg, store)
                : actionCreator.options.condition?.Invoke(thunkArg, store) ?? true;

            if (!conditionMet)
            {
                if (actionCreator.options.dispatchConditionRejection)
                {
                    store.Dispatch(actionCreator.rejected.Invoke(new RejectedMeta<TThunkArg>
                    {
                        arg = thunkArg,
                        reason = "The condition is not met.",
                        condition = true,
                        exception = new ConditionUnmetException(),
                    }));
                }
                return;
            }

            if (cToken.IsCancellationRequested)
            {
                store.Dispatch(actionCreator.rejected.Invoke(new RejectedMeta<TThunkArg>
                {
                    requestId = requestId,
                    arg = thunkArg,
                    exception = new OperationCanceledException("The operation has been canceled externally."),
                }));
                return;
            }

            requestId = actionCreator.options.idGenerator?.Invoke(thunkArg) ?? Guid.NewGuid().ToString();

            store.Dispatch(actionCreator.pending.Invoke(new PendingMeta<TThunkArg>
            {
                requestId = requestId,
                arg = thunkArg
            }));

            TPayload result;
            try
            {
                var thunk = actionCreator.payloadCreator.Invoke(thunkArg, this, cts.Token);
                result = await thunk;
            }
            catch (OperationCanceledException exception)
            {
                // The operation has been canceled.
                // We can check the internal status to know the reason.
                switch (m_Status)
                {
                    case ThunkStatus.RejectedWithValue:
                        store.Dispatch(actionCreator.rejected.Invoke(new RejectedMeta<TThunkArg>
                        {
                            requestId = requestId,
                            arg = thunkArg,
                            exception = exception,
                            rejectedWithValue = true,
                        }, m_RejectedValue));
                        return;
                    case ThunkStatus.Fulfilled:
                        store.Dispatch(actionCreator.fulfilled.Invoke(m_FulfilledValue, new FulfilledMeta<TThunkArg>
                        {
                            requestId = requestId,
                            arg = thunkArg,
                        }));
                        return;
                    case ThunkStatus.Aborted:
                        store.Dispatch(actionCreator.rejected.Invoke(new RejectedMeta<TThunkArg>
                        {
                            requestId = requestId,
                            arg = thunkArg,
                            exception = exception,
                            aborted = true,
                            reason = m_AbortReason
                        }));
                        return;
                    // case ThunkStatus.Initial:
                    default:
                        store.Dispatch(actionCreator.rejected.Invoke(new RejectedMeta<TThunkArg>
                        {
                            requestId = requestId,
                            arg = thunkArg,
                            exception = new OperationCanceledException("The operation has been canceled externally.", exception),
                        }));
                        return;
                }
            }
            catch (Exception e)
            {
                // An unknown exception occurred, reject the action.
                store.Dispatch(actionCreator.rejected.Invoke(new RejectedMeta<TThunkArg>
                {
                    requestId = requestId,
                    arg = thunkArg,
                    exception = e,
                }));
                return;
            }

            if (cToken.IsCancellationRequested)
            {
                store.Dispatch(actionCreator.rejected.Invoke(new RejectedMeta<TThunkArg>
                {
                    requestId = requestId,
                    arg = thunkArg,
                    exception = new OperationCanceledException("The operation has been canceled externally."),
                }));
                return;
            }

            store.Dispatch(actionCreator.fulfilled.Invoke(result, new FulfilledMeta<TThunkArg>
            {
                requestId = requestId,
                arg = thunkArg,
            }));
        }
    }
}
