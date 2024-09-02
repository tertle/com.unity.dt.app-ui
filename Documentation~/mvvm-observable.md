---
uid: mvvm-observable
---

# Observables

An observable is an object that can be observed for changes.
In the MVVM pattern, observables are used to notify the UI when a value has changed from the view model.
The UI will then update itself to reflect the new value.

For this reason, it is convenient for ViewModel classes to inherit from
the [ObservableObject](xref:Unity.AppUI.MVVM.ObservableObject) class directly.

An `ObservableObject` is a base class for objects that are observable by implementing the `INotifyPropertyChanged`
and `INotifyPropertyChanging` interfaces. It can be used as a starting point for all kinds of objects that need to
support property change notifications.

> [!NOTE]
> The `ObservableObject` class draws upon the concepts found in the
> [ObservableObject](https://docs.microsoft.com/en-us/dotnet/api/microsoft.toolkit.mvvm.componentmodel.observableobject?view=win-comm-toolkit-dotnet-stable)
> class from the [Windows Community Toolkit](https://docs.microsoft.com/en-us/windows/communitytoolkit/).

## How it works

- It provides a base implementation for `INotifyPropertyChanged` and `INotifyPropertyChanging`,
  exposing the `PropertyChanged` and `PropertyChanging` events.
- It provides a series of `SetProperty` methods that can be used to easily set property values from types inheriting from
  `ObservableObject`, and to automatically raise the appropriate events.
- It exposes the `OnPropertyChanged` and `OnPropertyChanging` methods, which can be overridden in derived types to customize
  how the notification events are raised.

## Usage

### Simple Property

Here's an example of how to implement notification support to a custom property:

```cs
public class MyViewModel : ObservableObject
{
    private string _name;
    public string Name
    {
        get => _name;
        set => SetProperty(ref _name, value);
    }
}
```

The provided `SetProperty<T>(ref T, T, string)` method checks the current value of the property, and updates it if
different, and then also raises the relevant events automatically.
The property name is automatically captured through the use of the `[CallerMemberName]` attribute, so there's no need to
manually specify which property is being updated.

### Wrap a non-observable model

A common scenario, for instance, when working with database items, is to create a wrapping "bindable" model that relays
properties of the database model, and raises the property changed notifications when needed.
This is also needed when wanting to inject notification support to models, that don't implement the `INotifyPropertyChanged`
interface. `ObservableObject` provides a dedicated method to make this process simpler. For the following example,
User is a model directly mapping a database table, without inheriting from `ObservableObject`:

```cs
public class ObservableUser : ObservableObject
{
    private readonly User user;

    public ObservableUser(User user) => this.user = user;

    public string Name
    {
        get => user.Name;
        set => SetProperty(user.Name, value, user, (u, n) => u.Name = n);
    }
}
```

The `SetProperty<TModel, T>(T, T, TModel, Action<TModel, T>)` method is slightly more complex than the previous one,
but it allows to specify a custom action to be invoked when the property value is updated. By giving the previous and
new values, and the model instance, this allows to update the model in a custom way, and then raise the relevant events.

## Additional Resources

- [ObservableObject](https://learn.microsoft.com/en-us/dotnet/communitytoolkit/mvvm/observableobject)
  documentation from the [Windows Community Toolkit](https://docs.microsoft.com/en-us/windows/communitytoolkit/).
