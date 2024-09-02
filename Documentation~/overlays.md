---
uid: overlays
---

# Overlays

Overlays are UI components that are displayed on top of the existing UI,
often used to display extra information, modal dialogs, or notifications.
App UI provides an overlay system to facilitate the creation of these components.

## The layering system

App UI's overlay system uses a layering system to determine the z-order of the overlays.
Each overlay belongs to a specific layer,
and the layers are ordered from lowest to highest.
Overlays in higher layers are displayed on top of overlays in lower layers.

The following layers are defined by default:
- `main-content`: The main content of the application.
  This is the default layer for the UI.
- `popup`: The layer for popups, such as modal dialogs or menus.
- `notification`: The layer for notifications, such as toasts or banners.
- `tooltip`: The layer for tooltips.

## Overlay components

The [Popup](xref:Unity.AppUI.UI.Popup)
class is the base class for all overlay components in App UI.
It provides the basic functionality for displaying and hiding an overlay.

To create and show a Popup element in your UI,
you need to use the `Build` method associated to the desired Popup type.

```csharp
var popover = Popover.Build(target, content)
        .SetPlacement(placement)
        .SetShouldFlip(shouldFlip)
        .SetOffset(offset)
        .SetCrossOffset(crossOffset)
        .SetArrowVisible(showArrow)
        .SetContainerPadding(containerPadding)
        .SetOutsideClickDismiss(outsideClickDismissEnabled)
        .SetModalBackdrop(modalBackdrop)
        .SetKeyboardDismiss(keyboardDismissEnabled);
popover.Show();

var modal = Modal.Build(content)
        .SetFullScreenMode(ModalFullScreenMode.None)
        // ...
```

### Popover

The [Popover](xref:Unity.AppUI.UI.Popover)
class is a popup that is displayed next to a target element.
It can display a small arrow pointing to this target.

By default, the popover's backdrop is transparent,
and doesn't block the user from interacting with the rest of the UI.
However, you can configure the popover to display a backdrop that works as a modal one.


> [!IMPORTANT]
> On small screen devices, it is recommended to use
> a [Tray](xref:Unity.AppUI.UI.Tray) instead of a Popover in most cases.

### Modal

The [Modal](xref:Unity.AppUI.UI.Modal)
class is a popup that is displayed as a modal dialog.
It displays a backdrop that blocks the user from interacting with the rest of the UI.
The dialog is positioned in the center of the screen. Its size is configurable,
and can take over the whole screen if needed.

By definition, you should use a Modal to display a dialog
that requires user interaction/decision.

See [Dialogs](xref:layouts#dialogs) for more information.

### Menu

The [MenuBuilder](xref:Unity.AppUI.UI.MenuBuilder)
class is a popover that is displayed next to a target element.
It displays a list of MenuItems, and can be used to display a submenus.
The MenuBuilder class is used to create a Menu via a fluent API:

```cs
MenuBuilder.Build(anchor)
    .AddAction(123, "An Item", "info", evt => Debug.Log("Item clicked"))
    .PushSubMenu(456, "My Sub Menu", "help")
        .AddAction(789, "Sub Menu Item", "info", evt => Debug.Log("Sub Item clicked"))
        .PushSubMenu(3455, "Another Sub Menu", "help")
            .AddAction(7823129, "Another Sub Menu Item", "info", evt => Debug.Log("Other Item clicked"))
        .Pop()
    .Pop()
    .Show();
```

> [!NOTE]
> Menus are displayed as Popovers on small screen devices for the moment.
> This will be changed in the future to display them as a Tray.

### Tray

The [Tray](xref:Unity.AppUI.UI.Tray)
class is a popup that is displayed at the bottom of the screen.
It displays a backdrop that blocks the user from interacting with the rest of the UI.
The tray is positioned at the bottom of the screen, and its height is configurable.

The Tray is mainly used on small screen devices,
to display a menu or a list of actions.


### Notifications

App UI provides a Notification system based on a single message queue.
That means you can use different [UIDocument](xref:UnityEngine.UIElements.UIDocument)
to trigger and display notifications.

For now, only Toasts are supported.

#### Toast

The [Toast](xref:Unity.AppUI.UI.Toast)
class is a popup that is displayed at the edge of the screen.
A Toast that doesn't require user interaction/decision is displayed for a few seconds,
and then automatically hides itself.
A Toast that requires user interaction/decision is displayed until the user interacts with it.

> [!NOTE]
> App UI decided to follow the Android design guidelines for Toasts.
> So only one Toast can be displayed at a time,
> and the new Toast will replace the previous one (even if it required user interaction).


### Tooltip

The [Tooltip](xref:Unity.AppUI.UI.Tooltip)
class is a popup that is displayed next to a target element.
The tooltip doesn't require user interaction/decision,
and is displayed as long as the target element is hovered.

In our layering system, the Tooltip layer is the highest one.

