// This file draws upon the concepts found in the RelayCommand implementation from the MVVM Toolkit library (CommunityToolkit/dotnet),
// more information in Third Party Notices.md

#nullable enable
using System;

namespace Unity.AppUI.MVVM
{
    /// <summary>
    /// A command whose sole purpose is to relay its functionality to other
    /// objects by invoking delegates. The default return value for the <see cref="CanExecute(object?)"/>
    /// method is <see langword="true"/>. This type does not allow you to accept command parameters
    /// in the <see cref="Execute(object?)"/> and <see cref="CanExecute(object?)"/> callback methods.
    /// </summary>
    public class RelayCommand : IRelayCommand
    {
        /// <summary>
        /// The <see cref="Action"/> to invoke when <see cref="Execute(object?)"/> is used.
        /// </summary>
        readonly Action m_Execute;

        /// <summary>
        /// The optional action to invoke when <see cref="CanExecute(object?)"/> is used.
        /// </summary>
        readonly Func<bool>? m_CanExecute;

        /// <summary>
        /// Initializes a new instance of the <see cref="RelayCommand"/> class.
        /// </summary>
        /// <param name="execute"> The action to execute. </param>
        /// <exception cref="ArgumentNullException"> Thrown if the action is null. </exception>
        public RelayCommand(Action? execute)
        {
            m_Execute = execute ?? throw new ArgumentNullException(nameof(execute));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RelayCommand"/> class.
        /// </summary>
        /// <param name="execute"> The action to execute. </param>
        /// <param name="canExecute"> Predicate used to determine if the command can execute. </param>
        /// <exception cref="ArgumentNullException"> Thrown if the action is null. </exception>
        public RelayCommand(Action execute, Func<bool> canExecute)
        {
            m_Execute = execute ?? throw new ArgumentNullException(nameof(execute));
            m_CanExecute = canExecute ?? throw new ArgumentNullException(nameof(canExecute));
        }

        /// <summary>
        /// Determines whether this <see cref="RelayCommand"/> can execute in its current state.
        /// </summary>
        /// <param name="parameter"> Data used by the command. </param>
        /// <returns> <see langword="true"/> if this command can be executed; otherwise, <see langword="false"/>. </returns>
        public bool CanExecute(object? parameter)
        {
            return m_CanExecute?.Invoke() ?? true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RelayCommand"/> class.
        /// </summary>
        /// <returns> <see langword="true"/> if this command can be executed; otherwise, <see langword="false"/>. </returns>
        public bool CanExecute() => CanExecute(null);

        /// <summary>
        /// Executes the <see cref="RelayCommand"/> on the current command target.
        /// </summary>
        /// <param name="parameter"> Data used by the command. </param>
        public void Execute(object? parameter)
        {
            if (CanExecute())
                m_Execute.Invoke();
        }

        /// <summary>
        /// Defines the method to be called when the command is invoked.
        /// </summary>
        public void Execute() => Execute(null);

        /// <summary>
        /// Occurs when changes occur that affect whether or not the command should execute.
        /// </summary>
        public event EventHandler? CanExecuteChanged;

        /// <summary>
        /// Notifies that the <see cref="CanExecuteChanged"/> property has changed.
        /// </summary>
        public void NotifyCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
