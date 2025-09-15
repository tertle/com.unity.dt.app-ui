using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

namespace Unity.AppUI.Redux
{
    /// <summary>
    /// An action creator.
    /// </summary>
    [Preserve]
    public class ActionCreator : IActionCreator
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
        [Preserve]
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
        public bool Match(IAction action)
        {
            return action.type == type;
        }

        /// <summary>
        /// Implicitly convert a string to an action creator.
        /// </summary>
        /// <param name="type"> The type of the action. </param>
        /// <returns> The action creator. </returns>
        public static implicit operator ActionCreator(string type)
        {
            return new ActionCreator(type);
        }
    }

    /// <summary>
    /// An action creator with a payload.
    /// </summary>
    /// <typeparam name="TPayload"> The type of the payload. </typeparam>
    [Preserve]
    public class ActionCreator<TPayload> : IActionCreator<TPayload>
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
        [Preserve]
        public ActionCreator(string type)
        {
            this.type = type;
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

        /// <summary>
        /// Check if the action matches the action creator.
        /// </summary>
        /// <param name="action"> The action creator. </param>
        /// <returns> True if the action matches the action creator. </returns>
        public bool Match(IAction action)
        {
            return action.type == type;
        }

        /// <summary>
        /// Implicitly convert a string to an action creator.
        /// </summary>
        /// <param name="type"> The type of the action. </param>
        /// <returns> The action creator. </returns>
        public static implicit operator ActionCreator<TPayload>(string type)
        {
            return new ActionCreator<TPayload>(type);
        }
    }

    /// <summary>
    /// An action without a payload.
    /// </summary>
    [Preserve]
    public class Action : IEquatable<Action>, IAction
    {
        /// <summary>
        /// The type of the action.
        /// </summary>
        public string type { get; }

        /// <summary>
        /// Creates a new action.
        /// </summary>
        /// <param name="type"> The type of the action. </param>
        [Preserve]
        internal Action(string type)
        {
            this.type = type;
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return Equals(obj as Action);
        }

        /// <inheritdoc/>
        public bool Equals(Action other)
        {
            if (ReferenceEquals(this, other)) return true;
            if (other is null || GetType() != other.GetType())
                return false;
            return type == other.type;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return type?.GetHashCode() ?? 0;
        }
    }

    /// <summary>
    /// An action with a payload.
    /// </summary>
    /// <typeparam name="TPayload"> The type of the payload. </typeparam>
    [Preserve]
    public class Action<TPayload> : Action, IEquatable<Action<TPayload>>, IAction<TPayload>
    {
        /// <summary>
        /// The payload of the action.
        /// </summary>
        public TPayload payload { get; }

        /// <summary>
        /// Creates a new action.
        /// </summary>
        /// <param name="type"> The type of the action. </param>
        /// <param name="payload"> The payload of the action. </param>
        [Preserve]
        internal Action(string type, TPayload payload)
            : base(type)
        {
            this.payload = payload;
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return Equals(obj as Action<TPayload>);
        }

        /// <inheritdoc/>
        public bool Equals(Action<TPayload> other)
        {
            return base.Equals(other) && EqualityComparer<TPayload>.Default.Equals(payload, other!.payload);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            var hash = base.GetHashCode();
            hash = (hash * 31) + (payload?.GetHashCode() ?? 0);
            return hash;
        }
    }
}
