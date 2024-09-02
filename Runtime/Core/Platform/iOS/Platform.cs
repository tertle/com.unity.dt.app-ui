#if UNITY_IOS && !UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using AOT;

namespace Unity.AppUI.Core
{
    class IOSPlatformImpl : PlatformImpl
    {
        static IOSPlatformImpl s_Instance;

        delegate void DebugLogDelegate(IntPtr messagePtr, uint len);
        delegate void LayoutDirectionChangedDelegate(byte layoutDirection);
        delegate void HighContrastChangedDelegate(bool highContrastEnabled);
        delegate void ReduceMotionChangedDelegate(bool reduceMotionEnabled);
        delegate void ThemeChangedDelegate(bool darkModeEnabled);
        delegate void TextScaleFactorChangedDelegate(float textScaleFactor);
        delegate void ScaleFactorChangedDelegate(float scaleFactor);
        delegate void MagnifyGestureEventDelegate(float magnification, TouchPhase phase);
        delegate void RotateGestureEventDelegate(float rotation, TouchPhase phase);
        delegate void SmartMagnifyEventDelegate();

        [StructLayout(LayoutKind.Sequential)]
        struct PluginConfigData
        {
            public bool isEditor;
            [MarshalAs(UnmanagedType.FunctionPtr)]
            public DebugLogDelegate DebugLogCSharpHandler;
            [MarshalAs(UnmanagedType.FunctionPtr)]
            public LayoutDirectionChangedDelegate LayoutDirectionChangedCSharpHandler;
            [MarshalAs(UnmanagedType.FunctionPtr)]
            public HighContrastChangedDelegate HighContrastChangedCSharpHandler;
            [MarshalAs(UnmanagedType.FunctionPtr)]
            public ReduceMotionChangedDelegate ReduceMotionChangedCSharpHandler;
            [MarshalAs(UnmanagedType.FunctionPtr)]
            public ThemeChangedDelegate ThemeChangedCSharpHandler;
            [MarshalAs(UnmanagedType.FunctionPtr)]
            public TextScaleFactorChangedDelegate TextScaleFactorChangedCSharpHandler;
            [MarshalAs(UnmanagedType.FunctionPtr)]
            public ScaleFactorChangedDelegate ScaleFactorChangedCSharpHandler;
            [MarshalAs(UnmanagedType.FunctionPtr)]
            public MagnifyGestureEventDelegate MagnifyGestureEventCSharpHandler;
            [MarshalAs(UnmanagedType.FunctionPtr)]
            public RotateGestureEventDelegate RotateGestureEventCSharpHandler;
            [MarshalAs(UnmanagedType.FunctionPtr)]
            public SmartMagnifyEventDelegate SmartMagnifyEventCSharpHandler;
        }

        [MonoPInvokeCallback(typeof(DebugLogDelegate))]
        static void DebugLogProxy(IntPtr message, uint len)
        {
            var messageStr = Marshal.PtrToStringAnsi(message, (int)len);
            Debug.Log(messageStr);
        }

        [MonoPInvokeCallback(typeof(LayoutDirectionChangedDelegate))]
        static void InvokeLayoutDirectionChangedProxy(byte layoutDirection) =>
            s_Instance?.InvokeLayoutDirectionChanged(layoutDirection);

        [MonoPInvokeCallback(typeof(HighContrastChangedDelegate))]
        static void InvokeHighContrastChangedProxy(bool highContrastEnabled) =>
            s_Instance?.InvokeHighContrastChanged(highContrastEnabled);

        [MonoPInvokeCallback(typeof(ReduceMotionChangedDelegate))]
        static void InvokeReduceMotionChangedProxy(bool reduceMotionEnabled) =>
            s_Instance?.InvokeReduceMotionChanged(reduceMotionEnabled);

        [MonoPInvokeCallback(typeof(ThemeChangedDelegate))]
        static void InvokeThemeChangedProxy(bool darkModeEnabled) =>
            s_Instance?.InvokeThemeChanged(darkModeEnabled);

        [MonoPInvokeCallback(typeof(TextScaleFactorChangedDelegate))]
        static void InvokeTextScaleFactorChangedProxy(float textScaleFactor) =>
            s_Instance?.InvokeTextScaleFactorChanged(textScaleFactor);

