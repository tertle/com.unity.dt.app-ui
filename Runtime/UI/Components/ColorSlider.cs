using System;
using UnityEngine;
using UnityEngine.UIElements;
#if ENABLE_RUNTIME_DATA_BINDINGS
using Unity.Properties;
#endif

namespace Unity.AppUI.UI
{
    /// <summary>
    /// A slider that allows the user to select a color value.
    /// </summary>
#if ENABLE_UXML_SERIALIZED_DATA
    [UxmlElement]
#endif
    public sealed partial class ColorSlider : SliderFloat
    {
#if ENABLE_RUNTIME_DATA_BINDINGS

        internal static readonly BindingId colorValueProperty = nameof(colorValue);

        internal static readonly BindingId colorRangeProperty = nameof(colorRange);

#endif

        /// <summary>
        /// The ColorSlider main styling class.
        /// </summary>
        public new const string ussClassName = "appui-color-slider";

        readonly ColorSwatch m_TrackSwatch;

        float m_IncrementFactor;

        /// <summary>
        /// The currently selected color value.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty(ReadOnly = true)]
#endif
        public Color colorValue => colorRange.Evaluate(m_Value);

        /// <summary>
        /// The current color range in the track.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public Gradient colorRange
        {
            get => m_TrackSwatch.value;
            set
            {
                var changed = !m_TrackSwatch.value?.Equals(value) ?? value != null;
                m_TrackSwatch.value = value;
                SetValueWithoutNotify(this.value);

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in colorRangeProperty);
#endif
            }
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ColorSlider()
        {
            AddToClassList(ussClassName);

            m_TrackSwatch = new ColorSwatch { pickingMode = PickingMode.Ignore };
            var g = new Gradient();
            g.SetKeys(new GradientColorKey[]
            {
                new GradientColorKey(Color.red, 0),
                new GradientColorKey(Color.red, 1),
            }, new GradientAlphaKey[]
            {
                new GradientAlphaKey(0, 0),
                new GradientAlphaKey(1, 1),
            });
            m_TrackSwatch.SetValueWithoutNotify(g);

            m_TrackElement.Add(m_TrackSwatch);
            m_TrackSwatch.StretchToParentSize();

            lowValue = 0;
            highValue = 1f;
            step = 0.01f;
            shiftStep = 0.1f;
            value = 0;
        }

        /// <inheritdoc />
        protected override void InvokeValueChangedCallbacks()
        {
            base.InvokeValueChangedCallbacks();
#if ENABLE_RUNTIME_DATA_BINDINGS
            NotifyPropertyChanged(in colorValueProperty);
#endif
        }

        /// <inheritdoc />
        protected override void RefreshUI()
        {
            base.RefreshUI();
            if (m_ThumbsContainer is not {childCount: >0})
                return;

            m_TrackSwatch.orientation = orientation;
            var thumb = (Thumb)m_ThumbsContainer[0];
            thumb.fill = colorValue;
        }

#if ENABLE_UXML_TRAITS

        /// <summary>
        /// Instantiates an <see cref="ColorSlider"/> using the data read from a UXML file.
        /// </summary>
        public new class UxmlFactory : UxmlFactory<ColorSlider, UxmlTraits> { }

        /// <summary>
        /// Class containing the <see cref="UxmlTraits"/> for the <see cref="ColorSlider"/>.
        /// </summary>
        public new class UxmlTraits : SliderFloat.UxmlTraits
        {
            readonly UxmlStringAttributeDescription m_ColorRange = new UxmlStringAttributeDescription
            {
                name = "color-range",
                defaultValue = null,
            };

            /// <summary>
            /// Initializes the VisualElement from the UXML attributes.
            /// </summary>
            /// <param name="ve"> The <see cref="VisualElement"/> to initialize.</param>
            /// <param name="bag"> The <see cref="IUxmlAttributes"/> bag to use to initialize the <see cref="VisualElement"/>.</param>
            /// <param name="cc"> The <see cref="CreationContext"/> to use to initialize the <see cref="VisualElement"/>.</param>
            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);

                var el = (ColorSlider)ve;
                var colorRange = m_ColorRange.GetValueFromBag(bag, cc);
                if (!string.IsNullOrEmpty(colorRange) && GradientExtensions.TryParse(colorRange, out var gradient))
                    el.colorRange = gradient;
            }
        }
#endif
    }
}
