---
uid: migrate
---

# Migration Guide

## UI Components

### SplitView

The `SplitView` component has been completely refactored and does not inherit from [TwoPaneSplitView](xref:UIElements.TwoPaneSplitView) anymore.
It's a brand new implementation that gives you more flexibility and control over the split view.
This component can now contain any number of [Pane](xref:Unity.AppUI.UI.Pane) elements as children.

```xml
<appui:SplitView style="flex-grow: 1;" direction="Horizontal" realtime-resize="true" show-expand-buttons="true">
    <appui:Pane name="hierarchyPane" style="width: 100px;">
        <!-- Content -->
    </appui:Pane>
    <appui:Pane name="viewportPane" stretch="true">
        <!-- Content -->
    </appui:Pane>
    <appui:Pane name="inspectorPane" style="width: 100px;">
        <!-- Content -->
    </appui:Pane>
</appui:SplitView>
```

### RadioGroup

The `RadioGroup.value` property is no more an `int` but a `string` type. This `string` value is based on the `Radio.key` property. This change allows you to not keep track of the index of the selected radio button but the key of the selected radio button, and avoid issues if you reorder the radio buttons.

Before:
```csharp
var radioGroup = new RadioGroup();
radioGroup.Add(new Radio { label = "Radio 1" });
radioGroup.Add(new Radio { label = "Radio 2" });
radioGroup.value = 1; // Select the second radio button
```

After:
```csharp
var radioGroup = new RadioGroup();
radioGroup.Add(new Radio { label = "Radio 1", key = "key1" });
radioGroup.Add(new Radio { label = "Radio 2", key = "key2" });
radioGroup.value = "key2"; // Select the second radio button
```

### Dropzone

Most of the `Dropzone` logic has been moved into a new `DropzoneController` class. The `Dropzone` class now only handles the visual representation of the dropzone, but you have access to the `DropzoneController` instance to handle the drop logic. Also, insead of trying to get droppables from paths or Unity objects, the controller offers a more generic `acceptDrag` delegate to determine if the dragged object should be accepted. The `dragStarted` event has been removed since the main goal of the Dropzone is to handle the drop event.

Before:
```csharp
var dropzone = new Dropzone();
dropzone.tryGetDroppableFromPath = TryGetDroppableFromPath;
dropzone.tryGetDroppablesFromUnityObjects = TryGetDroppableFromUnityObjects;
dropzone.dropped += OnDropped;
dropzone.dragStarted += OnEditorDragStarted;
dropzone.dragEnded += OnDragEndedOrCanceled;
```

After:
```csharp
var dropzone = new Dropzone();
var dropzoneController = dropzone.controller;
dropzoneController.acceptDrag = ShouldAcceptDrag;
dropzoneController.dropped += OnDropped;
dropzoneController.dragEnded += OnDragEndedOrCanceled;
```

### Toast

In order to add an Action in a Toast component, there was a `Toast.SetAction` method. This method has been removed and replaced by `Toast.AddAction` method which is more consistent with the possibility to remove an action with `Toast.RemoveAction`.

Before:
```csharp
var toast = Toast.Build(/* ... */);
toast.SetAction(DISMISS_ACTION, "Dismiss", () => Debug.Log("Dismissed"));
```

After:
```csharp
var toast = Toast.Build(/* ... */);
toast.AddAction(DISMISS_ACTION, "Dismiss", toastRef => Debug.Log("Dismissed"));
```

## Native Platform Integration

### Themes

Due to the incoming complexity of theme handling in different OS, we limited ourselves to only differentiate between **light** and **dark** themes.

The `Platform.systemThemeChanged` event and `Platform.systemTheme` string property has been replaced by the `Platform.darkModeChanged` event and `Platform.darkMode` boolean property.

Before:
```csharp
Platform.systemThemeChanged += theme => Debug.Log($"Theme changed to {theme}");
if (Platform.systemTheme == "dark")
{
    /* ... */
}
```

After:
```csharp
Platform.darkModeChanged += darkMode => Debug.Log($"Dark mode changed to {darkMode}");
if (Platform.darkMode)
{
    /* ... */
}
```

### Gestures

The `MagnificationGestureEvent` has been replaced by `PinchGestureEvent` to be more consistent with the gesture name inside the new `GestureRecognizer`. The `PanGestureEvent` has been removed and does not have a direct replacement. The UI-Toolkit's `WheelEvent` can be used to handle the pan gesture on most non-touch devices.

