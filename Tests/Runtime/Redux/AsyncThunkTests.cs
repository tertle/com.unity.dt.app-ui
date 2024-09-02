using System;
using System.Collections;
using System.Threading.Tasks;
using NUnit.Framework;
using Unity.AppUI.Redux;
using UnityEngine.TestTools;

namespace Unity.AppUI.Tests.Redux
{
    [TestFixture]
    public class AsyncThunkTests
    {
        [Test]
        public void CreateAsyncThunk_WhenDelegateIsNull_ThrowsArgumentNullException()
        {
            AsyncThunkActionCreator<string,string>.PayloadCreator asyncThunkDelegate = null;
            Assert.Throws<ArgumentNullException>(() => new AsyncThunkActionCreator<string,string>("myAsyncThunk", asyncThunkDelegate));
            Assert.Throws<ArgumentNullException>(() => Store.CreateAsyncThunk("myAsyncThunk", asyncThunkDelegate));
        }

        [Test]
        public void CreateAsyncThunk_WhenTypeIsNull_ThrowsArgumentNullException()
        {
            AsyncThunkActionCreator<string,string>.PayloadCreator asyncThunkDelegate = (_, _, _) => Task.FromResult("result");
            Assert.Throws<ArgumentNullException>(() => new AsyncThunkActionCreator<string,string>(null, asyncThunkDelegate));
        }

        [Test]
        public void DispatchAsyncThunk_WhenStoreIsNull_ThrowsArgumentNullException()
        {
            AsyncThunkActionCreator<string,string>.PayloadCreator asyncThunkDelegate = (_, _, _) => Task.FromResult("result");
            var asyncThunk = new AsyncThunkActionCreator<string,string>("myAsyncThunk", asyncThunkDelegate);
            Assert.ThrowsAsync<ArgumentNullException>(async () =>
                await StoreExtensions.DispatchAsyncThunk(null, asyncThunk.Invoke("arg")));
        }

        [Test]
        public void DispatchAsyncThunk_WhenAsyncThunkIsNull_ThrowsArgumentNullException()
        {
            var store = new Store();
            Assert.ThrowsAsync<ArgumentNullException>(async () =>
                await StoreExtensions.DispatchAsyncThunk<string,string>(store, null));
        }

        [Test]
        public void DispatchAsyncThunkCoroutine_WhenStoreIsNull_ThrowsArgumentNullException()
        {
            AsyncThunkActionCreator<string,string>.PayloadCreator asyncThunkDelegate = (_, _, _) => Task.FromResult("result");
            var asyncThunk = new AsyncThunkActionCreator<string,string>("myAsyncThunk", asyncThunkDelegate);
            Assert.Throws<ArgumentNullException>(() => StoreExtensions.DispatchAsyncThunkCoroutine(null, asyncThunk.Invoke("arg")));
        }

        [Test]
        public void DispatchAsyncThunkCoroutine_WhenAsyncThunkIsNull_ThrowsArgumentNullException()
        {
            var store = new Store();
            Assert.Throws<ArgumentNullException>(() => StoreExtensions.DispatchAsyncThunkCoroutine<string,string>(store, null));
        }

