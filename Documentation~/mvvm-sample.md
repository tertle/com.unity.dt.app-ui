---
uid: mvvm-sample
---

# MVVM Sample

The MVVM Sample shows how to adopt a MVVM architecture with the App UI Unity package.
If you are not familiar with MVVM, we suggest you to read the [MVVM documentation](xref:mvvm-intro).

> [!NOTE]
> The sample doesn't contain the concept of Repository and the state management is done inside the ViewModel for simplicity.
> A more complex sample is available in the [MVVM & Redux Sample](xref:mvvm-redux-sample) section.

## Getting Started

### Installation

To use the MVVM sample, you will need to have this package installed in your project.

To install the package, follow the instructions in the [Installation and Setup](xref:setup)
section of the documentation.

Inside the Unity Package Manager window, select the **App UI** package, then
go to **Samples** and select **MVVM**. Click **Install** to install the sample.

### Usage

To open the sample, in your Project panel go to
**Assets > Samples > App UI > MVVM > Scenes** and open the **MVVM** scene.

If you go in Play mode, you will see a simple UI with a button that increments a counter.

The scene contains one special component attached to the **[UIDocument](xref:UnityEngine.UIElements.UIDocument)** game object.
This component is called **MyAppBuilder** and is an implementation of the [UIToolkitAppBuilder](xref:Unity.AppUI.MVVM.UIToolkitAppBuilder`1) class.

With this **MyAppBuilder** class, you can define which [App](xref:Unity.AppUI.MVVM.App) implementation to use for your runtime app and
hook some events about the app lifecycle.

```cs
public class MyAppBuilder : UIToolkitAppBuilder<MyApp>
{
    protected override void OnAppInitialized(MyApp app)
    {
        base.OnAppInitialized(app);
        // Called after the app is initialized
    }

    protected override void OnConfiguringApp(AppBuilder builder)
    {
        base.OnConfiguringApp(builder);
        // Called during app configuration
    }

    protected override void OnAppShuttingDown(MyApp app)
    {
        base.OnAppShuttingDown(app);
        // Called before the app is shut down
    }
}
```

The **MyApp** class is an implementation of the [App](xref:Unity.AppUI.MVVM.App) class and is the entry point of your app.
It's a good place to define your main UI which will construct ViewModels and Views implicitly thanks to the Dependency Injection system.

```cs
public class MyApp : App
{
    public new static MyApp current => (MyApp)App.current;

    public override void InitializeComponent()
    {
        base.InitializeComponent();
        rootVisualElement.Add(services.GetRequiredService<MainPage>());
    }

    public override void Shutdown()
    {
        // Called when the app is shutting down
    }
}
```

The **MainPage** class is a [VisualElement](xref:UnityEngine.UIElements.VisualElement)
that will be constructed by the Dependency Injection system. It is defined as a **View** in the MVVM pattern.

```cs
public class MainPage : VisualElement
{
    public MainPage(MainViewModel viewModel) // <- Constructor injection, viewModel will be provided as a service
    {
        // Construct the UI and store the view model
    }
}
```

The **MainViewModel** class is a [ObservableObject](xref:Unity.AppUI.MVVM.ObservableObject) that will also be
constructed by the Dependency Injection system. It is defined as a **ViewModel** in the MVVM pattern.

```cs
public class MainViewModel : ObservableObject
{
    public MainViewModel()
    {
        // Construct the view model
    }

    // Define properties and commands
}
```
