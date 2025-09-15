using System;
using System.Collections.Generic;

namespace Unity.AppUI.Redux.DevTools
{
    /// <summary>
    /// The computed state of the monitored store.
    /// </summary>
    [Serializable]
    public record ComputedState
    {
        /// <summary>
        /// The computed state of the store.
        /// </summary>
        public object state { get; set; }

        /// <summary>
        /// The exception that occurred while computing the state (if any).
        /// </summary>
        public Exception exception { get; set; }
    }

    /// <summary>
    /// Represents a state wrapper used by Redux DevTools for debugging and time-travel functionality.
    /// This state maintains the action history, computed states, and configuration settings needed
    /// for development tools to function.
    /// </summary>
    /// <remarks>
    /// The lifted state tracks:
    /// - Action history with unique IDs
    /// - Computed states at each action
    /// - Staging and skipping of actions
    /// - Time-travel position via currentStateIndex
    /// - DevTools configuration state (locked/paused)
    /// </remarks>
    [Serializable]
    public record LiftedState
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LiftedState"/> class.
        /// </summary>
        public LiftedState() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="LiftedState"/> class with initial state and configuration.
        /// </summary>
        /// <param name="initialCommitState">The initial application state to commit.</param>
        /// <param name="options">Configuration options controlling DevTools behavior.</param>
        /// <remarks>
        /// Sets up initial state with:
        /// - Action ID counter starting at 1
        /// - Initial action (type: @@INIT) at index 0
        /// - Empty skipped actions set
        /// - DevTools configuration from options
        /// </remarks>
        public LiftedState(object initialCommitState, DevToolsConfiguration options)
        {
            nextActionId = 1;
            actionsById = new Dictionary<int, LiftedAction> { [0] = new PerformAction(new InitAction()) };
            stagedActionIds = new List<int> {0};
            skippedActionIds = new HashSet<int>();
            committedState = initialCommitState;
            currentStateIndex = -1;
            computedStates = new List<ComputedState>();
            isLocked = options.shouldStartLocked;
            isPaused = !options.shouldRecordChanges;
        }

        /// <summary>
        /// Gets or sets the incrementing identifier for the next action.
        /// Used to assign unique IDs to new actions as they are dispatched.
        /// </summary>
        public int nextActionId;

        /// <summary>
        /// Gets or sets the mapping of action IDs to their corresponding lifted actions.
        /// Index 0 always contains the initialization action.
        /// </summary>
        public Dictionary<int, LiftedAction> actionsById;

        /// <summary>
        /// Gets or sets the ordered list of action IDs representing the current action sequence.
        /// Used for maintaining action order and time-travel capabilities.
        /// </summary>
        public List<int> stagedActionIds;

        /// <summary>
        /// Gets or sets the set of action IDs that are currently disabled/skipped.
        /// Skipped actions are temporarily removed from state computation.
        /// </summary>
        public HashSet<int> skippedActionIds;

        /// <summary>
        /// Gets or sets the last confirmed application state.
        /// This represents the baseline state for computing future states.
        /// </summary>
        public object committedState;

        /// <summary>
        /// Gets or sets the index of the currently selected state in the time-travel sequence.
        /// -1 represents the initial state before any actions.
        /// </summary>
        public int currentStateIndex;

        /// <summary>
        /// Gets or sets the list of computed states resulting from action application.
        /// Each state represents the application state after applying actions up to that index.
        /// </summary>
        public List<ComputedState> computedStates;

        /// <summary>
        /// Gets or sets whether the DevTools are locked, preventing state changes.
        /// When true, dispatched actions are blocked from modifying state.
        /// </summary>
        public bool isLocked;

        /// <summary>
        /// Gets or sets whether action recording is paused.
        /// When true, new actions are not added to the action history.
        /// </summary>
        public bool isPaused;
    }
}
