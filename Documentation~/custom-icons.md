---
uid: custom-icons
---

# Custom Icons

Every icon provided by App UI is available as a PNG file.
They are all referenced inside USS files, which are used to load the icons.

## Adding Custom Icons

To add a custom icon, you need to add a PNG file to your project and reference it in a USS file.

The naming convention used for the USS class name is `appui-icon--<name>--<variant in lowercase>`.

The variant by default is `Regular`, but you can use the following variants:

- `Regular`
- `Bold`
- `Light`
- `DuoTone`
- `Thin`
- `Fill`

Here's an example of how to add a custom icon named `home` with the `Regular` variant:

```css
.appui-icon--home--regular {
    --unity-image: url("path/to/home.png");
}
```

> [!IMPORTANT]
> Your USS class name must start with `appui-icon--` followed by the name of your icon
> in order to work with the [Icon](xref:Unity.AppUI.UI.Icon) UI component.

## Using Phosphor Icons

Phosphor Icons is a set of over 2,000 open-source icons, designed for the modern web.
Each icon is designed on a 24x24 grid with an emphasis on simplicity, consistency, and flexibility.

To use Phosphor Icons, you need to add the `com.unity.replica.phosphor-icons` package to your project.

Then, you can reference the icons in your USS files.

Here's an example of how to use the `horse` icon:

```css
.appui-icon--horse--regular {
    --unity-image: url("/Packages/com.unity.replica.phosphor-icons/PackageResources/Icons/regular/horse.png");
}
```

Then you can use the `Icon` UI component to display the icon:

```xml
<appui:Icon name="horse" variant="Regular" />
```

## Icon Browser

The **Icon Browser** is an Editor tool that allows you to generate the USS classes
for your custom icons.

To open the **Icon Browser**, go to `Window > App UI > Icon Browser`.

<p align="center">
  <img src="images/icon-browser.png" alt="Icon Browser">
</p>

To use this tool you need to work on an existing USS file asset that includes only icons.
If you don't have one, you can create a new one by right-clicking in the Project window
and selecting `Create > App UI > Icon Stylesheet`.
You can also directly click on `Create a new App UI Icons style sheet`
button in the **Icon Browser** window.

Make sure the stylesheet (USS) asset is set in the **Icon Browser** window.

To include new Icons in this stylesheet, you can:
- Drag and drop texture assets from the Project window to the **Icon Browser** window.
- Click on the `Add Icons...` button in the **Icon Browser** window and select the texture asset.

> [!NOTE]
> When adding already existing texture assets as Icons via the **Icon Browser**,
> the tool expects that the **name of the folder containing the texture** asset is
> the **name of the icon's variant**.
> For example, if you have a texture asset named `home.png` in a folder named `Regular`,
> the tool will generate the USS class `appui-icon--home--regular`.

> [!NOTE]
> App UI provides a set of icons that you can use in your project.
> You can find them in the `Packages/com.unity.dt.app-ui/PackageResources/Icons` folder.