Before:
```csharp
public class MyComponent : VisualElement
{
    public MyComponent()
    {
        this.AddManipulator(new MagnificationManipulator());
        this.RegisterCallback<MagnificationGestureEvent>(OnMagnification);
        this.RegisterCallback<PanGestureEvent>(OnPan);
    }

    private void OnMagnification(MagnificationGestureEvent evt)
    {
        /* ... */
    }

    private void OnPan(PanGestureEvent evt)
    {
        /* ... */
    }
}
```

After:
```csharp
public class MyComponent : VisualElement
{
    public MyComponent()
    {
        this.AddManipulator(new PinchManipulator());
        this.RegisterCallback<PinchGestureEvent>(OnPinch);
        this.RegisterCallback<WheelEvent>(OnPan);
    }

    private void OnPinch(PinchGestureEvent evt)
    {
        /* ... */
    }

    private void OnPan(WheelEvent evt)
    {
        /* ... */
    }
}
```

## MVVM

The MVVM API has slightly changed for more flexibility.

### App creation

When inheriting from the `App` class, you can now override the `InitializeComponent` method to set up your visual tree.

Before:
```csharp
public class MyApp : App
{
    public MyApp(MainPage mainPage)
    {
        var panel = new Panel();
        panel.Add(mainPage);
        this.mainPage = panel;
    }
}
```

After:
```csharp
public class MyApp : App
{
    public new static MyApp current => (MyApp)App.current;

    public override void InitializeComponent()
    {
        base.InitializeComponent();
        rootVisualElement.Add(services.GetRequiredService<MainPage>());
    }
}
```

## Redux

The Redux API has been refactored for more flexibility and better performance.
It should become more intuitive to use.

Please read the [State Management](xref:state-management) guide for more information.

### Create a store and slices

