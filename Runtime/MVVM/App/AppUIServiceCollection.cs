namespace Unity.AppUI.MVVM
{
    /// <summary>
    /// A service collection for App Ui applications.
    /// </summary>
    public class AppUIServiceCollection : ServiceCollection
    {
        /// <summary>
        /// Gets or sets a value indicating whether the collection is read-only.
        /// </summary>
        public new bool IsReadOnly { get; set; }
    }
}
