using System;
using Unity.AppUI.Bridge;
using UnityEngine;
using UnityEngine.UIElements;
#if ENABLE_RUNTIME_DATA_BINDINGS
using Unity.Properties;
#endif

namespace Unity.AppUI.UI
{
    /// <summary>
    /// The type of ColorPicker to open when the ColorField is clicked.
    /// </summary>
    public enum ColorPickerType
    {
        /// <summary>
        /// The default ColorPicker from AppUI.
        /// </summary>
        Default,

        /// <summary>
        /// The ColorPicker from Unity's Editor.
        /// </summary>
        UnityEditor
    }

    /// <summary>
    /// Color Field UI element.
    /// </summary>
#if ENABLE_UXML_SERIALIZED_DATA
    [UxmlElement]
#endif
    public partial class ColorField : ExVisualElement, IInputElement<Color>, INotifyValueChanging<Color>, ISizeableElement, IPressable
    {
#if ENABLE_RUNTIME_DATA_BINDINGS

        internal static readonly BindingId sizeProperty = nameof(size);

        internal static readonly BindingId swatchSizeProperty = nameof(swatchSize);

        internal static readonly BindingId swatchOnlyProperty = nameof(swatchOnly);

        internal static readonly BindingId showTextProperty = nameof(showText);

        internal static readonly BindingId inlinePickerProperty = nameof(inlinePicker);

        internal static readonly BindingId invalidProperty = nameof(invalid);

        internal static readonly BindingId valueProperty = nameof(value);

        internal static readonly BindingId validateValueProperty = nameof(validateValue);

        internal static readonly BindingId colorPickerTypeProperty = nameof(colorPickerType);

        internal static readonly BindingId showAlphaProperty = nameof(showAlpha);

        internal static readonly BindingId hdrProperty = nameof(hdr);

#endif

        /// <summary>
        /// The ColorField main styling class.
        /// </summary>
        public const string ussClassName = "appui-colorfield";

        /// <summary>
        /// The ColorField color swatch styling class.
        /// </summary>
        public const string colorSwatchUssClassName = ussClassName + "__color-swatch";

        /// <summary>
        /// The ColorField label styling class.
        /// </summary>
        public const string labelUssClassName = ussClassName + "__label";

        /// <summary>
        /// The ColorField color picker icon styling class.
        /// </summary>
        public const string colorPickerIconUssClassName = ussClassName + "__color-picker-icon";

        /// <summary>
        /// The ColorField size styling class.
        /// </summary>
        [EnumName("GetSizeUssClassName", typeof(Size))]
        public const string sizeUssClassName = ussClassName + "--size-";

        /// <summary>
        /// The ColorField swatch only styling class.
        /// </summary>
        public const string swatchOnlyUssClassName = ussClassName + "--swatch-only";

        /// <summary>
        /// The ColorField showText styling class.
        /// </summary>
        public const string showTextUssClassName = ussClassName + "--show-text";

        readonly ColorSwatch m_SwatchElement;

        readonly UnityEngine.UIElements.TextField m_LabelElement;

        readonly Icon m_ColorPickerIcon;

        Color m_Value;

        Size m_Size;

        Type m_Type;

        Pressable m_Clickable;

        Color m_PreviousValue;

        ColorPicker m_Picker;

        bool m_InlinePicker;

        Func<Color, bool> m_ValidateValue;

        ColorPickerType m_ColorPickerType;

        bool m_ShowAlpha;

