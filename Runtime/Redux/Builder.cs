using System;
using System.Collections.Generic;
using System.Reflection;

namespace Unity.AppUI.Redux
{
    /// <summary>
    /// The Slice Reducer Switch Builder is used to build a reducer switch statement via method chaining.
    /// This builder does not require you to create Action Creators. It will automatically create them for you.
    /// </summary>
    /// <typeparam name="TState"> The type of the state slice. </typeparam>
    public class SliceReducerSwitchBuilder<TState>
    {
        readonly List<object> m_Reducers = new List<object>();
        readonly Dictionary<object, string> m_ReducerNames = new Dictionary<object, string>();

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

        /// <summary>
        /// Adds a case to the reducer switch statement.
        /// </summary>
        /// <param name="reducer"> The reducer function for the action type you want to handle. </param>
        /// <returns> The Slice Reducer Switch Builder. </returns>
        public SliceReducerSwitchBuilder<TState> Add(CaseReducer<TState> reducer)
        {
            if (reducer.Method.Name.StartsWith("<"))
                throw new ArgumentException("Lambda expressions are not supported. Please use a named method instead.");

            m_Reducers.Add(reducer);
            m_ReducerNames[reducer] = reducer.Method.Name;
            return this;
        }

        /// <summary>
        /// Adds a case to the reducer switch statement.
        /// </summary>
        /// <param name="actionType"> The action type you want to handle. </param>
        /// <param name="reducer"> The reducer function for the action type you want to handle. </param>
        /// <returns> The Slice Reducer Switch Builder. </returns>
        public SliceReducerSwitchBuilder<TState> Add(string actionType, CaseReducer<TState> reducer)
        {
            m_Reducers.Add(reducer);
            var actionCatAndName = actionType.Split('/');
            m_ReducerNames[reducer] = actionCatAndName[^1];
            return this;
        }

        /// <summary>
        /// Adds a case to the reducer switch statement.
        /// </summary>
        /// <param name="reducer"> The reducer function for the action type you want to handle. </param>
        /// <typeparam name="T"> The type of the payload. </typeparam>
        /// <returns> The Slice Reducer Switch Builder. </returns>
        public SliceReducerSwitchBuilder<TState> Add<T>(CaseReducer<T, TState> reducer)
        {
            if (reducer.Method.Name.StartsWith("<"))
                throw new ArgumentException("Lambda expressions are not supported. Please use a named method instead.");

            m_Reducers.Add(reducer);
            m_ReducerNames[reducer] = reducer.Method.Name;
            return this;
        }

        /// <summary>
        /// Adds a case to the reducer switch statement.
        /// </summary>
        /// <param name="actionType"> The action type you want to handle. </param>
        /// <param name="reducer"> The reducer function for the action type you want to handle. </param>
        /// <typeparam name="T"> The type of the payload. </typeparam>
        /// <returns> The Slice Reducer Switch Builder. </returns>
        public SliceReducerSwitchBuilder<TState> Add<T>(string actionType, CaseReducer<T, TState> reducer)
        {
            m_Reducers.Add(reducer);
            var actionCatAndName = actionType.Split('/');
            m_ReducerNames[reducer] = actionCatAndName[^1];
            return this;
        }

        /// <summary>
        /// Build Action Creators for each reducer.
        /// </summary>
        /// <returns> A dictionary of Action Creators. </returns>
        internal Dictionary<string, ActionCreator> BuildActionCreators()
        {
            var result = new Dictionary<string, ActionCreator>();
            foreach (var reducer in m_Reducers)
            {
                var reducerType = reducer.GetType();
                var actionTypeName = $"{name}/{m_ReducerNames[reducer]}";

                var genericArguments = reducerType.GetGenericArguments();
                if (genericArguments.Length == 1) // only TState is a generic argument
                {
                    result[actionTypeName] = Store.CreateAction(actionTypeName);
                }
                else // T and TState are generic arguments, so we need to create an Action<T>
                {
                    var actionType = typeof(ActionCreator<>).MakeGenericType(genericArguments[0]);
                    result[actionTypeName] = Store.CreateAction(actionTypeName, actionType);
                }
            }

            return result;
        }

