using System;
using System.Collections;
using System.Collections.Generic;
using Unity.AppUI.Core;
using UnityEngine;
using UnityEngine.UIElements;
#if ENABLE_RUNTIME_DATA_BINDINGS
using Unity.Properties;
#endif

namespace Unity.AppUI.UI
{
    /// <summary>
    /// An item used in <see cref="Tabs"/> bar.
    /// </summary>
#if ENABLE_UXML_SERIALIZED_DATA
    [UxmlElement]
#endif
    public partial class TabItem : BaseVisualElement, ISelectableElement, IPressable
    {
#if ENABLE_RUNTIME_DATA_BINDINGS

        internal static readonly BindingId selectedProperty = nameof(selected);

        internal static readonly BindingId labelProperty = nameof(label);

        internal static readonly BindingId iconProperty = nameof(icon);

#endif


        /// <summary>
        /// The TabItem main styling class.
        /// </summary>
        public const string ussClassName = "appui-tabitem";

        /// <summary>
        /// The TabItem label styling class.
        /// </summary>
        public const string labelUssClassName = ussClassName + "__label";

        /// <summary>
        /// The TabItem icon styling class.
        /// </summary>
        public const string iconUssClassName = ussClassName + "__icon";

        readonly Icon m_Icon;

        readonly LocalizedTextElement m_Label;

        Pressable m_Clickable;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public TabItem()
        {
            focusable = true;
            pickingMode = PickingMode.Position;
            tabIndex = 0;
            clickable = new Pressable(OnPressed);

            AddToClassList(ussClassName);

            m_Icon = new Icon { name = iconUssClassName, pickingMode = PickingMode.Ignore };
            m_Icon.AddToClassList(iconUssClassName);
            hierarchy.Add(m_Icon);
            m_Label = new LocalizedTextElement { name = labelUssClassName, pickingMode = PickingMode.Ignore };
            m_Label.AddToClassList(labelUssClassName);
            hierarchy.Add(m_Label);

            label = null;
            icon = null;
            selected = false;
        }

        void OnPressed()
        {
            using var evt = ActionTriggeredEvent.GetPooled();
            evt.target = this;
            SendEvent(evt);
        }

        /// <summary>
        /// The TabItem label.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public string label
        {
            get => m_Label.text;
            set
            {
                var changed = m_Label.text != value;
                m_Label.text = value;
                m_Label.EnableInClassList(Styles.hiddenUssClassName, string.IsNullOrEmpty(m_Label.text));

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in labelProperty);
#endif
            }
        }

        /// <summary>
        /// The TabItem icon.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public string icon
        {
            get => m_Icon.iconName;
            set
            {
                var changed = m_Icon.iconName != value;
                m_Icon.iconName = value;
                m_Icon.EnableInClassList(Styles.hiddenUssClassName, string.IsNullOrEmpty(m_Icon.iconName));

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in iconProperty);
#endif
            }
        }

        /// <summary>
        /// Clickable Manipulator for this TabItem.
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
        /// The selected state of the TabItem.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
        public bool selected
        {
            get => ClassListContains(Styles.selectedUssClassName);
            set
            {
                var changed = selected != value;
                SetSelectedWithoutNotify(value);

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in selectedProperty);
#endif
            }
        }

        /// <summary>
        /// Set the selected state of the TabItem without notifying the selection system.
        /// </summary>
        /// <param name="newValue"> The new selected state.</param>
        public void SetSelectedWithoutNotify(bool newValue)
        {
            EnableInClassList(Styles.selectedUssClassName, newValue);
        }

#if ENABLE_UXML_TRAITS

        /// <summary>
        /// Factory class to instantiate a <see cref="TabItem"/> using the data read from a UXML file.
        /// </summary>
        public new class UxmlFactory : UxmlFactory<TabItem, UxmlTraits> { }

        /// <summary>
        /// Class containing the <see cref="UxmlTraits"/> for the <see cref="TabItem"/>.
        /// </summary>
        public new class UxmlTraits : BaseVisualElement.UxmlTraits
        {

            readonly UxmlStringAttributeDescription m_Icon = new UxmlStringAttributeDescription
            {
                name = "icon",
                defaultValue = null
            };

            readonly UxmlStringAttributeDescription m_Label = new UxmlStringAttributeDescription
            {
                name = "label",
                defaultValue = null
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

                var el = (TabItem)ve;
                el.icon = m_Icon.GetValueFromBag(bag, cc);
                el.label = m_Label.GetValueFromBag(bag, cc);


            }
        }

#endif
    }
}