        bool m_Hdr;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ColorField()
        {
            AddToClassList(ussClassName);

            focusable = true;
            pickingMode = PickingMode.Position;
            tabIndex = 0;
            passMask = 0;
            clickable = new Pressable(OnClick);

            m_SwatchElement = new ColorSwatch
            {
                name = colorSwatchUssClassName,
                pickingMode = PickingMode.Ignore,
                round = true,
            };
            m_SwatchElement.AddToClassList(colorSwatchUssClassName);

            m_LabelElement = new UnityEngine.UIElements.TextField
            {
                name = labelUssClassName,
                isReadOnly = true
            };
            m_LabelElement.RegisterCallback<PointerDownEvent>(OnTextFieldPointerDown);
            m_LabelElement.AddToClassList(labelUssClassName);

            m_ColorPickerIcon = new Icon
            {
                name = colorPickerIconUssClassName,
                iconName = "color-picker",
                pickingMode = PickingMode.Ignore,
            };
            m_ColorPickerIcon.AddToClassList(colorPickerIconUssClassName);

            hierarchy.Add(m_SwatchElement);
            hierarchy.Add(m_LabelElement);
            hierarchy.Add(m_ColorPickerIcon);

            size = Size.M;
            swatchSize = Size.S;
            showText = true;
            showAlpha = true;
            hdr = false;
            SetValueWithoutNotify(Color.clear);
            this.AddManipulator(new KeyboardFocusController(OnKeyboardFocusIn, OnPointerFocusIn));
        }

        void OnTextFieldPointerDown(PointerDownEvent evt)
        {
            evt.StopPropagation();
        }

        void OnClick()
        {
            if (Application.isEditor && colorPickerType == ColorPickerType.UnityEditor)
                OpenUnityEditorPicker();
            else
                OpenDefaultPicker();
        }

        void OpenUnityEditorPicker()
        {
            if (panel == null)
                return;

            if (panel.contextType == ContextType.Player)
            {
                Debug.LogWarning("UnityEditor ColorPicker is not available in Player context.");
                return;
            }

            m_PreviousValue = value;
            ColorPickerExtensionsBridge.Show(OnPickerValueChanged, m_PreviousValue, showAlpha, hdr);
        }

        void OpenDefaultPicker()
        {
            var wasInline = m_Picker != null && m_Picker.parent == parent;
            m_Picker?.parent?.Remove(m_Picker);

            if (inlinePicker && wasInline)
            {
                RemoveFromClassList(Styles.focusedUssClassName);
                m_Picker.UnregisterValueChangedCallback(OnPickerValueChanged);
                using var evt = ChangeEvent<Color>.GetPooled(m_PreviousValue, m_Picker.value);
                SetValueWithoutNotify(m_Picker.value);
                evt.target = this;
                SendEvent(evt);
                return;
            }

            m_PreviousValue = value;
            m_Picker ??= new ColorPicker
            {
                showAlpha = showAlpha,
                showHex = true,
                showToolbar = true,
                hdr = hdr,
            };
            m_Picker.previousValue = m_PreviousValue;
            m_Picker.SetValueWithoutNotify(m_PreviousValue);
            m_Picker.RegisterValueChangedCallback(OnPickerValueChanged);
            if (inlinePicker)
            {
                var idx = parent.IndexOf(this) + 1;
                parent.Insert(idx, m_Picker);
            }
            else
            {
                var popover = Popover.Build(this, m_Picker);
                popover.dismissed += (_, _) =>
                {
                    RemoveFromClassList(Styles.focusedUssClassName);
                    m_Picker.UnregisterValueChangedCallback(OnPickerValueChanged);
                    if (m_PreviousValue != m_Picker.value)
                    {
                        using var evt = ChangeEvent<Color>.GetPooled(m_PreviousValue, m_Picker.value);
                        SetValueWithoutNotify(m_Picker.value);
                        evt.target = this;
                        SendEvent(evt);
                    }
                    Focus();
                };
                popover.Show();
            }
            AddToClassList(Styles.focusedUssClassName);
        }

        void OnPickerValueChanged(ChangeEvent<Color> e)
        {
            OnPickerValueChanged(e.newValue);
        }

        void OnPickerValueChanged(Color color)
        {
            if (color != value)
            {
                SetValueWithoutNotify(color);
                using var evt = ChangingEvent<Color>.GetPooled();
                evt.previousValue = m_PreviousValue;
                evt.newValue = color;
                evt.target = this;
                SendEvent(evt);
            }
        }

        void OnPointerFocusIn(FocusInEvent evt)
        {
            passMask = 0;
        }

