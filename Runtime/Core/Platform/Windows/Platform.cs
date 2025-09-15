#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
using System;
using System.Runtime.InteropServices;
using AOT;
using UnityEngine;
using UnityEngine.Assertions;

namespace Unity.AppUI.Core
{
    class WindowsPlatformImpl : PlatformImpl
    {
        static WindowsPlatformImpl s_Instance;

        delegate void DebugLogDelegate(IntPtr messagePtr, uint length);
        delegate void HighContrastChangedDelegate(bool highContrastEnabled);
        delegate void ReduceMotionChangedDelegate(bool reduceMotionEnabled);
        delegate void ThemeChangedDelegate(bool darkModeEnabled);
        delegate void TextScaleFactorChangedDelegate(float textScaleFactor);
        delegate void ScaleFactorChangedDelegate(float scaleFactor);
        delegate void LayoutDirectionChangedDelegate(byte layoutDirection);
        delegate void SystemColorChangedDelegate();

        [StructLayout(LayoutKind.Sequential)]
        struct PluginConfigData
        {
            [MarshalAs(UnmanagedType.FunctionPtr)]
            public DebugLogDelegate DebugLogCSharpHandler;
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
            public LayoutDirectionChangedDelegate LayoutDirectionChangedCSharpHandler;
            [MarshalAs(UnmanagedType.FunctionPtr)]
            public SystemColorChangedDelegate SystemColorChangedCSharpHandler;
        }

        [DllImport("AppUINativePlugin", CallingConvention = CallingConvention.Cdecl)]
        static extern bool NativeAppUI_Initialize(IntPtr configDataPtr);

        [DllImport("AppUINativePlugin", CallingConvention = CallingConvention.Cdecl)]
        static extern bool NativeAppUI_EnsureUnityWindowFound();

        [DllImport("AppUINativePlugin", CallingConvention = CallingConvention.Cdecl)]
        static extern void NativeAppUI_Uninitialize();

        [DllImport("AppUINativePlugin", CallingConvention = CallingConvention.Cdecl)]
        static extern float NativeAppUI_ScaleFactor();

        [DllImport("AppUINativePlugin", CallingConvention = CallingConvention.Cdecl)]
        static extern bool NativeAppUI_DarkMode();

        [DllImport("AppUINativePlugin", CallingConvention = CallingConvention.Cdecl)]
        static extern bool NativeAppUI_HighContrast();

        [DllImport("AppUINativePlugin", CallingConvention = CallingConvention.Cdecl)]
        static extern bool NativeAppUI_ReduceMotion();

        [DllImport("AppUINativePlugin", CallingConvention = CallingConvention.Cdecl)]
        static extern float NativeAppUI_TextScaleFactor();

        [DllImport("AppUINativePlugin", CallingConvention = CallingConvention.Cdecl)]
        static extern int NativeAppUI_LayoutDirection();

        [DllImport("AppUINativePlugin", CallingConvention = CallingConvention.Cdecl)]
        static extern Color NativeAppUI_GetSystemColor(SystemColorType elementType);

        [DllImport("AppUINativePlugin", CallingConvention = CallingConvention.Cdecl)]
        static extern UIntPtr NativeAppUI_GetPasteBoardDataLength(PasteboardType type);

        [DllImport("AppUINativePlugin", CallingConvention = CallingConvention.Cdecl)]
        static extern void NativeAppUI_GetPasteBoardData(PasteboardType type, UIntPtr size, IntPtr data);

        [DllImport("AppUINativePlugin", CallingConvention = CallingConvention.Cdecl)]
        static extern void NativeAppUI_SetPasteBoardData(PasteboardType type, UIntPtr size, IntPtr data);

        [MonoPInvokeCallback(typeof(DebugLogDelegate))]
        static void DebugLog(IntPtr messagePtr, uint length)
        {
            var message = Marshal.PtrToStringAnsi(messagePtr, (int)length);
            Debug.Log(message);
        }

        [MonoPInvokeCallback(typeof(HighContrastChangedDelegate))]
        static void OnHighContrastChanged(bool highContrastEnabled)
        {
            s_Instance?.InvokeHighContrastChanged(highContrastEnabled);
        }

        [MonoPInvokeCallback(typeof(ReduceMotionChangedDelegate))]
        static void OnReduceMotionChanged(bool reduceMotionEnabled)
        {
            s_Instance?.InvokeReduceMotionChanged(reduceMotionEnabled);
        }

        [MonoPInvokeCallback(typeof(ThemeChangedDelegate))]
        static void OnThemeChanged(bool darkModeEnabled)
        {
            s_Instance?.InvokeThemeChanged(darkModeEnabled);
        }

        [MonoPInvokeCallback(typeof(TextScaleFactorChangedDelegate))]
        static void OnTextScaleFactorChanged(float textScaleFactor)
        {
            s_Instance?.InvokeTextScaleFactorChanged(textScaleFactor);
        }

        [MonoPInvokeCallback(typeof(ScaleFactorChangedDelegate))]
        static void OnScaleFactorChanged(float scaleFactor)
        {
            s_Instance?.InvokeScaleFactorChanged(scaleFactor);
        }

