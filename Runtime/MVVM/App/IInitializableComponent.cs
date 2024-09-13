namespace Unity.AppUI.MVVM
{
    /// <summary>
    /// Interface for components that need to be initialized.
    /// </summary>
    public interface IInitializableComponent
    {
        /// <summary>
        /// Initializes the component.
        /// </summary>
        void InitializeComponent();
    }
}
