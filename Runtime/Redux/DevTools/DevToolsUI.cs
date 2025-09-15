using System;
using Unity.AppUI.Core;

namespace Unity.AppUI.Redux.DevTools
{
    class DevToolsUI : IDisposable
    {
        Reducer<DevToolsUIState> m_Reducer;

        IDisposableSubscription m_SelectedStoreChanged;

        IDisposableSubscription m_LiftedStoreSubscription;

        public IStore<DevToolsUIState> store { get; private set; }

        public DevToolsUI(DevToolsUIState initialState)
        {
            m_Reducer = new Reducer<DevToolsUIState>(Reduce);
            store = StoreFactory.CreateStore(m_Reducer, initialState,
                StoreFactory.ApplyMiddleware(Thunk.ThunkMiddleware<DevToolsUIState>()));
            m_SelectedStoreChanged = store.Subscribe(s => s.selectedStoreId, OnSelectedStoreChanged);
            DevToolsService.Instance.connectedStoresChanged += OnStoreListChanged;
        }

        void OnSelectedStoreChanged(string state)
        {
            m_LiftedStoreSubscription?.Dispose();

            if (string.IsNullOrEmpty(state))
                return;

            var liftedStore = DevToolsService.Instance.GetStoreById(state);
            if (liftedStore != null)
                m_LiftedStoreSubscription = liftedStore.Subscribe(s => s, OnLiftedStateChanged);
        }

        void OnLiftedStateChanged(LiftedState liftedState)
        {
            store.Dispatch(new SetLiftedStateAction(liftedState));
        }

        void OnStoreListChanged()
        {
            var connectedStores = DevToolsService.Instance.GetConnectedStores();
            var stores = new StoreData[connectedStores.Length];
            for (var i = 0; i < connectedStores.Length; i++)
            {
                stores[i] = new StoreData
                {
                    id = connectedStores[i].id,
                    name = connectedStores[i].displayName
                };
            }
            store.Dispatch(new SetStoreListAction(stores));
        }

        public void Sync()
        {
            OnStoreListChanged();
        }

        public void Dispatch(IAction action)
        {
            if (action is DevToolsUIAction or IAsyncThunkAction)
                store.Dispatch(action); // dispatch to UI store
            else if (DevToolsService.Instance.GetStoreById(store.GetState().selectedStoreId) is {} liftedStore)
                liftedStore.Dispatch(action); // dispatch to lifted store
        }

        public void Dispose()
        {
            m_LiftedStoreSubscription?.Dispose();
            m_LiftedStoreSubscription = null;
            m_SelectedStoreChanged.Dispose();
            m_SelectedStoreChanged = null;
            DevToolsService.Instance.connectedStoresChanged -= OnStoreListChanged;
            store.Dispose();
            store = null;
            m_Reducer = null;
        }

        static DevToolsUIState Reduce(DevToolsUIState state, IAction action)
        {
            if (state == null)
                throw new ArgumentNullException(nameof(state));

            switch (action)
            {
                case SetStoreListAction setStoreListAction:
                    return HandleSetStoreList(state, setStoreListAction);
                case SetLiftedStateAction setLiftedStateAction:
                    return HandleSetLiftedState(state, setLiftedStateAction);
                case SelectStoreAction selectStoreAction:
                    return HandleStoreSelection(state, selectStoreAction);
                case ToggleStoreInspectorAction:
                    return HandleToggleStoreInspector(state);
                case SelectActionForInspectionAction selectAction:
                    return HandleSelectActionForInspection(state, selectAction);
                case ClearActionSelectionAction:
                    return HandleClearActionSelection(state);
                case TogglePayloadVisibilityAction:
                    return HandleTogglePayloadVisibility(state);
                case ToggleStackTraceVisibilityAction:
                    return HandleToggleStackTraceVisibility(state);
                case SetActionSearchFilterAction setSearchFilterAction:
                    return HandleSetActionSearchFilter(state, setSearchFilterAction);
                case SelectInspectorTabAction selectInspectorTabAction:
                    return HandleSelectInspectorTab(state, selectInspectorTabAction);
                case ToggleAutoScrollAction:
                    return HandleToggleAutoScroll(state);
                case SetMaxActionsToShowAction setMaxActionsToShowAction:
                    return HandleSetMaxActionsToShow(state, setMaxActionsToShowAction);
                case ToggleShowSkippedActionsAction:
                    return HandleToggleShowSkippedActions(state);
                case ToggleShowTimestampsAction:
                    return HandleToggleShowTimestamps(state);
                case PausePlayingAction:
                    return state with { isPlaying = false };
            }

            return action.type switch
            {
                "startPlayingBack/pending" => state with {isPlaying = true},
                "startPlayingBack/fulfilled" => state with {isPlaying = false},
                "startPlayingBack/rejected" => state with {isPlaying = false},
                _ => state with { }
            };
        }

