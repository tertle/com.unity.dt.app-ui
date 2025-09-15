---
uid: mvvm-command
---

# Commanding

In the Model-View-ViewModel (MVVM) architectural pattern, commanding plays a crucial role in handling user interactions
and controlling the flow of commands between different components of the application.
This documentation page explores the commanding patterns in MVVM using App UI.

For a more detailed example of how to use commanding in App UI, see the [MVVM & Redux Sample](xref:mvvm-redux-sample).

## Introduction

Commanding in MVVM refers to the mechanism of connecting user interface elements (usually buttons, menu items, etc.)
with actions in the ViewModel. This separation allows for better testability, maintainability, and flexibility
in handling user inputs.

In App UI, like most other MVVM frameworks, commanding is implemented using the `ICommand` interface provided by the
`System.Windows.Input` namespace. The `ICommand` interface defines two methods: `CanExecute` and `Execute`.

## Commands

### RelayCommand

The [RelayCommand](xref:Unity.AppUI.MVVM.RelayCommand) class is a simple implementation of the `ICommand` interface.

You can create a `RelayCommand` instance by passing a delegate to the constructor. The delegate will be invoked when the
command is executed. Optionally, you can also pass a delegate to the constructor to determine whether the command can be
executed.

```cs
public class MyViewModel : ObservableObject
{
    public RelayCommand MyCommand { get; }

    public MyViewModel()
    {
        MyCommand = new RelayCommand(ExecuteMyCommand, CanExecuteMyCommand);
    }

    private void ExecuteMyCommand()
    {
        // Do something
    }

    private bool CanExecuteMyCommand()
    {
        // Return true or false depending on whether the command can be executed
    }
}
```

You can also use the generic version of `RelayCommand` to pass a parameter to the command delegate.

```cs
public class MyViewModel : ObservableObject
{
    public RelayCommand<string> MyCommand { get; }

    public MyViewModel()
    {
        MyCommand = new RelayCommand<string>(ExecuteMyCommand, CanExecuteMyCommand);
    }

    private void ExecuteMyCommand(string parameter)
    {
        // Do something
    }

    private bool CanExecuteMyCommand(string parameter)
    {
        // Return true or false depending on whether the command can be executed
    }
}
```

### AsyncRelayCommand

The [AsyncRelayCommand](xref:Unity.AppUI.MVVM.AsyncRelayCommand) class is the asynchronous version of `RelayCommand`.

> [!IMPORTANT]
> The `AsyncRelayCommand` class is not supported yet in WebGL builds.

You can create an `AsyncRelayCommand` instance by passing an asynchronous delegate to the constructor. The delegate will
be invoked when the command is executed. Optionally, you can also pass a delegate to the constructor to determine
whether the command can be executed.

You can also specify if the command accepts concurrent executions. By default, the command does not accept concurrent
executions. If the command is already executing and the user invokes the command again, the second execution will be
ignored. If you want to allow concurrent executions, you can set this option using [AsyncRelayCommandOptions](xref:Unity.AppUI.MVVM.AsyncRelayCommandOptions).

```cs
public class MyViewModel : ObservableObject
{
    public AsyncRelayCommand MyCommand { get; }

    public MyViewModel()
    {
        MyCommand = new AsyncRelayCommand(
            ExecuteMyCommand,
            CanExecuteMyCommand,
            AsyncRelayCommandOptions.AllowConcurrentExecutions);
    }

    private async Task ExecuteMyCommand()
    {
        // Do something
    }

    private bool CanExecuteMyCommand()
    {
        // Return true or false depending on whether the command can be executed
    }
}
```

Like `RelayCommand`, you can also use the generic version of `AsyncRelayCommand` to pass a parameter to the command delegate.

```cs
public class MyViewModel : ObservableObject
{
    public AsyncRelayCommand<string> MyCommand { get; }

    public MyViewModel()
    {
        MyCommand = new AsyncRelayCommand<string>(
            ExecuteMyCommand,
            CanExecuteMyCommand,
            AsyncRelayCommandOptions.AllowConcurrentExecutions);
    }

    private async Task ExecuteMyCommand(string parameter)
    {
        // Do something
    }

    private bool CanExecuteMyCommand(string parameter)
    {
        // Return true or false depending on whether the command can be executed
    }
}
```

