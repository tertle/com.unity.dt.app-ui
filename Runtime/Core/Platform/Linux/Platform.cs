#if UNITY_STANDALONE_LINUX || UNITY_EDITOR_LINUX
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using AOT;
using UnityEngine;

namespace Unity.AppUI.Core
{
    class LinuxPlatformImpl : PlatformImpl
    {
        static LinuxPlatformImpl s_Instance;

        delegate void DebugLogDelegate(IntPtr messagePtr, uint len);

        [StructLayout(LayoutKind.Sequential)]
        struct PluginConfigData
        {
            [MarshalAs(UnmanagedType.FunctionPtr)]
            public DebugLogDelegate DebugLogCSharpHandler;
        }

        [MonoPInvokeCallback(typeof(DebugLogDelegate))]
        static void DebugLogProxy(IntPtr message, uint len)
        {
            var messageStr = Marshal.PtrToStringAnsi(message, (int)len);
            Debug.Log(messageStr);
        }

        [DllImport("AppUINativePlugin")]
        static extern bool NativeAppUI_Initialize(ref PluginConfigData configData);

        [DllImport("AppUINativePlugin")]
        static extern void NativeAppUI_Uninitialize();

        [DllImport("AppUINativePlugin")]
        static extern UIntPtr NativeAppUI_GetPasteBoardDataLength(PasteboardType type);

        [DllImport("AppUINativePlugin")]
        static extern void NativeAppUI_GetPasteBoardData(PasteboardType type, UIntPtr size, IntPtr data);

        [DllImport("AppUINativePlugin")]
        static extern void NativeAppUI_SetPasteBoardData(PasteboardType type, UIntPtr size, IntPtr data);

        public LinuxPlatformImpl() => Setup();

        ~LinuxPlatformImpl() => Cleanup();

        void Setup()
        {
            Cleanup();
            var configData = new PluginConfigData
            {
                DebugLogCSharpHandler = DebugLogProxy
            };
            NativeAppUI_Initialize(ref configData);
            s_Instance = this;
        }

        void Cleanup()
        {
            if (s_Instance == null)
                return;

            try
            {
                NativeAppUI_Uninitialize();
            }
            catch (Exception)
            {
                // ignored
            }
            s_Instance = null;
        }

        public override bool HasPasteboardData(PasteboardType type)
        {
            return NativeAppUI_GetPasteBoardDataLength(type).ToUInt64() > 0;
        }

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
