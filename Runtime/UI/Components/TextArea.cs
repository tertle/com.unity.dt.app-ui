using System;
using System.Collections.Generic;
using Unity.AppUI.Bridge;
using UnityEngine;
using UnityEngine.UIElements;
#if ENABLE_RUNTIME_DATA_BINDINGS
using Unity.Properties;
#endif

namespace Unity.AppUI.UI
{
    /// <summary>
    /// Text Area UI element.
    /// </summary>
#if ENABLE_UXML_SERIALIZED_DATA
    [UxmlElement]
#endif
    public partial class TextArea : ExVisualElement, IInputElement<string>, INotifyValueChanging<string>
    {
#if ENABLE_RUNTIME_DATA_BINDINGS

        internal static readonly BindingId valueProperty = nameof(value);

        internal static readonly BindingId isReadOnlyProperty = nameof(isReadOnly);

        internal static readonly BindingId maxLengthProperty = nameof(maxLength);

        internal static readonly BindingId placeholderProperty = nameof(placeholder);

        internal static readonly BindingId autoResizeProperty = nameof(autoResize);

        internal static readonly BindingId submitOnEnterProperty = nameof(submitOnEnter);

        internal static readonly BindingId submitModifiersProperty = nameof(submitModifiers);

        internal static readonly BindingId validateValueProperty = nameof(validateValue);

        internal static readonly BindingId invalidProperty = nameof(invalid);

#endif


        /// <summary>
        /// The TextArea main styling class.
        /// </summary>
        public const string ussClassName = "appui-textarea";

        /// <summary>
        /// The TextArea input container styling class.
        /// </summary>
        public const string scrollViewUssClassName = ussClassName + "__scrollview";

        /// <summary>
        /// The TextArea resize handle styling class.
        /// </summary>
        public const string resizeHandleUssClassName = ussClassName + "__resize-handle";

        /// <summary>
        /// The TextArea input styling class.
        /// </summary>
        public const string inputUssClassName = ussClassName + "__input";

        /// <summary>
        /// The TextArea placeholder styling class.
        /// </summary>
        public const string placeholderUssClassName = ussClassName + "__placeholder";

        const bool k_IsReadOnlyDefault = false;

        const int k_MaxLengthDefault = -1;

        readonly UnityEngine.UIElements.TextField m_InputField;

        readonly LocalizedTextElement m_Placeholder;

#if !UNITY_2022_1_OR_NEWER
        readonly ScrollView m_ScrollView;
#endif

        Size m_Size;

        string m_Value;

        readonly VisualElement m_ResizeHandle;

        string m_PreviousValue;

        bool m_RequestSubmit;

        bool m_RequestTab;

        EventModifiers m_SubmitModifiers;

        bool m_SubmitOnEnter;

        Func<string, bool> m_ValidateValue;

        bool m_AutoResize;

        /// <summary>
        /// Event triggered when the user presses the Enter key and <see cref="submitOnEnter"/> is true.
        /// </summary>
        public event Action submitted;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public TextArea()
            : this(null) { }

