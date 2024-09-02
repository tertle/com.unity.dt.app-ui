// This file draws upon the concepts found in the IRelayCommand implementation from the MVVM Toolkit library (CommunityToolkit/dotnet),
// more information in Third Party Notices.md

#nullable enable
using System.Windows.Input;

namespace Unity.AppUI.MVVM
{
    /// <summary>
    /// An interface expanding ICommand with the ability to raise
    /// the ICommand.CanExecuteChanged event externally.
    /// </summary>
    public interface IRelayCommand : ICommand
    {
        /// <summary>
        /// Notifies that the ICommand.CanExecute property has changed.
        /// </summary>
        void NotifyCanExecuteChanged();
    }

    /// <summary>
    /// A generic interface representing a more specific version of <see cref="IRelayCommand"/>.
    /// </summary>
    /// <typeparam name="T">The type used as argument for the interface methods.</typeparam>
    public interface IRelayCommand<in T> : IRelayCommand
    {
        /// <summary>
        /// Provides a strongly-typed variant of ICommand.Execute(object).
        /// </summary>
        /// <param name="parameter">The input parameter.</param>
        /// <remarks>Use this overload to avoid boxing, if <typeparamref name="T"/> is a value type.</remarks>
        void Execute(T? parameter);

        /// <summary>
        /// Provides a strongly-typed variant of ICommand.CanExecute(object).
        /// </summary>
        /// <param name="parameter">The input parameter.</param>
        /// <returns>Whether or not the current command can be executed.</returns>
        /// <remarks>Use this overload to avoid boxing, if <typeparamref name="T"/> is a value type.</remarks>
        bool CanExecute(T? parameter);
    }
}
