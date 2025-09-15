---
uid: native-integration
---

# Native Integration

App UI supports communication with the operating system through [native plugins](xref:NativePlugins).
For now plugins have been developed for the following platforms:
- [Android](xref:AndroidNativePlugins)
- [iOS](xref:PluginsForIOS)
- [Windows](xref:PluginsForDesktop)
- [macOS](xref:PluginsForDesktop)

## Screen Scale Factor

Thanks to the [Screen](xref:UnityEngine.Screen) class from Unity, you can get information about the screen size and orientation.
However we found that the relationship between the [dpi](xref:UnityEngine.Screen.dpi) and
the [UI Toolkit PanelSettings](xref:UnityEngine.UIElements.PanelSettings) is different from a platform to another.

We provide a [scaleFactor](xref:Unity.AppUI.Core.Platform.scaleFactor) property which gives an accurate scale factor
that takes into account the dpi of the screen but also user-defined display scaling from the operating system.

When you enable the [Auto Scale Mode](xref:setup#app-ui-settings-file) on the UI Toolkit PanelSettings,
the UI elements will be scaled according to the [referenceDpi](xref:Unity.AppUI.Core.Platform.referenceDpi) value which is calculated
using the [scaleFactor](xref:Unity.AppUI.Core.Platform.scaleFactor) property.

Here is a small example of how you can use the [scaleFactor](xref:Unity.AppUI.Core.Platform.scaleFactor)
property when generating custom UI elements content:

```csharp
using Unity.AppUI.Core;
using Unity.AppUI.UI;
using UnityEngine;

class MyCustomElement : VisualElement
{
    RenderTexture m_RT;

    static readonly Vertex[] k_Vertices = new Vertex[4];
    static readonly ushort[] k_Indices = { 0, 1, 2, 2, 3, 0 };

    static MyCustomElement()
    {
        k_Vertices[0].tint = Color.white;
        k_Vertices[1].tint = Color.white;
        k_Vertices[2].tint = Color.white;
        k_Vertices[3].tint = Color.white;
    }

    public MyCustomElement()
    {
        generateVisualContent += OnGenerateVisualContent;
    }

    void OnGenerateVisualContent(MeshGenerationContext ctx)
    {
        var rect = contentRect;
        if (!rect.IsValid())
            return;

        // check the current scale factor to apply on the RenderTexture
        var dpi = Mathf.Max(Platform.scaleFactor, 1f);
        var rectSize = rect.size * dpi;

        // create or re-create the RenderTexture if the size has changed
        if (m_RT && (Mathf.Abs(m_RT.width - rectSize.x) > 1 || Mathf.Abs(m_RT.height - rectSize.y) > 1))
        {
            m_RT.Release();
            UnityObject.Destroy(m_RT);
            m_RT = null;
        }

        if (!m_RT)
        {
            m_RT = new RenderTexture((int) rectSize.x, (int) rectSize.y, 24)
            {
                hideFlags = HideFlags.HideAndDontSave,
            };
            m_RT.Create();
        }

        // TODO: draw the content on the RenderTexture here...

        // Place the RenderTexture in the MeshGenerationContext
        var left = paddingRect.xMin;
        var right = paddingRect.xMax;
        var top = paddingRect.yMin;
        var bottom = paddingRect.yMax;

        k_Vertices[0].position = new Vector3(left, bottom, Vertex.nearZ);
        k_Vertices[1].position = new Vector3(left, top, Vertex.nearZ);
        k_Vertices[2].position = new Vector3(right, top, Vertex.nearZ);
        k_Vertices[3].position = new Vector3(right, bottom, Vertex.nearZ);

        var mwd = mgc.Allocate(k_Vertices.Length, k_Indices.Length, m_RT);

#if !UNITY_2023_1_OR_NEWER
        // Since the texture may be stored in an atlas, the UV coordinates need to be
        // adjusted. Simply rescale them in the provided uvRegion.
        var uvRegion = mwd.uvRegion;
#else
        var uvRegion = new Rect(0, 0, 1, 1);
#endif
        k_Vertices[0].uv = new Vector2(0, 0) * uvRegion.size + uvRegion.min;
        k_Vertices[1].uv = new Vector2(0, 1) * uvRegion.size + uvRegion.min;
        k_Vertices[2].uv = new Vector2(1, 1) * uvRegion.size + uvRegion.min;
        k_Vertices[3].uv = new Vector2(1, 0) * uvRegion.size + uvRegion.min;

        mwd.SetAllVertices(k_Vertices);
        mwd.SetAllIndices(k_Indices);
    }
}
```

## Accessibility Features

### High-Contrast Mode

App UI provides a [highContrast](xref:Unity.AppUI.Core.Platform.highContrast) property that allows you to know if the high-contrast mode is enabled.

It is common to see apps that adapt their UI when the high-contrast mode is enabled.

You can also subscribe to the [highContrastChanged](xref:Unity.AppUI.Core.Platform.highContrastChanged) event in order to be notified
when the high-contrast mode changes.

### Reduced Motion

App UI provides a [reduceMotion](xref:Unity.AppUI.Core.Platform.reduceMotion) property that allows you to know if the reduce motion mode is enabled.

This way you can adapt your animations to be less distracting for users who have this mode enabled.

### Text Scale Factor

In addition to the overall screen scale, you can also get the scale of
the textual elements using the [textScaleFactor](xref:Unity.AppUI.Core.Platform.textScaleFactor) property.

### Layout Direction

App UI provides a [layoutDirection](xref:Unity.AppUI.Core.Platform.layoutDirection) property that allows you to know the layout direction of the system.

Since UI-Toolkit is based on a flexbox layout, you can use this property to adapt your UI to RTL languages
(by using `flexDirection: row-reverse` for example).

## Dark Mode

App UI provides a [darkMode](xref:Unity.AppUI.Core.Platform.darkMode) property that allows you to know if the system theme is dark or light.
We also provide a [darkModeChanged](xref:Unity.AppUI.Core.Platform.darkModeChanged) event that you can subscribe to in order to be notified
when the system theme changes.

It is common to see apps that let the user choose between a light and a dark theme, or stick to the system theme.
You can offer the same feature by using the [darkMode](xref:Unity.AppUI.Core.Platform.darkMode) property.

Here is an example using the [darkMode](xref:Unity.AppUI.Core.Platform.darkMode) property
inside a [RadioGroup](xref:Unity.AppUI.UI.RadioGroup):

```csharp
// Get the closest provider, assuming you call this method from a UI Toolkit element
// contained in a hierarchy that has a ThemeContext provider
var provider = this.GetContextProvider<ThemeContext>();

// Create the callback that will be called when the system theme changes
void OnDarkModeChanged(bool darkMode) => provider.theme = darkMode ? "dark" : "light";

// Setup the RadioGroup
var themeSwitcher = new RadioGroup();
themeSwitcher.Add(new Radio("System") { key = "system" });
themeSwitcher.Add(new Radio("Dark" { key = "dark" }));
themeSwitcher.Add(new Radio("Light" { key = "light" }));
void SetTheme()
{
    Platform.darkModeChanged -= OnDarkModeChanged;
    if (themeSwitcher.value == "system")
    {
        Platform.systemThemeChanged += OnDarkModeChanged;
        provider.theme = Platform.darkMode ? "dark" : "light";
    }
    else
    {
        provider.theme = themeSwitcher.value;
    }
    PlayerPrefs.SetInt("theme", themeSwitcher.value);
}
themeSwitcher.RegisterValueChangedCallback(_ => SetTheme());

// Load and set the theme value
themeSwitcher.SetValueWithoutNotify(PlayerPrefs.GetInt("theme", "dark"));
SetTheme();
```

To know more about theming, see [the dedicated documentation page](xref:theming).

## Haptic Feedback

App UI provides a [RunHapticFeedback](xref:Unity.AppUI.Core.Platform)
method that allows you to trigger haptic feedback on supported platforms.
For now, the only supported platforms are iOS and Android.

> [!NOTE]
> In the current state of App UI components, none of them use haptic feedback directly.
> However, you can use this method to trigger haptic feedback when you need it.


## Clipboard

We provide a way to interact with the Clipboard on the following platforms:
- iOS
- Windows
- macOS
- Linux

You can use [GetPasteboardData](xref:Unity.AppUI.Core.Platform.GetPasteboardData(Unity.AppUI.Core.PasteboardType))
and [SetPasteboardData](xref:Unity.AppUI.Core.Platform.SetPasteboardData(Unity.AppUI.Core.PasteboardType,System.Byte[])) to interact with the Clipboard.

You can also check if the Clipboard currently contains data using the [HasPasteboardData](xref:Unity.AppUI.Core.Platform.HasPasteboardData(Unity.AppUI.Core.PasteboardType)) method.

The supported formats are:
- `Text`: a UTF-8 string
- `PNG`: a PNG image

> [!NOTE]
> On Windows platform, Text content will be automatically converted from/to Unicode (UTF-16LE).
> You only need to deal with UTF-8 encoding in C# code.

```csharp
// Get the text content from the clipboard
byte[] clipboardContent = Platform.GetPasteboardData(PasteboardType.Text);
if (clipboardContent is { Length: >0 })
{
    string text = Encoding.UTF8.GetString(clipboardContent);
    Debug.Log($"Clipboard content: {text}");
}

// Set the text content to the clipboard
string textToCopy = "Hello, World!";
Platform.SetPasteboardData(PasteboardType.Text, Encoding.UTF8.GetBytes(textToCopy));

// Get the image content from the clipboard
byte[] clipboardContent = Platform.GetPasteboardData(PasteboardType.PNG);
if (clipboardContent is { Length: >0 })
{
    Texture2D texture = new Texture2D(2, 2);
    texture.LoadImage(clipboardContent);
    Debug.Log($"Clipboard image size: {texture.width}x{texture.height}");
}

// Set the image content to the clipboard
Texture2D texture = new Texture2D(2, 2);
byte[] imageBytes = texture.EncodeToPNG();
Platform.SetPasteboardData(PasteboardType.PNG, imageBytes);
```
