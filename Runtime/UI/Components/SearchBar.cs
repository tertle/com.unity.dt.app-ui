using System;
using UnityEngine;
using UnityEngine.UIElements;
#if ENABLE_RUNTIME_DATA_BINDINGS
using Unity.Properties;
#endif

namespace Unity.AppUI.UI
{
    /// <summary>
    /// SearchBar UI element.
    /// </summary>
#if ENABLE_UXML_SERIALIZED_DATA
    [UxmlElement]
#endif
    public partial class SearchBar : TextField
    {
        /// <summary>
        /// The SearchBar main styling class.
        /// </summary>
        public new const string ussClassName = "appui-searchbar";

        /// <summary>
        /// Default Constructor.
        /// </summary>
        public SearchBar()
        {
            AddToClassList(ussClassName);
            leadingIconName = "magnifying-glass";
            placeholder = "Search...";
        }

#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute("leading-icon-name")]
        [HideInInspector]
        string leadingIconNameOverride
        {
            get => leadingIconName;
            set => leadingIconName = value;
        }
#endif

#if ENABLE_UXML_TRAITS

        /// <summary>
        /// Factory class to instantiate a <see cref="SearchBar"/> using the data read from a UXML file.
        /// </summary>
        public new class UxmlFactory : UxmlFactory<SearchBar, UxmlTraits> { }

#endif
    }
}
