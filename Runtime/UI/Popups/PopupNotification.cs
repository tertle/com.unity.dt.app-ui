using System;
using Unity.AppUI.Core;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.AppUI.UI
{
    /// <summary>
    /// The animation used by Notification to appear and disappear.
    /// </summary>
    [GenerateLowerCaseStrings]
    public enum AnimationMode
    {
        /// <summary>
        /// The notification slides in and out of the screen.
        /// </summary>
        Slide,

        /// <summary>
        /// The notification fades in and out on the screen.
        /// </summary>
        Fade
    }

    /// <summary>
    /// The placement where the notification should be displayed.
    /// </summary>
    [GenerateLowerCaseStrings]
    public enum PopupNotificationPlacement
    {
        /// <summary>
        /// The notification is placed at the top of the screen.
        /// </summary>
        Top,

        /// <summary>
        /// The notification is placed at the bottom of the screen.
        /// </summary>
        Bottom,

        /// <summary>
        /// The notification is placed at the top left of the screen.
        /// </summary>
        TopLeft,

        /// <summary>
        /// The notification is placed at the top right of the screen.
        /// </summary>
        TopRight,

        /// <summary>
        /// The notification is placed at the bottom left of the screen.
        /// </summary>
        BottomLeft,

        /// <summary>
        /// The notification is placed at the bottom right of the screen.
        /// </summary>
        BottomRight,

        /// <summary>
        /// The notification is placed at the top start of the screen.
        /// </summary>
        TopStart,

        /// <summary>
        /// The notification is placed at the top end of the screen.
        /// </summary>
        TopEnd,

        /// <summary>
        /// The notification is placed at the bottom start of the screen.
        /// </summary>
        BottomStart,

        /// <summary>
        /// The notification is placed at the bottom end of the screen.
        /// </summary>
        BottomEnd
    }

    /// <summary>
    /// A base class for notification displayed at a specific anchor of the screen.
    /// </summary>
    /// <typeparam name="T">The sealed Notification popup class type.</typeparam>
    public abstract class PopupNotification<T> : Popup<T> where T : PopupNotification<T>
    {
        const string k_USSClassName = "appui-popup-notification";

        const string k_VariantClassName = k_USSClassName + "--";

        readonly ManagerCallback m_ManagerCallback;

        AnimationMode m_AnimationMode = AnimationMode.Fade;

        NotificationDuration m_Duration = NotificationDuration.Short;

        PopupNotificationPlacement m_Placement = PopupNotificationPlacement.Bottom;

        DismissType m_AnimateViewOutReason;

        readonly EventCallback<ITransitionEvent> m_OnInAnimationEnded;

        readonly EventCallback<ITransitionEvent> m_OnOutAnimationEnded;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="referenceView"> The view used as context provider for the popup notification.</param>
        /// <param name="view">The popup visual element itself.</param>
        protected PopupNotification(VisualElement referenceView, VisualElement view)
            : base(referenceView, view)
        {
            m_ManagerCallback = new ManagerCallback(this);
            keyboardDismissEnabled = false;
            view.EnableDynamicTransform(true);
            view.AddToClassList(k_USSClassName);
            view.AddToClassList(MemoryUtils.Concatenate(k_VariantClassName, m_Placement.ToLowerCase()));
            view.AddToClassList(MemoryUtils.Concatenate(k_VariantClassName, m_AnimationMode.ToLowerCase()));
            m_OnInAnimationEnded = OnInAnimationEnded;
            m_OnOutAnimationEnded = OnOutAnimationEnded;
        }

        /// <summary>
        /// Returns the animation used by the bar when it will be displayed.
        /// </summary>
        public AnimationMode animationMode => m_AnimationMode;

        /// <summary>
        /// Returns True if the bar is currently displayed on the screen, False otherwise.
        /// </summary>
        public bool isShown => global::Unity.AppUI.Core.AppUI.notificationManager.IsCurrent(m_ManagerCallback);

        /// <summary>
        /// Returns True if the bar is currently displayed or queued for display on the screen, False otherwise.
        /// </summary>
        public bool isShownOrQueued => global::Unity.AppUI.Core.AppUI.notificationManager.IsCurrentOrNext(m_ManagerCallback);

        /// <summary>
        /// Returns the specified display duration of the bar.
        /// </summary>
        public NotificationDuration duration => m_Duration;

        /// <summary>
        /// Returns the placement of the notification.
        /// </summary>
        public PopupNotificationPlacement position => m_Placement;

        /// <summary>
        /// Set a new value for the <see cref="animationMode"/> property.
        /// </summary>
        /// <param name="animation">THe new value</param>
        /// <returns>The current object instance to continuously build the element.</returns>
        public T SetAnimationMode(AnimationMode animation)
        {
            view.RemoveFromClassList(MemoryUtils.Concatenate(k_VariantClassName, m_AnimationMode.ToLowerCase()));
            m_AnimationMode = animation;
            view.AddToClassList(MemoryUtils.Concatenate(k_VariantClassName, m_AnimationMode.ToLowerCase()));
            return (T)this;
        }

        /// <summary>
        /// Set the duration the notification should be displayed.
        /// </summary>
        /// <param name="durationValue">A duration enum value.</param>
        /// <returns>The current object instance to continuously build the element.</returns>
        public virtual T SetDuration(NotificationDuration durationValue)
        {
            if (!isShownOrQueued)
                m_Duration = durationValue;
            else
                Debug.LogWarning("Unable to set a duration while the Bar is already shown or queued.");
            return (T)this;
        }

        /// <summary>
        /// Set the position of the notification.
        /// </summary>
        /// <param name="positionValue"> The position of the notification.</param>
        /// <returns> The current object instance to continuously build the element.</returns>
        public virtual T SetPosition(PopupNotificationPlacement positionValue)
        {
            view.RemoveFromClassList(MemoryUtils.Concatenate(k_VariantClassName, m_Placement.ToLowerCase()));
            m_Placement = positionValue;
            view.AddToClassList(MemoryUtils.Concatenate(k_VariantClassName, m_Placement.ToLowerCase()));
            return (T)this;
        }

        /// <summary>
        /// Implement this method to know if the popup should call
        /// <see cref="AnimateViewIn"/> and <see cref="AnimateViewOut"/> methods or not.
        /// </summary>
        /// <returns>`True` if you want to animate the popup, `False` otherwise.</returns>
        protected override bool ShouldAnimate()
        {
            return true;
        }

        /// <inheritdoc cref="Popup.AnimateViewIn"/>
        protected override void AnimateViewIn()
        {
            base.AnimateViewIn();
            view.RegisterCallback<TransitionEndEvent>(m_OnInAnimationEnded);
            view.RegisterCallback<TransitionCancelEvent>(m_OnInAnimationEnded);
        }

        /// <inheritdoc cref="Popup.AnimateViewOut"/>
        protected override void AnimateViewOut(DismissType reason)
        {
            m_AnimateViewOutReason = reason;
            view.RemoveFromClassList(Styles.openUssClassName);
            view.RegisterCallback<TransitionEndEvent>(m_OnOutAnimationEnded);
            view.RegisterCallback<TransitionCancelEvent>(m_OnOutAnimationEnded);
        }

        /// <inheritdoc cref="Popup{T}.InvokeShownEventHandlers"/>
        protected override void InvokeShownEventHandlers()
        {
            global::Unity.AppUI.Core.AppUI.notificationManager.OnShown(m_ManagerCallback);
            base.InvokeShownEventHandlers(); // invoke callbacks if any
        }

        /// <inheritdoc cref="Popup{T}.InvokeDismissedEventHandlers"/>
        protected override void InvokeDismissedEventHandlers(DismissType reason)
        {
            view.RemoveFromClassList(Styles.openUssClassName);
            global::Unity.AppUI.Core.AppUI.notificationManager.OnDismissed(m_ManagerCallback);
            base.InvokeDismissedEventHandlers(reason); // invoke callbacks if any
        }

        /// <inheritdoc cref="Popup.Dismiss(DismissType)"/>
        public override void Dismiss(DismissType reason)
        {
            global::Unity.AppUI.Core.AppUI.notificationManager.Dismiss(m_ManagerCallback, reason);
        }

        /// <inheritdoc cref="Popup.Show"/>
        public override void Show()
        {
            global::Unity.AppUI.Core.AppUI.notificationManager.Show(duration, m_ManagerCallback);
        }

        /// <inheritdoc cref="Popup.FindSuitableParent"/>
        protected override VisualElement FindSuitableParent(VisualElement element)
        {
            return Panel.FindNotificationLayer(element);
        }

        void OnInAnimationEnded(ITransitionEvent evt)
        {
            view.UnregisterCallback<TransitionEndEvent>(m_OnInAnimationEnded);
            view.UnregisterCallback<TransitionCancelEvent>(OnInAnimationEnded);
            m_InvokeShownAction();
        }

        void OnOutAnimationEnded(ITransitionEvent evt)
        {
            view.UnregisterCallback<TransitionEndEvent>(m_OnOutAnimationEnded);
            view.UnregisterCallback<TransitionCancelEvent>(OnOutAnimationEnded);
            InvokeDismissedEventHandlers(m_AnimateViewOutReason);
        }

        /// <summary>
        /// Implementation of the Notification Manager callback interface for <see cref="PopupNotification{T}"/> objects.
        /// </summary>
        class ManagerCallback : NotificationManager.ICallback
        {
            public ManagerCallback(PopupNotification<T> element)
            {
                obj = element;
            }

            public void Show()
            {
                if (obj is PopupNotification<T> popup)
                    popup.ShowView();
            }

            public void Dismiss(DismissType reason)
            {
                if (obj is PopupNotification<T> popup)
                    popup.HideView(reason);
            }

            public object obj { get; }
        }
    }
}
