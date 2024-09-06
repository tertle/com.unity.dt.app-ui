using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.AppUI.UI
{
    /// <summary>
    /// The tooltip popup type.
    /// </summary>
    public sealed class Tooltip : AnchorPopup<Tooltip>
    {
        /// <summary>
        /// The default placement of the tooltip.
        /// </summary>
        public const PopoverPlacement defaultPlacement = PopoverPlacement.Bottom;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="referenceView"> The element used as context provider for the tooltip. </param>
        /// <param name="contentView">The content to display inside the popup.</param>
        Tooltip(VisualElement referenceView, VisualElement contentView)
            : base(referenceView, contentView)
        {
            contentView.style.position = Position.Absolute; // force to absolute.
            keyboardDismissEnabled = false;
        }

        TooltipVisualElement tooltip => (TooltipVisualElement)view;

        /// <summary>
        /// The text to display inside the popup.
        /// </summary>
        public string text => tooltip.text;

        /// <summary>
        /// Set a new value for the <see cref="text"/> property.
        /// </summary>
        /// <param name="value"> The new value (will be localized). </param>
        /// <returns>The Tooltip.</returns>
        public Tooltip SetText(string value)
        {
            if (!string.IsNullOrEmpty(value))
                tooltip.contentContainer.Clear();
            tooltip.text = value;
            return this;
        }

        /// <summary>
        /// The template to display inside the popup.
        /// </summary>
        public VisualElement template => tooltip.contentContainer.childCount > 0 ? tooltip.contentContainer[0] : null;

        /// <summary>
        /// The content Visual Element of the tooltip (if any).
        /// </summary>
        public VisualElement content => tooltip.contentContainer.childCount > 0 ? tooltip.contentContainer[0] : null;

        /// <summary>
        /// Set the content of the tooltip.
        /// </summary>
        /// <remarks>
        /// Passing null will clear the content of the tooltip.
        /// </remarks>
        /// <param name="content"> The content to display inside the tooltip. </param>
        /// <returns> The Tooltip. </returns>
        public Tooltip SetContent(VisualElement content)
        {
            if (content?.parent == tooltip.contentContainer)
                return this;

            tooltip.contentContainer.Clear();
            tooltip.text = null;
            if (content != null)
                tooltip.contentContainer.Add(content);
            return this;
        }

        /// <inheritdoc cref="Popup.ShouldAnimate"/>
        protected override bool ShouldAnimate()
        {
            return true;
        }

        /// <inheritdoc cref="Popup.FindSuitableParent"/>
        protected override VisualElement FindSuitableParent(VisualElement element)
        {
            return Panel.FindTooltipLayer(element);
        }

        /// <summary>
        /// Build a new Tooltip.
        /// </summary>
        /// <param name="referenceView">An arbitrary UI element used as reference for the application
        /// context to attach to the popup.</param>
        /// <returns>A Tooltip instance.</returns>
        /// <remarks>
        /// In the Application element, only one Tooltip is create and moved at the right place when hovering others UI
        /// elements. The Tooltip is handled by the <see cref="TooltipManipulator"/>.
        /// </remarks>
        /// <exception cref="ArgumentNullException">If <paramref name="referenceView"/> is null.</exception>
        public static Tooltip Build(VisualElement referenceView)
        {
            if (referenceView == null)
                throw new ArgumentNullException(nameof(referenceView));

            var tooltipElement = new Tooltip(referenceView, new TooltipVisualElement())
                .SetPlacement(defaultPlacement);

            return tooltipElement;
        }

        /// <summary>
        /// The Tooltip UI Element.
        /// </summary>
        sealed class TooltipVisualElement : VisualElement, IPlaceableElement
        {
            public const string ussClassName = "appui-tooltip";

            public const string containerUssClassName = ussClassName + "__container";

            public const string contentUssClassName = ussClassName + "__content";

            public const string tipUssClassName = ussClassName + "__tip";

            public const string upDirectionUssClassName = ussClassName + "--up";

            public const string downDirectionUssClassName = ussClassName + "--down";

            public const string leftDirectionUssClassName = ussClassName + "--left";

            public const string rightDirectionUssClassName = ussClassName + "--right";

            readonly ExVisualElement m_Container;

            PopoverPlacement m_Placement;

            readonly LocalizedTextElement m_Content;

            /// <summary>
            /// Default constructor.
            /// </summary>
            public TooltipVisualElement()
            {
                AddToClassList(ussClassName);

                m_Container = new ExVisualElement
                {
                    name = containerUssClassName,
                    usageHints = UsageHints.DynamicTransform,
                    pickingMode = PickingMode.Ignore,
                    passMask = ExVisualElement.Passes.Clear | ExVisualElement.Passes.OutsetShadows
                };
                m_Container.AddToClassList(containerUssClassName);
                hierarchy.Add(m_Container);

                tipElement = new VisualElement { name = tipUssClassName, pickingMode = PickingMode.Ignore };
                tipElement.AddToClassList(tipUssClassName);
                hierarchy.Add(tipElement);

                m_Content = new LocalizedTextElement { name = contentUssClassName, pickingMode = PickingMode.Ignore };
                m_Content.AddToClassList(contentUssClassName);
                m_Container.hierarchy.Add(m_Content);

                placement = defaultPlacement;
            }

            public override VisualElement contentContainer => m_Content;

            public VisualElement tipElement { get; }

            /// <summary>
            /// The text to display inside the Tooltip.
            /// </summary>
            public string text
            {
                get => m_Content.text;
                set => m_Content.text = value;
            }

            /// <summary>
            /// The popup placement, used to display the arrow at the right place.
            /// </summary>
            public PopoverPlacement placement
            {
                get => m_Placement;

                set
                {
                    m_Placement = value;
                    RefreshPlacement();
                }
            }

            void RefreshPlacement()
            {
                bool up = false, down = false, left = false, right = false;

                switch (m_Placement)
                {
                    case PopoverPlacement.Bottom:
                    case PopoverPlacement.BottomLeft:
                    case PopoverPlacement.BottomRight:
                    case PopoverPlacement.BottomStart:
                    case PopoverPlacement.BottomEnd:
                        up = true;
                        break;
                    case PopoverPlacement.Top:
                    case PopoverPlacement.TopLeft:
                    case PopoverPlacement.TopRight:
                    case PopoverPlacement.TopStart:
                    case PopoverPlacement.TopEnd:
                        down = true;
                        break;
                    case PopoverPlacement.Left:
                    case PopoverPlacement.LeftTop:
                    case PopoverPlacement.LeftBottom:
                    case PopoverPlacement.Start:
                    case PopoverPlacement.StartTop:
                    case PopoverPlacement.StartBottom:
                        right = true;
                        break;
                    case PopoverPlacement.Right:
                    case PopoverPlacement.RightTop:
                    case PopoverPlacement.RightBottom:
                    case PopoverPlacement.End:
                    case PopoverPlacement.EndTop:
                    case PopoverPlacement.EndBottom:
                        left = true;
                        break;
                    default:
                        throw new ValueOutOfRangeException(nameof(m_Placement), m_Placement);
                }

                EnableInClassList(upDirectionUssClassName, up);
                EnableInClassList(downDirectionUssClassName, down);
                EnableInClassList(leftDirectionUssClassName, left);
                EnableInClassList(rightDirectionUssClassName, right);
            }
        }
    }
}
