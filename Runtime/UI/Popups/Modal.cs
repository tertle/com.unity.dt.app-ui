using System;
using Unity.AppUI.Bridge;
using Unity.AppUI.Core;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.AppUI.UI
{
    /// <summary>
    /// The FullScreen mode used by a <see cref="Modal"/> component.
    /// </summary>
    public enum ModalFullScreenMode
    {
        /// <summary>
        /// The <see cref="Modal"/> is displayed as a normal size.
        /// </summary>
        None,

        /// <summary>
        /// The <see cref="Modal"/> is displayed in fullscreen but a small margin still present
        /// to display the <see cref="Modal"/> smir.
        /// </summary>
        FullScreen,

        /// <summary>
        /// The <see cref="Modal"/> is displayed in fullscreen without any margin.
        /// The <see cref="Modal"/> smir won't be reachable.
        /// </summary>
        FullScreenTakeOver
    }

    /// <summary>
    /// Interface that must be implemented by any UI component which wants to
    /// request a <see cref="Popup.Dismiss(DismissType)"/> if this component is displayed
    /// inside a <see cref="Popup"/> component.
    /// </summary>
    public interface IDismissInvocator
    {
        /// <summary>
        /// Event triggered when the UI component wants to request a <see cref="Popup.Dismiss(DismissType)"/>
        /// </summary>
        event Action<DismissType> dismissRequested;
    }

    /// <summary>
    /// The Modal Popup class.
    /// </summary>
    public sealed class Modal : Popup<Modal>
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="referenceView">The view used as context provider for the Modal.</param>
        /// <param name="modalView">The popup visual element itself.</param>
        /// <param name="content">The content that will appear inside this popup.</param>
        Modal(VisualElement referenceView, ModalVisualElement modalView, VisualElement content)
            : base(referenceView, modalView, content)
        { }

        ModalVisualElement modal => (ModalVisualElement)view;

        /// <summary>
        /// <para>Set the fullscreen mode for this <see cref="Modal"/>.</para>
        /// <para>
        /// See <see cref="ModalFullScreenMode"/> values for more info.
        /// </para>
        /// </summary>
        public ModalFullScreenMode fullscreenMode
        {
            get => modal.fullScreenMode;
            set => modal.fullScreenMode = value;
        }

        /// <summary>
        /// `True` if the Modal can be dismissed by clicking outside of it, `False` otherwise.
        /// </summary>
        public bool outsideClickDismissEnabled { get; set; }

        /// <summary>
        /// The strategy used to determine if the click is outside the Modal.
        /// </summary>
        public OutsideClickStrategy outsideClickStrategy { get; set; } = OutsideClickStrategy.Bounds;

        /// <summary>
        /// Set a new value for <see cref="fullscreenMode"/> property.
        /// </summary>
        /// <param name="mode">The new value.</param>
        /// <returns>The <see cref="Modal"/> object.</returns>
        public Modal SetFullScreenMode(ModalFullScreenMode mode)
        {
            fullscreenMode = mode;
            return this;
        }

        /// <summary>
        /// Activate the possibility to dismiss the Modal by clicking outside of it.
        /// </summary>
        /// <param name="dismissEnabled"> `True` to activate the feature, `False` otherwise.</param>
        /// <returns> The modal </returns>
        public Modal SetOutsideClickDismiss(bool dismissEnabled)
        {
            outsideClickDismissEnabled = dismissEnabled;
            return this;
        }

        /// <summary>
        /// Set the strategy used to determine if the click is outside the Modal.
        /// </summary>
        /// <param name="strategy"> The strategy to use.</param>
        /// <returns> The modal </returns>
        public Modal SetOutsideClickStrategy(OutsideClickStrategy strategy)
        {
            outsideClickStrategy = strategy;
            return this;
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
                shouldDismiss = !modal.contentContainer.worldBound.Contains((Vector2)evt.position);

            if (shouldDismiss && (outsideClickStrategy & OutsideClickStrategy.Pick) != 0)
            {
                var picked = view.panel.Pick(evt.position);
                var commonAncestor = picked?.FindCommonAncestor(view);
                if (commonAncestor == view) // if the picked element is a child of the popover, don't dismiss
                    shouldDismiss = false;
            }

            if (!shouldDismiss)
                return;

            // prevent reopening the same modal again...
            evt.StopImmediatePropagation();
            Dismiss(DismissType.OutOfBounds);
        }

        /// <inheritdoc cref="Popup.ShouldDismiss"/>
        protected override bool ShouldDismiss(DismissType reason) => outsideClickDismissEnabled || base.ShouldDismiss(reason);

        /// <inheritdoc cref="Popup.ShouldAnimate"/>
        protected override bool ShouldAnimate() => true;

        /// <inheritdoc cref="Popup.InvokeShownEventHandlers"/>
        protected override void InvokeShownEventHandlers()
        {
            if (outsideClickDismissEnabled)
                global::Unity.AppUI.Core.AppUI.RegisterPopup(containerView.panel, this);
            base.InvokeShownEventHandlers();
            containerView?.panel?.visualTree?.RegisterCallback<PointerDownEvent>(OnTreeDown, TrickleDown.TrickleDown);
        }

        /// <inheritdoc cref="Popup.HideView"/>
        protected override void HideView(DismissType reason)
        {
            containerView.panel?.visualTree?.UnregisterCallback<PointerDownEvent>(OnTreeDown, TrickleDown.TrickleDown);
            global::Unity.AppUI.Core.AppUI.UnregisterPopup(containerView.panel, this);
            base.HideView(reason);
        }

        /// <summary>
        /// Build a new Modal component.
        /// </summary>
        /// <param name="referenceView">An arbitrary UI element inside the UI panel.</param>
        /// <param name="content">The <see cref="VisualElement"/> UI element to display inside this <see cref="Modal"/>.</param>
        /// <returns>The <see cref="Modal"/> instance.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="referenceView"/> is null.</exception>
        public static Modal Build(VisualElement referenceView, VisualElement content)
        {
            if (referenceView == null)
                throw new ArgumentNullException(nameof(referenceView));

            var popup = new Modal(referenceView, new ModalVisualElement(content), content)
                .SetLastFocusedElement(referenceView);
            return popup;
        }

        /// <inheritdoc cref="Popup.GetFocusableElement"/>
        protected override VisualElement GetFocusableElement()
        {
            return modal.contentContainer;
        }

        /// <summary>
        /// The Modal UI Element.
        /// </summary>
        class ModalVisualElement : VisualElement
        {
            public const string ussClassName = "appui-modal";

            public const string fullScreenUssClassName = ussClassName + "--fullscreen";

            public const string fullScreenTakeOverUssClassName = ussClassName + "--fullscreen-takeover";

            public const string contentContainerUssClassName = ussClassName + "__content";

            readonly VisualElement m_ContentContainer;

            ModalFullScreenMode m_FullScreenMode = ModalFullScreenMode.None;

            public ModalFullScreenMode fullScreenMode
            {
                get => m_FullScreenMode;
                set
                {
                    m_FullScreenMode = value;
                    EnableInClassList(fullScreenUssClassName, m_FullScreenMode == ModalFullScreenMode.FullScreen);
                    EnableInClassList(fullScreenTakeOverUssClassName, m_FullScreenMode == ModalFullScreenMode.FullScreenTakeOver);
                }
            }

            public ModalVisualElement(VisualElement content)
            {
                AddToClassList(ussClassName);

                pickingMode = PickingMode.Position;

                m_ContentContainer = new ExVisualElement { name = contentContainerUssClassName, pickingMode = PickingMode.Position, focusable = true, passMask = ExVisualElement.Passes.Clear | ExVisualElement.Passes.OutsetShadows };
                m_ContentContainer.SetIsCompositeRoot(true);
                m_ContentContainer.SetExcludeFromFocusRing(true);
                m_ContentContainer.delegatesFocus = true;

                m_ContentContainer.AddToClassList(contentContainerUssClassName);

                hierarchy.Add(m_ContentContainer);

                m_ContentContainer.Add(content);
                fullScreenMode = ModalFullScreenMode.None;
            }

            public override VisualElement contentContainer => m_ContentContainer;
        }
    }
}
