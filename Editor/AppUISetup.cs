using UnityEditor;
using UnityEngine;

namespace Unity.AppUI.Editor
{
    /// <summary>
    /// This class is used to add the App UI Shaders to the GraphicsSettings PreloadedShaders list.
    /// </summary>
    [InitializeOnLoad]
    public static class AppUISetup
    {
        static AppUISetup()
        {
            // nothing for now
        }
    }
}
