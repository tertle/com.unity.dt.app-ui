using System;
using UnityEngine;
using UnityEngine.UIElements;
#if ENABLE_RUNTIME_DATA_BINDINGS
using Unity.Properties;
#endif

namespace Unity.AppUI.UI
{
    /// <summary>
    /// This element can be used in the visual tree to wrap a part of the user-interface where the context
    /// of the application needs to be overriden.
    /// </summary>
#if ENABLE_UXML_SERIALIZED_DATA
    [UxmlElement]
#endif
    public partial class ContextProvider : BaseVisualElement
    {
        /// <summary>
        /// Main Uss Class Name.
        /// </summary>
        public const string ussClassName = "appui-context-provider";

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ContextProvider()
        {
            AddToClassList(ussClassName);

            pickingMode = PickingMode.Ignore;
        }

#if ENABLE_UXML_TRAITS
        /// <summary>
        /// A class responsible for creating a <see cref="ContextProvider"/> from UXML.
        /// </summary>
        public new class UxmlFactory : UxmlFactory<ContextProvider, UxmlTraits> { }
#endif
    }
}