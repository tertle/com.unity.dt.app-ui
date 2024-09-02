using UnityEngine;
using UnityEngine.UIElements;
#if ENABLE_RUNTIME_DATA_BINDINGS
using Unity.Properties;
#endif

namespace Unity.AppUI.UI
{
    /// <summary>
    /// A preloader visual element.
    /// </summary>
#if ENABLE_UXML_SERIALIZED_DATA
    [UxmlElement]
#endif
    public partial class Preloader : BaseVisualElement
    {
        /// <summary>
        /// The Preloader's USS class name.
        /// </summary>
        public const string ussClassName = "appui-preloader";

        /// <summary>
        /// The Preloader's circular progress USS class name.
        /// </summary>
        public const string circularProgressUssClassName = ussClassName + "__circular-progress";

        /// <summary>
        /// The Preloader's logo USS class name.
        /// </summary>
        public const string logoUssClassName = ussClassName + "__logo";

        /// <summary>
        /// Constructor.
        /// </summary>
        public Preloader()
        {
            pickingMode = PickingMode.Ignore;

            AddToClassList(ussClassName);

            var progress = new CircularProgress
            {
                innerRadius = 0.49f,
                pickingMode = PickingMode.Ignore
            };
            progress.AddToClassList(circularProgressUssClassName);

            hierarchy.Add(progress);

            var logo = new Image
            {
                pickingMode = PickingMode.Ignore
            };
            logo.AddToClassList(logoUssClassName);

            progress.Add(logo);
        }

#if ENABLE_UXML_TRAITS

        /// <summary>
        /// USS class name of elements of this type.
        /// </summary>
        public new class UxmlFactory : UxmlFactory<Preloader, UxmlTraits> { }

        /// <summary>
        /// Class containing the <see cref="UxmlTraits"/> for the <see cref="AccordionItem"/>.
        /// </summary>
        public new class UxmlTraits : BaseVisualElement.UxmlTraits
        {

        }

#endif
    }
}
