using System;

namespace Unity.AppUI.Redux.DevTools
{
    /// <summary>
    /// Available actions for the DevTools.
    /// </summary>
    /// <param name="type"> The type of the action. </param>
    public record DevToolsAction(string type) : IAction
    {
        /// <summary>
        /// Main initialization action type.
        /// </summary>
        internal const string initActionType = "@@INIT";

        /// <summary>
        /// Performs an action. This is the default lift action.
        /// </summary>
        internal const string performActionType = "PERFORM_ACTION";

        /// <summary>
        /// Resets the state to the initial state.
        /// </summary>
        internal const string resetActionType = "RESET";

        /// <summary>
        /// Rolls back to the state at the time of the last commit.
        /// </summary>
        internal const string rollbackActionType = "ROLLBACK";

        /// <summary>
        /// Clears the action history, but preserves the current state.
        /// </summary>
        internal const string commitActionType = "COMMIT";

        /// <summary>
        /// Sweeps the actions that have been performed.
        /// </summary>
        internal const string sweepActionType = "SWEEP";

        /// <summary>
        /// Toggles on/off the inclusion of an action in the state computation.
        /// </summary>
        internal const string toggleActionType = "TOGGLE_ACTION";

        /// <summary>
        /// Moves the current State Index to a specified index.
        /// </summary>
        internal const string jumpToStateActionType = "JUMP_TO_STATE";

        /// <summary>
        /// Moves the current Action Index to a specified index.
        /// </summary>
        internal const string jumpToActionActionType = "JUMP_TO_ACTION";

        /// <summary>
        /// Imports the state from a serialized state.
        /// </summary>
        internal const string importStateActionType = "IMPORT_STATE";

        /// <summary>
        /// Locks the changes to the state.
        /// </summary>
        internal const string lockChangesActionType = "LOCK_CHANGES";

        /// <summary>
        /// Pauses the recording of actions.
        /// </summary>
        internal const string pauseRecordingActionType = "PAUSE_RECORDING";

        /// <summary>
        /// Pause mark for action history.
        /// </summary>
        internal const string pausedActionType = "@@PAUSED";

        /// <inheritdoc />
        public string type { get; } = type;

        /// <summary>
        /// Whether the action is a global Redux init or replace action.
        /// </summary>
        /// <param name="action"> The action to check. </param>
        /// <returns> True if the action is a global Redux init or replace action. </returns>
        internal static bool IsGlobalReduxInitOrReplace(IAction action)
        {
            return action.type is "@@redux/INIT" or "@@redux/REPLACE";
        }
    }

    /// <summary>
    /// A lifted action is an action that is wrapped in another action. The
    /// lifted reducer used in the DevTools <see cref="IInstrumentedStore{TState}"/> will
    /// unwrap the action and perform it.
    /// </summary>
    /// <param name="type"> The type of the action. </param>
    /// <param name="action"> The action to perform. </param>
    public record LiftedAction(string type, IAction action) : DevToolsAction(type)
    {
        /// <summary>
        /// The timestamp of the action creation.
        /// </summary>
        public long timestamp { get; } = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        /// <summary>
        /// The action to perform.
        /// </summary>
        public IAction action { get; } = action;

        /// <summary>
        /// Returns a lifted action from the specified action.
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        internal static IAction From(IAction action)
        {
            if (action is LiftedAction)
                throw new InvalidOperationException("Cannot lift an action that is already lifted.");
            if (action is DevToolsAction || IsGlobalReduxInitOrReplace(action))
                return action;
            return new PerformAction(action);
        }
    }

    /// <summary>
    /// Performs an action.
    /// </summary>
    /// <param name="action"></param>
    record PerformAction(IAction action) : LiftedAction(performActionType, action) { }

    /// <summary>
    /// Initializes the DevTools.
    /// </summary>
    record InitAction() : DevToolsAction(initActionType) { }

    /// <summary>
    /// Resets the state to the initial state.
    /// </summary>
    record ResetAction() : DevToolsAction(resetActionType) { }

    /// <summary>
    /// Rolls back to the state at the time of the last commit.
    /// </summary>
    record RollbackAction() : DevToolsAction(rollbackActionType) { }

    /// <summary>
    /// Clears the action history, but preserves the current state.
    /// </summary>
    record CommitAction() : DevToolsAction(commitActionType) { }

    /// <summary>
    /// Sweeps the actions that have been performed.
    /// Sweeping removes the actions that are not included in the state computation.
    /// </summary>
    record SweepAction() : DevToolsAction(sweepActionType) { }

    /// <summary>
    /// Toggles on/off the inclusion of an action in the state computation.
    /// </summary>
    /// <param name="start"> The start index of the action. </param>
    /// <param name="end"> The end index of the action. </param>
    /// <param name="active"> The status of the action. </param>
    record ToggleAction(int start, int end, bool active) : DevToolsAction(toggleActionType)
    {
        /// <summary>
        /// The start index of the action.
        /// </summary>
        public int start { get; } = start;

        /// <summary>
        /// The end index of the action.
        /// </summary>
        public int end { get; } = end;

        /// <summary>
        /// The status of the action.
        /// </summary>
        public bool active { get; } = active;
    }

    /// <summary>
    /// Moves the current State Index to a specified index.
    /// </summary>
    /// <param name="index"> The index to jump to. </param>
    record JumpToStateAction(int index) : DevToolsAction(jumpToStateActionType)
    {
        /// <summary>
        /// The index to jump to.
        /// </summary>
        public int index { get; } = index;
    }

    /// <summary>
    /// Moves the current Action Index to a specified index.
    /// </summary>
    /// <param name="actionId"> The id of the action to jump to. </param>
    record JumpToActionAction(int actionId) : DevToolsAction(jumpToActionActionType)
    {
        /// <summary>
        /// The id of the action to jump to.
        /// </summary>
        public int actionId { get; } = actionId;
    }

    /// <summary>
    /// Imports the state from a serialized state.
    /// </summary>
    record ImportStateAction() : DevToolsAction(importStateActionType) { }

    /// <summary>
    /// Locks the changes to the state.
    /// </summary>
    /// <param name="status"> The status of the lock. </param>
    record LockChangesAction(bool status = false) : DevToolsAction(lockChangesActionType)
    {
        /// <summary>
        /// The status of the lock.
        /// </summary>
        public bool status { get; } = status;
    }

    /// <summary>
    /// Pauses the recording of actions.
    /// </summary>
    /// <param name="status"> The status of the pause. </param>
    record PauseRecordingAction(bool status = false) : DevToolsAction(pauseRecordingActionType)
    {
        /// <summary>
        /// The status of the pause.
        /// </summary>
        public bool status { get; } = status;
    }

    /// <summary>
    /// Pause mark for action history.
    /// </summary>
    record PausedAction() : DevToolsAction(pausedActionType) { }
}
