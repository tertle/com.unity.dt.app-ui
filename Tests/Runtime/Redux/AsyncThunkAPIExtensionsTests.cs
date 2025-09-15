using System;
using System.Threading.Tasks;
using NUnit.Framework;
using Unity.AppUI.Redux;

namespace Unity.AppUI.Tests.Redux
{
    [TestFixture]
    [TestOf(typeof(AsyncThunkAPIExtensions))]
    class AsyncThunkAPIExtensionsTests
    {
        const string dummySliceName = "dummySlice";

        record DummyState
        {
            public IAction action;
        }

        static readonly ActionCreator k_DummyAction = dummySliceName + "/action";
        static readonly ActionCreator<int> k_DummyActionWithPayload = dummySliceName + "/actionWithPayload";

        static DummyState DummyActionReducer(DummyState state, IAction action)
        {
            return state with { action = action };
        }

        static DummyState DummyActionWithPayloadReducer(DummyState state, IAction<int> action)
        {
            return state with { action = action };
        }

        static readonly AsyncThunkCreator<bool,int> k_DummyAsyncThunk = new ("dummyAsyncThunk", async _ => await Task.FromResult(42));

        [Test]
        public void CanDispatchAction()
        {
            var store = CreateStore();
            var action = k_DummyAsyncThunk.Invoke(true);
            Assert.Throws<ArgumentNullException>(() => AsyncThunkAPIExtensions.Dispatch((ThunkAPI<bool,int>)null, (IAction)null));
            Assert.Throws<ArgumentNullException>(() => AsyncThunkAPIExtensions.Dispatch(new ThunkAPI<bool,int>(store,action), (IAction)null));

            var thunkApi = new ThunkAPI<bool,int>(store,action);
            Assert.DoesNotThrow(() => AsyncThunkAPIExtensions.Dispatch(thunkApi, k_DummyAction.Invoke()));
        }

        [Test]
        public void CanDispatchActionWithPayload()
        {
            var store = CreateStore();
            var action = k_DummyAsyncThunk.Invoke(true);
            Assert.Throws<ArgumentNullException>(() => AsyncThunkAPIExtensions.Dispatch((ThunkAPI<bool,int>)null, (IAction<int>)null));
            Assert.Throws<ArgumentNullException>(() => AsyncThunkAPIExtensions.Dispatch(new ThunkAPI<bool,int>(store,action), (IAction<int>)null));
            var thunkApi = new ThunkAPI<bool,int>(store,action);
            Assert.DoesNotThrow(() => AsyncThunkAPIExtensions.Dispatch(thunkApi, k_DummyActionWithPayload.Invoke(99)));
        }

        [Test]
        public void CanDispatchActionCreator()
        {
            var store = CreateStore();
            var action = k_DummyAsyncThunk.Invoke(true);
            Assert.Throws<ArgumentNullException>(() => AsyncThunkAPIExtensions.Dispatch((ThunkAPI<bool,int>)null, (ActionCreator)null));
            Assert.Throws<ArgumentNullException>(() => AsyncThunkAPIExtensions.Dispatch(new ThunkAPI<bool,int>(store,action), (ActionCreator)null));

            var thunkApi = new ThunkAPI<bool,int>(store,action);
            Assert.DoesNotThrow(() => AsyncThunkAPIExtensions.Dispatch(thunkApi, k_DummyAction));
        }

        [Test]
        public void CanDispatchActionCreatorWithPayload()
        {
            var store = CreateStore();
            var action = k_DummyAsyncThunk.Invoke(true);
            Assert.Throws<ArgumentNullException>(() => AsyncThunkAPIExtensions.Dispatch((ThunkAPI<bool,int>)null, (ActionCreator<int>)null, 99));
            Assert.Throws<ArgumentNullException>(() => AsyncThunkAPIExtensions.Dispatch(new ThunkAPI<bool,int>(store,action), (ActionCreator<int>)null, 99));
            var thunkApi = new ThunkAPI<bool,int>(store,action);
            Assert.DoesNotThrow(() => AsyncThunkAPIExtensions.Dispatch(thunkApi, k_DummyActionWithPayload, 99));
        }

        [Test]
        public void CanDispatchActionType()
        {
            var store = CreateStore();
            var action = k_DummyAsyncThunk.Invoke(true);
            Assert.Throws<ArgumentNullException>(() => AsyncThunkAPIExtensions.Dispatch((ThunkAPI<bool,int>)null, k_DummyAction.type));
            Assert.Throws<ArgumentNullException>(() => AsyncThunkAPIExtensions.Dispatch(new ThunkAPI<bool,int>(store,action), (string)null));
            var thunkApi = new ThunkAPI<bool,int>(store,action);
            Assert.DoesNotThrow(() => AsyncThunkAPIExtensions.Dispatch(thunkApi, k_DummyAction.type));
        }

        [Test]
        public void CanDispatchActionTypeWithPayload()
        {
            var store = CreateStore();
            var action = k_DummyAsyncThunk.Invoke(true);
            Assert.Throws<ArgumentNullException>(() => AsyncThunkAPIExtensions.Dispatch((ThunkAPI<bool,int>)null, k_DummyActionWithPayload.type, 99));
            Assert.Throws<ArgumentNullException>(() => AsyncThunkAPIExtensions.Dispatch(new ThunkAPI<bool,int>(store,action), (string)null, 99));
            var thunkApi = new ThunkAPI<bool,int>(store,action);
            Assert.DoesNotThrow(() => AsyncThunkAPIExtensions.Dispatch(thunkApi, k_DummyActionWithPayload.type, 99));
        }

        static IStore<PartitionedState> CreateStore()
        {
            return StoreFactory.CreateStore(new[]
            {
                StoreFactory.CreateSlice("dummySlice", new DummyState(), reducers =>
                {
                    reducers.AddCase(k_DummyAction, DummyActionReducer);
                    reducers.AddCase(k_DummyActionWithPayload, DummyActionWithPayloadReducer);
                }, extraReducers =>
                {
                    extraReducers.AddCase(k_DummyAsyncThunk.pending, (state, action) => state);
                    extraReducers.AddCase(k_DummyAsyncThunk.fulfilled, (state, action) => state);
                    extraReducers.AddCase(k_DummyAsyncThunk.rejected, (state, action) => state);

                    extraReducers.AddCase(_ => false, DummyActionReducer);
                    extraReducers.AddCase<int>(_ => false, DummyActionWithPayloadReducer);

                    extraReducers.AddDefault((state, action) => state);
                })
            });
        }
    }
}
