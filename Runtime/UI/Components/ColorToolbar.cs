using System;
using UnityEngine;
using UnityEngine.UIElements;
#if ENABLE_RUNTIME_DATA_BINDINGS
using Unity.Properties;
#endif

namespace Unity.AppUI.UI
{
    /// <summary>
    /// A toolbar that contains a color swatch for the previous color, a color swatch for the current color, and an eye dropper button.
    /// </summary>
    public class ColorToolbar : BaseVisualElement
    {
#if ENABLE_RUNTIME_DATA_BINDINGS

        internal static readonly BindingId previousColorPropertyKey = new BindingId(nameof(previousColor));

        internal static readonly BindingId currentColorPropertyKey = new BindingId(nameof(currentColor));

#endif

        /// <summary>
        /// The main Uss class name of this element.
        /// </summary>
        public const string ussClassName = "appui-colortoolbar";

        /// <summary>
        /// The Uss class name of the eye dropper button.
        /// </summary>
        public const string eyeDropperUssClassName = ussClassName + "__eyedropper";

        /// <summary>
        /// The Uss class name of the swatch container.
        /// </summary>
        public const string swatchContainerUssClassName = ussClassName + "__swatchcontainer";

        /// <summary>
        /// The Uss class name of the previous color swatch.
        /// </summary>
        public const string previousColorSwatchUssClassName = ussClassName + "__previouscolorswatch";

        /// <summary>
        /// The Uss class name of the current color swatch.
        /// </summary>
        public const string currentColorSwatchUssClassName = ussClassName + "__currentcolorswatch";

        /// <summary>
        /// The event that is invoked when the previous color swatch is clicked.
        /// </summary>
        public event Action previousColorSwatchClicked;

        readonly ActionButton m_EyeDropperButton;

        readonly VisualElement m_SwatchContainer;

        readonly ColorSwatch m_PreviousColorSwatch;

        readonly ColorSwatch m_CurrentColorSwatch;

        readonly Pressable m_PreviousColorSwatchClickable;

        /// <summary>
        /// The previous color swatch value.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
        public Color previousColor
        {
            get => m_PreviousColorSwatch.color;
            set
            {
                if (m_PreviousColorSwatch.color == value)
                    return;

                m_PreviousColorSwatch.color = value;

#if ENABLE_RUNTIME_DATA_BINDINGS
                NotifyPropertyChanged(in previousColorPropertyKey);
#endif
            }
        }

        /// <summary>
        /// The current color swatch value.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
        public Color currentColor
        {
            get => m_CurrentColorSwatch.color;
            set
            {
                if (m_CurrentColorSwatch.color == value)
                    return;

                m_CurrentColorSwatch.color = value;

#if ENABLE_RUNTIME_DATA_BINDINGS
                NotifyPropertyChanged(in currentColorPropertyKey);
#endif
            }
        }

        /// <summary>
        /// The eye dropper button.
        /// </summary>
        public ActionButton eyeDropperButton => m_EyeDropperButton;

        /// <summary>
        /// The previous color swatch.
        /// </summary>
        public ColorSwatch previousColorSwatch => m_PreviousColorSwatch;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ColorToolbar()
        {
            AddToClassList(ussClassName);

            focusable = false;
            pickingMode = PickingMode.Ignore;

            m_EyeDropperButton = new ActionButton { name = eyeDropperUssClassName, focusable = true, pickingMode = PickingMode.Position, icon = "color-picker" };
            m_EyeDropperButton.AddToClassList(eyeDropperUssClassName);
            hierarchy.Add(m_EyeDropperButton);

            m_SwatchContainer = new VisualElement { name = swatchContainerUssClassName, focusable = false, pickingMode = PickingMode.Ignore };
            m_SwatchContainer.AddToClassList(swatchContainerUssClassName);
            hierarchy.Add(m_SwatchContainer);

            m_PreviousColorSwatch = new ColorSwatch { name = previousColorSwatchUssClassName, focusable = true, pickingMode = PickingMode.Position };
            m_PreviousColorSwatch.AddToClassList(previousColorSwatchUssClassName);
            m_SwatchContainer.hierarchy.Add(m_PreviousColorSwatch);

            m_CurrentColorSwatch = new ColorSwatch { name = currentColorSwatchUssClassName, focusable = false, pickingMode = PickingMode.Ignore };
            m_CurrentColorSwatch.AddToClassList(currentColorSwatchUssClassName);
            m_SwatchContainer.hierarchy.Add(m_CurrentColorSwatch);

            m_PreviousColorSwatchClickable = new Pressable(OnPreviousSwatchClicked);
            m_PreviousColorSwatch.AddManipulator(m_PreviousColorSwatchClickable);
        }

        void OnPreviousSwatchClicked()
        {
            previousColorSwatchClicked?.Invoke();
        }
    }
}
