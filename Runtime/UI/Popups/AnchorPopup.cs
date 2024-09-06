using System;
using Unity.AppUI.Core;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.AppUI.UI
{
    /// <summary>
    /// Interface that should be implemented in UI elements that can be anchored to another UI element.
    /// </summary>
    public interface IPlaceableElement
    {
        /// <summary>
        /// The popover placement.
        /// </summary>
        PopoverPlacement placement { get; set; }

        /// <summary>
        /// The popover tip/arrow element.
        /// </summary>
        VisualElement tipElement { get; }
    }

    /// <summary>
    /// The strategy used to determine if a click is outside of the popup.
    /// </summary>
    [Flags]
    public enum OutsideClickStrategy
    {
        /// <summary>
        /// A click is considered outside if the cursor position is outside of the popup's bounds.
        /// </summary>
        Bounds = 1,

        /// <summary>
        /// A click is considered outside if the picked element at the cursor position is not a child of the popup.
        /// </summary>
        Pick = 2,
    }

    /// <summary>
    /// Base class for Popup that can be anchored to another UI Element.
    /// </summary>
    /// <typeparam name="T">The sealed anchor popup class type.</typeparam>
    public abstract class AnchorPopup<T> : Popup<T> where T : AnchorPopup<T>
    {
        VisualElement m_Anchor;

        Rect m_AnchorBounds;

        int m_CrossOffset;

        PopoverPlacement m_CurrentPlacement;

        int m_Offset;

        PopoverPlacement m_Placement = PopoverPlacement.Bottom;

        bool m_ShouldFlip = true;

        Rect m_ContentBounds;

        /// <summary>
        /// Callback for Event triggered when the popup has been shown.
        /// </summary>
        protected readonly EventCallback<ITransitionEvent> m_OnAnimatedInAction;

        /// <summary>
        /// Callback for Event triggered when the content geometry has changed.
        /// </summary>
        protected readonly EventCallback<GeometryChangedEvent> m_OnContentGeometryChangedAction;

        /// <summary>
        /// Callback for Event triggered when the anchor geometry has changed.
        /// </summary>
        protected readonly EventCallback<GeometryChangedEvent> m_OnAnchorGeometryChangedAction;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="referenceView"> The visual element used as context provider for the popup.</param>
        /// <param name="view">The popup visual element itself.</param>
        /// <param name="contentView">The content that will appear inside this popup.</param>
        protected AnchorPopup(VisualElement referenceView, VisualElement view, VisualElement contentView = null)
            : base(referenceView, view, contentView)
        {
            m_OnAnimatedInAction = new EventCallback<ITransitionEvent>(OnAnimatedIn);
            m_OnContentGeometryChangedAction = new EventCallback<GeometryChangedEvent>(OnContentGeometryChanged);
            m_OnAnchorGeometryChangedAction = new EventCallback<GeometryChangedEvent>(OnAnchorGeometryChanged);
        }

        /// <summary>
        /// The desired placement.
        /// </summary>
        /// <remarks>
        /// You can set the desired placement using <see cref="SetPlacement"/>.
        /// </remarks>
        public PopoverPlacement placement => m_Placement;

        /// <summary>
        /// The current placement.
        /// </summary>
        /// <remarks>
        /// The current placement can be different from the placement set with <see cref="SetPlacement"/>, based
        /// on the current position of the anchor on the screen and the ability to flip placement.
        /// </remarks>
        public PopoverPlacement currentPlacement => m_CurrentPlacement;

        /// <summary>
        /// The offset in pixels, in the direction of the primary placement vector.
        /// </summary>
        public int offset => m_Offset;

        /// <summary>
        /// The offset in pixels, in the direction of the secondary placement vector.
        /// </summary>
        public int crossOffset => m_CrossOffset;

        /// <summary>
        /// The padding in pixels, inside the popup panel's container.
        /// </summary>
        public int containerPadding { get; private set; }

        /// <summary>
        /// `True` if the popup will be displayed at the opposite position if there's not enough
        /// place using the preferred <see cref="placement"/>, `False` otherwise.
        /// </summary>
        public bool shouldFlip => m_ShouldFlip;

        /// <summary>
        /// `True` if the small arrow used next to the anchor should be visible, `False` otherwise.
        /// </summary>
        public bool arrowVisible { get; private set; } = true;

        /// <summary>
        /// `True` if the popup can be dismissed by clicking outside of it, `False` otherwise.
        /// </summary>
        public bool outsideClickDismissEnabled { get; protected set; } = true;

        /// <summary>
        /// `True` if the popup let the user scroll outside of it, `False` otherwise.
        /// </summary>
        public bool outsideScrollEnabled { get; protected set; } = false;

        /// <summary>
        /// The strategy used to determine if the click is outside the popup.
        /// </summary>
        public OutsideClickStrategy outsideClickStrategy { get; protected set; } = OutsideClickStrategy.Bounds;

        /// <summary>
        /// The popup's anchor.
        /// </summary>
        public VisualElement anchor => m_Anchor;

        /// <summary>
        /// Set the preferred <see cref="placement"/> value.
        /// </summary>
        /// <remarks>This will trigger a refresh of the current popup position automatically.</remarks>
        /// <param name="popoverPlacement">The new value.</param>
        /// <returns>The popup.</returns>
        public T SetPlacement(PopoverPlacement popoverPlacement)
        {
            m_Placement = popoverPlacement;
            RefreshPosition();
            return (T)this;
        }

        /// <summary>
        /// Set a new value for the <see cref="offset"/> property.
        /// </summary>
        /// <param name="value">The new value.</param>
        /// <returns>The popup.</returns>
        public T SetOffset(int value)
        {
            m_Offset = value;
            return (T)this;
        }

        /// <summary>
        /// Set a new value for the <see cref="crossOffset"/> property.
        /// </summary>
        /// <param name="value">The new value.</param>
        /// <returns>The popup.</returns>
        public T SetCrossOffset(int value)
        {
            m_CrossOffset = value;
            return (T)this;
        }

        /// <summary>
        /// Set a new value for the <see cref="containerPadding"/> property.
        /// </summary>
        /// <remarks>This will trigger a refresh of the current popup position automatically.</remarks>
        /// <param name="value">The new value.</param>
        /// <returns>The popup.</returns>
        public T SetContainerPadding(int value)
        {
            containerPadding = value;
            view.contentContainer.style.paddingBottom = containerPadding;
            view.contentContainer.style.paddingLeft = containerPadding;
            view.contentContainer.style.paddingRight = containerPadding;
            view.contentContainer.style.paddingTop = containerPadding;
            RefreshPosition();
            return (T)this;
        }

        /// <summary>
        /// Set a new value for the <see cref="shouldFlip"/> property.
        /// </summary>
        /// <remarks>This will trigger a refresh of the current popup position automatically.</remarks>
        /// <param name="value">The new value.</param>
        /// <returns>The popup.</returns>
        public T SetShouldFlip(bool value)
        {
            m_ShouldFlip = value;
            RefreshPosition();
            return (T)this;
        }

        /// <summary>
        /// Set a new value for the <see cref="arrowVisible"/> property.
        /// </summary>
        /// <remarks>This will trigger a refresh of the current popup position automatically.</remarks>
        /// <param name="visible">The new value.</param>
        /// <returns>The popup.</returns>
        public T SetArrowVisible(bool visible)
        {
            arrowVisible = visible;
            view.EnableInClassList(Styles.noArrowUssClassName, !arrowVisible);
            RefreshPosition();
            return (T)this;
        }

        /// <summary>
        /// Set a new value for the <see cref="anchor"/> property.
        /// </summary>
        /// <remarks>This will trigger a refresh of the current popup position automatically.</remarks>
        /// <param name="value">The new value.</param>
        /// <returns>The popup.</returns>
        public T SetAnchor(VisualElement value)
        {
            m_Anchor?.UnregisterCallback(m_OnAnchorGeometryChangedAction);
            m_Anchor = value;
            m_Anchor?.RegisterCallback(m_OnAnchorGeometryChangedAction);
            RefreshPosition();
            return (T)this;
        }

        /// <summary>
        /// Activate the possibility to dismiss the popup by clicking outside of it.
        /// </summary>
        /// <param name="dismissEnabled"> `True` to activate the feature, `False` otherwise.</param>
        /// <returns> The popup of type <typeparamref name="T"/>.</returns>
        public T SetOutsideClickDismiss(bool dismissEnabled)
        {
            outsideClickDismissEnabled = dismissEnabled;
            return (T)this;
        }

        /// <summary>
        /// Activate the possibility to scroll outside of the popup.
        /// </summary>
        /// <param name="scrollEnabled"> `True` to activate the feature, `False` otherwise.</param>
        /// <returns> The popup of type <typeparamref name="T"/>.</returns>
        public T SetOutsideScrollEnabled(bool scrollEnabled)
        {
            outsideScrollEnabled = scrollEnabled;
            return (T)this;
        }

        /// <summary>
        /// Set the strategy used to determine if the click is outside of the popup.
        /// </summary>
        /// <param name="strategy"> The strategy to use.</param>
        /// <returns> The popup of type <typeparamref name="T"/>.</returns>
        public T SetOutsideClickStrategy(OutsideClickStrategy strategy)
        {
            outsideClickStrategy = strategy;
            return (T)this;
        }

        /// <inheritdoc />
        protected override void OnLayoutReadyToAnimateIn()
        {
            base.OnLayoutReadyToAnimateIn();
            RefreshPosition();
            contentView?.RegisterCallback(m_OnContentGeometryChangedAction);
        }

        /// <summary>
        /// Start the animation for this popup.
        /// </summary>
        protected override void AnimateViewIn()
        {
            base.AnimateViewIn();
            view.RegisterCallback<TransitionEndEvent>(m_OnAnimatedInAction);
            view.RegisterCallback<TransitionCancelEvent>(m_OnAnimatedInAction);
        }

        /// <summary>
        /// Called when the popup has been animated in.
        /// </summary>
        /// <param name="evt"> The transition event.</param>
        protected virtual void OnAnimatedIn(ITransitionEvent evt)
        {
            view.UnregisterCallback<TransitionEndEvent>(m_OnAnimatedInAction);
            view.UnregisterCallback<TransitionCancelEvent>(m_OnAnimatedInAction);
            m_InvokeShownAction();
        }

        /// <summary>
        /// Returns `True` if the popup should be dismissed, `False` otherwise.
        /// </summary>
        /// <param name="reason"> The reason for the dismissal.</param>
        /// <returns> `True` if the popup should be dismissed, `False` otherwise.</returns>
        protected override bool ShouldDismiss(DismissType reason) => outsideClickDismissEnabled || base.ShouldDismiss(reason);

        /// <summary>
        /// Called when the popup's <see cref="Handler"/> has received a <see cref="Popup.k_PopupDismiss"/> message.
        /// </summary>
        /// <param name="reason">The reason why the popup should be dismissed.</param>
        protected override void HideView(DismissType reason)
        {
            if (containerView?.panel != null)
                global::Unity.AppUI.Core.AppUI.UnregisterPopup(containerView.panel, this);
            contentView?.UnregisterCallback<GeometryChangedEvent>(OnContentGeometryChanged);
            base.HideView(reason);
        }

        /// <inheritdoc />
        protected override void InvokeShownEventHandlers()
        {
            global::Unity.AppUI.Core.AppUI.RegisterPopup(containerView.panel, this);
            base.InvokeShownEventHandlers();
        }

        /// <summary>
        /// Called when the popup has been dismissed. This method will invoke any handlers attached to the dismissed event.
        /// </summary>
        /// <param name="reason"> The reason for the dismissal.</param>
        protected override void InvokeDismissedEventHandlers(DismissType reason)
        {
            m_Anchor?.UnregisterCallback(m_OnAnchorGeometryChangedAction);
            base.InvokeDismissedEventHandlers(reason);
        }

        /// <summary>
        /// Start the hide animation for this popup.
        /// </summary>
        /// <param name="reason"> The reason for the dismissal.</param>
        protected override void AnimateViewOut(DismissType reason)
        {
            view.visible = false;
            InvokeDismissedEventHandlers(reason);
        }

        void OnContentGeometryChanged(GeometryChangedEvent evt)
        {
            if (!Mathf.Approximately(evt.newRect.width, evt.oldRect.width) || !Mathf.Approximately(evt.newRect.height, evt.oldRect.height))
                RefreshPosition();
        }

        void OnAnchorGeometryChanged(GeometryChangedEvent evt)
        {
            if (contentView == null)
            {
                m_Anchor.UnregisterCallback(m_OnAnchorGeometryChangedAction);
                return;
            }

            if (m_AnchorBounds != m_Anchor.worldBound)
            {
                m_AnchorBounds = m_Anchor.worldBound;
                RefreshPosition();
            }
            else if (contentView.worldBound != m_ContentBounds)
            {
                m_ContentBounds = contentView.worldBound;
                RefreshPosition();
            }
        }

        /// <summary>
        /// Recompute the position of the popup based on the anchor's position and size, but also others properties such
        /// as the <see cref="offset"/>, <see cref="crossOffset"/>, <see cref="shouldFlip"/> and <see cref="placement"/>.
        /// </summary>
        protected void RefreshPosition()
        {
            if (m_Anchor == null || !view.visible)
                return;

            var movableElement = GetMovableElement();
            var result = AnchorPopupUtils.ComputePosition(movableElement, m_Anchor, containerView, new PositionOptions(placement, offset, crossOffset, shouldFlip));
            movableElement.style.left = result.left;
            movableElement.style.top = result.top;
            movableElement.style.marginLeft = result.marginLeft;
            movableElement.style.marginTop = result.marginTop;
            if (view is IPlaceableElement placeableElement)
            {
                placeableElement.placement = result.finalPlacement;
                if (placeableElement.tipElement is { } tip)
                {
                    tip.style.bottom = result.tipBottom < 0 ? StyleKeyword.Auto : result.tipBottom;
                    tip.style.top = result.tipTop < 0 ? StyleKeyword.Auto : result.tipTop;
                    tip.style.left = result.tipLeft < 0 ? StyleKeyword.Auto : result.tipLeft;
                    tip.style.right = result.tipRight < 0 ? StyleKeyword.Auto : result.tipRight;
                }
            }
            m_CurrentPlacement = result.finalPlacement;
        }

        /// <summary>
        /// Method which must return the visual element that needs to be moved, based on the anchor position and size.
        /// </summary>
        /// <returns>The visual element which will be moved. The default value is <see cref="Popup.view"/>.</returns>
        public virtual VisualElement GetMovableElement()
        {
            return view;
        }
    }
}
