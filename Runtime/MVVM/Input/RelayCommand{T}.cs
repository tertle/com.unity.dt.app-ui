// This file draws upon the concepts found in the RelayCommand implementation from the MVVM Toolkit library (CommunityToolkit/dotnet),
// more information in Third Party Notices.md

#nullable enable
using System;

namespace Unity.AppUI.MVVM
{
    /// <summary>
    /// Implementation of <see cref="IRelayCommand"/> that allows for CommandParameter to be passed in.
    /// </summary>
    /// <typeparam name="T"> The type of the CommandParameter. </typeparam>
    public class RelayCommand<T> : IRelayCommand<T>
    {
        readonly Action<T?> m_Execute;

        readonly Predicate<T?>? m_CanExecute;

        /// <summary>
        /// Event raised when <see cref="CanExecute(object?)"/> changes.
        /// </summary>
        public event EventHandler? CanExecuteChanged;

        /// <summary>
        /// Initializes a new instance of the <see cref="RelayCommand{T}"/> class.
        /// </summary>
        /// <param name="execute"> The action to execute. </param>
        /// <exception cref="ArgumentNullException"> Thrown if the action is null. </exception>
        public RelayCommand(Action<T?> execute)
        {
            m_Execute = execute ?? throw new ArgumentNullException(nameof(execute));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RelayCommand{T}"/> class.
        /// </summary>
        /// <param name="execute"> The action to execute. </param>
        /// <param name="canExecute"> The predicate executed to determine if the command can execute. </param>
        /// <exception cref="ArgumentNullException"> Thrown if the action or the predicate is null. </exception>
        public RelayCommand(Action<T?> execute, Predicate<T?> canExecute)
        {
            m_Execute = execute ?? throw new ArgumentNullException(nameof(execute));
            m_CanExecute = canExecute ?? throw new ArgumentNullException(nameof(canExecute));
        }

        /// <summary>
        /// Determines whether this <see cref="RelayCommand{T}"/> can execute in its current state.
        /// </summary>
        /// <param name="parameter"> Data used by the command. </param>
        /// <returns> <see langword="true"/> if this command can be executed; otherwise, <see langword="false"/>. </returns>
        /// <exception cref="InvalidOperationException"> Thrown if the <paramref name="parameter"/> is not of type <typeparamref name="T"/>. </exception>
        public bool CanExecute(object? parameter)
        {
            if (parameter == null && default(T) is not null)
                return false;

            if (!TryGetCommandArg(parameter, out var result))
                throw new InvalidOperationException("");

            return CanExecute(result);
        }

        /// <summary>
        /// Executes the <see cref="RelayCommand{T}"/> on the current command target.
        /// </summary>
        /// <param name="parameter"> Data used by the command. </param>
        /// <exception cref="InvalidOperationException"> Thrown if the <paramref name="parameter"/> is not of type <typeparamref name="T"/>. </exception>
        public void Execute(object? parameter)
        {
            if (!TryGetCommandArg(parameter, out var result))
                throw new InvalidOperationException("Invalid parameter type.");

            Execute(result);
        }

        /// <summary>
        /// Notifies that the <see cref="CanExecuteChanged"/> property has changed.
        /// </summary>
        public void NotifyCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Executes the <see cref="RelayCommand{T}"/> on the current command target.
        /// </summary>
        /// <param name="parameter"> Data used by the command. </param>
        public void Execute(T? parameter)
        {
            if (CanExecute(parameter))
                m_Execute.Invoke(parameter);
        }

        /// <summary>
        /// Determines whether this <see cref="RelayCommand{T}"/> can execute in its current state.
        /// </summary>
        /// <param name="parameter"> Data used by the command. </param>
        /// <returns> <see langword="true"/> if this command can be executed; otherwise, <see langword="false"/>. </returns>
        public bool CanExecute(T? parameter)
        {
            return m_CanExecute?.Invoke(parameter) ?? true;
        }

        internal static bool TryGetCommandArg(object? param, out T? result)
        {
            if (param == null && default(T) is null)
            {
                result = default;
                return true;
            }

            if (param is T arg)
            {
                result = arg;
                return true;
            }

            result = default;
            return false;
        }
    }
}
