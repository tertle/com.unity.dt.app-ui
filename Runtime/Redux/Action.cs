namespace Unity.AppUI.Redux
{
    /// <summary>
    /// An action creator.
    /// </summary>
    public class ActionCreator
    {
        /// <summary>
        /// The type of the action.
        /// This is used to determine which reducer to call.
        /// </summary>
        public string type { get; }

        /// <summary>
        /// Creates a new action creator.
        /// </summary>
        /// <param name="type"> The type of the action. </param>
        public ActionCreator(string type)
        {
            this.type = type;
        }

        /// <summary>
        /// Create the action to dispatch.
        /// </summary>
        /// <returns> The action to dispatch. </returns>
        public Action Invoke()
        {
            return new Action(type);
        }

        /// <summary>
        /// Check if the action matches the action creator.
        /// </summary>
        /// <param name="action"> The action creator. </param>
        /// <returns> True if the action matches the action creator. </returns>
        public bool Match(Action action)
        {
            return action.type == type;
        }
    }

    /// <summary>
    /// An action creator with a payload.
    /// </summary>
    /// <typeparam name="TPayload"> The type of the payload. </typeparam>
    public class ActionCreator<TPayload> : ActionCreator
    {
        /// <summary>
        /// Creates a new action creator.
        /// </summary>
        /// <param name="type"> The type of the action. </param>
        public ActionCreator(string type) : base(type)
        {
        }

        /// <summary>
        /// Create the action to dispatch.
        /// </summary>
        /// <param name="payload"> The payload of the action. </param>
        /// <returns> The action to dispatch. </returns>
        public Action<TPayload> Invoke(TPayload payload)
        {
            return new Action<TPayload>(type, payload);
        }
    }

    /// <summary>
    /// An action without a payload.
    /// </summary>
    /// <param name="type"> The type of the action. </param>
    public record Action(string type)
    {
        /// <summary>
        /// The type of the action.
        /// </summary>
        public string type { get; } = type;
    }

    /// <summary>
    /// An action with a payload.
    /// </summary>
    /// <param name="type"> The type of the action. </param>
    /// <param name="payload"> The payload of the action. </param>
    /// <typeparam name="TPayload"> The type of the payload. </typeparam>
    public record Action<TPayload>(string type, TPayload payload) : Action(type)
    {
        /// <summary>
        /// The payload of the action.
        /// </summary>
        public TPayload payload { get; } = payload;
    }
}