        /// <summary>
        /// Build the reducer switch statement.
        /// </summary>
        /// <param name="actionCreatorCollection"> The collection of Action Creators. </param>
        /// <returns> The reducer switch statement. </returns>
        public System.Action<ReducerSwitchBuilder<TState>> BuildReducers(IEnumerable<ActionCreator> actionCreatorCollection)
        {
            var actionCreators = new List<ActionCreator>(actionCreatorCollection);
            MethodInfo addCaseMethod = null;
            foreach (var method in typeof(ReducerSwitchBuilder<TState>).GetMethods(BindingFlags.Public | BindingFlags.Instance))
            {
                if (method.Name == nameof(ReducerSwitchBuilder<TState>.AddCase) &&
                    method.GetGenericArguments().Length == 1)
                {
                    addCaseMethod = method;
                    break;
                }
            }

            return builder =>
            {
                for (var i = 0; i < m_Reducers.Count; i++)
                {
                    var reducer = m_Reducers[i];
                    var reducerType = reducer.GetType();
                    var genericArguments = reducerType.GetGenericArguments();
                    if (genericArguments.Length == 1) // only TState is a generic argument
                    {
                        builder.AddCase(actionCreators[i], (CaseReducer<TState>)reducer);
                    }
                    else // T and TState are generic arguments, so we need to call ReducerSwitchBuilder.AddCase<T>
                    {
                        var addCaseGenericMethod = addCaseMethod!.MakeGenericMethod(genericArguments[0]);
                        var castAction = typeof(ActionCreator<>).MakeGenericType(genericArguments[0]);
                        var actionCreator = Convert.ChangeType(actionCreators[i], castAction);
                        addCaseGenericMethod.Invoke(builder, new object[]
                        {
                            actionCreator,
                            reducer
                        });
                    }
                }
            };
        }
    }

    /// <summary>
    /// The Reducer Switch Builder is used to build a reducer switch statement via method chaining.
    /// You must have created Action Creators for each action type you want to handle prior to using this.
    /// </summary>
    /// <typeparam name="TState"> The type of the state slice. </typeparam>
    public class ReducerSwitchBuilder<TState>
    {
        readonly List<KeyValuePair<object, object>> m_CaseReducers = new List<KeyValuePair<object, object>>();

        /// <summary>
        /// Adds a case to the reducer switch statement.
        /// </summary>
        /// <param name="action"> The action creator for the action type you want to handle. </param>
        /// <param name="reducer"> The reducer function for the action type you want to handle. </param>
        /// <returns> The Reducer Switch Builder. </returns>
        public ReducerSwitchBuilder<TState> AddCase(ActionCreator action, CaseReducer<TState> reducer)
        {
            m_CaseReducers.Add(new KeyValuePair<object, object>(action, reducer));
            return this;
        }

        /// <summary>
        /// Adds a case to the reducer switch statement.
        /// </summary>
        /// <param name="action"> The action creator for the action type you want to handle. </param>
        /// <param name="reducer"> The reducer function for the action type you want to handle. </param>
        /// <typeparam name="T"> The type of the action payload. </typeparam>
        /// <returns> The Reducer Switch Builder. </returns>
        public ReducerSwitchBuilder<TState> AddCase<T>(ActionCreator<T> action, CaseReducer<T, TState> reducer)
        {
            m_CaseReducers.Add(new KeyValuePair<object, object>(action, reducer));
            return this;
        }

