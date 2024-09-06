using System;
using Unity.AppUI.Core;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.AppUI.UI
{
    /// <summary>
    /// This is the base class for any UI component that needs to be displayed over the rest of the user interface.
    /// </summary>
    public abstract class Popup
    {
        /// <summary>
        /// The message id used to show the popup.
        /// </summary>
        protected const int k_PopupShow = 1;

        /// <summary>
        /// The message id used to dismiss the popup.
        /// </summary>
        protected const int k_PopupDismiss = 2;

        /// <summary>
        /// A pre-allocated Action that calls <see cref="InvokeShownEventHandlers"/>.
        /// </summary>
        protected readonly Action m_InvokeShownAction;

        Handler m_Handler;

        readonly Action m_PrepareAnimateViewInAction;

        readonly Action m_OnLayoutReadyToAnimateInAction;

        IVisualElementScheduledItem m_ScheduledPrepareAnimateViewIn;

        IVisualElementScheduledItem m_ScheduledAnimateViewIn;

        IVisualElementScheduledItem m_ScheduledLayoutReadyToAnimateIn;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="referenceView">The reference view used as context provider for the popup.</param>
        /// <param name="view">The popup visual element itself.</param>
        /// <param name="contentView">The content that will appear inside this popup.</param>
        /// <exception cref="ArgumentNullException">The referenceView can't be null.</exception>
        /// <exception cref="ArgumentNullException">The view can't be null.</exception>
        protected Popup(VisualElement referenceView, VisualElement view, VisualElement contentView = null)
        {
            this.contentView = contentView;
            this.referenceView = referenceView ?? throw new ArgumentNullException(nameof(referenceView));
            this.view = view ?? throw new ArgumentNullException(nameof(view));

            if (contentView is IDismissInvocator invocator)
                invocator.dismissRequested += Dismiss;

            m_InvokeShownAction = new Action(InvokeShownEventHandlers);
            m_PrepareAnimateViewInAction = new Action(PrepareAnimateViewInInternal);
            m_OnLayoutReadyToAnimateInAction = new Action(OnLayoutReadyToAnimateInInternal);
        }

        /// <summary>
        /// The handler that receives and dispatches messages. This is useful in multi-threaded applications.
        /// </summary>
        protected Handler handler
        {
            get
            {
                if (m_Handler == null)
                    m_Handler = new Handler(global::Unity.AppUI.Core.AppUI.mainLooper, message =>
                    {
                        switch (message.what)
                        {
                            case k_PopupShow:
                                ((Popup)message.obj).ShowView();
                                return true;
                            case k_PopupDismiss:
                                ((Popup)message.obj).HideView((DismissType)message.arg1);
                                return true;
                            default:
                                return false;
                        }
                    });

                return m_Handler;
            }
        }

        /// <summary>
        /// <para>`True` if the the popup can be dismissed by pressing the escape key or the return button on mobile, `False` otherwise.</para>
        /// <para>
        /// The default value is `True`.
        /// </para>
        /// </summary>
        public bool keyboardDismissEnabled { get; protected set; } = true;

        /// <summary>
        /// Returns the popup's <see cref="VisualElement"/>.
        /// </summary>
        public VisualElement view { get; }

        /// <summary>
        /// The parent of the <see cref="view"/> when the popup will be displayed.
        /// </summary>
        public VisualElement containerView { get; protected set; }

        /// <summary>
        /// The view used as context provider for the popup.
        /// </summary>
        public VisualElement referenceView { get; }

        /// <summary>
        /// The content of the popup.
        /// </summary>
        public VisualElement contentView { get; }

        /// <summary>
        /// Dismiss the <see cref="Popup"/>.
        /// </summary>
        public virtual void Dismiss()
        {
            Dismiss(DismissType.Manual);
        }

        /// <summary>
        /// Dismiss the <see cref="Popup"/>.
        /// </summary>
        /// <param name="reason">Why the element has been dismissed.</param>
        public virtual void Dismiss(DismissType reason)
        {
            if (ShouldDismiss(reason))
                handler.SendMessage(handler.ObtainMessage(k_PopupDismiss, (int)reason, this));
        }

        /// <summary>
        /// Check if the popup should be dismissed or not, depending on the reason.
        /// </summary>
        /// <param name="reason"> Why the element has been dismissed.</param>
        /// <returns> `True` if the popup should be dismissed, `False` otherwise.</returns>
        protected virtual bool ShouldDismiss(DismissType reason)
        {
            // By default, we don't allow to dismiss the popup if the user clicks outside of it.
            if (reason == DismissType.OutOfBounds)
                return false;
            return true;
        }

        /// <summary>
        /// Show the <see cref="Popup"/>.
        /// </summary>
        public virtual void Show()
        {
            handler.SendMessage(handler.ObtainMessage(k_PopupShow, this));
        }

        /// <summary>
        /// Called when the popup's <see cref="Handler"/> has received a <see cref="k_PopupShow"/> message.
        /// </summary>
        /// <remarks>
        /// In this method the view should become visible at some point (directly or via an animation).
        /// </remarks>
        /// <exception cref="InvalidOperationException">Unable to find a suitable parent for the popup.</exception>
        protected virtual void ShowView()
        {
            m_ScheduledLayoutReadyToAnimateIn?.Pause();

            if (view.panel == null) // not added into the visual tree yet
            {
                // find a suitable parent for the popup
                containerView ??= FindSuitableParent(referenceView);
                if (containerView == null)
                    throw new InvalidOperationException("Unable to find a suitable parent for the popup.");

                // set invisible in order to calculate layout before displaying the element (avoid flickering)
                view.visible = false;
                // add the view to the container
                containerView.Add(view);
            }

            view.RegisterCallback<DetachFromPanelEvent>(OnDetachedFromPanel);
            view.RegisterCallback<KeyDownEvent>(OnViewKeyDown);

            if (ShouldAnimate())
            {
                m_ScheduledLayoutReadyToAnimateIn = view.schedule.Execute(m_OnLayoutReadyToAnimateInAction);
            }
            else
            {
                // be sure its visible
                view.visible = true;
                InvokeShownEventHandlers();
            }
        }

        /// <summary>
        /// <para>Returns the element that will be focused when the view will become visible.</para>
        /// <para>
        /// The default value is `null`.
        /// </para>
        /// </summary>
        /// <returns>The element that will be focused when the view will become visible.</returns>
        protected virtual VisualElement GetFocusableElement()
        {
            return null;
        }

        /// <summary>
        /// Implement this method to know if the popup should call
        /// <see cref="AnimateViewIn"/> and <see cref="AnimateViewOut"/> methods or not.
        /// </summary>
        /// <returns>`True` if you want to animate the popup, `False` otherwise.</returns>
        protected virtual bool ShouldAnimate()
        {
            // todo we can check here if any Accessibility flags is currently in use that should prevent animations.
            return false;
        }

        /// <summary>
        /// Called when the popup has become visible.
        /// </summary>
        protected virtual void InvokeShownEventHandlers()
        {
            var focusableElement = GetFocusableElement();
            if (focusableElement != null)
                focusableElement.schedule.Execute(() =>
                {
                    // Instead of force focusing an element in the popup content,
                    // we should change the current FocusController of the panel:
                    // focusableElement.panel.focusController = new FocusController(new VisualElementFocusRing(focusableElement));
                    // but UITK doesnt provide any accessible way to do it
                    focusableElement.Focus();
                });
        }

        /// <summary>
        /// <para>Called when the popup has received a <see cref="KeyDownEvent"/>.</para>
        /// <para>
        /// By default this method handles the dismiss of the popup via the Escape key or a Return button.
        /// </para>
        /// </summary>
        /// <param name="evt">The event the popup has received.</param>
        protected virtual void OnViewKeyDown(KeyDownEvent evt)
        {
            var focusableElement = GetFocusableElement();
            if (keyboardDismissEnabled && focusableElement != null && evt.keyCode == KeyCode.Escape)
            {

                evt.StopPropagation();
                Dismiss(DismissType.Cancel);
            }
        }

        void OnLayoutReadyToAnimateInInternal()
        {
            m_ScheduledPrepareAnimateViewIn?.Pause();
            OnLayoutReadyToAnimateIn();
            // delay the animation preparation to the next frame in case OnLayoutReadyToAnimateIn overrides the layout
            m_ScheduledPrepareAnimateViewIn = view.schedule.Execute(m_PrepareAnimateViewInAction);
        }

        void PrepareAnimateViewInInternal()
        {
            m_ScheduledAnimateViewIn?.Pause();
            PrepareAnimateViewIn();
            // delay the animation to the next frame in case PrepareAnimateViewIn overrides the layout
            m_ScheduledAnimateViewIn = view.schedule.Execute(AnimateViewIn);
        }

        /// <summary>
        /// Called when the layout is ready to be animated in.
        /// </summary>
        protected virtual void OnLayoutReadyToAnimateIn()
        {
            view.visible = true;
        }

        /// <summary>
        /// Called a frame before <see cref="AnimateViewIn"/> to prepare the layout a final time.
        /// </summary>
        protected virtual void PrepareAnimateViewIn()
        {
            view.AddToClassList(Styles.animateInUssClassName);
        }

        /// <summary>
        /// Start the animation for this popup.
        /// </summary>
        protected virtual void AnimateViewIn()
        {
            view.RemoveFromClassList(Styles.animateInUssClassName);
            view.AddToClassList(Styles.openUssClassName);
        }

        /// <summary>
        /// Called when the popup's <see cref="Handler"/> has received a <see cref="k_PopupDismiss"/> message.
        /// </summary>
        /// <param name="reason">The reason why the popup should be dismissed.</param>
        protected virtual void HideView(DismissType reason)
        {
            m_ScheduledPrepareAnimateViewIn?.Pause();
            view.UnregisterCallback<KeyDownEvent>(OnViewKeyDown);

            if (ShouldAnimate())
            {
                AnimateViewOut(reason);
            }
            else
            {
                InvokeDismissedEventHandlers(reason);
            }
        }

        /// <summary>
        /// Called when the popup has completed its dismiss process.
        /// </summary>
        /// <param name="reason">The reason why the popup has been dismissed.</param>
        protected virtual void InvokeDismissedEventHandlers(DismissType reason)
        {
            view.RemoveFromClassList(Styles.animateInUssClassName);
            view.RemoveFromClassList(Styles.openUssClassName);
            view.UnregisterCallback<DetachFromPanelEvent>(OnDetachedFromPanel);
            view.visible = false;
        }

        /// <summary>
        /// Called when the popup's <see cref="VisualElement"/> has been removed from the panel.
        /// </summary>
        /// <param name="evt"> The event that has been received.</param>
        void OnDetachedFromPanel(DetachFromPanelEvent evt)
        {
            // The panel has been destroyed, we need to manually do the cleanup.
            if (view.visible)
                InvokeDismissedEventHandlers(DismissType.PanelDestroyed);
        }

        /// <summary>
        /// Start the hide animation for this popup.
        /// </summary>
        /// <param name="reason">The reason why the popup should be dismissed.</param>
        protected virtual void AnimateViewOut(DismissType reason)
        {
            InvokeDismissedEventHandlers(reason);
        }

        /// <summary>
        /// Find the parent <see cref="VisualElement"/> where the popup will be added.
        /// </summary>
        /// <param name="element">An arbitrary UI element inside the panel.</param>
        /// <returns>The popup container <see cref="VisualElement"/> in the current panel.</returns>
        /// <remarks>
        /// This is usually one of the layers from the <see cref="Panel"/> root UI element.
        /// If no <see cref="Panel"/> is found, the method will return the visual tree root of the given element.
        /// </remarks>
        protected virtual VisualElement FindSuitableParent(VisualElement element)
        {
            return Panel.FindPopupLayer(element) ?? element?.panel?.visualTree;
        }
    }

    /// <summary>
    /// A generic base class for popups.
    /// </summary>
    /// <typeparam name="T">A sealed popup class type.</typeparam>
    public abstract class Popup<T> : Popup where T : Popup<T>
    {
        /// <summary>
        /// The last focused element before the popup was shown.
        /// </summary>
        protected Focusable m_LastFocusedElement;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="referenceView">The reference view used as context provider for the popup.</param>
        /// <param name="view">The popup visual element itself.</param>
        /// <param name="contentView">The content that will appear inside this popup.</param>
        protected Popup(VisualElement referenceView, VisualElement view, VisualElement contentView = null)
            : base(referenceView, view, contentView) { }

        /// <summary>
        /// Event triggered when the popup has become visible.
        /// </summary>
        public event Action<T> shown;

        /// <summary>
        /// Event triggered when the popup has been dismissed.
        /// </summary>
        public event Action<T, DismissType> dismissed;

        /// <summary>
        /// Set the container view where the popup will be displayed.
        /// </summary>
        /// <param name="element"> The container view.</param>
        /// <returns> The popup of type <typeparamref name="T"/>.</returns>
        /// <remarks>
        /// By default, the popup will be added to popup container of the first <see cref="Panel"/>
        /// ancestor of the given reference view during construction.
        /// </remarks>
        public T SetContainerView(VisualElement element)
        {
            if (contentView == element)
                return (T)this;

            var isAlreadyInVisualTree = view.panel != null;
            containerView = element;
            if (isAlreadyInVisualTree)
            {
                Debug.LogWarning("Changing the container view of a popup that is already " +
                    "part of the visual tree can lead to unexpected behavior.");
                containerView.Add(view);
            }
            return (T)this;
        }

        /// <summary>
        /// Activate the possibility to dismiss the popup via Escape key or Return button.
        /// </summary>
        /// <param name="dismissEnabled">`True` to activate the feature, `False` otherwise.</param>
        /// <returns>The popup of type <typeparamref name="T"/>.</returns>
        public T SetKeyboardDismiss(bool dismissEnabled)
        {
            keyboardDismissEnabled = dismissEnabled;
            return (T)this;
        }

        /// <summary>
        /// Set the last focused element before the popup was shown.
        /// </summary>
        /// <param name="focusable"> The last focused element.</param>
        /// <returns> The popup of type <typeparamref name="T"/>.</returns>
        public T SetLastFocusedElement(Focusable focusable)
        {
            m_LastFocusedElement = focusable;
            return (T)this;
        }

        /// <summary>
        /// Called when the popup has become visible.
        /// This method will invoke any handlers attached to the <see cref="shown"/> event.
        /// </summary>
        protected override void InvokeShownEventHandlers()
        {
            base.InvokeShownEventHandlers();
            shown?.Invoke((T)this);
        }

        /// <summary>
        /// Called when the popup has been dismissed.
        /// This method will invoke any handlers attached to the <see cref="dismissed"/> event.
        /// </summary>
        /// <param name="reason">The reason for the dismissal.</param>
        protected override void InvokeDismissedEventHandlers(DismissType reason)
        {
            base.InvokeDismissedEventHandlers(reason);
            dismissed?.Invoke((T)this, reason);

            // we can safely remove the notification element from the visual tree now.
            view.RemoveFromHierarchy();

            // focus last focused element (if any)
            if (reason != DismissType.OutOfBounds)
                m_LastFocusedElement?.Focus();
        }
    }
}