When the [Store&lt;TState&gt;](xref:Unity.AppUI.Redux.Store`1) instance is constructed and returned, it is ready to be used.
You cannot add slices to the store after it has been created.

Before:
```csharp
var store = new Store();
store.AddSlice("sliceName", new MySliceState(), builder => { /* ... */ });
store.AddSlice("slice2Name", /* ... */ );
```

After:
```csharp
var store = StoreFactory.CreateStore(new [] {
    StoreFactory.CreateSlice("sliceName", new MySliceState(), builder => { /* ... */ }),
    StoreFactory.CreateSlice("slice2Name", /* ... */ )
    // ...
});
```

### Build switch cases for reducers

When creating slice, a builder callback is passed as argument for both the
primary reducers and the extra reducers of this slice.
For the ease of use, both builders inherit from the same base class. Instead of calling `Add`
method to add a reducer in the primary reducers builder, you can use the `AddCase` method.

The `AddDefault` and `AddMatcher` methods are still only available in the extra reducers builder.

Before:
```csharp
store.AddSlice("sliceName", new MySliceState(), builder => {
    builder.Add("actionType", (state, action) => { /* ... */ });
});
```

After:
```csharp
StoreFactory.CreateSlice("sliceName", new MySliceState(), builder => {
    builder.AddCase("actionType", (state, action) => { /* ... */ });
});
```

Passing a `string` value as actionType will automatically create an action creator for this action type, but
you will have to explicitly cast the string to either a `ActionCreator` or `ActionCreator<T>`.
You can also directly pass an `ActionCreator` or `ActionCreator<T>` instance.

Before:
```csharp
store.AddSlice("sliceName", new MySliceState(), builder => {
    builder.Add("actionType", (state, action) => { /* ... */ });
});
```

After:
```csharp
static readonly ActionCreator actionType0 = "actionType0"; // no payload
static readonly ActionCreator<int> actionType1 = nameof(actionType1); // with int payload

StoreFactory.CreateSlice("sliceName", new MySliceState(), builder => {
    builder.AddCase((ActionCreator<int>)"actionType2", (state, action) => { /* ... */ }); // cast to ActionCreator<int>
});
```

### Action creator declaration

To declare an action creator, you can use the `ActionCreator` or `ActionCreator<T>` classes directly instead of using the `Store.CreateAction` factory method.

We suggest you to use the `string` implicit conversion to `ActionCreator` or `ActionCreator<T>` to declare your action creators.

Before:
```csharp
static readonly ActionCreator actionType0 = Store.CreateAction("actionType0");
static readonly ActionCreator<int> actionType1 = Store.CreateAction<int>("actionType1");
```

After:
```csharp
static readonly ActionCreator actionType0 = "actionType0"; // no payload
static readonly ActionCreator<int> actionType1 = nameof(actionType1); // with int payload
```

### Reducer declaration

Reducers now takes an `IAction` interface type as the second parameter instead of `Action`.

Before:
```csharp
// with no payload
static MyAppState MyReducer1(MyAppState state, Action action) { /* ... */ }
// with int payload
static MyAppState MyReducer2(MyAppState state, Action<int> action) { /* ... */ }
```

After:
```csharp
// with no payload
static MyAppState MyReducer1(MyAppState state, IAction action) { /* ... */ }
// with int payload
static MyAppState MyReducer2(MyAppState state, IAction<int> action) { /* ... */ }
```

### Subscribe to store changes

When subscribing to store changes, instead of returning a function to unsubscribe,
an [IDisposableSubscription](xref:Unity.AppUI.Redux.IDisposableSubscription) instance is returned.
You can check its validity and dispose of it.

Before:
```csharp
Unsubscriber unsub;
// Subscribe to store changes
unsub = store.Subscribe("sliceName", state => { /* ... */ });
// Unsubscribe
unsub();
```

After:
```csharp
IDisposableSubscription subscription;
// Subscribe to store changes
subscription = store.Subscribe("sliceName", state => { /* ... */ });
// Unsubscribe
subscription.Dispose();
// Check if subscription is valid
Assert.IsFalse(subscription.IsValid());
```

## Navigation

### INavVisualController implementation

The `INavVisualController` interface has a new method `SetupNavigationRail` that you have to implement to set up the navigation rail. Like the others setup methods, you can early check if the navigation destination has to use the navigation rail.

Also, `NavDestination` does not have the `showBottombar`, `showAppBar`, and `showDrawer` properties anymore.
These properties have moved into the `DefaultDestinationTemplate` class, set in `NavDestination.destinationTemplate`.
This specialized template type is used with the default `NavigationScreen` which contain the `BottomNavBar`, `AppBar`, `Drawer`, and `NavigationRail` components.

In the case you are creating your own implementation of `INavigationScreen`, make sure to create also an implementation of `NavDestinationTemplate` that contains
all the logic you need to set up your screen creation. Implementing your own `INavigationScreen` means you have to call the `SetupXXX` methods of `INavVisualController` yourself during `OnEnter` callback. See [Navigation](xref:navigation) for more information.

Before:
```csharp
public class MyNavController : INavVisualController
{
    public void SetupBottomNavBar(BottomNavBar bottomNavBar, NavDestination destination, NavController navController)
    {
        if (destination.showBottombar)
        {
            /* ... */
        }
    }

    public void SetupAppBar(AppBar appBar, NavDestination destination, NavController navController)
    {
        if (destination.showAppBar)
        {
            /* ... */
        }
    }

    public void SetupDrawer(Drawer drawer, NavDestination destination, NavController navController)
    {
        if (destination.showDrawer)
        {
            /* ... */
        }
    }
}
```

After:
```csharp
public class MyNavController : INavVisualController
{
    public void SetupBottomNavBar(BottomNavBar bottomNavBar, NavDestination destination, NavController navController)
    {
        if (destination.destinationTemplate is DefaultDestinationTemplate defaultTemplate && defaultTemplate.showBottombar)
        {
            /* ... */
        }
    }

    public void SetupAppBar(AppBar appBar, NavDestination destination, NavController navController)
    {
        if (destination.destinationTemplate is DefaultDestinationTemplate defaultTemplate && defaultTemplate.showAppBar)
        {
            /* ... */
        }
    }

    public void SetupDrawer(Drawer drawer, NavDestination destination, NavController navController)
    {
        if (destination.destinationTemplate is DefaultDestinationTemplate defaultTemplate && defaultTemplate.showDrawer)
        {
            /* ... */
        }
    }

    public void SetupNavigationRail(NavigationRail navigationRail, NavDestination destination, NavController navController)
    {
        if (destination.destinationTemplate is DefaultDestinationTemplate defaultTemplate && defaultTemplate.showNavigationRail)
        {
            // Set up the navigation rail
            // You can access the destination and navController to customize the navigation rail
        }
    }
}
```

## Misc

### StoryBookEnumProperty

The `StoryBookEnumProperty` class has become a generic class to handle any enum type. It is easier to user and avoid casting mistakes.

Before:
```csharp
var enumProperty = new StoryBookEnumProperty(
    nameof(Button.variant),
    (btn) => ((Button)btn).variant,
    (btn, val) => ((Button)btn).variant = (ButtonVariant)val);
```

After:
```csharp
var enumProperty = new StoryBookEnumProperty<ButtonVariant>(
    nameof(Button.variant),
    (btn) => ((Button)btn).variant,
    (btn, val) => ((Button)btn).variant = val);
```