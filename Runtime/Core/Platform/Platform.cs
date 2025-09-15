// #define APPUI_PLATFORM_DISABLED
using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Unity.AppUI.Core
{
    /// <summary>
    /// Utility methods and properties related to the Target Platform.
    /// </summary>
#if UNITY_EDITOR
    [UnityEditor.InitializeOnLoad]
#endif
    public static class Platform
    {
        static IPlatformImpl s_Impl;

        static Platform()
        {
            Initialize();
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void Initialize()
        {
            if (s_Impl != null)
                return;
#if APPUI_PLATFORM_DISABLED
            s_Impl = new PlatformImpl();
#else // APPUI_PLATFORM_DISABLED
            try
            {
#if UNITY_IOS && !UNITY_EDITOR
                s_Impl = new IOSPlatformImpl();
#elif UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX
                s_Impl = new OSXPlatformImpl();
#elif UNITY_STANDALONE_LINUX || UNITY_EDITOR_LINUX
                s_Impl = new LinuxPlatformImpl();
#elif UNITY_ANDROID && !UNITY_EDITOR
                s_Impl = new AndroidPlatformImpl();
#elif UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
                s_Impl = new WindowsPlatformImpl();
#else
                s_Impl = new PlatformImpl();
#endif
            }
#pragma warning disable 0168
            catch (Exception e)
#pragma warning restore 0168
            {
                Debug.LogException(e);
                s_Impl = new PlatformImpl();
            }
#endif // APPUI_PLATFORM_DISABLED
        }

#if UNITY_EDITOR
        [UnityEditor.InitializeOnEnterPlayMode]
#endif
        static void OnEnteredPlayMode()
        {
            if (s_Impl == null)
                Initialize();
        }

        /// <summary>
        /// The base DPI value used in <see cref="UnityEngine.UIElements.PanelSettings"/>.
        /// </summary>
        public const float baseDpi = 96f;

        /// <summary>
        /// Event triggered when the system dark mode changes.
        /// </summary>
        public static event Action<bool> darkModeChanged
        {
            add => s_Impl.darkModeChanged += value;
            remove => s_Impl.darkModeChanged -= value;
        }

        /// <summary>
        /// Event triggered when the high contrast mode changes.
        /// </summary>
        public static event Action<bool> highContrastChanged
        {
            add => s_Impl.highContrastChanged += value;
            remove => s_Impl.highContrastChanged -= value;
        }

        /// <summary>
        /// Event triggered when the reduce motion accessibility setting changes.
        /// </summary>
        public static event Action<bool> reduceMotionChanged
        {
            add => s_Impl.reduceMotionChanged += value;
            remove => s_Impl.reduceMotionChanged -= value;
        }

        /// <summary>
        /// Event triggered when the layout direction of the platform changes.
        /// </summary>
        public static event Action<Dir> layoutDirectionChanged
        {
            add => s_Impl.layoutDirectionChanged += value;
            remove => s_Impl.layoutDirectionChanged -= value;
        }

        /// <summary>
        /// Event that is triggered when the scale factor of the Game view's window changes.
        /// </summary>
        /// <remarks>
        /// The Game View's window refers to the window that the game is running in (either standalone or in the editor).
        /// Multiple game views are not supported.
        /// </remarks>
        public static event Action<float> scaleFactorChanged
        {
            add => s_Impl.scaleFactorChanged += value;
            remove => s_Impl.scaleFactorChanged -= value;
        }

        /// <summary>
        /// Event that is triggered when the text scale factor of system changes.
        /// </summary>
        /// <remarks>
        /// This is not window specific. The system text scale factor is the scale factor of the text globally applied by the system.
        /// For the window specific scale factor, use <see cref="scaleFactorChanged"/>.
        /// </remarks>
        public static event Action<float> textScaleFactorChanged
        {
            add => s_Impl.textScaleFactorChanged += value;
            remove => s_Impl.textScaleFactorChanged -= value;
        }

        /// <summary>
        /// <para>
        /// The DPI value that should be used in UI-Toolkit PanelSettings
        /// <see cref="UnityEngine.UIElements.PanelSettings.referenceDpi"/>.
        /// </para>
        /// <para>
        /// This value is computed differently depending on the platform.
        /// </para>
        /// </summary>
        public static float referenceDpi => s_Impl.referenceDpi;

        /// <summary>
        /// The current scale factor applied to the Game view's window.
        /// </summary>
        /// <remarks>
        /// The Game View's window refers to the window that the game is running in (either standalone or in the editor).
        /// Multiple game views are not supported.
        /// </remarks>
        public static float scaleFactor => s_Impl.scaleFactor;

        /// <summary>
        /// The current system-wide text scale factor.
        /// </summary>
        /// <remarks>
        /// This is not window specific. The system text scale factor is the scale factor of the text globally applied by the system.
        /// For the window specific display scale factor (not text-only), use <see cref="scaleFactor"/>.
        /// </remarks>
        public static float textScaleFactor => s_Impl.textScaleFactor;

        /// <summary>
        /// Whether the current platform supports touch gestures.
        /// </summary>
        public static bool isTouchGestureSupported => s_Impl.isTouchGestureSupported;

        /// <summary>
        /// Whether the current system is in dark mode.
        /// </summary>
        public static bool darkMode => s_Impl.darkMode;

        /// <summary>
        /// Whether the current system is in high contrast mode.
        /// </summary>
        public static bool highContrast => s_Impl.highContrast;

        /// <summary>
        /// Whether the current system uses the "Reduce Motion" accessibility setting.
        /// </summary>
        public static bool reduceMotion => s_Impl.reduceMotion;

        /// <summary>
        /// Whether the current platform supports haptic feedback.
        /// </summary>
        public static bool isHapticFeedbackSupported => s_Impl.isHapticFeedbackSupported;

        /// <summary>
        /// The current touches events for the current platform.
        /// </summary>
        /// <remarks>
        /// This can be either coming from the Old or New Input System, but also from custom the App UI Input System for
        /// trackpad/touchpad support.
        /// </remarks>
        public static ReadOnlySpan<AppUITouch> touches => s_Impl.touches;

        /// <summary>
        /// Run a haptic feedback on the current platform.
        /// </summary>
        /// <param name="feedbackType">The type of haptic feedback to trigger.</param>
        public static void RunHapticFeedback(HapticFeedbackType feedbackType)
        {
            s_Impl.RunNativeHapticFeedback(feedbackType);
        }

        /// <summary>
        /// The current layout direction of the platform.
        /// </summary>
        public static int layoutDirection => s_Impl.layoutDirection;

        /// <summary>
        /// Get the system color for the given color type.
        /// </summary>
        /// <param name="colorType"> The type of system color to get.</param>
        /// <returns> The system color for the given color type if any, otherwise Color.clear.</returns>
        public static Color GetSystemColor(SystemColorType colorType) => s_Impl.GetSystemColor(colorType);

        /// <summary>
        /// Whether the current platform's pasteboard has data for the given type.
        /// </summary>
        /// <param name="type"> The type of data to check for.</param>
        /// <returns> True if the pasteboard has data for the given type, otherwise false.</returns>
        public static bool HasPasteboardData(PasteboardType type) => s_Impl.HasPasteboardData(type);

        /// <summary>
        /// Get the pasteboard data.
        /// </summary>
        /// <param name="type"> The type of data to get.</param>
        /// <returns> The pasteboard data.</returns>
        public static byte[] GetPasteboardData(PasteboardType type) => s_Impl.GetPasteboardData(type);

        /// <summary>
        /// Set the pasteboard data.
        /// </summary>
        /// <param name="type"> The type of data to set.</param>
        /// <param name="data"> The data to set.</param>
        public static void SetPasteboardData(PasteboardType type, byte[] data) => s_Impl.SetPasteboardData(type, data);

        /// <summary>
        /// Handle an native message coming from a native App UI plugin.
        /// </summary>
        /// <param name="message"> The message to handle.</param>
        internal static void HandleNativeMessage(string message)
        {
            s_Impl.HandleNativeMessage(message);
        }

        /// <summary>
        /// Update the platform utility.
        /// </summary>
        internal static void Update()
        {
            s_Impl.UpdateLoop();
        }
    }
}
