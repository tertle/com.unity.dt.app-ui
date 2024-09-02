using System;
using UnityEngine;
using UnityEngine.UIElements;
#if ENABLE_RUNTIME_DATA_BINDINGS
using Unity.Properties;
#endif

namespace Unity.AppUI.UI
{
    /// <summary>
    /// A Pane is a visual element that can be used as a child of a <see cref="SplitView"/>.
    /// </summary>
#if ENABLE_UXML_SERIALIZED_DATA
    [UxmlElement]
#endif
    public partial class Pane : BaseVisualElement
    {
#if ENABLE_RUNTIME_DATA_BINDINGS
        internal static readonly BindingId compactThresholdProperty = nameof(compactThreshold);

        internal static readonly BindingId compactProperty = nameof(compact);

        internal static readonly BindingId stretchFactorProperty = nameof(stretchFactor);

        internal static readonly BindingId stretchProperty = nameof(stretch);
#endif

        static readonly EventCallback<GeometryChangedEvent> k_OnGeometryChanged = OnGeometryChanged;

        /// <summary>
        /// Event that is triggered when the pane is toggled between compact and expanded mode.
        /// </summary>
        public event Action<Pane> compactChanged;

        /// <summary>
        /// The default threshold used to snap to compact mode when the pane is resized.
        /// </summary>
        internal const float defaultCompactThreshold = 16f;

        /// <summary>
        /// The USS class name of a <see cref="Pane"/>.
        /// </summary>
        public const string ussClassName = "appui-pane";

        /// <summary>
        /// The USS class name of a <see cref="Pane"/> in compact mode.
        /// </summary>
        public const string compactUssClassName = ussClassName + "--compact";

        bool m_Compact;

        float m_CompactThreshold = defaultCompactThreshold;

        /// <summary>
        /// A threshold used to snap to compact mode when the pane is resized.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public float compactThreshold
        {
            get => m_CompactThreshold;
            set
            {
#if ENABLE_RUNTIME_DATA_BINDINGS
                var changed = Mathf.Approximately(m_CompactThreshold, value);
#endif
                m_CompactThreshold = value;

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in compactThresholdProperty);
#endif
            }
        }

        /// <summary>
        /// Whether the pane is in compact mode or not.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
        internal bool compact
        {
            get => m_Compact;
            set
            {
                var changed = m_Compact != value;
                SetCompact(value);

                if (changed)
                {
                    compactChanged?.Invoke(this);
#if ENABLE_RUNTIME_DATA_BINDINGS
                    NotifyPropertyChanged(in compactProperty);
#endif
                }
            }
        }

        /// <summary>
        /// The stretch factor of the pane.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public float stretchFactor
        {
            get => resolvedStyle.flexGrow;
            set
            {
#if ENABLE_RUNTIME_DATA_BINDINGS
                var changed = Mathf.Approximately(resolvedStyle.flexGrow, value);
                var stretchChanged = stretch != (value > 0);
#endif

                style.flexGrow = value;
                style.flexShrink = value > 0 ? 1 : 0;

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in stretchFactorProperty);
                if (stretchChanged)
                    NotifyPropertyChanged(in stretchProperty);
#endif
            }
        }

        /// <summary>
        /// Whether the pane can be stretched or not.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
        public bool stretch
        {
            get => stretchFactor > 0;
            set => stretchFactor = value ? 1 : 0;
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public Pane()
        {
            AddToClassList(ussClassName);
            pickingMode = PickingMode.Ignore;

            compactThreshold = defaultCompactThreshold;
            SetCompact(false);
            stretchFactor = 0;

            RegisterCallback(k_OnGeometryChanged);
        }

        static void OnGeometryChanged(GeometryChangedEvent evt)
        {
            if (evt.target is Pane { parent: SplitView splitView } pane)
                splitView.RefreshSplitterPosition(splitView.IndexOf(pane));
        }

        /// <summary>
        /// Toggles the pane between compact and expanded mode.
        /// </summary>
        public void ToggleCompact()
        {
            compact = !compact;
        }

        void SetCompact(bool value)
        {
            m_Compact = value;
            EnableInClassList(compactUssClassName, m_Compact);
        }

        /// <summary>
        /// Saves the state of the <see cref="Pane"/>.
        /// </summary>
        /// <returns> The state of the <see cref="Pane"/>.</returns>
        public State SaveState()
        {
            return new State
            {
                compact = compact,
                compactThreshold = compactThreshold
            };
        }

        /// <summary>
        /// Restores the state of the <see cref="Pane"/>.
        /// </summary>
        /// <param name="state"> The state of the <see cref="Pane"/>.</param>
        public void RestoreState(State state)
        {
            compactThreshold = state.compactThreshold;
            SetCompact(state.compact);
        }

        /// <summary>
        /// The state of a <see cref="Pane"/>.
        /// </summary>
        [Serializable]
        public struct State
        {
            /// <summary>
            /// Whether the pane is in compact mode or not.
            /// </summary>
            public bool compact;

            /// <summary>
            /// A threshold used to snap to compact mode when the pane is resized.
            /// </summary>
            public float compactThreshold;
        }

#if ENABLE_UXML_TRAITS

        /// <summary>
        /// Factory class to instantiate a <see cref="Pane"/> using the data read from a UXML file.
        /// </summary>
        public new class UxmlFactory : UxmlFactory<Pane, UxmlTraits> { }

        /// <summary>
        /// Class containing the <see cref="UxmlTraits"/> for the <see cref="Pane"/>.
        /// </summary>
        public new class UxmlTraits : BaseVisualElement.UxmlTraits
        {
            readonly UxmlFloatAttributeDescription m_CompactThreshold = new UxmlFloatAttributeDescription
            {
                name = "compact-threshold",
                defaultValue = defaultCompactThreshold
            };

            readonly UxmlFloatAttributeDescription m_StretchFactor = new UxmlFloatAttributeDescription
            {
                name = "stretch-factor",
                defaultValue = 0
            };

            /// <summary>
            /// Initializes the VisualElement from the UXML attributes.
            /// </summary>
            /// <param name="ve"> The <see cref="VisualElement"/> to initialize.</param>
            /// <param name="bag"> The <see cref="IUxmlAttributes"/> bag to use to initialize the <see cref="VisualElement"/>.</param>
            /// <param name="cc"> The <see cref="CreationContext"/> to use to initialize the <see cref="VisualElement"/>.</param>
            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                m_PickingMode.defaultValue = PickingMode.Ignore;
                base.Init(ve, bag, cc);
                var el = (Pane)ve;

                el.compactThreshold = m_CompactThreshold.GetValueFromBag(bag, cc);
                el.stretchFactor = m_StretchFactor.GetValueFromBag(bag, cc);
            }
        }
#endif
    }
}
