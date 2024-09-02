using System;

namespace Unity.AppUI.UI
{
    /// <summary>
    /// Attribute to specify the path to the UXML file for a VisualElement
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class UxmlFilePathAttribute : Attribute
    {
        /// <summary>
        /// The path to the UXML file
        /// </summary>
        public string filePath { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="filePath"> The path to the UXML file </param>
        public UxmlFilePathAttribute(string filePath)
        {
            this.filePath = filePath;
        }
    }
}
