using UnityEngine;

namespace Unity.AppUI.Core
{
    /// <summary>
    /// A touch event received from a magic trackpad.
    /// </summary>
    /// <remarks>
    /// Theses Touch events can be received from a magic trackpad on macOS.
    /// </remarks>
    public struct AppUITouch
    {
        /// <summary>
        /// The unique identifier of the touch.
        /// </summary>
        public int fingerId { get; internal set; }

        /// <summary>
        /// The position of the touch in normalized coordinates.
        /// </summary>
        public Vector2 position { get; internal set; }

        /// <summary>
        /// The delta position of the touch since the last frame.
        /// </summary>
        public Vector2 deltaPos { get; internal set; }

        /// <summary>
        /// The delta time since the last frame.
        /// </summary>
        public float deltaTime { get; internal set; }

        /// <summary>
        /// The phase of the touch.
        /// </summary>
        public TouchPhase phase { get; internal set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="fingerId"> The unique identifier of the touch.</param>
        /// <param name="position"> The position of the touch in normalized coordinates.</param>
        /// <param name="deltaPos"> The delta position of the touch since the last frame.</param>
        /// <param name="deltaTime"> The delta time since the last frame.</param>
        /// <param name="phase"> The phase of the touch.</param>
        public AppUITouch(int fingerId, Vector2 position, Vector2 deltaPos, float deltaTime,
            TouchPhase phase)
        {
            this.fingerId = fingerId;
            this.position = position;
            this.deltaPos = deltaPos;
            this.deltaTime = deltaTime;
            this.phase = phase;
        }

        /// <summary>
        /// Returns a string representation of the touch.
        /// </summary>
        /// <returns> A string representation of the touch.</returns>
        public override string ToString()
        {
            return $"AppUITouch: {fingerId} {position} {deltaPos} {deltaTime} {phase}";
        }
    }
}