        /// <summary>
        /// Adds a matcher case to the reducer switch statement.
        /// A matcher case is a case that will be executed if the action type matches the predicate.
        /// </summary>
        /// <param name="actionMatcher"> The predicate that will be used to match the action type. </param>
        /// <param name="reducer"> The reducer function for the action type you want to handle. </param>
        /// <returns> The Reducer Switch Builder. </returns>
        public ReducerSwitchBuilder<TState> AddMatcher(ActionMatcher actionMatcher, CaseReducer<TState> reducer)
        {
            m_CaseReducers.Add(new KeyValuePair<object, object>(actionMatcher, reducer));
            return this;
        }

        /// <summary>
        /// Adds a matcher case to the reducer switch statement.
        /// A matcher case is a case that will be executed if the action type matches the predicate.
        /// </summary>
        /// <param name="actionMatcher"> The predicate that will be used to match the action type. </param>
        /// <param name="reducer"> The reducer function for the action type you want to handle. </param>
        /// <typeparam name="T"> The type of the action payload. </typeparam>
        /// <returns> The Reducer Switch Builder. </returns>
        public ReducerSwitchBuilder<TState> AddMatcher<T>(ActionMatcher actionMatcher, CaseReducer<T, TState> reducer)
        {
            m_CaseReducers.Add(new KeyValuePair<object, object>(actionMatcher, reducer));
            return this;
        }

        /// <summary>
        /// Adds a default case to the reducer switch statement.
        /// A default case is a case that will be executed if no other cases match.
        /// </summary>
        /// <param name="reducer"> The reducer function for the default case. </param>
        /// <returns> The Reducer Switch Builder. </returns>
        public ReducerSwitchBuilder<TState> AddDefaultCase(CaseReducer<TState> reducer)
        {
            m_CaseReducers.Add(new KeyValuePair<object, object>(null, reducer));
            return this;
        }

        /// <summary>
        /// Adds a default case to the reducer switch statement.
        /// A default case is a case that will be executed if no other cases match.
        /// </summary>
        /// <param name="reducer"> The reducer function for the default case. </param>
        /// <typeparam name="T"> The type of the action payload. </typeparam>
        /// <returns> The Reducer Switch Builder. </returns>
        public ReducerSwitchBuilder<TState> AddDefaultCase<T>(CaseReducer<T, TState> reducer)
        {
            m_CaseReducers.Add(new KeyValuePair<object, object>(null, reducer));
            return this;
        }

        /// <summary>
        /// Builds the reducer switch statement.
        /// </summary>
        /// <param name="initialState"> The initial state of the reducer. </param>
        /// <returns> The reducer switch statement. </returns>
        public Reducer BuildReducer(TState initialState)
        {
            return (state, action) =>
            {
                state ??= initialState;

                // find cases that match the action
                var matchingCases = new List<KeyValuePair<object, object>>();

                foreach (var caseReducer in m_CaseReducers)
                {
                    switch (caseReducer.Key)
                    {
                        case ActionCreator actionCreator when actionCreator.Match(action):
                        case ActionMatcher actionMatcher when actionMatcher(action):
                            matchingCases.Add(caseReducer);
                            break;
                    }
                }

                // if there are no matching cases, use the default case
                if (matchingCases.Count == 0)
                {
                    foreach (var caseReducer in m_CaseReducers)
                    {
                        if (caseReducer.Key == null)
                            matchingCases.Add(caseReducer);
                    }
                }

                // execute the matching cases

                var newState = state;

                foreach (var x in matchingCases)
                {
                    if (x.Value is CaseReducer<TState> simpleReducer)
                    {
                        newState = simpleReducer((TState) newState, action);
                        continue;
                    }

                    var actionType = action.GetType();
                    var caseReducerGenericType =
                        typeof(CaseReducer<,>).MakeGenericType(actionType.GetGenericArguments()[0], typeof(TState));
                    newState = (TState) caseReducerGenericType.GetMethod("Invoke")?.Invoke(x.Value, new object[] {newState, action});
                }

                return newState;
            };
        }
    }
}
