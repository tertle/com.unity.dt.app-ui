using System;
using System.Collections.Generic;

namespace Unity.AppUI.Redux
{
    /// <summary>
    /// A case in the switch statement.
    /// </summary>
    struct Case<TState>
    {
        /// <summary>
        /// The action creator or matcher that will be used to match the action.
        /// </summary>
        public object actionCreatorOrMatcher;

        /// <summary>
        /// The reducer to call when the action is matched.
        /// The reducer is wrapped in order to keep the action type generic during instantiation.
        /// </summary>
        public Func<TState,IAction,TState> reducer;
    }

    /// <summary>
    /// Base implementation of a switch statement builder to generate a reducer.
    /// </summary>
    /// <typeparam name="TBuilder"> The type of the builder. </typeparam>
    /// <typeparam name="TState"> The type of the state. </typeparam>
    public class SwitchBuilder<TBuilder,TState> : ISwitchBuilder<TBuilder,TState>
        where TBuilder : SwitchBuilder<TBuilder,TState>
    {
        /// <summary>
        /// The cases in the switch statement.
        /// </summary>
        internal readonly List<Case<TState>> cases = new List<Case<TState>>();

        /// <summary>
        /// The action types that are already handled.
        /// </summary>
        protected readonly HashSet<string> m_ActionTypes = new HashSet<string>();

        /// <summary>
        /// Validates the case to ensure it is not a duplicate.
        /// </summary>
        /// <param name="actionCreator"> The action creator. </param>
        /// <param name="reducer"> The reducer. </param>
        /// <exception cref="ArgumentNullException"> Thrown if the action creator or reducer is null. </exception>
        /// <exception cref="InvalidOperationException"> Thrown if the action type is already handled. </exception>
        protected virtual void ValidateCase(IActionCreator actionCreator, object reducer)
        {
            if (actionCreator == null)
                throw new ArgumentNullException(nameof(actionCreator));
            if (reducer == null)
                throw new ArgumentNullException(nameof(reducer));
            if (m_ActionTypes.Contains(actionCreator.type))
                throw new InvalidOperationException($"Action type '{actionCreator.type}' is already handled.");
        }

        /// <inheritdoc />
        public virtual TBuilder AddCase(IActionCreator actionCreator, Reducer<TState> reducer)
        {
            ValidateCase(actionCreator, reducer);
            cases.Add(new Case<TState> { actionCreatorOrMatcher = actionCreator, reducer = (state, action) => reducer(state, action) });
            m_ActionTypes.Add(actionCreator.type);
            return (TBuilder)this;
        }

        /// <inheritdoc />
        public virtual TBuilder AddCase<TPayload>(IActionCreator<TPayload> actionCreator, Reducer<TPayload, TState> reducer)
        {
            ValidateCase(actionCreator, reducer);
            cases.Add(new Case<TState> { actionCreatorOrMatcher = actionCreator, reducer = (state, action) => reducer(state, (IAction<TPayload>)action) });
            m_ActionTypes.Add(actionCreator.type);
            return (TBuilder)this;
        }

        /// <inheritdoc />
        public virtual Dictionary<string, IActionCreator> GetActionCreators()
        {
            var result = new Dictionary<string, IActionCreator>();
            foreach (var switchCase in cases)
            {
                if (switchCase.actionCreatorOrMatcher is IActionCreator actionCreator)
                    result[actionCreator.type] = actionCreator;
            }
            return result;
        }

        /// <inheritdoc />
        public Reducer<TState> GetReducer()
        {
            return (state, action) =>
            {
                var newState = state;
                var defaultCases = new List<Case<TState>>();
                var anyMatch = false;
                foreach (var sCase in cases)
                {
                    if (sCase.actionCreatorOrMatcher == null)
                        defaultCases.Add(sCase);
                    if (CaseMatch(sCase, action))
                    {
                        anyMatch = true;
                        newState = sCase.reducer(state, action);
                    }
                }

                if (!anyMatch)
                {
                    foreach (var sCase in defaultCases)
                    {
                        newState = sCase.reducer(state, action);
                    }
                }

                return newState;
            };
        }

        /// <summary>
        /// Checks if the case matches the action.
        /// </summary>
        /// <param name="sCase"> The case to check. </param>
        /// <param name="action"> The action to check. </param>
        /// <returns> True if the case matches the action, false otherwise. </returns>
        static bool CaseMatch(Case<TState> sCase, IAction action)
        {
            return sCase switch
            {
                { actionCreatorOrMatcher: IActionCreator actionCreator } => actionCreator.Match(action),
                { actionCreatorOrMatcher: ActionMatcher actionMatcher } => actionMatcher(action),
                _ => false
            };
        }
    }

    /// <summary>
    /// The Slice Reducer Switch Builder is used to build a reducer switch statement via method chaining.
    /// This builder does not require you to create Action Creators. It will automatically create them for you.
    /// </summary>
    /// <typeparam name="TState"> The type of the state slice. </typeparam>
    public class SliceReducerSwitchBuilder<TState> : SwitchBuilder<SliceReducerSwitchBuilder<TState>,TState>
    {
        /// <summary>
        /// The name of the slice.
        /// </summary>
        public string name { get; }

        /// <summary>
        /// Creates a new Slice Reducer Switch Builder.
        /// </summary>
        /// <param name="name"> The name of the slice. </param>
        public SliceReducerSwitchBuilder(string name)
        {
            this.name = name;
        }

        /// <inheritdoc/>
        /// <exception cref="InvalidOperationException"> Thrown if the action type does not start with the slice name. </exception>
        protected override void ValidateCase(IActionCreator actionCreator, object reducer)
        {
            base.ValidateCase(actionCreator, reducer);

            if (string.IsNullOrEmpty(actionCreator.type) || !actionCreator.type.StartsWith($"{name}/"))
                throw new InvalidOperationException($"Action type must start with '{name}/'.");
        }
    }

    /// <summary>
    /// The Reducer Switch Builder is used to build a reducer switch statement via method chaining.
    /// You must have created Action Creators for each action type you want to handle prior to using this.
    /// </summary>
    /// <typeparam name="TState"> The type of the state slice. </typeparam>
    public class ReducerSwitchBuilder<TState> :
        SwitchBuilder<ReducerSwitchBuilder<TState>,TState>,
        ISwitchMatchBuilder<ReducerSwitchBuilder<TState>,TState>
    {
        /// <summary>
        /// Constructs a new Reducer Switch Builder based on a Slice Reducer Switch Builder.
        /// </summary>
        internal ReducerSwitchBuilder(SliceReducerSwitchBuilder<TState> builder)
        {
            cases.AddRange(builder.cases);
        }

        /// <inheritdoc />
        public ReducerSwitchBuilder<TState> AddCase(ActionMatcher actionMatcher, Reducer<TState> reducer)
        {
            cases.Add(new Case<TState> { actionCreatorOrMatcher = actionMatcher, reducer = (state, action) => reducer(state, action) });
            return this;
        }

        /// <inheritdoc />
        public ReducerSwitchBuilder<TState> AddCase<TPayload>(ActionMatcher actionMatcher, Reducer<TPayload, TState> reducer)
        {
            cases.Add(new Case<TState> { actionCreatorOrMatcher = actionMatcher, reducer = (state, action) => reducer(state, (IAction<TPayload>)action) });
            return this;
        }

        /// <inheritdoc />
        public ReducerSwitchBuilder<TState> AddDefault(Reducer<TState> reducer)
        {
            cases.Add(new Case<TState> { actionCreatorOrMatcher = null, reducer = (state, action) => reducer(state, action) });
            return this;
        }
    }
}