        /// <summary>
        /// Construct a TextArea with a predefined text value.
        /// </summary>
        /// <param name="value">A default text value.</param>
        /// <remarks>
        /// No event will be triggered when setting the text value during construction.
        /// </remarks>
        public TextArea(string value)
        {
            AddToClassList(ussClassName);

            focusable = true;
            pickingMode = PickingMode.Position;
            passMask = 0;
            tabIndex = 0;
            this.SetIsCompositeRoot(true);
            this.SetExcludeFromFocusRing(true);
            delegatesFocus = true;

            m_Placeholder = new LocalizedTextElement
            {
                name = placeholderUssClassName,
                pickingMode = PickingMode.Ignore,
                focusable = false
            };
            m_Placeholder.AddToClassList(placeholderUssClassName);
            hierarchy.Add(m_Placeholder);

            m_InputField = new UnityEngine.UIElements.TextField { name = inputUssClassName, multiline = true };
            m_InputField.AddToClassList(inputUssClassName);
            m_InputField.AddManipulator(new BlinkingCursor());
#if UNITY_2022_1_OR_NEWER
#if UNITY_2023_1_OR_NEWER
            m_InputField.verticalScrollerVisibility = ScrollerVisibility.Auto;
#else
            m_InputField.SetVerticalScrollerVisibility(ScrollerVisibility.Auto);
#endif
            m_InputField.style.position = Position.Absolute;
            m_InputField.style.top = 0;
            m_InputField.style.left = 0;
            m_InputField.style.right = 0;
            m_InputField.style.bottom = 0;
            hierarchy.Add(m_InputField);
#else
            m_ScrollView = new ScrollView
            {
                name = scrollViewUssClassName,
                elasticity = 0,
                horizontalScrollerVisibility = ScrollerVisibility.Auto,
                verticalScrollerVisibility = ScrollerVisibility.Auto,
#if UITK_NESTED_INTERACTION_KIND
                nestedInteractionKind = ScrollView.NestedInteractionKind.StopScrolling,
#endif
            };
            m_ScrollView.AddToClassList(scrollViewUssClassName);
            hierarchy.Add(m_ScrollView);
            m_ScrollView.Add(m_InputField);
#endif

            m_ResizeHandle = new VisualElement
            {
                name = resizeHandleUssClassName,
                pickingMode = PickingMode.Position,
            };
            m_ResizeHandle.AddToClassList(resizeHandleUssClassName);
            hierarchy.Add(m_ResizeHandle);
            var dragManipulator = new Draggable(null, OnDrag, null)
            {
                dragDirection = Draggable.DragDirection.Vertical
            };
            m_ResizeHandle.AddManipulator(dragManipulator);
            m_ResizeHandle.RegisterCallback<ClickEvent>(OnResizeHandleClicked);

            isReadOnly = k_IsReadOnlyDefault;
            maxLength = k_MaxLengthDefault;
            autoResize = false;
            placeholder = string.Empty;
            submitModifiers = EventModifiers.None;
            submitOnEnter = false;

            SetValueWithoutNotify(value);
            m_InputField.AddManipulator(new KeyboardFocusController(OnKeyboardFocusedIn, OnFocusedIn, OnFocusedOut));
            m_InputField.RegisterValueChangedCallback(OnInputValueChanged);
            m_Placeholder.RegisterValueChangedCallback(OnPlaceholderValueChanged);
            RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);
            RegisterCallback<KeyDownEvent>(OnKeyDown, TrickleDown.TrickleDown);
        }

        void OnKeyDown(KeyDownEvent evt)
        {
            if (evt.keyCode == KeyCode.Tab && !evt.shiftKey)
            {
                evt.StopPropagation();
#if !UNITY_2023_2_OR_NEWER
                evt.PreventDefault();
#endif
                m_RequestTab = true;
                return;
            }

            if (m_RequestTab && evt.keyCode == KeyCode.None)
            {
                evt.StopPropagation();
#if !UNITY_2023_2_OR_NEWER
                evt.PreventDefault();
#endif
                focusController.FocusNextInDirectionEx(this, VisualElementFocusChangeDirection.right);
            }

            if (submitOnEnter && evt.keyCode is KeyCode.Return or KeyCode.KeypadEnter && evt.modifiers == submitModifiers)
            {
                evt.StopPropagation();
#if !UNITY_2023_2_OR_NEWER
                evt.PreventDefault();
#endif
                m_RequestSubmit = true;
                return;
            }

            if (m_RequestSubmit && evt.keyCode == KeyCode.None)
            {
                evt.StopPropagation();
#if !UNITY_2023_2_OR_NEWER
                evt.PreventDefault();
#endif
                submitted?.Invoke();
            }

            m_RequestSubmit = false;
            m_RequestTab = false;
        }

        void OnResizeHandleClicked(ClickEvent evt)
        {
            if (evt.clickCount == 2)
            {
                evt.StopPropagation();
                autoResize = true;
                AutoResize();
            }
        }

        void OnGeometryChanged(GeometryChangedEvent evt)
        {
            var newHeight = contentRect.height;
            var currentHeight = m_InputField.resolvedStyle.minHeight;

            if (currentHeight.keyword == StyleKeyword.Auto || !Mathf.Approximately(newHeight, currentHeight.value))
                m_InputField.style.minHeight = newHeight;
        }

        static void OnPlaceholderValueChanged(ChangeEvent<string> evt)
        {
            evt.StopPropagation();
        }