## ICommand Attribute

You can use the [ICommand](xref:Unity.AppUI.MVVM.ICommandAttribute) attribute to mark a method as a command in the ViewModel.
When you use the `ICommand` attribute, the method will be automatically wrapped in a `RelayCommand` instance.

Example usage:
```cs
[ObservableObject]
public partial class MyViewModel
{
    [ICommand]
    void DoSomething() { /* Your logic here */ }

    [ICommand]
    Task DoSomethingAsync(string textParameter, CancellationToken token) { /* Your logic here */ }
}
```

Will generate:
```csharp
public partial class MyViewModel
{
    RelayCommand m_DoSomethingCommand;
    public RelayCommand DoSomethingCommand => m_DoSomethingCommand ??= new RelayCommand(DoSomething);

    AsyncRelayCommand<string> m_DoSomethingAsyncCommand;
    public AsyncRelayCommand<string> DoSomethingAsyncCommand => m_DoSomethingAsyncCommand ??= new AsyncRelayCommand<string>(DoSomethingAsync);
}
```

## Command Binding

Usually in MVVM, you will want to bind a command to a user interface element. In App UI, you can do this by listening to
events triggered by the user interface element and invoking the command when the event is triggered.

For example, you can bind a command to a button by listening to the `clicked` event of a [Button](xref:Unity.AppUI.UI.Button).

```cs
myButton.clicked += () => MyCommand.Execute();
```

You can also bind the `CanExecuteChanged` event of the command to to enable or disable the button depending on whether
the command can be executed.

```cs
MyCommand.CanExecuteChanged += (sender, args) => myButton.SetEnabled(MyCommand.CanExecute());
```

> [!NOTE]
> It is up to the user to call `NotifyCanExecuteChanged` when the state of the command should change.

You can also bind commands via UXML or UI Builder to any UI element that provides a `clickable.command` property.

Here is an example of how to bind a command to a button in UXML:

```xml
<ui:UXML
    xmlns:ui="UnityEngine.UIElements"
    xmlns:appui="Unity.AppUI.UI">
    <appui:Panel
        data-source-type="MyNamespace.ClickableScript, Assembly-CSharp"
        data-source="project://database/Assets/MyAsset.asset">
        <appui:Button title="Click me!">
            <Bindings>
                <ui:DataBinding
                    property="clickable.command"
                    binding-mode="ToTarget"
                    data-source-path="clickCommand"/>
                <ui:DataBinding
                    property="enabledSelf"
                    binding-mode="ToTarget"
                    data-source-path="canExecute"/>
            </Bindings>
        </appui:Button>
    </appui:Panel>
</ui:UXML>
```

And the corresponding C# code to create the `ClickableScript` asset:

```csharp
using System;
using System.Threading.Tasks;
using Unity.AppUI.MVVM;
using UnityEngine;
using Unity.Properties;
using UnityEngine.UIElements;

namespace MyNamespace
{
    [CreateAssetMenu(fileName = "MyAsset", menuName = "ClickableScript")]
    public class ClickableScript : ScriptableObject, INotifyBindablePropertyChanged
    {
        static readonly BindingId canExecuteProperty = nameof(canExecute);

        [CreateProperty(ReadOnly = true)]
        public AsyncRelayCommand clickCommand { get; private set; }

        [CreateProperty(ReadOnly = true)]
        public bool canExecute => clickCommand.CanExecute(null);

        void Awake()
        {
            clickCommand = new AsyncRelayCommand(OnClick, AsyncRelayCommandOptions.None);
            clickCommand.CanExecuteChanged += (sender, args) =>
            {
                // Notify the UI that the command can execute state has changed
                propertyChanged.Invoke(
                    this,
                    new BindablePropertyChangedEventArgs(in canExecuteProperty));
            };
        }

        async Task OnClick()
        {
            await Task.Delay(1000); // Simulate some async work
            Debug.Log("Button clicked!");
        }

        public event EventHandler<BindablePropertyChangedEventArgs> propertyChanged;
    }
}
```
