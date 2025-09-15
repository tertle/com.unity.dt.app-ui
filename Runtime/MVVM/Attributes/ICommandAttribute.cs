using System;

namespace Unity.AppUI.MVVM
{
    /// <summary>
    /// Attribute to mark a method as a command that will generate a RelayCommand.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public sealed class ICommandAttribute : Attribute
    {
        /// <summary>
        /// The name of the method that must be called to know if the command can be executed.
        /// </summary>
        public string CanExecuteMethod { get; set; }

        /// <summary>
        /// An optional parameter to specify the options for the generated AsyncRelayCommand.
        /// </summary>
        /// <remarks>
        /// This is only used when the method returns a <see cref="System.Threading.Tasks.Task"/>.
        /// </remarks>
        public AsyncRelayCommandOptions AsyncOptions { get; set; }
    }
}