        void OnKeyboardFocusIn(FocusInEvent evt)
        {
            passMask = Passes.Clear | Passes.Outline;
        }

        /// <summary>
        /// The content container of this ColorField. This is null for ColorField.
        /// </summary>
        public override VisualElement contentContainer => null;

        /// <summary>
        /// Clickable Manipulator for this AssetTargetField.
        /// </summary>
        public Pressable clickable
        {
            get => m_Clickable;
            set
            {
                if (m_Clickable != null && m_Clickable.target == this)
                    this.RemoveManipulator(m_Clickable);
                m_Clickable = value;
                if (m_Clickable == null)
                    return;
                this.AddManipulator(m_Clickable);
            }
        }

        /// <summary>
        /// The ColorField color picker type.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public ColorPickerType colorPickerType
        {
            get => m_ColorPickerType;
            set
            {
                var changed = m_ColorPickerType != value;
                m_ColorPickerType = value;

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in colorPickerTypeProperty);
#endif
            }
        }

        /// <summary>
        /// The ColorField size.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public Size size
        {
            get => m_Size;
            set
            {
                var changed = m_Size != value;
                RemoveFromClassList(GetSizeUssClassName(m_Size));
                m_Size = value;
                AddToClassList(GetSizeUssClassName(m_Size));
                m_ColorPickerIcon.size = m_Size.ToIconSize();

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in sizeProperty);
#endif
            }
        }

        /// <summary>
        /// The ColorField swatch size.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public Size swatchSize
        {
            get => m_SwatchElement.size;
            set
            {
                var changed = m_SwatchElement.size != value;
                m_SwatchElement.size = value;

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in swatchSizeProperty);
#endif
            }
        }

        /// <summary>
        /// The ColorField type. When this is true, the ColorField will only show the swatch.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public bool swatchOnly
        {
            get => ClassListContains(swatchOnlyUssClassName);
            set
            {
                var changed = ClassListContains(swatchOnlyUssClassName) != value;
                EnableInClassList(swatchOnlyUssClassName, value);

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in swatchOnlyProperty);
#endif
            }
        }

        /// <summary>
        /// Whether to show the text label for the ColorField.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public bool showText
        {
            get => ClassListContains(showTextUssClassName);
            set
            {
                var changed = ClassListContains(showTextUssClassName) != value;
                EnableInClassList(showTextUssClassName, value);

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in showTextProperty);
#endif
            }
        }

        /// <summary>
        /// The ColorPicker position relative to the ColorField. When this is true, the ColorPicker will be inlined
        /// instead of being displayed in a Popover.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public bool inlinePicker
        {
            get => m_InlinePicker;
            set
            {
                var changed = m_InlinePicker != value;
                m_InlinePicker = value;

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in inlinePickerProperty);
#endif
            }
        }

        /// <summary>
        /// The ColorField invalid state.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public bool invalid
        {
            get => ClassListContains(Styles.invalidUssClassName);
            set
            {
                var changed = ClassListContains(Styles.invalidUssClassName) != value;
                EnableInClassList(Styles.invalidUssClassName, value);

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in invalidProperty);
#endif
            }
        }

        /// <summary>
        /// The ColorField validation function.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
        public Func<Color, bool> validateValue
        {
            get => m_ValidateValue;
            set
            {
                var changed = m_ValidateValue != value;
                m_ValidateValue = value;

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in validateValueProperty);
#endif
            }
        }

        /// <summary>
        /// Sets the ColorField value without notifying the ColorField.
        /// </summary>
        /// <param name="newValue"> The new ColorField value. </param>
        public void SetValueWithoutNotify(Color newValue)
        {
            m_Value = newValue;
            m_LabelElement.SetValueWithoutNotify($"#{ColorExtensions.ColorToRgbaHex(m_Value, showAlpha)}");
            m_SwatchElement.color = m_Value;
            if (validateValue != null) invalid = !validateValue(m_Value);
        }

        /// <summary>
        /// The ColorField value.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public Color value
        {
            get => m_Value;
            set
            {
                if (m_Value == value)
                    return;

                using var evt = ChangeEvent<Color>.GetPooled(m_Value, value);
                evt.target = this;
                SetValueWithoutNotify(value);
                SendEvent(evt);

#if ENABLE_RUNTIME_DATA_BINDINGS
                NotifyPropertyChanged(in valueProperty);
#endif
            }
        }

        /// <summary>
        /// Whether to show the alpha channel in the ColorPicker.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public bool showAlpha
        {
            get => m_ShowAlpha;
            set
            {
                var changed = m_ShowAlpha != value;
                m_ShowAlpha = value;

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in showAlphaProperty);
#endif

                if (!m_ShowAlpha)
                    this.value = new Color(m_Value.r, m_Value.g, m_Value.b, 1);
            }
        }

        /// <summary>
        /// Whether to show the HDR colors in the ColorPicker.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public bool hdr
        {
            get => m_Hdr;
            set
            {
                var changed = m_Hdr != value;
                m_Hdr = value;

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in hdrProperty);
#endif
            }
        }


