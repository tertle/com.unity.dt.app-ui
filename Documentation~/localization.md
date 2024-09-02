---
uid: localization
---

# Localization

## Introduction

App UI provides localization features through the [**Localization**](https://docs.unity3d.com/Packages/com.unity.localization@1.4/manual/index.html)
Unity package from UPM.

## Language Context

Thanks to the [Context feature](xref:contexts), App UI provides a way to translate your UI into different languages.

By default, the root [Panel](xref:Unity.AppUI.UI.Panel) element will provide an initial [LangContext](xref:Unity.AppUI.Core.LangContext) context to its children.

Here is an example of how to get a the localized string of a given entry using the locale defined inside the [LangContext](xref:Unity.AppUI.Core.LangContext):

```csharp

using Unity.AppUI.UI;

public class MyComponent : BaseVisualElement
{
    public MyComponent()
    {
        this.RegisterContextChangedCallback<LangContext>(OnLangContextChanged);
    }

    // This method will be called when the LangContext changes
    void OnLangContextChanged(ContextChangedEvent<LangContext> evt)
    {
        var ctx = evt.context;
        if (ctx != null)
        {
            var translatedString = LocalizationSettings.Instance.GetLocalizedString("table_name", "entry_key", ctx.locale);
        }
    }

    // This method will check for the currently provided locale
    // without using any listener
    public void TranslateNow()
    {
        var ctx = this.GetContext<LangContext>();
        if (ctx != null)
        {
            var translatedString = LocalizationSettings.Instance.GetLocalizedString("table_name", "entry_key", ctx.locale);
        }
    }
}

```

And if you want to provide a new language to a part of your UI, just provide a new [LangContext](xref:Unity.AppUI.Core.LangContext) to the root element of this part of the UI:

```csharp

using Unity.AppUI.UI;

public class MyComponent : BaseVisualElement
{
    public MyComponent()
    {
        var newLangContext = new LangContext();
        newLangContext.locale = "fr-FR";
        this.AddContext(newLangContext);
    }
}

```

Note that the Localization Unity package also provides features for pluralization and formatting of localized strings.
You can find more information about these features in [their package documentation](https://docs.unity3d.com/Packages/com.unity.localization@1.4/manual/index.html).

## App UI Localized elements

Every App UI element that displays text supports localization.
If an element has a `text`, `title` or `label` property for example, you can define its value with a string starting with `@`,
which enables localization for the element. The string value is then used as the resource key for the localized string.
If the resource key is not found, the string value is used as the displayed text.

The naming convention for the resource key is the following:

```
@<table_name>:<entry_key>
```

Here is an example of how to use localization with a button:

* UXML
  ```xml
  <appui:Button title="@table_name:entry_key" />
  ```

* C#
  ```csharp
  myButton.title = "@table_name:entry_key";
  ```