        void OnInputValueChanged(ChangeEvent<string> e)
        {
            e.StopPropagation();

            if (autoResize)
                AutoResize();

            using var evt = ChangingEvent<string>.GetPooled();
            evt.target = this;
            evt.previousValue = m_Value;
            m_Value = e.newValue;
            evt.newValue = m_Value;

            if (validateValue != null) invalid = !validateValue(m_Value);
            RefreshUI();
            SendEvent(evt);
        }

        void AutoResize()
        {
            if (panel == null || !contentRect.IsValid())
                return;

            var width = m_InputField.resolvedStyle.width -
                m_InputField.resolvedStyle.borderLeftWidth -
                m_InputField.resolvedStyle.borderRightWidth -
                m_InputField.resolvedStyle.paddingLeft -
                m_InputField.resolvedStyle.paddingRight;

            var textSize = m_InputField.MeasureTextSize(
                m_InputField.text,
                width, MeasureMode.Exactly,
                0, MeasureMode.Undefined);

            var newHeight = textSize.y +
                resolvedStyle.paddingTop +
                resolvedStyle.paddingBottom +
                resolvedStyle.borderTopWidth +
                resolvedStyle.borderBottomWidth +
                m_InputField.resolvedStyle.borderTopWidth +
                m_InputField.resolvedStyle.borderBottomWidth +
                m_InputField.resolvedStyle.marginTop +
                m_InputField.resolvedStyle.marginBottom +
                m_InputField.resolvedStyle.paddingTop +
                m_InputField.resolvedStyle.paddingBottom;

            newHeight = Mathf.Max(resolvedStyle.minHeight.value, newHeight);

            if (newHeight > resolvedStyle.height)
                style.height = newHeight;
        }

        void OnDrag(Draggable draggable)
        {
            autoResize = false;
            style.height = Mathf.Max(resolvedStyle.minHeight.value, resolvedStyle.height + draggable.deltaPos.y);
        }

        /// <summary>
        /// The content container of the TextArea.
        /// </summary>
        public override VisualElement contentContainer => m_InputField.contentContainer;

        /// <summary>
        /// The TextArea placeholder text.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public string placeholder
        {
            get => m_Placeholder.text;
            set
            {
                var changed = m_Placeholder.text != value;
                m_Placeholder.text = value;

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in placeholderProperty);
#endif
            }
        }

        /// <summary>
        /// The validation function for the TextArea.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
        public Func<string, bool> validateValue
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
        /// The invalid state of the TextArea.
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
        /// Whether the TextArea is read-only.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public bool isReadOnly
        {
            get => m_InputField.isReadOnly;
            set
            {
                var changed = m_InputField.isReadOnly != value;
                m_InputField.isReadOnly = value;

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in isReadOnlyProperty);
#endif
            }
        }

        /// <summary>
        /// The maximum length of the TextArea.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public int maxLength
        {
            get => m_InputField.maxLength;
            set
            {
                var changed = m_InputField.maxLength != value;
                m_InputField.maxLength = value;

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in maxLengthProperty);
#endif
            }
        }

        /// <summary>
        /// Automatically resize the <see cref="TextArea"/> if the content is larger than the current size.
        /// </summary>
        /// <remarks>
        /// <para>This will only grow the <see cref="TextArea"/>. It will not shrink it.</para>
        /// <para>If the user manually resizes the <see cref="TextArea"/>, the auto resize will be disabled.</para>
        /// </remarks>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public bool autoResize
        {
            get => m_AutoResize;
            set
            {
                var changed = m_AutoResize != value;
                m_AutoResize = value;

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in autoResizeProperty);
#endif
            }
        }

        /// <summary>
        /// Set the TextArea value without notifying the change.
        /// </summary>
        /// <param name="newValue"> The new value of the TextArea. </param>
        public void SetValueWithoutNotify(string newValue)
        {
            m_Value = newValue;
            m_InputField.SetValueWithoutNotify(m_Value);
            RefreshUI();
            if (validateValue != null) invalid = !validateValue(m_Value);
        }

        /// <summary>
        /// Whether the TextArea should invoke the <see cref="submitted"/> event when the user presses the Enter key.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public bool submitOnEnter
        {
            get => m_SubmitOnEnter;
            set
            {
                var changed = m_SubmitOnEnter != value;
                m_SubmitOnEnter = value;

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in submitOnEnterProperty);
#endif
            }
        }

        /// <summary>
        /// The modifiers required to submit the TextArea.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public EventModifiers submitModifiers
        {
            get => m_SubmitModifiers;
            set
            {
                var changed = m_SubmitModifiers != value;
                m_SubmitModifiers = value;

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in submitModifiersProperty);
#endif
            }
        }

        /// <summary>
        /// The TextArea value.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public string value
        {
            get => m_InputField.value;
            set
            {
                if (m_Value == value && m_PreviousValue == value)
                {
                    RefreshUI();
                    return;
                }

                using var evt = ChangeEvent<string>.GetPooled(m_PreviousValue, value);
                m_PreviousValue = m_Value;
                evt.target = this;
                SetValueWithoutNotify(value);
                SendEvent(evt);

#if ENABLE_RUNTIME_DATA_BINDINGS
                NotifyPropertyChanged(in valueProperty);
#endif
            }
        }

        void OnFocusedOut(FocusOutEvent e)
        {
            RemoveFromClassList(Styles.focusedUssClassName);
            RemoveFromClassList(Styles.keyboardFocusUssClassName);
            passMask = 0;
            value = m_InputField.value;
        }

        void OnFocusedIn(FocusInEvent evt)
        {
            AddToClassList(Styles.focusedUssClassName);
            passMask = 0;
            m_PreviousValue = m_Value;
        }

        void OnKeyboardFocusedIn(FocusInEvent evt)
        {
            AddToClassList(Styles.focusedUssClassName);
            AddToClassList(Styles.keyboardFocusUssClassName);
            passMask = Passes.Clear | Passes.Outline;
            m_PreviousValue = m_Value;
        }

        void RefreshUI()
        {
            m_Placeholder.EnableInClassList(Styles.hiddenUssClassName, !string.IsNullOrEmpty(m_Value));
        }

