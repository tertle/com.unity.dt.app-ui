using System;
using UnityEngine;

namespace Unity.AppUI.Core
{
    /// <summary>
    /// A pinch gesture result that is defined by a <see cref="PinchGestureRecognizer"/>.
    /// </summary>
    public struct PinchGesture : IEquatable<PinchGesture>
    {
        /// <summary>
        /// The magnification delta of the gesture since the last frame.
        /// </summary>
        public float deltaMagnification { get; }

        /// <summary>
        /// The scroll delta of the gesture since the last frame.
        /// </summary>
        /// <remarks>
        /// This is a convenience property to convert the magnification delta to a scroll delta.
        /// </remarks>
        public Vector2 scrollDelta => new Vector2(0, -deltaMagnification * 50f);

        /// <summary>
        /// The state of the gesture.
        /// </summary>
        public GestureRecognizerState state { get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="deltaMagnification">The magnification delta of the gesture since the last frame.</param>
        /// <param name="state">The phase of the gesture.</param>
        public PinchGesture(float deltaMagnification, GestureRecognizerState state)
        {
            this.state = state;
            this.deltaMagnification = deltaMagnification;
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="other"> The object to compare with the current object.</param>
        /// <returns> True if objects are equal, false otherwise.</returns>
        public bool Equals(PinchGesture other)
        {
            return Mathf.Approximately(deltaMagnification, other.deltaMagnification) && state == other.state;
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="obj"> The object to compare with the current object.</param>
        /// <returns> True if the first PinchGesture is equal to the second PinchGesture, false otherwise.</returns>
        public override bool Equals(object obj)
        {
            return obj is PinchGesture other && Equals(other);
        }

        /// <summary>
        /// Serves as the default hash function.
        /// </summary>
        /// <returns> A hash code for the current object.</returns>
        public override int GetHashCode()
        {
            return HashCode.Combine(deltaMagnification, (int) state);
        }

        /// <summary>
        /// Determines whether two specified PinchGesture objects are equal.
        /// </summary>
        /// <param name="left"> The first PinchGesture to compare.</param>
        /// <param name="right"> The second PinchGesture to compare.</param>
        /// <returns> True if the first PinchGesture is equal to the second PinchGesture, false otherwise.</returns>
        public static bool operator ==(PinchGesture left, PinchGesture right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Determines whether two specified PinchGesture objects are not equal.
        /// </summary>
        /// <param name="left"> The first PinchGesture to compare.</param>
        /// <param name="right"> The second PinchGesture to compare.</param>
        /// <returns> True if the first PinchGesture is not equal to the second PinchGesture, false otherwise.</returns>
        public static bool operator !=(PinchGesture left, PinchGesture right)
        {
            return !left.Equals(right);
        }

        /// <summary>
        /// Returns a string that represents the current PinchGesture.
        /// </summary>
        /// <returns> A string that represents the current PinchGesture.</returns>
        public override string ToString()
        {
            return $"PinchGesture: {deltaMagnification}, {state}";
        }
    }
}
