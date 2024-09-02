---
uid: contexts
---

# Contexts

Contexts in App UI are a powerful way to manage application
state in a more predictable and testable way.
Instead of using global variables or singletons,
you can encapsulate state and behavior into smaller,
more focused units called contexts. A context is associated to a specific [VisualElement](xref:UnityEngine.UIElements.VisualElement) and will be propagated down its children.

## Default Contexts

App UI comes with several contexts types that are mandatory for the UI to work properly.

### Theme

The theme context provides a way to customize the appearance of the UI.
You can switch between different themes at runtime, and the UI will
automatically update to reflect the new settings.

Internally the theme context is defined as a USS class.
For example, if the current theme is `dark`, the context will be defined as `dark`
and the corresponding USS class `appui--dark` will be applied to the context provider element
that sets the context.

Currently, App UI provides four themes:

- `light`
- `dark` (default)
- `editor-light`
- `editor-dark`

### Language

The language context provides a way to translate your UI into different languages.
The context will give you the current locale identifier,
which you can use to load the correct translation for your UI.

> [!NOTE]
> The language context works in conjunction with the
> [Unity Localization Package](https://docs.unity3d.com/Packages/com.unity.localization@1.4/manual/index.html).
>
> Localized text elements provided by App UI will automatically
> update their text when the language context changes.

### Scale

The scale context provides a way to adjust the size of the UI based on the device's
pixel density. You can define different scaling factors for different devices,
and the UI will automatically adjust to the appropriate size.

Currently, App UI provides three scaling factors:

- `small`
- `medium` (default)
- `large`

### Layout Direction

The layout direction context provides a way to adjust the layout of the UI based on the
current language. Some languages are read from right to left, so the UI should be mirrored
to reflect this behavior. The possible values are
[LTR](xref:Unity.AppUI.Core.Dir.Ltr) and [RTL](xref:Unity.AppUI.Core.Dir.Rtl).
For more information about layout direction, see the [Accessibility](xref:accessibility) page.

## Context Components

Any component can be a context provider. But App UI provides some components that are designed to be used as context providers.

- [Panel](xref:Unity.AppUI.UI.Panel)
- [ContextProvider](xref:Unity.AppUI.UI.ContextProvider)

## Root Panel

The [App UI Panel](xref:Unity.AppUI.UI.Panel) is a special
[VisualElement](xref:UnityEngine.UIElements.VisualElement) that
must be added to the root of the [UIDocument](xref:UnityEngine.UIElements.UIDocument). It provides access to the global contexts for this [UIDocument](xref:UnityEngine.UIElements.UIDocument).

## Provide and Consume your own Contexts

If you want to create your own context, you can use [IContext](xref:Unity.AppUI.Core.IContext) along with
[ProvideContext](xref:Unity.AppUI.UI.VisualElementExtensions.ProvideContext``1(UnityEngine.UIElements.VisualElement,``0))
and [RegisterContextChangedCallback](xref:Unity.AppUI.UI.VisualElementExtensions.RegisterContextChangedCallback``1(UnityEngine.UIElements.VisualElement,UnityEngine.UIElements.EventCallback{Unity.AppUI.UI.ContextChangedEvent{``0}}))
to provide and consume your own context.

Contexts will be propagated down the visual tree starting at the provider element (included), so this element or any of its child element can access the context.
The propagation will stop as soon as a child element provides a new context of the same type.

The [ContextChangedEvent](xref:Unity.AppUI.UI.ContextChangedEvent`1) will be triggered whenever one of the visual tree ancestors
provides a new context of the same type, or when the listener element is attached to the visual tree panel.

> [!NOTE]
> Passing a `null` context to [ProvideContext](xref:Unity.AppUI.UI.VisualElementExtensions.ProvideContext``1(UnityEngine.UIElements.VisualElement,``0))
> method will remove the context from the provider. The element will not be defined as a context provider anymore, and
> the context from the closest ancestor provider will be propagated down the visual tree (or a `null` context if there is no ancestor provider).

The following example shows how to create a custom context provider

```cs
using Unity.AppUI.Core;
using Unity.AppUI.UI;

// Define a custom context
public record MyContext(int value) : IContext
{
    public int value { get; } = value;
}

// Create a custom context provider
public void CreateContextProvider()
{
    var myElement = new VisualElement();
    myElement.ProvideContext(new MyContext(42));
}

// Consume the custom context
public void ConsumeContext()
{
    var myElement = new VisualElement();
    myElement.RegisterContextChangedCallback<MyContext>(OnMyContextChanged);

    void OnMyContextChanged(ContextChangedEvent<MyContext> evt)
    {
        Debug.Log($"MyContext changed to {evt.context.value}");
    }

    // You can also unregister the callback
    myElement.UnregisterContextChangedCallback<MyContext>(OnMyContextChanged);
}
```

### Custom Components as Context Providers

If you want to create a custom component that is also a context provider or listener, make sure to inherit from the [BaseVisualElement](xref:Unity.AppUI.UI.BaseVisualElement) or [BaseTextElement](xref:Unity.AppUI.UI.BaseTextElement) class.

While this is optional, it will optimize memory usage.

```cs
using Unity.AppUI.Core;
using Unity.AppUI.UI;

public class MyCustomComponent : BaseVisualElement
{
    public MyCustomComponent()
    {
        // Provide a custom context
        ProvideContext(new MyContext(42));

        // Or Register the context changed callback
        // Note that this callback will be also called when the listener element is also a the provider
        RegisterContextChangedCallback<MyContext>(OnMyContextChanged);
    }

    public void OnContextChanged<T>(ContextChangedEvent<T> evt) where T : class, IContext
    {
        // Do something with the context
    }

    void OnMyContextChanged(ContextChangedEvent<MyContext> evt)
    {
        // Do something with the context
    }
}
```