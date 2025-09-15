using System;
using System.Threading.Tasks;
using Unity.AppUI.Core;
using UnityEngine;

namespace Unity.AppUI.Redux.DevTools
{
    /// <summary>
    /// Base class for all DevTools UI-specific actions.
    /// </summary>
    class DevToolsUIAction : IAction
    {
        /// <summary>
        /// The name of the slice used by DevTools User interface.
        /// </summary>
        internal const string sliceName = "devTools";

        /// <summary>
        /// The type of the action.
        /// </summary>
        public string type { get; }

        /// <summary>
        /// Creates a new instance of the action.
        /// </summary>
        protected DevToolsUIAction()
        {
            type = MemoryUtils.Concatenate(sliceName, "/", GetType().Name);
        }

        /// <summary>
        /// Async thunk creator to start playing back actions in the DevTools UI.
        /// </summary>
        internal static readonly AsyncThunkCreator<DevToolsUI,bool> startPlayingBack =
            new (nameof(startPlayingBack), async api =>
            {
                var devToolsUI = api.arg;
                while (devToolsUI.store.GetState() is
                       {
                           isPlaying: true,
                           settings: { playbackSpeed: > 0 },
                       })
                {
                    var liftedState = devToolsUI.store.GetState().liftedState;
                    if (liftedState == null)
                        break;

                    var index = liftedState.currentStateIndex;
                    if (index < 0 || index >= liftedState.computedStates.Count - 1)
                        break;

                    await Task.Delay(devToolsUI.store.GetState().settings.playbackSpeed);
                    index = Mathf.Min(index + 1, liftedState.computedStates.Count - 1);
                    devToolsUI.Dispatch(new JumpToStateAction(index));
                }
                return true;
            });
    }

    /// <summary>
    /// Action to set the available list of stores for inspection in the DevTools UI.
    /// </summary>
    class SetStoreListAction : DevToolsUIAction
    {
        /// <summary>
        /// The list of stores to set.
        /// </summary>
        public StoreData[] stores { get; }

        /// <summary>
        /// Creates a new instance of the action.
        /// </summary>
        /// <param name="stores"> The list of stores to set. </param>
        public SetStoreListAction(StoreData[] stores)
        {
            this.stores = stores;
        }
    }

    /// <summary>
    /// Action to set the lifted state for the DevTools UI.
    /// </summary>
    class SetLiftedStateAction : DevToolsUIAction
    {
        /// <summary>
        /// The lifted state to set.
        /// </summary>
        public LiftedState liftedState { get; }

        /// <summary>
        /// Creates a new instance of the action.
        /// </summary>
        /// <param name="liftedState"> The lifted state to set. </param>
        public SetLiftedStateAction(LiftedState liftedState)
        {
            this.liftedState = liftedState;
        }
    }

    /// <summary>
    /// Action to select a store for inspection in the DevTools UI.
    /// </summary>
    /// <remarks>
    /// When dispatched:
    /// - Updates the selected store ID in the UI state
    /// - Triggers a re-render of store-specific panels
    /// - Maintains the current inspector visibility state
    /// </remarks>
    class SelectStoreAction : DevToolsUIAction
    {
        /// <summary>
        /// The ID of the store to select.
        /// </summary>
        public string storeId { get; }

        /// <summary>
        /// Creates a new instance of the action.
        /// </summary>
        /// <param name="storeId"> The ID of the store to select. </param>
        public SelectStoreAction(string storeId)
        {
            this.storeId = storeId;
        }
    }

    /// <summary>
    /// Select a tab in the inspector panel.
    /// </summary>
    class SelectInspectorTabAction : DevToolsUIAction
    {
        /// <summary>
        /// The name of the tab to select.
        /// </summary>
        public string tabName { get; }

        /// <summary>
        /// Creates a new instance of the action.
        /// </summary>
        /// <param name="tabName"> The name of the tab to select. </param>
        public SelectInspectorTabAction(string tabName)
        {
            this.tabName = tabName;
        }
    }

    /// <summary>
    /// Pause the playing of actions in the DevTools UI.
    /// </summary>
    class PausePlayingAction : DevToolsUIAction { }

    /// <summary>
    /// Action to toggle the visibility of the store inspector panel.
    /// </summary>
    /// <remarks>
    /// When dispatched:<br/>
    /// - Toggles the visibility state of the store inspector<br/>
    /// - Does not affect the currently selected store<br/>
    /// - Can be used when no store is selected<br/>
    /// </remarks>
    class ToggleStoreInspectorAction : DevToolsUIAction { }

