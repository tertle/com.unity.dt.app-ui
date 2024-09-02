using UnityEngine.UIElements;

namespace Unity.AppUI.Bridge
{
    static class PanelExtensionsBridge
    {
#if APPUI_USE_INTERNAL_API_BRIDGE

        internal static PanelSettings GetPanelSettings(this IPanel panel)
        {
            if (panel is RuntimePanel runtimePanel)
                return runtimePanel.panelSettings;

            return null;
        }

#else // REFLECTION

        internal static PanelSettings GetPanelSettings(this IPanel panel)
        {
            var prop = panel.GetType().GetProperty("panelSettings",
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);

            return prop?.GetValue(panel) as PanelSettings;
        }

#endif
    }
}
