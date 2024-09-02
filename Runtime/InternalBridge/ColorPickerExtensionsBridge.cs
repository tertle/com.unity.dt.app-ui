using System;
using System.Reflection;
using UnityEngine;

namespace Unity.AppUI.Bridge
{
    static class ColorPickerExtensionsBridge
    {

#if APPUI_USE_INTERNAL_API_BRIDGE


        internal static void Show(
            Action<Color> colorChangedCallback,
            Color col,
            bool showAlpha,
            bool hdr)
        {
#if UNITY_EDITOR
            UnityEditor.ColorPicker.Show(null, colorChangedCallback, col, showAlpha, hdr);
#endif
        }


#else // REFLECTION


        static Type s_ColorPickerType;

        static MethodInfo s_ShowMethod;

        internal static void Show(
            Action<Color> colorChangedCallback,
            Color col,
            bool showAlpha,
            bool hdr)
        {
#if UNITY_EDITOR
            s_ColorPickerType ??= typeof(UnityEditor.EditorWindow).Assembly.GetType("UnityEditor.ColorPicker");

            if (s_ShowMethod == null)
            {
                foreach (var methodInfo in s_ColorPickerType.GetMethods(BindingFlags.NonPublic | BindingFlags.Static))
                {
                    if (methodInfo.Name == "Show" && methodInfo.GetParameters().Length == 5)
                    {
                        s_ShowMethod = methodInfo;
                        break;
                    }
                }
            }

            s_ShowMethod?.Invoke(null, new object[] { null, colorChangedCallback, col, showAlpha, hdr });
#endif
        }


#endif

    }
}
