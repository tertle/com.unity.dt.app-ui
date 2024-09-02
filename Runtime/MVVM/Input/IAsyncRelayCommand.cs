#nullable enable
// This file draws upon the concepts found in the IAsyncRelayCommand implementation from the MVVM Toolkit library (CommunityToolkit/dotnet),
// more information in Third Party Notices.md

using System.ComponentModel;
using System.Threading.Tasks;

namespace Unity.AppUI.MVVM
{
    /// <summary>
    /// An interface expanding <see cref="IRelayCommand"/> to support asynchronous operations.
    /// </summary>
    public interface IAsyncRelayCommand : IRelayCommand, INotifyPropertyChanged
    {
        /// <summary>
        /// Gets the last scheduled <see cref="Task"/>, if available.
        /// This property notifies a change when the <see cref="Task"/> completes.
        /// </summary>
        Task? executionTask { get; }

        /// <summary>
        /// Gets a value indicating whether a running operation for this command can currently be canceled.
        /// </summary>
        /// <remarks>
        /// The exact sequence of events that types implementing this interface should raise is as follows:
        /// <list type="bullet">
        /// <item>
        /// <description>
        /// The command is initially not running: <see cref="isRunning"/>, <see cref="canBeCancelled"/>
        /// and <see cref="isCancellationRequested"/> are <see langword="false"/>.
        /// </description>
        /// </item>
        /// <item>
        /// <description>
        /// The command starts running: <see cref="isRunning"/> and <see cref="canBeCancelled"/> switch to
        /// <see langword="true"/>. <see cref="isCancellationRequested"/> is set to <see langword="false"/>.
        /// </description>
        /// </item>
        /// <item>
        /// <description>
        /// If the operation is canceled: <see cref="canBeCancelled"/> switches to <see langword="false"/>
        /// and <see cref="isCancellationRequested"/> switches to <see langword="true"/>.
        /// </description>
        /// </item>
        /// <item>
        /// <description>
        /// The operation completes: <see cref="isRunning"/> and <see cref="canBeCancelled"/> switch
        /// to <see langword="false"/>. The state of <see cref="isCancellationRequested"/> is undefined.
        /// </description>
        /// </item>
        /// </list>
        /// This only applies if the underlying logic for the command actually supports cancellation. If that is
        /// not the case, then <see cref="canBeCancelled"/> and <see cref="isCancellationRequested"/> will always remain
        /// <see langword="false"/> regardless of the current state of the command.
        /// </remarks>
        bool canBeCancelled { get; }

        /// <summary>
        /// Gets a value indicating whether a running operation for this command has been cancelled.
        /// </summary>
        bool isCancellationRequested { get; }

        /// <summary>
        /// Gets a value indicating whether an operation for this command is currently running.
        /// </summary>
        bool isRunning { get; }

        /// <summary>
        /// Executes the command asynchronously.
        /// </summary>
        /// <param name="parameter"> The input parameter.</param>
        /// <returns> A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task ExecuteAsync(object? parameter);

        /// <summary>
        /// Attempts to cancel the currently running operation for this command.
        /// </summary>
        void Cancel();
    }

    /// <summary>
    /// A generic interface representing a more specific version of <see cref="IAsyncRelayCommand"/>.
    /// </summary>
    /// <typeparam name="T"> The type used as argument for the interface methods.</typeparam>
    public interface IAsyncRelayCommand<in T> : IAsyncRelayCommand, IRelayCommand<T>
    {
        /// <summary>
        /// Provides a strongly-typed variant of <see cref="IAsyncRelayCommand.ExecuteAsync(object)"/>.
        /// </summary>
        /// <param name="parameter"> The input parameter.</param>
        /// <returns> A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task ExecuteAsync(T? parameter);
    }
}