        static DevToolsUIState HandleSetStoreList(DevToolsUIState state, SetStoreListAction setStoreListAction)
        {
            var s = state with
            {
                stores = setStoreListAction.stores
            };

            // check if the selected store is still in the list
            var found = false;
            foreach (var store in setStoreListAction.stores)
            {
                if (store.id == state.selectedStoreId)
                {
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                var newStoreId = setStoreListAction.stores.Length > 0 ? setStoreListAction.stores[0].id : null;
                var newLiftedState = DevToolsService.Instance.GetStoreById(newStoreId)?.GetLiftedState();
                s = s with
                {
                    selectedStoreId = newStoreId,
                    liftedState = newLiftedState
                };
            }

            return s;
        }

        static DevToolsUIState HandleSetLiftedState(DevToolsUIState state, SetLiftedStateAction setLiftedStateAction)
        {
            return state with
            {
                liftedState = setLiftedStateAction.liftedState
            };
        }

        static DevToolsUIState HandleStoreSelection(DevToolsUIState state, SelectStoreAction selectStoreAction)
        {
            var s = state with
            {
                selectedStoreId = selectStoreAction.storeId
            };

            var liftedState = DevToolsService.Instance.GetStoreById(selectStoreAction.storeId)?.GetLiftedState();
            s = s with
            {
                liftedState = liftedState
            };

            return s;
        }

        static DevToolsUIState HandleToggleStoreInspector(DevToolsUIState state)
        {
            return state with
            {
                isInspectorVisible = !state.isInspectorVisible
            };
        }

        static DevToolsUIState HandleSelectActionForInspection(DevToolsUIState state, SelectActionForInspectionAction selectAction)
        {
            return state with
            {
                actionInspector = state.actionInspector with
                {
                    selectedActionId = selectAction.id
                }
            };
        }

        static DevToolsUIState HandleClearActionSelection(DevToolsUIState state)
        {
            return state with
            {
                actionInspector = state.actionInspector with
                {
                    selectedActionId = Optional<int>.none
                }
            };
        }

        static DevToolsUIState HandleTogglePayloadVisibility(DevToolsUIState state)
        {
            return state with
            {
                actionInspector = state.actionInspector with
                {
                    isPayloadVisible = !state.actionInspector.isPayloadVisible
                }
            };
        }

        static DevToolsUIState HandleToggleStackTraceVisibility(DevToolsUIState state)
        {
            return state with
            {
                actionInspector = state.actionInspector with
                {
                    isStackTraceVisible = !state.actionInspector.isStackTraceVisible
                }
            };
        }

        static DevToolsUIState HandleSetActionSearchFilter(DevToolsUIState state, SetActionSearchFilterAction setSearchFilterAction)
        {
            return state with
            {
                actionInspector = state.actionInspector with
                {
                    searchFilter = setSearchFilterAction.searchFilterText
                }
            };
        }

        static DevToolsUIState HandleSelectInspectorTab(DevToolsUIState state, SelectInspectorTabAction selectInspectorTabAction)
        {
            return state with
            {
                actionInspector = state.actionInspector with
                {
                    selectedTab = selectInspectorTabAction.tabName
                }
            };
        }

        static DevToolsUIState HandleToggleAutoScroll(DevToolsUIState state)
        {
            return state with
            {
                settings = state.settings with
                {
                    autoScrollToBottom = !state.settings.autoScrollToBottom
                }
            };
        }

        static DevToolsUIState HandleSetMaxActionsToShow(DevToolsUIState state, SetMaxActionsToShowAction setMaxActionsToShowAction)
        {
            return state with
            {
                settings = state.settings with
                {
                    maxActionsToShow = setMaxActionsToShowAction.maxAge
                }
            };
        }

        static DevToolsUIState HandleToggleShowSkippedActions(DevToolsUIState state)
        {
            return state with
            {
                settings = state.settings with
                {
                    showSkippedActions = !state.settings.showSkippedActions
                }
            };
        }

        static DevToolsUIState HandleToggleShowTimestamps(DevToolsUIState state)
        {
            return state with
            {
                settings = state.settings with
                {
                    showTimestamps = !state.settings.showTimestamps
                }
            };
        }
    }
}
