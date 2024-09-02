
namespace Unity.AppUI.Core
{
    /// <summary>
    /// Extension methods for the Touch classes.
    /// </summary>
    public static class TouchExtensions
    {
        /// <summary>
        /// Converts a UnityEngine.Touch to an AppUITouch.
        /// </summary>
        /// <param name="touch"> The UnityEngine.Touch to convert. </param>
        /// <returns> The converted AppUITouch. </returns>
        public static AppUITouch ToAppUITouch(this UnityEngine.Touch touch)
        {
            return new AppUITouch(
                fingerId: touch.fingerId,
                position: touch.position,
                deltaPos: touch.deltaPosition,
                deltaTime: touch.deltaTime,
                phase: touch.phase
            );
        }

#if UNITY_INPUTSYSTEM_PRESENT
        /// <summary>
        /// Converts a UnityEngine.InputSystem.Touch to an AppUITouch.
        /// </summary>
        /// <param name="touch"> The UnityEngine.InputSystem.Touch to convert. </param>
        /// <returns> The converted AppUITouch. </returns>
        public static AppUITouch ToAppUITouch(this UnityEngine.InputSystem.EnhancedTouch.Touch touch)
        {
            return new AppUITouch(
                fingerId: touch.touchId,
                position: touch.screenPosition,
                deltaPos: touch.delta,
                deltaTime: 0,
                phase: ToUnityEngineTouchPhase(touch.phase)
            );
        }

        /// <summary>
        /// Converts a UnityEngine.InputSystem.Controls.TouchControl to an AppUITouch.
        /// </summary>
        /// <param name="touch"> The UnityEngine.InputSystem.Controls.TouchControl to convert. </param>
        /// <returns> The converted AppUITouch. </returns>
        public static AppUITouch ToAppUITouch(this UnityEngine.InputSystem.Controls.TouchControl touch)
        {
            return new AppUITouch(
                fingerId: touch.touchId.ReadValue(),
                position: touch.position.ReadValue(),
                deltaPos: touch.delta.ReadValue(),
                deltaTime: 0,
                phase: ToUnityEngineTouchPhase(touch.phase.ReadValue())
            );
        }

        /// <summary>
        /// Converts a UnityEngine.InputSystem.TouchPhase to a UnityEngine.TouchPhase.
        /// </summary>
        /// <param name="phase"> The UnityEngine.InputSystem.TouchPhase to convert. </param>
        /// <returns> The converted UnityEngine.TouchPhase. </returns>
        public static UnityEngine.TouchPhase ToUnityEngineTouchPhase(this UnityEngine.InputSystem.TouchPhase phase)
        {
            switch (phase)
            {
                case UnityEngine.InputSystem.TouchPhase.Began:
                    return UnityEngine.TouchPhase.Began;
                case UnityEngine.InputSystem.TouchPhase.Moved:
                    return UnityEngine.TouchPhase.Moved;
                case UnityEngine.InputSystem.TouchPhase.Stationary:
                    return UnityEngine.TouchPhase.Stationary;
                case UnityEngine.InputSystem.TouchPhase.Ended:
                    return UnityEngine.TouchPhase.Ended;
                case UnityEngine.InputSystem.TouchPhase.Canceled:
                    return UnityEngine.TouchPhase.Canceled;
                default:
                    return UnityEngine.TouchPhase.Canceled;
            }
        }
#endif
    }
}
