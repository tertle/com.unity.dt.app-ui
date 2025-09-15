---
uid: mvvm-di
---

# Dependency Injection

Dependency injection is a software design pattern that implements inversion of control for resolving dependencies.
It is a technique in which an object receives other objects that it depends on.
These other objects are called dependencies.

In the typical "using" relationship, the receiving object would create or find the dependency.
Instead, in dependency injection, the class itself receives the dependency from an external source.

While Dependency Injection is not tied to the MVVM pattern, it is a very useful pattern to use when building
applications with this kind of architecture.

> [!NOTE]
> The dependency injection pattern provided by the App UI framework is a subset of what you can find in
> .NET Runtime Extensions. For more information, see the
> [.NET Dependency Injection Extension](https://learn.microsoft.com/en-us/dotnet/core/extensions/dependency-injection) documentation.

## Types of Dependencies

There are two types of dependencies that you can register in the App UI framework.

### Transient Dependencies

Transient dependencies are created each time they are requested. This is the default behavior when registering a dependency.

You can use [AddTransient](xref:Unity.AppUI.MVVM.ServicesCollectionExtensions.AddTransient``1(Unity.AppUI.MVVM.IServiceCollection)) to register a transient dependency.

```csharp
using Unity.AppUI.MVVM;

public class MyViewModel : ObservableObject
{
    public MyViewModel(IServiceProvider serviceProvider)
    {
        var transientService = serviceProvider.GetRequiredService<ITransientService>();
        var transientService2 = serviceProvider.GetRequiredService<ITransientService>();
        Debug.Assert(transientService != transientService2);
    }
}
```

### Singleton Dependencies

Singleton dependencies are created the first time they are requested, and then reused for all subsequent requests.

You can use [AddSingleton](xref:Unity.AppUI.MVVM.ServicesCollectionExtensions.AddSingleton``1(Unity.AppUI.MVVM.IServiceCollection)) to register a singleton dependency.

Requesting a singleton service from scoped service providers will return the same instance for all requests.

```csharp
using Unity.AppUI.MVVM;

public class MyViewModel : ObservableObject
{
    public MyViewModel(IServiceProvider serviceProvider)
    {
        var singletonService = serviceProvider.GetRequiredService<ISingletonService>();

        using (var scope = serviceProvider.CreateScope())
        {
            var scopedService = scope.ServiceProvider.GetRequiredService<ISingletonService>();
            Debug.Assert(singletonService == scopedService);
        }
    }
}
```

### Scoped Dependencies

Scoped dependencies are created once per scope. A scope is an object that handles the lifetime of dependencies.

You can use [AddScoped](xref:Unity.AppUI.MVVM.ServicesCollectionExtensions.AddScoped``1(Unity.AppUI.MVVM.IServiceCollection)) to register a scoped dependency.

You can create a scope from a service provider by calling the [CreateScope](xref:Unity.AppUI.MVVM.ServiceProvider.CreateScope) method.

```csharp
using Unity.AppUI.MVVM;

public class MyViewModel : ObservableObject
{
    public MyViewModel(IServiceProvider serviceProvider)
    {
        using (var scope = serviceProvider.CreateScope())
        {
            var scopedService = scope.ServiceProvider.GetRequiredService<IScopedService>();
        }
    }
}
```

## Registering Dependencies

There are several ways to register your classes as dependencies in the App UI framework. The registration process is
done through the [IServiceCollection](xref:Unity.AppUI.MVVM.IServiceCollection) interface.

When building your own [AppBuilder](xref:Unity.AppUI.MVVM.UIToolkitAppBuilder`1) implementation, you can access the service collection
during the [OnConfiguringApp](xref:Unity.AppUI.MVVM.UIToolkitAppBuilder`1.OnConfiguringApp(Unity.AppUI.MVVM.AppBuilder)) event.

```cs
public class MyAppBuilder : UIToolkitAppBuilder<MyApp>
{
    protected override void OnConfiguringApp(AppBuilder appBuilder)
    {
        base.OnConfiguringApp(appBuilder);
        // Register dependencies here
        // ex: appBuilder.services.AddSingleton<IMyService, MyService>();
    }
}
```

### Register the class itself

The simplest way to register a dependency is to register the class itself. This is useful when you only need to
register a single implementation of a class.

```cs
public class MyService
{
    public void DoSomething()
    {
        // Do something
    }
}
// Register the service inside the app builder
public class MyAppBuilder : UIToolkitAppBuilder<MyApp>
{
    protected override void OnConfiguringApp(AppBuilder appBuilder)
    {
        base.OnConfiguringApp(appBuilder);
        appBuilder.services.AddSingleton<MyService>();
    }
}
```

### Register as an Interface

The most common way to register a dependency is to register it as an interface.
This gives you the ability to register multiple implementations of the same interface, and instantiate them
depending on the context.

```cs
public interface IMyService
{
    void DoSomething();
}
public class MyDebugService : IMyService
{
    public void DoSomething()
    {
        // Do something
    }
}
public class MyProductionService : IMyService
{
    public void DoSomething()
    {
        // Do something
    }
}
// Register the services inside the app builder
public class MyAppBuilder : UIToolkitAppBuilder<MyApp>
{
    protected override void OnConfiguringApp(AppBuilder appBuilder)
    {
        base.OnConfiguringApp(appBuilder);

        if (IsDebugMode())
            appBuilder.services.AddSingleton<IMyService, MyDebugService>();
        else
            appBuilder.services.AddSingleton<IMyService, MyProductionService>();
    }
}
```

## Use Dependencies

You have two ways to inject dependencies into your classes: constructor injection and property/field injection.

### Constructor Injection

Constructor injection is the most common way to use dependency injection.
It is used when the dependency is required by the class.
The dependency is passed to the class through its constructor.

```cs
public class MyViewModel : ObservableObject
{
    public MyViewModel(MyModel model)
    {
        Model = model;
    }
    public MyModel Model { get; }
}
```

### Property &amp; Field Injection

Property and field injection is another way to use dependency injection.
This method is used when the dependency can be set after the class is created,
and gives the opportunity to not have too many parameters in the constructor.

Please remind that the target property or field **is still not
initialized** when the constructor of your class is called.

For properties, it is also mandatory to have a **setter**
(although it can be private) to allow the framework to set the value.

To mark injection your property or field for injection,
you need to use the [Service](xref:Unity.AppUI.MVVM.ServiceAttribute) attribute on it.

```cs
using Unity.AppUI.MVVM;

public class MyViewModel : ObservableObject
{
    [Service]
    public MyModel Model { get; set; }
}
```

#### Listen to Dependency Changes

When using property injection, you can listen to changes on the injected property by implementing the
[IDependencyInjectionListener](xref:Unity.AppUI.MVVM.IDependencyInjectionListener) interface.

```cs
using Unity.AppUI.MVVM;

public class MyViewModel : ObservableObject, IDependencyInjectionListener
{
    [Service]
    public MyModel Model { get; private set; }

    public void OnDependenciesInjected()
    {
        // Do something related to the injected dependencies...
    }
}
```

## Conditional Resolution

Conditional resolution allows you to register multiple implementations of the same service interface and choose which implementation to use based on runtime context. This powerful feature enables context-aware dependency injection where different services can receive different implementations of the same interface.

### When to Use Conditional Resolution

Conditional resolution is particularly useful for:

- **Context-specific implementations**: Different loggers for different subsystems
- **Environment-based selection**: Debug vs production implementations
- **Scope-aware services**: Different caching strategies based on scope context
- **Modular architectures**: Feature-specific service implementations

### Registration APIs

The App UI framework provides several methods for registering conditional services:

#### Direct Conditional Registration

Use the `When` methods to register services with conditions:

```csharp
// Register different loggers for different services
services.AddSingletonWhen(typeof(ILogger), typeof(FileLogger),
    ctx => ctx.RequestingType == typeof(DatabaseService));

services.AddSingletonWhen(typeof(ILogger), typeof(ConsoleLogger),
    ctx => ctx.RequestingType == typeof(UIController));

services.AddSingletonWhen(typeof(ILogger), typeof(NetworkLogger),
    ctx => ctx.RequestingType == typeof(ApiClient));
```

### Resolution Context

The `ResolutionContext` provides information about the current resolution request:

| Property | Description |
|----------|-------------|
| `ServiceType` | The interface or service type being requested |
| `RequestingType` | The class that is requesting this service |
| `IsScoped` | Whether the resolution is happening in a scoped context |
| `ServiceProvider` | The service provider performing the resolution |

### Common Conditional Patterns

#### Context-Based Selection

Different implementations based on the requesting class:

```csharp
public class MyAppBuilder : UIToolkitAppBuilder<MyApp>
{
    protected override void OnConfiguringApp(AppBuilder appBuilder)
    {
        base.OnConfiguringApp(appBuilder);

        // Database service gets file logger
        appBuilder.services.AddSingletonWhen(typeof(ILogger), typeof(FileLogger),
            ctx => ctx.RequestingType == typeof(DatabaseService));

        // UI components get console logger
        appBuilder.services.AddSingletonWhen(typeof(ILogger), typeof(ConsoleLogger),
            ctx => ctx.RequestingType?.Name.EndsWith("Controller") == true);

        // Network services get network logger
        appBuilder.services.AddSingletonWhen(typeof(ILogger), typeof(NetworkLogger),
            ctx => ctx.RequestingType?.Name.Contains("Api") == true);
    }
}
```

#### Environment-Based Conditions

Different implementations for different environments:

```csharp
// Debug vs production implementations
services.AddSingletonWhen(typeof(IPaymentProcessor), typeof(MockPaymentProcessor),
    ctx => Application.isEditor);

services.AddSingletonWhen(typeof(IPaymentProcessor), typeof(StripePaymentProcessor),
    ctx => !Application.isEditor);

// Platform-specific implementations
services.AddTransientWhen(typeof(IInputHandler), typeof(MobileInputHandler),
    ctx => Application.isMobilePlatform);

services.AddTransientWhen(typeof(IInputHandler), typeof(DesktopInputHandler),
    ctx => !Application.isMobilePlatform);
```

#### Scope-Aware Selection

Different implementations based on scoped context:

```csharp
// Root scope gets shared cache, scoped contexts get isolated cache
services.AddSingletonWhen(typeof(ICache), typeof(SharedCache),
    ctx => !ctx.IsScoped);

services.AddScopedWhen(typeof(ICache), typeof(IsolatedCache),
    ctx => ctx.IsScoped);
```

### Advanced Usage Examples

#### Service Hierarchy Conditions

```csharp
// All controllers get UI logger
services.AddScopedWhen(typeof(ILogger), typeof(UILogger),
    ctx => ctx.RequestingType?.BaseType?.Name.Contains("Controller") == true);

// All repositories get data logger
services.AddScopedWhen(typeof(ILogger), typeof(DataLogger),
    ctx => ctx.RequestingType?.BaseType?.Name.Contains("Repository") == true);
```

#### Complex Business Logic

```csharp
// Custom condition logic
services.AddTransientWhen(typeof(INotificationService), typeof(EmailNotificationService),
    ctx => {
        // Complex business logic
        var isProductionBuild = !Application.isEditor;
        var isNetworkAvailable = Application.internetReachability != NetworkReachability.NotReachable;
        var isBusinessHours = DateTime.Now.Hour >= 9 && DateTime.Now.Hour <= 17;

        return isProductionBuild && isNetworkAvailable && isBusinessHours;
    });

services.AddTransientWhen(typeof(INotificationService), typeof(LocalNotificationService),
    ctx => Application.isEditor || Application.internetReachability == NetworkReachability.NotReachable);
```

### Usage in ViewModels

Conditional resolution works seamlessly with both constructor and attribute injection:

#### Constructor Injection

```csharp
public class DatabaseViewModel : ObservableObject
{
    private readonly ILogger logger; // Gets FileLogger due to conditional registration

    public DatabaseViewModel(ILogger logger)
    {
        this.logger = logger; // Automatically resolved based on requesting type
    }
}

public class UIController : ObservableObject
{
    private readonly ILogger logger; // Gets ConsoleLogger due to conditional registration

    public UIController(ILogger logger)
    {
        this.logger = logger;
    }
}
```

#### Attribute Injection

```csharp
public class ApiClientService : IDependencyInjectionListener
{
    [Service]
    private ILogger logger; // Gets NetworkLogger due to conditional registration

    public void OnDependenciesInjected()
    {
        logger.Log("API client service initialized");
    }
}
```

### Best Practices

1. **Keep Conditions Simple**: Avoid complex logic in condition delegates for better performance
2. **Use Meaningful Names**: Make condition logic self-documenting
3. **Consider Caching**: Conditional singletons are cached per context automatically
4. **Test Thoroughly**: Ensure all condition branches are tested
5. **Document Context**: Clearly document which services get which implementations

### Performance Considerations

- **Condition Evaluation**: Conditions are evaluated during service resolution
- **Caching Strategy**: Conditional services use context-aware caching
- **Memory Usage**: Each conditional singleton creates separate cache entries
- **Resolution Order**: First matching condition wins, so order registration strategically

### Debugging Conditional Resolution

To debug conditional resolution issues:

1. **Add Logging**: Include debug logs in condition delegates
2. **Check Registration Order**: Ensure desired conditions are registered first
3. **Verify Context**: Confirm `RequestingType` and other context values
4. **Test Isolation**: Test each condition branch separately

```csharp
// Debug conditional registration
services.AddSingletonWhen(typeof(ILogger), typeof(DebugLogger),
    ctx => {
        Debug.Log($"Evaluating condition for {ctx.RequestingType?.Name}");
        var matches = ctx.RequestingType == typeof(MyService);
        Debug.Log($"Condition result: {matches}");
        return matches;
    });
```
