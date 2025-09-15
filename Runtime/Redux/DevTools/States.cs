using System;
using Unity.AppUI.Core;

namespace Unity.AppUI.Redux.DevTools
{
    /// <summary>
    /// The state of the DevTools UI. Each DevTools window has its own state.
    /// </summary>
    [Serializable]
    public record DevToolsUIState
    {
        /// <summary>
        /// Whether the DevTools UI is currently playing back actions.
        /// </summary>
        public bool isPlaying;

        /// <summary>
        /// The list of stores available for inspection.
        /// </summary>
        public StoreData[] stores;

        /// <summary>
        /// The ID of the selected store.
        /// </summary>
        public string selectedStoreId;

        /// <summary>
        /// The lifted state of the selected store.
        /// </summary>
        public LiftedState liftedState;

        /// <summary>
        /// Whether the DevTools inspector is visible.
        /// </summary>
        public bool isInspectorVisible;

        /// <summary>
        /// The data of the selected action.
        /// </summary>
        public ActionInspector actionInspector = new ();

        /// <summary>
        /// The settings of the DevTools UI.
        /// </summary>
        public UISettings settings = new ();
    }

    /// <summary>
    /// The state of an available store.
    /// </summary>
    [Serializable]
    public record StoreData
    {
        /// <summary>
        /// The ID of the store.
        /// </summary>
        public string id;

        /// <summary>
        /// The name of the store.
        /// </summary>
        public string name;
    }

    /// <summary>
    /// The state of the selected action for inspection.
    /// </summary>
    [Serializable]
    public record ActionInspector
    {
        /// <summary>
        /// The ID of the selected action.
        /// </summary>
        public Optional<int> selectedActionId;

        /// <summary>
        /// Whether the action payload is visible in the inspector.
        /// </summary>
        public bool isPayloadVisible;

        /// <summary>
        /// Whether the action stack trace is visible.
        /// </summary>
        public bool isStackTraceVisible;

        /// <summary>
        /// The search filter for the action list.
        /// </summary>
        public string searchFilter;

        /// <summary>
        /// The selected tab in the action inspector.
        /// </summary>
        public string selectedTab = "Action";
    }

    /// <summary>
    /// Miscellaneous settings for the DevTools UI.
    /// </summary>
    [Serializable]
    public record UISettings
    {
        /// <summary>
        /// Whether the action list should auto-scroll to the bottom.
        /// </summary>
        public bool autoScrollToBottom;

        /// <summary>
        /// THe maximum number of actions to show in the action list.
        /// </summary>
        public int maxActionsToShow = 100;

        /// <summary>
        /// Whether to show skipped actions in the action list.
        /// </summary>
        public bool showSkippedActions = true;

        /// <summary>
        /// Whether to show timestamps in the action list.
        /// </summary>
        public bool showTimestamps = true;

        /// <summary>
        /// The playback speed of the DevTools UI.
        /// </summary>
        public int playbackSpeed = 500;
    }
}