    // Action inspector actions
    /// <summary>
    /// Action to select an action for detailed inspection in the action inspector panel.
    /// </summary>
    /// <remarks>
    /// When dispatched:
    /// - Updates the selected action ID in the inspector
    /// - Maintains current payload and stack trace visibility settings
    /// - Triggers a re-render of the action details panel
    /// </remarks>
    class SelectActionForInspectionAction : DevToolsUIAction
    {
        /// <summary>
        /// The ID of the action to inspect.
        /// </summary>
        public int id { get; }

        public SelectActionForInspectionAction(int id)
        {
            this.id = id;
        }
    }

    /// <summary>
    /// Action to clear the currently selected action in the inspector.
    /// </summary>
    /// <remarks>
    /// When dispatched:
    /// - Clears the selected action ID
    /// - Maintains inspector panel visibility settings
    /// - Typically used when switching stores or clearing the inspection state
    /// </remarks>
    class ClearActionSelectionAction : DevToolsUIAction { }

    /// <summary>
    /// Action to toggle the visibility of the action payload section in the inspector.
    /// </summary>
    /// <remarks>
    /// When dispatched:
    /// - Toggles the visibility of the action payload details
    /// - Maintains the current action selection
    /// - Has no effect if no action is selected
    /// </remarks>
    class TogglePayloadVisibilityAction : DevToolsUIAction { }

    /// <summary>
    /// Action to toggle the visibility of the action stack trace in the inspector.
    /// </summary>
    /// <remarks>
    /// When dispatched:
    /// - Toggles the visibility of the stack trace information
    /// - Maintains the current action selection
    /// - Has no effect if no action is selected or if stack trace is not available
    /// </remarks>
    class ToggleStackTraceVisibilityAction : DevToolsUIAction { }

    /// <summary>
    /// Action to update the search/filter text for the action list.
    /// </summary>
    /// <remarks>
    /// When dispatched:
    /// - Updates the search filter text
    /// - Triggers re-filtering of the action list
    /// - Can affect the visibility of actions in the list based on the filter
    /// - Empty or null filter shows all actions
    /// </remarks>
    class SetActionSearchFilterAction : DevToolsUIAction
    {
        /// <summary>
        /// The filter text to apply to the action list.
        /// Can be null or empty to show all actions.
        /// </summary>
        public string searchFilterText { get; }

        /// <summary>
        /// Creates a new instance of the action.
        /// </summary>
        /// <param name="searchFilterText"> The filter text to apply to the action list. </param>
        public SetActionSearchFilterAction(string searchFilterText)
        {
            this.searchFilterText = searchFilterText;
        }
    }

    /// <summary>
    /// Action to toggle automatic scrolling behavior in the action list.
    /// </summary>
    /// <remarks>
    /// When dispatched:
    /// - Toggles the auto-scroll setting
    /// - When enabled, the action list automatically scrolls to show new actions
    /// - When disabled, the list maintains its current scroll position
    /// </remarks>
    class ToggleAutoScrollAction : DevToolsUIAction { }

    /// <summary>
    /// Action to set the maximum number of actions to display in the action list.
    /// </summary>
    /// <remarks>
    /// When dispatched:
    /// - Updates the maximum actions limit
    /// - May trigger cleanup of older actions from the display
    /// - Does not affect the actual action history, only the UI display
    /// </remarks>
    class SetMaxActionsToShowAction : DevToolsUIAction
    {
        /// <summary>
        /// The maximum number of actions to show in the list.
        /// Must be greater than 0.
        /// </summary>
        public int maxAge { get; }

        /// <summary>
        /// Creates a new instance of the action.
        /// </summary>
        /// <param name="maxAge"> The maximum number of actions to show in the list. </param>
        /// <exception cref="ArgumentException"> Thrown if <paramref name="maxAge"/> is less than or equal to 0. </exception>
        public SetMaxActionsToShowAction(int maxAge)
        {
            if (maxAge <= 0)
                throw new ArgumentException("MaxActions must be greater than 0", nameof(maxAge));

            this.maxAge = maxAge;
        }
    }

    /// <summary>
    /// Action to toggle the visibility of skipped actions in the action list.
    /// </summary>
    /// <remarks>
    /// When dispatched:
    /// - Toggles whether skipped actions are shown in the list
    /// - Affects the action list filtering
    /// - May trigger a re-render of the action list
    /// - Does not affect the actual skipped state of actions
    /// </remarks>
    class ToggleShowSkippedActionsAction : DevToolsUIAction { }

    /// <summary>
    /// Action to toggle the display of timestamps in the action list.
    /// </summary>
    /// <remarks>
    /// When dispatched:
    /// - Toggles the visibility of action timestamps
    /// - Affects all displayed actions
    /// - May trigger a re-render of the action list to update the display format
    /// </remarks>
    class ToggleShowTimestampsAction : DevToolsUIAction { }
}
