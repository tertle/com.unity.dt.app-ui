using UnityEngine;

namespace Unity.AppUI.Core
{
    /// <summary>
    /// The layout direction of the application.
    /// </summary>
    public enum Dir
    {
        /// <summary>
        /// Left to right.
        /// </summary>
        [InspectorName("Left to Right")]
        Ltr,

        /// <summary>
        /// Right to left.
        /// </summary>
        [InspectorName("Right to Left")]
        Rtl
    }

    /// <summary>
    /// The layout direction context of the application.
    /// </summary>
    /// <param name="dir"> The layout direction. </param>
    public record DirContext(Dir dir) : IContext
    {
        /// <summary>
        /// The current layout direction.
        /// </summary>
        public Dir dir { get; } = dir;
    }
}
