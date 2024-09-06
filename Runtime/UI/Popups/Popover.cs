using System;
using Unity.AppUI.Core;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.AppUI.UI
{
    /// <summary>
    /// Possible placements for a Popover.
    /// </summary>
    [Serializable]
    public enum PopoverPlacement
    {
        /// <summary>
        /// The Popover will be placed at the bottom center of the target.
        /// </summary>
        Bottom,

        /// <summary>
        /// The Popover will be placed at the bottom-left of the target.
        /// </summary>
        BottomLeft,

        /// <summary>
        /// The Popover will be placed at the bottom-right of the target.
        /// </summary>
        BottomRight,

        /// <summary>
        /// The Popover will be placed at the bottom-start of the target.
        /// </summary>
        BottomStart,

        /// <summary>
        /// The Popover will be placed at the bottom-end of the target.
        /// </summary>
        BottomEnd,

        /// <summary>
        /// The Popover will be placed at the top center of the target.
        /// </summary>
        Top,

        /// <summary>
        /// The Popover will be placed at the top-left of the target.
        /// </summary>
        TopLeft,

        /// <summary>
        /// The Popover will be placed at the top-right of the target.
        /// </summary>
        TopRight,

        /// <summary>
        /// The Popover will be placed at the top-start of the target.
        /// </summary>
        TopStart,

        /// <summary>
        /// The Popover will be placed at the top-end of the target.
        /// </summary>
        TopEnd,

        /// <summary>
        /// The Popover will be placed at the left center of the target.
        /// </summary>
        Left,

        /// <summary>
        /// The Popover will be placed at the left-top of the target.
        /// </summary>
        LeftTop,

        /// <summary>
        /// The Popover will be placed at the left-bottom of the target.
        /// </summary>
        LeftBottom,

        /// <summary>
        /// The Popover will be placed at the start center of the target.
        /// </summary>
        Start,

        /// <summary>
        /// The Popover will be placed at the start-top of the target.
        /// </summary>
        StartTop,

        /// <summary>
        /// The Popover will be placed at the start-bottom of the target.
        /// </summary>
        StartBottom,

        /// <summary>
        /// The Popover will be placed at the right center of the target.
        /// </summary>
        Right,

        /// <summary>
        /// The Popover will be placed at the right-top of the target.
        /// </summary>
        RightTop,

        /// <summary>
        /// The Popover will be placed at the right-bottom of the target.
        /// </summary>
        RightBottom,

        /// <summary>
        /// The Popover will be placed at the end center of the target.
        /// </summary>
        End,

        /// <summary>
        /// The Popover will be placed at the end-top of the target.
        /// </summary>
        EndTop,

        /// <summary>
        /// The Popover will be placed at the end-bottom of the target.
        /// </summary>
        EndBottom,

        /// <summary>
        /// The Popover will be placed inside the target, at the top left.
        /// </summary>
        InsideTopStart,

        /// <summary>
        /// The Popover will be placed inside the target, at the top left.
        /// </summary>
        InsideTopLeft,

        /// <summary>
        /// The Popover will be placed inside the target, at the top center.
        /// </summary>
        InsideTop,

        /// <summary>
        /// The Popover will be placed inside the target, at the top right.
        /// </summary>
        InsideTopEnd,

        /// <summary>
        /// The Popover will be placed inside the target, at the bottom left.
        /// </summary>
        InsideBottomStart,

        /// <summary>
        /// The Popover will be placed inside the target, at the bottom center.
        /// </summary>
        InsideBottom,

        /// <summary>
        /// The Popover will be placed inside the target, at the bottom right.
        /// </summary>
        InsideBottomEnd,

        /// <summary>
        /// The Popover will be placed inside the target, at the center left.
        /// </summary>
        InsideStart,

        /// <summary>
        /// The Popover will be placed inside the target, at the center right.
        /// </summary>
        InsideEnd,

        /// <summary>
        /// The Popover will be placed inside the target, at the center.
        /// </summary>
        InsideCenter,
    }

    /// <summary>
    /// The position result data structure returned in <see cref="AnchorPopupUtils.ComputePosition"/> utility method.
    /// </summary>
    public struct PositionResult
    {
        /// <summary>
        /// The Y Position from the top, in pixels.
        /// </summary>
        public float top { get; set; }

        /// <summary>
        /// The X Position from the left, in pixels.
        /// </summary>
        public float left { get; set; }

        /// <summary>
        /// The top margin, in pixels.
        /// </summary>
        public float marginTop { get; set; }

        /// <summary>
        /// The left margin, in pixels.
        /// </summary>
        public float marginLeft { get; set; }

        /// <summary>
        /// The computed placement, that may differ from the desired one.
        /// </summary>
        public PopoverPlacement finalPlacement { get; set; }

        /// <summary>
        /// The USS left value for the tip element.
        /// </summary>
        public float tipLeft { get; set; }

        /// <summary>
        /// The USS right value for the tip element.
        /// </summary>
        public float tipRight { get; set; }

        /// <summary>
        /// The USS top value for the tip element.
        /// </summary>
        public float tipTop { get; set; }

        /// <summary>
        /// The USS bottom value for the tip element.
        /// </summary>
        public float tipBottom { get; set; }
    }

    /// <summary>
    /// Options to pass as argument to <see cref="AnchorPopupUtils.ComputePosition"/> utility method.
    /// </summary>
    public struct PositionOptions
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="preferredPlacement"> The preferred placement for the popover.</param>
        /// <param name="offset"> The offset from the anchor element.</param>
        /// <param name="crossOffset"> The cross offset from the anchor element.</param>
        /// <param name="shouldFlip"> Whether the popover should flip if it doesn't fit in the viewport.</param>
        /// <param name="crossSnap"> Whether the popover should snap not to go outside the viewport.</param>
        public PositionOptions(PopoverPlacement preferredPlacement = PopoverPlacement.Top, int offset = 0, int crossOffset = 0, bool shouldFlip = true, bool crossSnap = true)
        {
            favoritePlacement = preferredPlacement;
            this.offset = offset;
            this.crossOffset = crossOffset;
            this.shouldFlip = shouldFlip;
            this.crossSnap = crossSnap;
        }

        /// <summary>
        /// The preferred placement for the popover.
        /// </summary>
        public PopoverPlacement favoritePlacement { get; set; }

        /// <summary>
        /// The offset from the anchor element.
        /// </summary>
        public int offset { get; set; }

        /// <summary>
        /// The cross offset from the anchor element.
        /// </summary>
        public int crossOffset { get; set; }

        /// <summary>
        /// Whether the popover should flip if it doesn't fit in the viewport.
        /// </summary>
        public bool shouldFlip { get; set; }

        /// <summary>
        /// Whether the popover should snap not to go outside the viewport.
        /// </summary>
        public bool crossSnap { get; set; }
    }

    /// <summary>
    /// A popup usually anchored to another UI element.
    /// </summary>
    public sealed class Popover : AnchorPopup<Popover>
    {
        /// <summary>
        /// Enable or disable the blocking of outside click events.
        /// </summary>
        public bool modalBackdrop
        {
            get => popover.modalBackdrop;
            set => popover.modalBackdrop = value;
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="referenceView">The view used as context provider for the Popover.</param>
        /// <param name="popover">The popup visual element itself.</param>
        /// <param name="contentView">The content that will appear inside this popup.</param>
        Popover(VisualElement referenceView, PopoverVisualElement popover, VisualElement contentView)
            : base(referenceView, popover, contentView)
        { }

        PopoverVisualElement popover => (PopoverVisualElement)view;

        /// <summary>
        /// Build a new <see cref="Popover"/> instance.
        /// </summary>
        /// <param name="referenceView">An arbitrary UI element in the current panel.</param>
        /// <param name="contentView">The content that will appear inside this popup.</param>
        /// <returns>The <see cref="Popover"/> instance.</returns>
        /// <exception cref="ArgumentNullException">If the referenceView is null.</exception>
        public static Popover Build(VisualElement referenceView, VisualElement contentView)
        {
            if (referenceView == null)
                throw new ArgumentNullException(nameof(referenceView));

            var popoverVisualElement = new PopoverVisualElement(contentView);
            var popoverElement = new Popover(referenceView, popoverVisualElement, contentView)
                .SetAnchor(referenceView)
                .SetLastFocusedElement(referenceView);
            return popoverElement;
        }

        void OnWheel(WheelEvent evt)
        {
            if (outsideScrollEnabled)
                return;

            var inside = GetMovableElement().worldBound.Contains((Vector2)evt.mousePosition);
            if (!inside)
                evt.StopImmediatePropagation();
        }

        void OnTreeDown(PointerDownEvent evt)
        {
            if (!outsideClickDismissEnabled || outsideClickStrategy == 0 || view.parent == null)
                return;

            var index = view.parent.IndexOf(view);
            if (index != view.parent.childCount - 1)
                return;

            var shouldDismiss = true;
            if ((outsideClickStrategy & OutsideClickStrategy.Bounds) != 0)
                shouldDismiss = !GetMovableElement().worldBound.Contains((Vector2)evt.position);

            if (shouldDismiss && (outsideClickStrategy & OutsideClickStrategy.Pick) != 0)
            {
                var picked = view.panel.Pick(evt.position);
                var commonAncestor = picked?.FindCommonAncestor(view);
                if (commonAncestor == view) // if the picked element is a child of the popover, don't dismiss
                    shouldDismiss = false;
            }

            if (!shouldDismiss)
                return;

            var insideAnchor = anchor?.worldBound.Contains((Vector2)evt.position) ?? false;
            var insideLastFocusedElement = (m_LastFocusedElement as VisualElement)?.worldBound.Contains((Vector2)evt.position) ?? false;
            if (insideAnchor || insideLastFocusedElement)
            {
                // prevent reopening the same popover again...
                evt.StopImmediatePropagation();
            }
            Dismiss(DismissType.OutOfBounds);
        }

        /// <inheritdoc cref="Popup.ShouldAnimate"/>
        protected override bool ShouldAnimate()
        {
            return true;
        }

        /// <summary>
        /// Enable or disable the blocking of outside click events.
        /// </summary>
        /// <param name="enableModalBackdrop"> Whether to enable the blocking of outside click events.</param>
        /// <returns> The <see cref="Popover"/> instance.</returns>
        public Popover SetModalBackdrop(bool enableModalBackdrop)
        {
            modalBackdrop = enableModalBackdrop;
            return this;
        }

        /// <inheritdoc cref="Popup.GetFocusableElement"/>
        protected override VisualElement GetFocusableElement()
        {
            return popover.popoverElement;
        }

        /// <inheritdoc cref="AnchorPopup{T}.GetMovableElement"/>
        public override VisualElement GetMovableElement()
        {
            return popover.popoverElement;
        }

        /// <inheritdoc />
        protected override void InvokeShownEventHandlers()
        {
            base.InvokeShownEventHandlers();
            containerView?.panel?.visualTree?.RegisterCallback<PointerDownEvent>(OnTreeDown, TrickleDown.TrickleDown);
            containerView?.panel?.visualTree?.RegisterCallback<WheelEvent>(OnWheel, TrickleDown.TrickleDown);
        }

        /// <inheritdoc />
        protected override void HideView(DismissType reason)
        {
            containerView?.panel?.visualTree?.UnregisterCallback<PointerDownEvent>(OnTreeDown, TrickleDown.TrickleDown);
            containerView?.panel?.visualTree?.UnregisterCallback<WheelEvent>(OnWheel, TrickleDown.TrickleDown);
            base.HideView(reason);
        }

        /// <summary>
        /// The UI element used as a Popover.
        /// </summary>
        internal class PopoverVisualElement : VisualElement, IPlaceableElement
        {
            public const string ussClassName = "appui-popover";

            public const string modalBackdropUssClassName = ussClassName + "--modal-backdrop";

            public const string popoverUssClassName = ussClassName + "__popover";

            public const string containerUssClassName = ussClassName + "__container";

            public const string shadowElementUssClassName = ussClassName + "__shadow-element";

            public const string tipUssClassName = ussClassName + "__tip";

            public const string upUssClassName = ussClassName + "--up";

            public const string downUssClassName = ussClassName + "--down";

            public const string leftUssClassName = ussClassName + "--left";

            public const string rightUssClassName = ussClassName + "--right";

            readonly VisualElement m_ContentContainer;

            PopoverPlacement m_Placement = PopoverPlacement.Top;

            /// <summary>
            /// Default constructor.
            /// </summary>
            /// <param name="content">The content of the popup.</param>
            public PopoverVisualElement(VisualElement content)
            {
                AddToClassList(ussClassName);

                modalBackdrop = false;

                popoverElement = new VisualElement
                {
                    name = popoverUssClassName,
                    pickingMode = PickingMode.Ignore,
                    focusable = true,
                    usageHints = UsageHints.DynamicTransform,
                };
                popoverElement.AddToClassList(popoverUssClassName);
                hierarchy.Add(popoverElement);

                var shadowElement = new ExVisualElement
                {
                    name = shadowElementUssClassName,
                    pickingMode = PickingMode.Ignore,
                    focusable = false,
                    passMask = ExVisualElement.Passes.Clear | ExVisualElement.Passes.OutsetShadows
                };
                shadowElement.AddToClassList(shadowElementUssClassName);
                popoverElement.hierarchy.Add(shadowElement);

                tipElement = new VisualElement { name = tipUssClassName, pickingMode = PickingMode.Ignore, focusable = false };
                tipElement.AddToClassList(tipUssClassName);
                popoverElement.hierarchy.Add(tipElement);

                m_ContentContainer = new VisualElement
                {
                    name = containerUssClassName,
                    pickingMode = PickingMode.Ignore,
                    focusable = false,
                };
                m_ContentContainer.AddToClassList(containerUssClassName);
                popoverElement.hierarchy.Add(m_ContentContainer);

                m_ContentContainer.Add(content);

                RefreshPlacement();
            }

            /// <summary>
            /// The popover UI element.
            /// <remarks>This is the real popover element that needs to be anchored. Its parent is usually a smir.</remarks>
            /// </summary>
            public VisualElement popoverElement { get; }

            public VisualElement tipElement { get; }

            public override VisualElement contentContainer => m_ContentContainer;

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

            public bool modalBackdrop
            {
                get => ClassListContains(modalBackdropUssClassName);
                set
                {
                    EnableInClassList(modalBackdropUssClassName, value);
                    pickingMode = value ? PickingMode.Position : PickingMode.Ignore;
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
                    case PopoverPlacement.InsideTopStart:
                    case PopoverPlacement.InsideTopLeft:
                    case PopoverPlacement.InsideTop:
                        up = true;
                        break;
                    case PopoverPlacement.Top:
                    case PopoverPlacement.TopLeft:
                    case PopoverPlacement.TopRight:
                    case PopoverPlacement.TopStart:
                    case PopoverPlacement.TopEnd:
                    case PopoverPlacement.InsideBottomStart:
                    case PopoverPlacement.InsideBottom:
                    case PopoverPlacement.InsideBottomEnd:
                        down = true;
                        break;
                    case PopoverPlacement.Left:
                    case PopoverPlacement.LeftTop:
                    case PopoverPlacement.LeftBottom:
                    case PopoverPlacement.Start:
                    case PopoverPlacement.StartTop:
                    case PopoverPlacement.StartBottom:
                    case PopoverPlacement.InsideEnd:
                        right = true;
                        break;
                    case PopoverPlacement.Right:
                    case PopoverPlacement.RightTop:
                    case PopoverPlacement.RightBottom:
                    case PopoverPlacement.End:
                    case PopoverPlacement.EndTop:
                    case PopoverPlacement.EndBottom:
                    case PopoverPlacement.InsideStart:
                        left = true;
                        break;
                    case PopoverPlacement.InsideCenter:
                        break;
                    default:
                        throw new ValueOutOfRangeException(nameof(m_Placement), m_Placement);
                }

                EnableInClassList(upUssClassName, up);
                EnableInClassList(downUssClassName, down);
                EnableInClassList(leftUssClassName, left);
                EnableInClassList(rightUssClassName, right);
            }
        }
    }
}
