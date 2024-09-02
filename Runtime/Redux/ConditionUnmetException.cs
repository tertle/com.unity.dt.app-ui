using System;

namespace Unity.AppUI.Redux
{
    /// <summary>
    /// An action dispatched when the condition set in
    /// the options of the Async Thunk creation is not met.
    /// </summary>
    /// <seealso cref="AsyncThunkOptions{TThunkArg}.ConditionHandler"/>
    /// <seealso cref="AsyncThunkOptions{TThunkArg}.ConditionHandlerAsync"/>
    public class ConditionUnmetException : Exception { }
}
