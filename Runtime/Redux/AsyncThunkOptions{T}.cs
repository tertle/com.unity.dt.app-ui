using System.Threading.Tasks;

namespace Unity.AppUI.Redux
{
    /// <summary>
    /// Options for the async thunk with arguments.
    /// </summary>
    /// <typeparam name="TThunkArg"> The type of the argument to pass to the thunk. </typeparam>
    public class AsyncThunkOptions<TThunkArg> : AsyncThunkOptions
    {
        /// <summary>
        /// A condition to check before dispatching the action.
        /// </summary>
        /// <param name="dispatchedArguments"> The dispatched arguments. </param>
        /// <param name="store"> The store. </param>
        /// <returns> True if the thunk should be processed; false otherwise. </returns>
        public delegate bool ConditionHandler(TThunkArg dispatchedArguments, Store store);

        /// <summary>
        /// A condition to check before dispatching the action asynchronously.
        /// </summary>
        /// <param name="dispatchedArguments"> The dispatched arguments. </param>
        /// <param name="store"> The store. </param>
        /// <returns> True if the thunk should be processed; false otherwise. </returns>
        public delegate Task<bool> ConditionHandlerAsync(TThunkArg dispatchedArguments, Store store);

        /// <summary>
        /// A generator to create a unique ID for the request.
        /// </summary>
        /// <param name="dispatchedArguments"> The dispatched arguments. </param>
        /// <returns> The unique ID for the request. </returns>
        public delegate string IDGeneratorHandler(TThunkArg dispatchedArguments);

        /// <summary>
        /// The condition to check before dispatching the action.
        /// </summary>
        public ConditionHandler condition { get; set; }

        /// <summary>
        /// The condition to check before dispatching the action asynchronously.
        /// </summary>
        public ConditionHandlerAsync conditionAsync { get; set; }

        /// <summary>
        /// The generator to create a unique ID for the request.
        /// </summary>
        public IDGeneratorHandler idGenerator { get; set; }
    }
}
