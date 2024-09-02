using UnityEngine;

namespace Unity.AppUI.UI
{
    /// <summary>
    /// Use this attribute to draw a property with the optional ScaleDrawer.
    /// </summary>
    public class OptionalScaleDrawerAttribute : PropertyAttribute {}

    /// <summary>
    /// Use this attribute to draw a property with the ScaleDrawer.
    /// </summary>
    public class ScaleDrawerAttribute : PropertyAttribute { }

    /// <summary>
    /// Use this attribute to draw a property with the optional ThemeDrawer.
    /// </summary>
    public class OptionalThemeDrawerAttribute : PropertyAttribute {}

    /// <summary>
    /// Use this attribute to draw a property with the ThemeDrawer.
    /// </summary>
    public class ThemeDrawerAttribute : PropertyAttribute { }

    /// <summary>
    /// Use this attribute to draw a property with the default PropertyField.
    /// </summary>
    public class DefaultPropertyDrawerAttribute : PropertyAttribute { }
}
