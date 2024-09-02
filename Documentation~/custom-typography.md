---
uid: custom-typography
---

# Custom Typography

The App UI package provides a way to customize the typography of your application.

By default, App UI uses [Inter](https://rsms.me/inter/) as the main font. The available font weights are `Regular` and `SemiBold`.

Theses weights are defined in the Unity Stylesheet (USS) file `Global.uss` located in the `Packages/com.unity.dt.app-ui/PackageResources/Styles/Themes` folder.

```css
:root {
    --appui-font-weights-100: url("../../Fonts/Inter-Regular.ttf");
    --appui-font-weights-200: url("../../Fonts/Inter-SemiBold.ttf");
}
```

If you want to use a different font, you can replace the `url` value with the path to your font file. You can also create a Unity Font Asset to setup fallbacks when a font doesn't support a specific character. See [Unity documentation](https://docs.unity3d.com/Manual/class-Font.html) for more information.