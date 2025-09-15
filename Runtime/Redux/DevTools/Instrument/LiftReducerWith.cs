using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using UnityEngine;

namespace Unity.AppUI.Redux.DevTools
{
    public static partial class Instrument<TState>
    {
        internal static Reducer<LiftedState> LiftReducerWith(
            Reducer<TState> reducer,
            TState initialCommittedState,
            DevToolsConfiguration options)
        {
            var initialLiftedState = new LiftedState(initialCommittedState, options);

            return (liftedState, iAction) =>
            {
                liftedState ??= initialLiftedState;

                // decompose the state
                var nextActionId = liftedState.nextActionId;
                var committedState = liftedState.committedState;
                var actionsById = new Dictionary<int, LiftedAction>(liftedState.actionsById);
                var stagedActionIds = new List<int>(liftedState.stagedActionIds);
                var skippedActionIds = new HashSet<int>(liftedState.skippedActionIds);
                var computedStates = new List<ComputedState>(liftedState.computedStates);
                var currentStateIndex = liftedState.currentStateIndex;
                var isLocked = liftedState.isLocked;
                var isPaused = liftedState.isPaused;

                // keep track of changes
                var minInvalidatedStateIndex = 0;

                if (DevToolsAction.IsGlobalReduxInitOrReplace(iAction))
                {
                    // re-initialize the state
                    actionsById = new Dictionary<int, LiftedAction> { [0] = new PerformAction(new InitAction()) };
                    nextActionId = 1;
                    stagedActionIds = new List<int> {0};
                    skippedActionIds = new HashSet<int>();
                    committedState = computedStates.Count > 0 ? computedStates[currentStateIndex].state : initialCommittedState;
                    currentStateIndex = 0;
                    computedStates = new List<ComputedState>();

                    minInvalidatedStateIndex = 0;

                    if (options.maxAge > 0 && stagedActionIds.Count > options.maxAge)
                    {
                        computedStates = RecomputeStates(
                            computedStates,
                            minInvalidatedStateIndex,
                            reducer,
                            committedState,
                            actionsById,
                            stagedActionIds,
                            skippedActionIds,
                            options.shouldCatchExceptions);

                        CommitExcessActions(stagedActionIds.Count - options.maxAge);

                        minInvalidatedStateIndex = int.MaxValue;
                    }
                }
                else
                {
                    switch (iAction)
                    {
                        case PerformAction liftedAction:
                        {
                            // Performs the action and adds it to the history.
                            if (isLocked)
                                return liftedState;
                            if (isPaused)
                                return ComputePausedAction();

                            if (options.maxAge >= 0 && stagedActionIds.Count >= options.maxAge)
                                CommitExcessActions(stagedActionIds.Count - options.maxAge + 1);

                            if (currentStateIndex == stagedActionIds.Count -1)
                                currentStateIndex++;

                            var actionId = nextActionId++;
                            actionsById = new Dictionary<int, LiftedAction>(actionsById) {[actionId] = liftedAction };
                            stagedActionIds = new List<int>(stagedActionIds) {actionId};
                            minInvalidatedStateIndex = stagedActionIds.Count - 1;
                            break;
                        }
                        case ResetAction:
                        {
                            // Resets the state to the initial state.
                            actionsById = new Dictionary<int, LiftedAction> { [0] = new PerformAction(new InitAction()) };
                            nextActionId = 1;
                            stagedActionIds = new List<int> {0};
                            skippedActionIds = new HashSet<int>();
                            committedState = initialCommittedState;
                            currentStateIndex = 0;
                            computedStates = new List<ComputedState>();
                            break;
                        }
                        case RollbackAction:
                        {
                            // Rolls back to the state at the time of the last commit.
                            // dont change committedState here
                            actionsById = new Dictionary<int, LiftedAction> { [0] = new PerformAction(new InitAction()) };
                            nextActionId = 1;
                            stagedActionIds = new List<int> {0};
                            skippedActionIds = new HashSet<int>();
                            currentStateIndex = 0;
                            computedStates = new List<ComputedState>();
                            break;
                        }
                        case CommitAction:
                        {
                            // Clears the action history, but preserves the current state.
                            committedState = computedStates[currentStateIndex].state;
                            actionsById = new Dictionary<int, LiftedAction> { [0] = new PerformAction(new InitAction()) };
                            nextActionId = 1;
                            stagedActionIds = new List<int> { 0 };
                            skippedActionIds = new HashSet<int>();
                            currentStateIndex = 0;
                            computedStates = new List<ComputedState>();
                            break;
                        }
                        case ToggleAction toggleAction:
                        {
                            // Toggles on/off the inclusion of an action in the state computation.
                            Span<int> actionIds = stackalloc int[toggleAction.end - toggleAction.start + 1];
                            for (var i = 0; i < actionIds.Length; i++)
                                actionIds[i] = toggleAction.start + i;

                            if (toggleAction.active)
                            {
                                skippedActionIds = new HashSet<int>(skippedActionIds);
                                foreach (var id in actionIds)
                                    skippedActionIds.Remove(id);
                            }
                            else
                            {
                                skippedActionIds = new HashSet<int>(skippedActionIds);
                                foreach (var id in actionIds)
                                    skippedActionIds.Add(id);
                            }
                            minInvalidatedStateIndex = stagedActionIds.IndexOf(toggleAction.start);
                            break;
                        }
                        case SweepAction:
                        {
                            // Forget any actions that are currently being skipped.
                            var skippedActionIdsSet = new HashSet<int>(skippedActionIds);
                            stagedActionIds = stagedActionIds.FindAll(id => !skippedActionIdsSet.Contains(id));
                            skippedActionIds = new HashSet<int>();
                            currentStateIndex = Mathf.Min(currentStateIndex, stagedActionIds.Count - 1);
                            break;
                        }
                        case JumpToStateAction jumpToStateAction:
                        {
                            // Moves the current State Index to a specified index.
                            currentStateIndex = jumpToStateAction.index;
                            minInvalidatedStateIndex = int.MaxValue;
                            break;
                        }
                        case JumpToActionAction jumpToAction:
                        {
                            // Jumps to a corresponding state to a specific action.
                            // Useful when filtering actions.
                            var idx = stagedActionIds.IndexOf(jumpToAction.actionId);
                            if (idx != -1)
                                currentStateIndex = idx;
                            minInvalidatedStateIndex = int.MaxValue;
                            break;
                        }
                        case ImportStateAction:
                        {
                            break;
                        }
                        case LockChangesAction lockChangesAction:
                        {
                            // Locks or unlocks the state.
                            isLocked = lockChangesAction.status;
                            break;
                        }
                        case PauseRecordingAction pauseRecordingAction:
                        {
                            // Pauses or resumes the recording of actions.
                            isPaused = pauseRecordingAction.status;
                            if (isPaused)
                                return ComputePausedAction(true);
                            // Commit when unpause
                            actionsById = new Dictionary<int, LiftedAction> { [0] = new PerformAction(new InitAction()) };
                            nextActionId = 1;
                            stagedActionIds = new List<int> {0};
                            skippedActionIds = new HashSet<int>();
                            committedState = computedStates.Count > 0 ? computedStates[currentStateIndex].state : initialCommittedState;
                            currentStateIndex = 0;
                            computedStates = new List<ComputedState>();
                            break;
                        }
                        default:
                        {
                            minInvalidatedStateIndex = int.MaxValue;
                            break;
                        }
                    }
                }

                var recomputedStates = RecomputeStates(
                    computedStates,
                    minInvalidatedStateIndex,
                    reducer,
                    committedState,
                    actionsById,
                    stagedActionIds,
                    skippedActionIds,
                    options.shouldCatchExceptions);

                return new LiftedState
                {
                    nextActionId = nextActionId,
                    actionsById = actionsById,
                    stagedActionIds = stagedActionIds,
                    skippedActionIds = skippedActionIds,
                    committedState = committedState,
                    currentStateIndex = currentStateIndex,
                    computedStates = recomputedStates,
                    isLocked = isLocked,
                    isPaused = isPaused
                };

                void CommitExcessActions(int n)
                {
                    // Auto-commits n-number of excess actions.
                    var excess = n;
                    if (stagedActionIds.Count <= 1)
                        return;

                    var idsToDelete = stagedActionIds.GetRange(1, Mathf.Max(0, excess - 1));
                    for (var i = 0; i < idsToDelete.Count; i++)
                    {
                        if (computedStates[i + 1].exception != null)
                        {
                            excess = i;
                            idsToDelete = stagedActionIds.GetRange(1, Mathf.Max(0, excess - 1));
                            break;
                        }

                        actionsById = new Dictionary<int, LiftedAction>(actionsById);
                        actionsById.Remove(idsToDelete[i]);
                    }

                    var newSkippedActionIds = new HashSet<int>();
                    foreach (var id in skippedActionIds)
                    {
                        if (!idsToDelete.Contains(id))
                            newSkippedActionIds.Add(id);
                    }
                    skippedActionIds = newSkippedActionIds;
                    var previousIds = stagedActionIds;
                    stagedActionIds = new List<int>{0};
                    stagedActionIds.AddRange(previousIds.GetRange(excess + 1, previousIds.Count - (excess + 1)));
                    committedState = computedStates[excess].state;
                    computedStates = computedStates.GetRange(excess, computedStates.Count - excess);
                    currentStateIndex = currentStateIndex > excess ? currentStateIndex - excess : 0;
                }

                LiftedState ComputePausedAction(bool fromPauseAction = false)
                {
                    ComputedState computedState;
                    if (fromPauseAction)
                    {
                        computedState = computedStates[currentStateIndex];
                    }
                    else
                    {
                        if (iAction is not PerformAction performAction)
                            throw new InvalidOperationException("Unexpected action type");

                        var previousEntry = ElementAtOrDefault(computedStates, currentStateIndex);
                        var previousState = previousEntry != null ? previousEntry.state : committedState;

                        computedState = ComputeNextEntry(reducer, performAction.action, (TState)previousState, false);
                    }

                    if (nextActionId == 1)
                    {
                        return new LiftedState
                        {
                            actionsById = new Dictionary<int, LiftedAction> { [0] = new PerformAction(new InitAction()) },
                            nextActionId = 1,
                            stagedActionIds = new List<int> {0},
                            skippedActionIds = new HashSet<int>(),
                            committedState = computedState.state,
                            currentStateIndex = 0,
                            computedStates = new List<ComputedState> { computedState },
                            isLocked = isLocked,
                            isPaused = true,
                        };
                    }

                    if (fromPauseAction)
                    {
                        if (currentStateIndex == stagedActionIds.Count - 1)
                            currentStateIndex++;
                        stagedActionIds = new List<int>(stagedActionIds) {nextActionId};
                        nextActionId++;
                    }

                    actionsById = new Dictionary<int, LiftedAction>(actionsById)
                    {
                        [nextActionId - 1] = new PerformAction(new PausedAction())
                    };

                    var count = Mathf.Min(stagedActionIds.Count, computedStates.Count);
                    computedStates = computedStates.GetRange(0, count);
                    computedStates.Add(computedState);

                    return new LiftedState
                    {
                        actionsById = actionsById,
                        nextActionId = nextActionId,
                        stagedActionIds = stagedActionIds,
                        skippedActionIds = skippedActionIds,
                        committedState = committedState,
                        currentStateIndex = currentStateIndex,
                        computedStates = computedStates,
                        isLocked = isLocked,
                        isPaused = true,
                    };
                }
            };
        }