        [Test]
        public void ThunkAPI_Constructor_WhenStoreIsNull_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new ThunkAPI<string,string>(null));
        }

        [Test]
        public void ThunkAPI_DispatchThunk_WhenAsyncThunkIsNull_ThrowsArgumentNullException()
        {
            var store = new Store();
            var api = new ThunkAPI<string,string>(store);
            Assert.ThrowsAsync<ArgumentNullException>(async () => await api.DispatchThunk(null));
        }

        [Test]
        public void Store_CreateAsyncThunk_ShouldCreateAsyncThunk()
        {
            AsyncThunkActionCreator<string, string> asyncThunkActionCreator = null;
            Assert.DoesNotThrow(() =>
                asyncThunkActionCreator = Store.CreateAsyncThunk<string, string>(
                    "myAsyncThunk", (_, _, _) => Task.FromResult("result")));
            Assert.IsNotNull(asyncThunkActionCreator);
            Assert.AreEqual("myAsyncThunk", asyncThunkActionCreator.type);

            Assert.IsNotNull(asyncThunkActionCreator.pending);
            Assert.AreEqual("myAsyncThunk/pending", asyncThunkActionCreator.pending.type);
            Assert.IsNotNull(asyncThunkActionCreator.fulfilled);
            Assert.AreEqual("myAsyncThunk/fulfilled", asyncThunkActionCreator.fulfilled.type);
            Assert.IsNotNull(asyncThunkActionCreator.rejected);
            Assert.AreEqual("myAsyncThunk/rejected", asyncThunkActionCreator.rejected.type);

            asyncThunkActionCreator = null;
            Assert.DoesNotThrow(() =>
                asyncThunkActionCreator = new AsyncThunkActionCreator<string, string>(
                    "myAsyncThunk", (_, _, _) => Task.FromResult("result")));
            Assert.IsNotNull(asyncThunkActionCreator);
        }

        [Test]
        public void PendingActionCreator_Invoke_ShouldCreatePendingAction()
        {
            var pendingActionCreator = new PendingActionCreator<string>("myAsyncThunk/pending");
            var pendingAction = pendingActionCreator.Invoke(new PendingMeta<string>
            {
                requestId = "requestId",
                arg = "arg"
            });
            Assert.IsNotNull(pendingAction);
            Assert.AreEqual("myAsyncThunk/pending", pendingAction.type);
            Assert.IsNull(pendingAction.payload);
            Assert.IsNotNull(pendingAction.meta);
            Assert.AreEqual("requestId", pendingAction.meta.requestId);
            Assert.AreEqual("arg", pendingAction.meta.arg);
            Assert.AreEqual(ThunkStatus.Pending, pendingAction.meta.thunkStatus);
        }

        [Test]
        public void FulfilledActionCreator_Invoke_ShouldCreateFulfilledAction()
        {
            var fulfilledActionCreator = new FulfilledActionCreator<string>("myAsyncThunk/fulfilled");
            var fulfilledAction = fulfilledActionCreator.Invoke("result", new FulfilledMeta<string>
            {
                requestId = "requestId",
                arg = "arg",
            });
            Assert.IsNotNull(fulfilledAction);
            Assert.AreEqual("myAsyncThunk/fulfilled", fulfilledAction.type);
            Assert.AreEqual("result", fulfilledAction.payload);
            Assert.IsNotNull(fulfilledAction.meta);
            Assert.AreEqual("requestId", fulfilledAction.meta.requestId);
            Assert.AreEqual("arg", fulfilledAction.meta.arg);
            Assert.AreEqual(ThunkStatus.Fulfilled, fulfilledAction.meta.thunkStatus);
        }

        [Test]
        public void RejectedActionCreator_Invoke_ShouldCreateRejectedAction()
        {
            var rejectedActionCreator = new RejectedActionCreator<string>("myAsyncThunk/rejected");
            var rejectedAction = rejectedActionCreator.Invoke(new RejectedMeta<string>
            {
                requestId = "requestId",
                arg = "arg",
            });
            Assert.IsNotNull(rejectedAction);
            Assert.AreEqual("myAsyncThunk/rejected", rejectedAction.type);
            Assert.IsNull(rejectedAction.payload);
            Assert.IsNotNull(rejectedAction.meta);
            Assert.AreEqual("requestId", rejectedAction.meta.requestId);
            Assert.AreEqual("arg", rejectedAction.meta.arg);
            Assert.IsFalse(rejectedAction.meta.aborted);
            Assert.IsNull(rejectedAction.meta.reason);
            Assert.AreEqual(ThunkStatus.Rejected, rejectedAction.meta.thunkStatus);
        }

        [Test]
        public void RejectedActionCreator_Abort_ShouldCreateRejectedAction()
        {
            var rejectedActionCreator = new RejectedActionCreator<string>("myAsyncThunk/rejected");
            var rejectedAction = rejectedActionCreator.Invoke(new RejectedMeta<string>
            {
                requestId = "requestId",
                arg = "arg",
                aborted = true,
                reason = "reason"
            });
            Assert.IsNotNull(rejectedAction);
            Assert.AreEqual("myAsyncThunk/rejected", rejectedAction.type);
            Assert.IsNull(rejectedAction.payload);
            Assert.IsNotNull(rejectedAction.meta);
            Assert.AreEqual("requestId", rejectedAction.meta.requestId);
            Assert.AreEqual("arg", rejectedAction.meta.arg);
            Assert.IsTrue(rejectedAction.meta.aborted);
            Assert.AreEqual("reason", rejectedAction.meta.reason);
            Assert.AreEqual(ThunkStatus.Rejected, rejectedAction.meta.thunkStatus);
        }

        [Test]
        public void RejectedActionCreator_InvokeWithValue_ShouldCreateRejectedAction()
        {
            var rejectedActionCreator = new RejectedActionCreator<string>("myAsyncThunk/rejected");
            var rejectedAction = rejectedActionCreator.Invoke(new RejectedMeta<string>
            {
                requestId = "requestId",
                arg = "arg",
            }, "error");
            Assert.IsNotNull(rejectedAction);
            Assert.AreEqual("myAsyncThunk/rejected", rejectedAction.type);
            Assert.AreEqual("error", rejectedAction.payload);
            Assert.IsNotNull(rejectedAction.meta);
            Assert.AreEqual("requestId", rejectedAction.meta.requestId);
            Assert.AreEqual("arg", rejectedAction.meta.arg);
            Assert.AreEqual(ThunkStatus.Rejected, rejectedAction.meta.thunkStatus);
        }

        [Test]
        public async Task Store_DispatchAsyncThunk_ShouldDispatchAsyncThunk()
        {
            var store = new Store();
            var asyncThunk = Store.CreateAsyncThunk<string, string>(
                "myAsyncThunk", async (_, _, _) => await Task.FromResult("result"));

            store.CreateSlice("mySlice", new MyState(), null, builder =>
            {
                builder.AddCase(asyncThunk.fulfilled, (state, action) => state with { value = action.payload });
            });

            await store.DispatchAsyncThunk(asyncThunk.Invoke("arg"));
            Assert.AreEqual("result", store.GetState<MyState>("mySlice").value);
        }

        [UnityTest]
        public IEnumerator Store_DispatchAsyncThunkCoroutine_ShouldDispatchAsyncThunk()
        {
            var store = new Store();
            var asyncThunk = Store.CreateAsyncThunk<string, string>(
                "myAsyncThunk", async (_, _, _) => await Task.FromResult("result"));

            store.CreateSlice("mySlice", new MyState(), null, builder =>
            {
                builder.AddCase(asyncThunk.fulfilled, (state, action) => state with { value = action.payload });
            });

            yield return store.DispatchAsyncThunkCoroutine(asyncThunk.Invoke("arg"));
            Assert.AreEqual("result", store.GetState<MyState>("mySlice").value);
        }

        [Test]
        [TestCase(false, true)]
        [TestCase(false, false)]
        [TestCase(true, true)]
        [TestCase(true, false)]
        public async Task Store_DispatchAsyncThunk_WithCondition_ShouldAbort(bool shouldReject, bool asyncCondition)
        {
            var store = new Store();
            var options = new AsyncThunkOptions<string>
            {
                dispatchConditionRejection = shouldReject
            };
            if (asyncCondition)
                options.conditionAsync = async (arg, _) => await Task.FromResult(arg != "abort");
            else
                options.condition = (arg, _) => arg != "abort";

            var asyncThunk = Store.CreateAsyncThunk<string, string>(
                "myAsyncThunk", async (_, _, _) => await Task.FromResult("result"),
                options);

            store.CreateSlice("mySlice", new MyState(), null, builder =>
            {
                builder.AddCase(asyncThunk.pending, (state, _) => state with { value = "pending" });
                builder.AddCase(asyncThunk.fulfilled, (state, _) => state with { value = "fulfilled" });
                builder.AddCase(asyncThunk.rejected, (state, _) => state with { value = "rejected" });
            });

            await store.DispatchAsyncThunk(asyncThunk.Invoke("abort"));
            Assert.AreEqual(shouldReject ? "rejected" : null, store.GetState<MyState>("mySlice").value);
        }

        [UnityTest]
        public IEnumerator Store_DispatchAsyncThunk_CanBeAbortedExternally()
        {
            var store = new Store();
            var asyncThunk = Store.CreateAsyncThunk<string, string>(
                "myAsyncThunk", async (_, _, token) =>
                {
                    await Task.Delay(1000, token);
                    return "result";
                });

            store.CreateSlice("mySlice", new MyState(), null, builder =>
            {
                builder.AddCase(asyncThunk.pending, (state, action) => state with { value = action });
                builder.AddCase(asyncThunk.fulfilled, (state, action) => state with { value = action });
                builder.AddCase(asyncThunk.rejected, (state, action) => state with { value = action });
            });

            // dispatch will already set the thunk status as pending, so we need to Cancel the token before dispatching
            using var cancellationTokenSource = new System.Threading.CancellationTokenSource();
            cancellationTokenSource.Cancel();

            store.DispatchAsyncThunk(asyncThunk.Invoke("arg"), cancellationTokenSource.Token);

            var rejectedAction = store.GetState<MyState>("mySlice").value as RejectedAction<string, string>;
            Assert.IsNotNull(rejectedAction);
            Assert.IsFalse(rejectedAction.meta.aborted,
                "Aborted property should be false when the thunk is rejected because of external cancellation.");
            Assert.IsNull(rejectedAction.meta.reason);
            var cancelException = rejectedAction.meta.exception as OperationCanceledException;
            Assert.IsNotNull(cancelException);
            Assert.AreEqual("The operation has been canceled externally.", cancelException.Message);

            // now we will dispatch the thunk again and cancel it after a while
            using var cancellationTokenSource2 = new System.Threading.CancellationTokenSource();
            store.DispatchAsyncThunk(asyncThunk.Invoke("arg"), cancellationTokenSource2.Token);
            Assert.AreEqual(ThunkStatus.Pending,
                (store.GetState<MyState>("mySlice").value as PendingAction<string,string>)?.meta.thunkStatus);
            yield return new UnityEngine.WaitForSeconds(0.1f);
            cancellationTokenSource2.Cancel();

            yield return new WaitUntilOrTimeOut(
                () => store.GetState<MyState>("mySlice").value is RejectedAction<string, string>,
                false,
                TimeSpan.FromSeconds(1));

            rejectedAction = store.GetState<MyState>("mySlice").value as RejectedAction<string, string>;
            Assert.IsNotNull(rejectedAction);
            Assert.IsFalse(rejectedAction.meta.aborted,
                "Aborted property should be false when the thunk is rejected because of external cancellation.");
            Assert.IsNull(rejectedAction.meta.reason);
            cancelException = rejectedAction.meta.exception as OperationCanceledException;
            Assert.IsNotNull(cancelException);
            Assert.AreEqual("The operation has been canceled externally.", cancelException.Message);
        }

        [UnityTest]
        public IEnumerator Store_DispatchAsyncThunk_CanBeAbortedInternally()
        {
            var store = new Store();
            var asyncThunk = Store.CreateAsyncThunk<string, string>(
                "myAsyncThunk", async (_, api, token) =>
                {
                    await Task.Delay(100, token);
                    api.Abort("reason");

                    // the operation should be canceled before this line
                    await Task.Delay(100, token);
                    return "result";
                });

            store.CreateSlice("mySlice", new MyState(), null, builder =>
            {
                builder.AddCase(asyncThunk.pending, (state, action) => state with { value = action });
                builder.AddCase(asyncThunk.fulfilled, (state, action) => state with { value = action });
                builder.AddCase(asyncThunk.rejected, (state, action) => state with { value = action });
            });

            store.DispatchAsyncThunk(asyncThunk.Invoke("arg"));

            yield return new WaitUntilOrTimeOut(
                () => store.GetState<MyState>("mySlice").value is RejectedAction<string, string>,
                false,
                TimeSpan.FromSeconds(1));

            var rejectedAction = store.GetState<MyState>("mySlice").value as RejectedAction<string, string>;
            Assert.IsNotNull(rejectedAction);
            Assert.IsFalse(rejectedAction.meta.rejectedWithValue);
            Assert.IsTrue(rejectedAction.meta.aborted,
                "Aborted property should be true when the thunk is rejected via a call to Abort method.");
            Assert.AreEqual("reason", rejectedAction.meta.reason);
            var taskCanceledException = rejectedAction.meta.exception as TaskCanceledException;
            Assert.IsNotNull(taskCanceledException);
        }

        [UnityTest]
        public IEnumerator Store_DispatchAsyncThunk_CanBeRejectedWithValue()
        {
            var store = new Store();
            var asyncThunk = Store.CreateAsyncThunk<string, string>(
                "myAsyncThunk", async (_, api, token) =>
                {
                    await Task.Delay(100, token);
                    api.RejectWithValue("error");

                    // the operation should be canceled before this line
                    await Task.Delay(100, token);
                    return "result";
                });

            store.CreateSlice("mySlice", new MyState(), null, builder =>
            {
                builder.AddCase(asyncThunk.pending, (state, action) => state with { value = action });
                builder.AddCase(asyncThunk.fulfilled, (state, action) => state with { value = action });
                builder.AddCase(asyncThunk.rejected, (state, action) => state with { value = action });
            });

            store.DispatchAsyncThunk(asyncThunk.Invoke("arg"));

            yield return new WaitUntilOrTimeOut(
                () => store.GetState<MyState>("mySlice").value is RejectedAction<string, string>,
                false,
                TimeSpan.FromSeconds(1));

            var rejectedAction = store.GetState<MyState>("mySlice").value as RejectedAction<string, string>;
            Assert.IsNotNull(rejectedAction);
            Assert.IsTrue(rejectedAction.meta.rejectedWithValue);
            Assert.IsFalse(rejectedAction.meta.aborted,
                "Aborted property should be false when the thunk is rejected via a call to RejectWithValue method.");
            Assert.AreEqual(null, rejectedAction.meta.reason);
            Assert.AreEqual("error", rejectedAction.payload);
            var taskCanceledException = rejectedAction.meta.exception as TaskCanceledException;
            Assert.IsNotNull(taskCanceledException);
        }

        [UnityTest]
        public IEnumerator Store_DispatchAsyncThunk_CanBeFulfilledPrematurelyWithValue()
        {
            var store = new Store();
            var asyncThunk = Store.CreateAsyncThunk<string, string>(
                "myAsyncThunk", async (_, api, token) =>
                {
                    await Task.Delay(100, token);
                    api.FulFillWithValue("premature result");

                    // the operation should be canceled before this line
                    await Task.Delay(100, token);
                    return "result";
                });

            store.CreateSlice("mySlice", new MyState(), null, builder =>
            {
                builder.AddCase(asyncThunk.pending, (state, action) => state with { value = action });
                builder.AddCase(asyncThunk.fulfilled, (state, action) => state with { value = action });
                builder.AddCase(asyncThunk.rejected, (state, action) => state with { value = action });
            });

            store.DispatchAsyncThunk(asyncThunk.Invoke("arg"));

            yield return new WaitUntilOrTimeOut(
                () => store.GetState<MyState>("mySlice").value is FulfilledAction<string, string>,
                false,
                TimeSpan.FromSeconds(1));

            var fulfilledAction = store.GetState<MyState>("mySlice").value as FulfilledAction<string, string>;
            Assert.IsNotNull(fulfilledAction);
            Assert.AreEqual("premature result", fulfilledAction.payload);
        }

        record MyState
        {
            public object value { get; set; } = null;
        }
    }
}