#if ENABLE_UXML_TRAITS

        /// <summary>
        /// Class to instantiate a <see cref="ColorField"/> using the data read from a UXML file.
        /// </summary>
        public new class UxmlFactory : UxmlFactory<ColorField, UxmlTraits> { }

        /// <summary>
        /// Class containing the <see cref="UxmlTraits"/> for the <see cref="ColorField"/>.
        /// </summary>
        public new class UxmlTraits : ExVisualElement.UxmlTraits
        {

            readonly UxmlBoolAttributeDescription m_Invalid = new UxmlBoolAttributeDescription
            {
                name = "invalid",
                defaultValue = false
            };

            readonly UxmlBoolAttributeDescription m_SwatchOnly = new UxmlBoolAttributeDescription
            {
                name = "swatch-only",
                defaultValue = false
            };

            readonly UxmlBoolAttributeDescription m_ShowText = new UxmlBoolAttributeDescription
            {
                name = "show-text",
                defaultValue = true
            };

            readonly UxmlBoolAttributeDescription m_InlinePicker = new UxmlBoolAttributeDescription
            {
                name = "inline-picker",
                defaultValue = false
            };

            readonly UxmlEnumAttributeDescription<Size> m_Size = new UxmlEnumAttributeDescription<Size>
            {
                name = "size",
                defaultValue = Size.M,
            };

            readonly UxmlEnumAttributeDescription<Size> m_SwatchSize = new UxmlEnumAttributeDescription<Size>
            {
                name = "swatch-size",
                defaultValue = Size.S,
            };

            readonly UxmlEnumAttributeDescription<ColorPickerType> m_ColorPickerType = new UxmlEnumAttributeDescription<ColorPickerType>
            {
                name = "color-picker-type",
                defaultValue = ColorPickerType.Default,
            };

            readonly UxmlBoolAttributeDescription m_ShowAlpha = new UxmlBoolAttributeDescription
            {
                name = "show-alpha",
                defaultValue = true
            };

            readonly UxmlBoolAttributeDescription m_Hdr = new UxmlBoolAttributeDescription
            {
                name = "hdr",
                defaultValue = false
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

                var element = (ColorField)ve;
                element.size = m_Size.GetValueFromBag(bag, cc);
                element.swatchSize = m_SwatchSize.GetValueFromBag(bag, cc);
                element.invalid = m_Invalid.GetValueFromBag(bag, cc);
                element.swatchOnly = m_SwatchOnly.GetValueFromBag(bag, cc);
                element.showText = m_ShowText.GetValueFromBag(bag, cc);
                element.inlinePicker = m_InlinePicker.GetValueFromBag(bag, cc);
                element.colorPickerType = m_ColorPickerType.GetValueFromBag(bag, cc);
                element.showAlpha = m_ShowAlpha.GetValueFromBag(bag, cc);
                element.hdr = m_Hdr.GetValueFromBag(bag, cc);
            }
        }
#endif
    }
}
