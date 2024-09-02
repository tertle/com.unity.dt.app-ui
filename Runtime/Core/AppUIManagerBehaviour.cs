// #define APPUI_PLATFORM_EDITOR_ONLY
using System;
using UnityEngine;

namespace Unity.AppUI.Core
{
    /// <summary>
    /// A <see cref="MonoBehaviour"/> which is responsible for updating the AppUI system every frame.
    /// </summary>
    /// <remarks>
    /// A single instance of this class should be present.
    /// </remarks>
    [DisallowMultipleComponent]
    public class AppUIManagerBehaviour : MonoBehaviour
    {
        internal static AppUIManagerBehaviour instance { get; private set; }

        /// <summary>
        /// Creates the AppUIManagerBehaviour instance.
        /// </summary>
#if !APPUI_PLATFORM_EDITOR_ONLY
        [RuntimeInitializeOnLoadMethod(loadType: RuntimeInitializeLoadType.AfterSceneLoad)]
#endif
        public static void Create()
        {
            if (!instance)
            {
                var availableUpdaters = Resources.FindObjectsOfTypeAll<AppUIManagerBehaviour>();
                if (availableUpdaters is {Length: > 0})
                {
                    for (var i = availableUpdaters.Length - 1; i >= 0; i--)
                    {
                        Destroy(availableUpdaters[i].gameObject);
                    }
                }
                var obj = new GameObject("AppUIUpdater")
                {
                    hideFlags = HideFlags.HideAndDontSave
                };
                instance = obj.AddComponent<AppUIManagerBehaviour>();
                DontDestroyOnLoad(obj);
            }
        }

        void OnApplicationQuit()
        {
            Destroy(gameObject);
        }

        void Update()
        {
            if (!Application.isEditor)
            {
                AppUI.EnsureInitialized();
                AppUI.Update();
            }
        }

        void OnNativeMessageReceived(string message)
        {
            Platform.HandleNativeMessage(message);
        }

        void OnApplicationFocus(bool hasFocus)
        {
            if (!Application.isEditor)
            {
                AppUI.EnsureInitialized();
                AppUI.OnApplicationFocus(hasFocus);
            }
        }
    }
}