        [MonoPInvokeCallback(typeof(LayoutDirectionChangedDelegate))]
        static void OnLayoutDirectionChanged(byte layoutDirection)
        {
            s_Instance?.InvokeLayoutDirectionChanged(layoutDirection);
        }

        [MonoPInvokeCallback(typeof(SystemColorChangedDelegate))]
        static void OnSystemColorChanged()
        {
            s_Instance?.InvokeSystemColorChanged();
        }

        static IntPtr s_ConfigDataPtr = IntPtr.Zero;

        PluginConfigData m_ConfigData;

        public WindowsPlatformImpl()
        {
            CleanUp();

            m_ConfigData = new PluginConfigData
            {
                DebugLogCSharpHandler = DebugLog,
                HighContrastChangedCSharpHandler = OnHighContrastChanged,
                ReduceMotionChangedCSharpHandler = OnReduceMotionChanged,
                ThemeChangedCSharpHandler = OnThemeChanged,
                TextScaleFactorChangedCSharpHandler = OnTextScaleFactorChanged,
                ScaleFactorChangedCSharpHandler = OnScaleFactorChanged,
                LayoutDirectionChangedCSharpHandler = OnLayoutDirectionChanged,
                SystemColorChangedCSharpHandler = OnSystemColorChanged
            };
            s_ConfigDataPtr = Marshal.AllocHGlobal(Marshal.SizeOf<PluginConfigData>());
            Assert.AreNotEqual(IntPtr.Zero, s_ConfigDataPtr,
                "Failed to allocate memory for the config data");
            Marshal.StructureToPtr(m_ConfigData, s_ConfigDataPtr, false);
            if (!NativeAppUI_Initialize(s_ConfigDataPtr))
                Debug.LogError("Failed to initialize the native plugin");
            s_Instance = this;
        }

        ~WindowsPlatformImpl() => CleanUp();

        void CleanUp()
        {
            try
            {
                NativeAppUI_Uninitialize();
            }
            catch (Exception)
            {
                // ignored
            }
            if (s_ConfigDataPtr != IntPtr.Zero)
                Marshal.FreeHGlobal(s_ConfigDataPtr);
            s_Instance = null;
        }

        public override float referenceDpi
        {
            get
            {
                // On Windows we can use a value of 96dpi because UI Toolkit scales correctly the UI based on
                // Operating System's DPI and ScaleFactor changes.
                return Platform.baseDpi;
            }
        }

        public override float scaleFactor
        {
            get
            {
                // On Windows Screen.dpi is already the result of the dpi multiplied by the scale factor.
                // For example on a 27in 4k monitor at 100% scale, the dpi is 96 (really small UI), but the recommended
                // Scale factor with a 4k monitor is 150%, which gives 96 * 1.5 = 144dpi.
                // Unity Engine sets the DPI awareness per monitor, so the UI will scale automatically :
                // https://docs.microsoft.com/en-us/windows/win32/api/windef/ne-windef-dpi_awareness
                return Screen.dpi / Platform.baseDpi;
            }
        }

        protected override void LowFrequencyUpdate()
        {
            NativeAppUI_EnsureUnityWindowFound();
            PollLayoutDirection();
        }

        public override ReadOnlySpan<AppUITouch> touches => AppUIInput.GetCurrentInputSystemTouches();

        public override bool darkMode => NativeAppUI_DarkMode();

        public override bool highContrast => NativeAppUI_HighContrast();

        public override bool reduceMotion => NativeAppUI_ReduceMotion();

        public override int layoutDirection => NativeAppUI_LayoutDirection();

        public override float textScaleFactor => NativeAppUI_TextScaleFactor();

        public override Color GetSystemColor(SystemColorType elementType) => NativeAppUI_GetSystemColor(elementType);

        public override bool HasPasteboardData(PasteboardType type) => NativeAppUI_GetPasteBoardDataLength(type).ToUInt64() > 0;

        public override byte[] GetPasteboardData(PasteboardType type)
        {
            var length = NativeAppUI_GetPasteBoardDataLength(type);
            var size = length.ToUInt64();
            if (size <= 0)
                return Array.Empty<byte>();

            var dataBuffer = new byte[size];
            var handle = GCHandle.Alloc(dataBuffer, GCHandleType.Pinned);
            try
            {
                var dataPtr = handle.AddrOfPinnedObject();
                NativeAppUI_GetPasteBoardData(type, length, dataPtr);
            }
            finally
            {
                handle.Free();
            }

            return dataBuffer;
        }

        public override void SetPasteboardData(PasteboardType type, byte[] data)
        {
            if (data == null || data.Length == 0)
                return;

            var size = new UIntPtr((ulong)data.Length);
            var handle = GCHandle.Alloc(data, GCHandleType.Pinned);
            try
            {
                var dataPtr = handle.AddrOfPinnedObject();
                NativeAppUI_SetPasteBoardData(type, size, dataPtr);
            }
            finally
            {
                handle.Free();
            }
        }
    }
}
#endif