        [MonoPInvokeCallback(typeof(ScaleFactorChangedDelegate))]
        static void InvokeScaleFactorChangedProxy(float scaleFactor) =>
            s_Instance?.InvokeScaleFactorChanged(scaleFactor);

        [MonoPInvokeCallback(typeof(MagnifyGestureEventDelegate))]
        static void InvokeMagnifyGestureEventProxy(float magnification, TouchPhase phase) { }

        [MonoPInvokeCallback(typeof(RotateGestureEventDelegate))]
        static void InvokeRotateGestureEventProxy(float rotation, TouchPhase phase) { }

        [MonoPInvokeCallback(typeof(SmartMagnifyEventDelegate))]
        static void InvokeSmartMagnifyEventProxy() { }

        [DllImport("__Internal")]
        static extern bool NativeAppUI_Initialize(ref PluginConfigData configData);

        [DllImport("__Internal")]
        static extern void NativeAppUI_Uninitialize();

        [DllImport("__Internal")]
        static extern void NativeAppUI_Update();

        [DllImport("__Internal")]
        static extern float NativeAppUI_ScaleFactor();

        [DllImport("__Internal")]
        static extern bool NativeAppUI_DarkMode();

        [DllImport("__Internal")]
        static extern bool NativeAppUI_HighContrast();

        [DllImport("__Internal")]
        static extern bool NativeAppUI_ReduceMotion();

        [DllImport("__Internal")]
        static extern float NativeAppUI_TextScaleFactor();

        [DllImport("__Internal")]
        static extern int NativeAppUI_LayoutDirection();

        [DllImport("__Internal")]
        static extern void NativeAppUI_RunHapticFeedback(HapticFeedbackType feedbackType);

        [DllImport("__Internal")]
        static extern UnityEngine.Color NativeAppUI_GetSystemColor(SystemColorType colorType);

        public IOSPlatformImpl() => Setup();

        void Setup()
        {
            CleanUp();

            var configData = new PluginConfigData
            {
                isEditor = Application.isEditor,
                DebugLogCSharpHandler = DebugLogProxy,
                LayoutDirectionChangedCSharpHandler = InvokeLayoutDirectionChangedProxy,
                HighContrastChangedCSharpHandler = InvokeHighContrastChangedProxy,
                ReduceMotionChangedCSharpHandler = InvokeReduceMotionChangedProxy,
                ThemeChangedCSharpHandler = InvokeThemeChangedProxy,
                TextScaleFactorChangedCSharpHandler = InvokeTextScaleFactorChangedProxy,
                ScaleFactorChangedCSharpHandler = InvokeScaleFactorChangedProxy,
                MagnifyGestureEventCSharpHandler = InvokeMagnifyGestureEventProxy,
                RotateGestureEventCSharpHandler = InvokeRotateGestureEventProxy,
                SmartMagnifyEventCSharpHandler = InvokeSmartMagnifyEventProxy,
            };
            NativeAppUI_Initialize(ref configData);
            s_Instance = this;
        }

        ~IOSPlatformImpl() => CleanUp();

        void CleanUp()
        {
            NativeAppUI_Uninitialize();
            s_Instance = null;
        }

        public override float referenceDpi => Screen.dpi / scaleFactor;

        public override float scaleFactor => NativeAppUI_ScaleFactor();

        public override bool darkMode => NativeAppUI_DarkMode();

        public override bool highContrast => NativeAppUI_HighContrast();

        public override bool reduceMotion => NativeAppUI_ReduceMotion();

        public override float textScaleFactor => NativeAppUI_TextScaleFactor();

        public override int layoutDirection => NativeAppUI_LayoutDirection();

        public override Color GetSystemColor(SystemColorType colorType) => NativeAppUI_GetSystemColor(colorType);

        protected override void HighFrequencyUpdate()
        {
            NativeAppUI_Update();
        }

        public override AppUITouch[] touches => AppUIInput.GetCurrentInputSystemTouches();

        public override void RunNativeHapticFeedback(HapticFeedbackType feedbackType)
        {
            NativeAppUI_RunHapticFeedback(feedbackType);
        }

        protected override void LowFrequencyUpdate()
        {
            // nothing to do
        }
    }
}
#endif