        static ComputedState ComputeNextEntry(
            Reducer<TState> reducer,
            IAction liftedAction,
            TState state,
            bool shouldCatchExceptions = false)
        {
            if (!shouldCatchExceptions)
                return new ComputedState { state = reducer(state, liftedAction) };
            return ComputeWithTryCatch(reducer, liftedAction, state);
        }

        [Pure]
        static T ElementAtOrDefault<T>(IList<T> list, int index)
        {
            return list != null && index >= 0 && index < list.Count ? list[index] : default;
        }

        static ComputedState ComputeWithTryCatch(
            Reducer<TState> reducer,
            IAction liftedAction,
            TState state)
        {
            TState nextState = default;
            Exception nextException = null;
            try
            {
                nextState = reducer(state, liftedAction);
            }
            catch (Exception e)
            {
                nextException = e;
                Debug.LogException(e);
            }

            // if (ReferenceEquals(nextState, state))
            //     throw new InvalidOperationException("Reducer must not return the previous state object to preserve immutability.");

            return new ComputedState { state = nextState, exception = nextException };
        }

        static List<ComputedState> RecomputeStates(
            List<ComputedState> computedStates,
            int minInvalidatedStateIndex,
            Reducer<TState> reducer,
            object committedState,
            Dictionary<int, LiftedAction> actionsById,
            List<int> stagedActionIds,
            HashSet<int> skippedActionIds,
            bool shouldCatchExceptions)
        {
            // if we know nothing has changed, just early return
            if (computedStates == null || minInvalidatedStateIndex == -1 ||
                (minInvalidatedStateIndex >= computedStates.Count && computedStates.Count == stagedActionIds.Count))
                return computedStates;

            var nextComputedStates = computedStates.GetRange(0, Mathf.Min(minInvalidatedStateIndex, computedStates.Count));
            for (var i = minInvalidatedStateIndex; i < stagedActionIds.Count; i++)
            {
                var actionId = stagedActionIds[i];
                var action = actionsById[actionId].action;

                var previousEntry = ElementAtOrDefault(nextComputedStates, i - 1);
                var previousState = previousEntry != null ? previousEntry.state : committedState;

                var shouldSkip = skippedActionIds.Contains(actionId);

                ComputedState entry;
                if (shouldSkip)
                {
                    entry = previousEntry != null ? previousEntry with {} : new ComputedState { state = previousState };
                }
                else
                {
                    if (shouldCatchExceptions && previousEntry is {exception: not null})
                    {
                        entry = new ComputedState
                        {
                            state = previousState,
                            exception = previousEntry.exception
                        };
                    }
                    else
                    {
                        entry = ComputeNextEntry(reducer, action, (TState)previousState, shouldCatchExceptions);
                    }
                }

                nextComputedStates.Add(entry);
            }

            return nextComputedStates;
        }
    }
}
