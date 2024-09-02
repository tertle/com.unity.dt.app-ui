// This file draws upon the concepts found in the AsyncRelayCommand implementation from the MVVM Toolkit library (CommunityToolkit/dotnet),
// more information in Third Party Notices.md

#nullable enable
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Unity.AppUI.MVVM
{
    /// <summary>
    /// Options for <see cref="AsyncRelayCommand"/>.
    /// </summary>
    [Flags]
    public enum AsyncRelayCommandOptions
    {
        /// <summary>
        /// No options.
        /// </summary>
        None,

        /// <summary>
        /// Allow concurrent executions.
        /// </summary>
        AllowConcurrentExecutions
    }

    /// <summary>
    /// A command that mirrors the functionality of <see cref="RelayCommand"/>, with the addition of
    /// accepting a <see cref="Func{TResult}"/> returning a <see cref="Task"/> as the execute
    /// action, and providing an <see cref="executionTask"/> property that notifies changes when
    /// <see cref="ExecuteAsync"/> is invoked and when the returned <see cref="Task"/> completes.
    /// </summary>
    public class AsyncRelayCommand : IAsyncRelayCommand
    {
        readonly Func<Task>? m_Execute;

        readonly Func<CancellationToken, Task>? m_CancellableExecute;

        readonly Func<bool>? m_CanExecute;

        readonly AsyncRelayCommandOptions m_Options;

        CancellationTokenSource? m_CancellableTokenSource;

        Task? m_ExecutionTask;

        /// <summary>
        /// The arguments for the <see cref="PropertyChanged"/> event raised by the <see cref="executionTask"/> property.
        /// </summary>
        public static readonly PropertyChangedEventArgs ExecutionTaskChangedEventArgs = new (nameof(executionTask));

        /// <summary>
        /// The arguments for the <see cref="PropertyChanged"/> event raised by the <see cref="isRunning"/> property.
        /// </summary>
        public static readonly PropertyChangedEventArgs IsRunningChangedEventArgs = new (nameof(isRunning));

        /// <summary>
        /// The arguments for the <see cref="PropertyChanged"/> event raised by the <see cref="canBeCancelled"/> property.
        /// </summary>
        public static readonly PropertyChangedEventArgs CanBeCanceledChangedEventArgs = new (nameof(canBeCancelled));

        /// <summary>
        /// The arguments for the <see cref="PropertyChanged"/> event raised by the <see cref="isCancellationRequested"/> property.
        /// </summary>
        public static readonly PropertyChangedEventArgs IsCancellationRequestedChangedEventArgs = new (nameof(isCancellationRequested));

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncRelayCommand"/> class.
        /// </summary>
        /// <param name="execute"> The <see cref="Func{TResult}"/> to invoke when <see cref="ExecuteAsync"/> is called.</param>
        /// <exception cref="ArgumentNullException"> If the execute argument is null.</exception>
        public AsyncRelayCommand(Func<Task> execute)
        {
            m_Execute = execute ?? throw new ArgumentNullException(nameof(execute));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncRelayCommand"/> class.
        /// </summary>
        /// <param name="execute"> The <see cref="Func{TResult}"/> to invoke when <see cref="ExecuteAsync"/> is called.</param>
        /// <param name="options"> The <see cref="AsyncRelayCommandOptions"/> to use.</param>
        /// <exception cref="ArgumentNullException"> If the execute argument is null.</exception>
        public AsyncRelayCommand(Func<Task> execute, AsyncRelayCommandOptions options)
        {
            m_Execute = execute ?? throw new ArgumentNullException(nameof(execute));
            m_Options = options;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncRelayCommand"/> class.
        /// </summary>
        /// <param name="cancellableExecute"> The <see cref="Func{TResult}"/> to invoke when <see cref="ExecuteAsync"/> is called.</param>
        /// <exception cref="ArgumentNullException"> If the execute argument is null.</exception>
        public AsyncRelayCommand(Func<CancellationToken, Task> cancellableExecute)
        {
            m_CancellableExecute = cancellableExecute ?? throw new ArgumentNullException(nameof(cancellableExecute));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncRelayCommand"/> class.
        /// </summary>
        /// <param name="cancellableExecute"> The <see cref="Func{TResult}"/> to invoke when <see cref="ExecuteAsync"/> is called.</param>
        /// <param name="options"> The <see cref="AsyncRelayCommandOptions"/> to use.</param>
        /// <exception cref="ArgumentNullException"> If the execute argument is null.</exception>
        public AsyncRelayCommand(Func<CancellationToken, Task> cancellableExecute, AsyncRelayCommandOptions options)
        {
            m_CancellableExecute = cancellableExecute ?? throw new ArgumentNullException(nameof(cancellableExecute));
            m_Options = options;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncRelayCommand"/> class.
        /// </summary>
        /// <param name="execute"> The <see cref="Func{TResult}"/> to invoke when <see cref="ExecuteAsync"/> is called.</param>
        /// <param name="canExecute"> The <see cref="Func{TResult}"/> to invoke when <see cref="CanExecute()"/> is called.</param>
        /// <exception cref="ArgumentNullException"> If the execute argument is null.</exception>
        public AsyncRelayCommand(Func<Task> execute, Func<bool> canExecute)
        {
            m_Execute = execute ?? throw new ArgumentNullException(nameof(execute));
            m_CanExecute = canExecute ?? throw new ArgumentNullException(nameof(canExecute));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncRelayCommand"/> class.
        /// </summary>
        /// <param name="execute"> The <see cref="Func{TResult}"/> to invoke when <see cref="ExecuteAsync"/> is called.</param>
        /// <param name="canExecute"> The <see cref="Func{TResult}"/> to invoke when <see cref="CanExecute()"/> is called.</param>
        /// <param name="options"> The <see cref="AsyncRelayCommandOptions"/> to use.</param>
        /// <exception cref="ArgumentNullException"> If the execute argument is null.</exception>
        public AsyncRelayCommand(Func<Task> execute, Func<bool> canExecute, AsyncRelayCommandOptions options)
        {
            m_Execute = execute ?? throw new ArgumentNullException(nameof(execute));
            m_CanExecute = canExecute ?? throw new ArgumentNullException(nameof(canExecute));
            m_Options = options;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncRelayCommand"/> class.
        /// </summary>
        /// <param name="cancellableExecute"> The <see cref="Func{TResult}"/> to invoke when <see cref="ExecuteAsync"/> is called.</param>
        /// <param name="canExecute"> The <see cref="Func{TResult}"/> to invoke when <see cref="CanExecute()"/> is called.</param>
        /// <exception cref="ArgumentNullException"> If the execute argument is null.</exception>
        public AsyncRelayCommand(Func<CancellationToken, Task> cancellableExecute, Func<bool> canExecute)
        {
            m_CancellableExecute = cancellableExecute ?? throw new ArgumentNullException(nameof(cancellableExecute));
            m_CanExecute = canExecute ?? throw new ArgumentNullException(nameof(canExecute));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncRelayCommand"/> class.
        /// </summary>
        /// <param name="cancellableExecute"> The <see cref="Func{TResult}"/> to invoke when <see cref="ExecuteAsync"/> is called.</param>
        /// <param name="canExecute"> The <see cref="Func{TResult}"/> to invoke when <see cref="CanExecute()"/> is called.</param>
        /// <param name="options"> The <see cref="AsyncRelayCommandOptions"/> to use.</param>
        /// <exception cref="ArgumentNullException"> If the execute argument is null.</exception>
        public AsyncRelayCommand(Func<CancellationToken, Task> cancellableExecute, Func<bool> canExecute, AsyncRelayCommandOptions options)
        {
            m_CancellableExecute = cancellableExecute ?? throw new ArgumentNullException(nameof(cancellableExecute));
            m_CanExecute = canExecute ?? throw new ArgumentNullException(nameof(canExecute));
            m_Options = options;
        }

        /// <summary>
        /// Determines whether this <see cref="AsyncRelayCommand"/> can execute in its current state.
        /// </summary>
        /// <param name="parameter"> The parameter to use when determining whether this <see cref="AsyncRelayCommand"/> can execute.</param>
        /// <returns> <see langword="true"/> if this <see cref="AsyncRelayCommand"/> can execute; otherwise, <see langword="false"/>.</returns>
        public bool CanExecute(object? parameter)
        {
            var canExecute = m_CanExecute?.Invoke() ?? true;
            return canExecute &&
                ((m_Options & AsyncRelayCommandOptions.AllowConcurrentExecutions) != 0 || executionTask is not { IsCompleted: false });
        }

        /// <summary>
        /// Determines whether this <see cref="AsyncRelayCommand"/> can execute in its current state.
        /// </summary>
        /// <returns> <see langword="true"/> if this <see cref="AsyncRelayCommand"/> can execute; otherwise, <see langword="false"/>.</returns>
        public bool CanExecute() => CanExecute(null);

        /// <summary>
        /// Executes the <see cref="AsyncRelayCommand"/> on the current command target.
        /// </summary>
        /// <param name="parameter"> The parameter to use when executing the <see cref="AsyncRelayCommand"/>.</param>
        public void Execute(object? parameter)
        {
            if (!CanExecute(parameter))
                return;

            ExecuteAsync(parameter);
        }

        /// <summary>
        /// Executes the <see cref="AsyncRelayCommand"/> on the current command target.
        /// </summary>
        public void Execute() => Execute(null);

        /// <summary>
        /// Event that is raised when changes occur that affect whether or not the <see cref="AsyncRelayCommand"/> should execute.
        /// </summary>
        public event EventHandler? CanExecuteChanged;

        /// <summary>
        /// Raises the <see cref="CanExecuteChanged"/> event.
        /// </summary>
        public void NotifyCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Event that is raised when the <see cref="executionTask"/> property changes.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Gets the <see cref="Task"/> that represents the asynchronous operation.
        /// </summary>
        public Task? executionTask
        {
            get => m_ExecutionTask;
            set
            {
                if (ReferenceEquals(m_ExecutionTask, value))
                    return;

                m_ExecutionTask = value;
                PropertyChanged?.Invoke(this, AsyncRelayCommand.ExecutionTaskChangedEventArgs);
                PropertyChanged?.Invoke(this, AsyncRelayCommand.IsRunningChangedEventArgs);

                var isAlreadyCompletedOrNull = value?.IsCompleted ?? true;

                if (m_CancellableTokenSource is not null)
                {
                    PropertyChanged?.Invoke(this, AsyncRelayCommand.CanBeCanceledChangedEventArgs);
                    PropertyChanged?.Invoke(this, AsyncRelayCommand.IsCancellationRequestedChangedEventArgs);
                }

                if (isAlreadyCompletedOrNull)
                    return;

                static async void MonitorTask(AsyncRelayCommand command, Task task)
                {
                    await task;
                    if (ReferenceEquals(command.executionTask, task))
                    {
                        command.PropertyChanged?.Invoke(command, AsyncRelayCommand.ExecutionTaskChangedEventArgs);
                        command.PropertyChanged?.Invoke(command, AsyncRelayCommand.IsRunningChangedEventArgs);

                        if (command.m_CancellableTokenSource is not null)
                        {
                            command.PropertyChanged?.Invoke(command, AsyncRelayCommand.CanBeCanceledChangedEventArgs);
                        }

                        if ((command.m_Options & AsyncRelayCommandOptions.AllowConcurrentExecutions) == 0)
                        {
                            command.CanExecuteChanged?.Invoke(command, EventArgs.Empty);
                        }
                    }
                }

                MonitorTask(this, value!);
            }
        }

        /// <summary>
        /// Gets the <see cref="CancellationTokenSource"/> that can be used to cancel the asynchronous operation.
        /// </summary>
        public bool canBeCancelled => isRunning && m_CancellableTokenSource is {IsCancellationRequested: false};

        /// <summary>
        /// Gets the <see cref="CancellationTokenSource"/> that can be used to cancel the asynchronous operation.
        /// </summary>
        public bool isCancellationRequested => m_CancellableTokenSource is {IsCancellationRequested: true};

        /// <summary>
        /// Gets the <see cref="CancellationTokenSource"/> that can be used to cancel the asynchronous operation.
        /// </summary>
        public bool isRunning => executionTask is {IsCompleted: false};

        /// <summary>
        /// Executes the <see cref="AsyncRelayCommand"/> on the current command target.
        /// </summary>
        /// <param name="parameter"> The parameter to use when executing the <see cref="AsyncRelayCommand"/>.</param>
        /// <returns> The <see cref="Task"/> that represents the asynchronous operation.</returns>
        public Task ExecuteAsync(object? parameter)
        {
            Task task;

            if (!CanExecute(parameter))
                throw new InvalidOperationException("ExecuteAsync should not be called when CanExecute returns false.");

            if (m_Execute is not null)
            {
                // Non cancelable command delegate
                task = executionTask = m_Execute();
            }
            else
            {
                // Cancel the previous operation, if one is pending
                m_CancellableTokenSource?.Cancel();

                var cancellationTokenSource = m_CancellableTokenSource = new CancellationTokenSource();

                // Invoke the cancelable command delegate with a new linked token
                task = executionTask = m_CancellableExecute!(cancellationTokenSource.Token);
            }

            // If concurrent executions are disabled, notify the can execute change as well
            if ((m_Options & AsyncRelayCommandOptions.AllowConcurrentExecutions) == 0)
            {
                CanExecuteChanged?.Invoke(this, EventArgs.Empty);
            }

            return task;
        }

        /// <summary>
        /// Cancels the <see cref="AsyncRelayCommand"/> on the current command target.
        /// </summary>
        public void Cancel()
        {
            if (m_CancellableTokenSource is { IsCancellationRequested: false } cancellationTokenSource)
            {
                cancellationTokenSource.Cancel();
                PropertyChanged?.Invoke(this, CanBeCanceledChangedEventArgs);
                PropertyChanged?.Invoke(this, IsCancellationRequestedChangedEventArgs);
            }
        }
    }
}