#if ENABLE_UXML_TRAITS

        /// <summary>
        /// Factory class to instantiate a <see cref="TextArea"/> using the data read from a UXML file.
        /// </summary>
        public new class UxmlFactory : UxmlFactory<TextArea, UxmlTraits> { }

        /// <summary>
        /// Class containing the <see cref="UxmlTraits"/> for the <see cref="TextArea"/>.
        /// </summary>
        public new class UxmlTraits : ExVisualElement.UxmlTraits
        {
            readonly UxmlStringAttributeDescription m_Placeholder = new()
            {
                name = "placeholder",
                defaultValue = null
            };

            readonly UxmlStringAttributeDescription m_Value = new()
            {
                name = "value",
                defaultValue = null
            };

            readonly UxmlBoolAttributeDescription m_AutoResize = new()
            {
                name = "auto-resize",
                defaultValue = false
            };

            readonly UxmlBoolAttributeDescription m_SubmitOnEnter = new()
            {
                name = "submit-on-enter",
                defaultValue = false
            };

            readonly UxmlEnumAttributeDescription<EventModifiers> m_SubmitModifiers = new()
            {
                name = "submit-modifiers",
                defaultValue = EventModifiers.None
            };

            readonly UxmlBoolAttributeDescription m_IsReadOnly = new()
            {
                name = "is-read-only",
                defaultValue = k_IsReadOnlyDefault
            };

            readonly UxmlIntAttributeDescription m_MaxLength = new()
            {
                name = "max-length",
                defaultValue = k_MaxLengthDefault
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

                var el = (TextArea)ve;

                el.placeholder = m_Placeholder.GetValueFromBag(bag, cc);
                el.autoResize = m_AutoResize.GetValueFromBag(bag, cc);
                el.value = m_Value.GetValueFromBag(bag, cc);

                el.submitOnEnter = m_SubmitOnEnter.GetValueFromBag(bag, cc);
                el.submitModifiers = m_SubmitModifiers.GetValueFromBag(bag, cc);
                el.isReadOnly = m_IsReadOnly.GetValueFromBag(bag, cc);
                el.maxLength = m_MaxLength.GetValueFromBag(bag, cc);
            }
        }

#endif
    }
}
