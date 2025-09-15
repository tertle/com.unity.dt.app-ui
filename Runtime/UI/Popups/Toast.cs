using System;
using System.Collections.Generic;
using Unity.AppUI.Core;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.AppUI.UI
{
    /// <summary>
    /// The style used to display the Notification element.
    /// </summary>
    public enum NotificationStyle
    {
        /// <summary>
        /// Default style. This is for general purpose notification.
        /// </summary>
        Default,

        /// <summary>
        /// Informative style. This style is used to display an informative message.
        /// </summary>
        Informative,

        /// <summary>
        /// Positive style. This style is used to display a success message.
        /// </summary>
        Positive,

        /// <summary>
        /// Negative style. This style is used to display an error message.
        /// </summary>
        Negative,

        /// <summary>
        /// Warning style. This style is used to display a warning message.
        /// </summary>
        Warning
    }

    /// <summary>
    /// The definition of a Toast Action.
    /// </summary>
    struct ToastActionItem
    {
        /// <summary>
        /// The action ID.
        /// </summary>
        public int key { get; }

        /// <summary>
        /// The display text for this action.
        /// </summary>
        public string text { get; }

        /// <summary>
        /// The callback which will be called when the UI Component bound to this action will be interacted with.
        /// </summary>
        public Action<Toast> callback { get; }

        /// <summary>
        /// Whether the toast should be dismissed automatically after the action is triggered.
        /// </summary>
        public bool autoDismiss { get; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="key"> The action ID. </param>
        /// <param name="text"> The display text for this action. </param>
        /// <param name="callback"> The callback which will be called when the UI Component bound to this action will be interacted with. </param>
        /// <param name="autoDismiss"> Whether the toast should be dismissed automatically after the action is triggered. </param>
        public ToastActionItem(int key, string text, Action<Toast> callback, bool autoDismiss = true)
        {
            this.key = key;
            this.text = text;
            this.callback = callback;
            this.autoDismiss = autoDismiss;
        }
    }

    /// <summary>
    /// A toast is a view containing a quick little message for the user.
    /// </summary>
    public sealed class Toast : PopupNotification<Toast>
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="referenceView">The view used as context provider for the Toast.</param>
        /// <param name="contentView">The content inside the popup.</param>
        Toast(VisualElement referenceView, ToastVisualElement contentView)
            : base(referenceView, contentView)
        { }

        ToastVisualElement toast => (ToastVisualElement)view;

        /// <summary>
        /// The icon used inside the Toast as leading UI element.
        /// </summary>
        public string icon => toast.icon;

        /// <summary>
        /// Returns the styling used by the bar. See <see cref="NotificationStyle"/> for more information.
        /// </summary>
        public NotificationStyle style => toast.notificationStyle;

        /// <summary>
        /// Returns the raw message or Localization dictionary key used by the bar.
        /// </summary>
        public string text => toast.text;

        /// <summary>
        /// Add an Action to display in the Toast bar.
        /// </summary>
        /// <param name="actionId">The Action ID, which is a unique identifier for your action.</param>
        /// <param name="message"> The raw message or Localization dictionary key for the action to be displayed.</param>
        /// <param name="callback"> The callback which will be called when the action is triggered.</param>
        /// <param name="autoDismiss"> Whether the toast should be dismissed automatically after the action is triggered.</param>
        /// <returns>The <see cref="Toast"/> instance, if no exception has occured.</returns>
        public Toast AddAction(int actionId, string message, Action<Toast> callback, bool autoDismiss = true)
        {
            toast.AddAction(actionId, new ToastActionItem(actionId, message, callback, autoDismiss));
            return this;
        }

        /// <summary>
        /// Remove an already existing action.
        /// </summary>
        /// <param name="actionId">The Action ID.</param>
        /// <returns>The <see cref="Toast"/> instance, if no exception has occured.</returns>
        public Toast RemoveAction(int actionId)
        {
            toast.RemoveAction(actionId);
            return this;
        }

        /// <summary>
        /// <para>Build and return a <see cref="Toast"/> UI element.</para>
        /// <para>
        /// The method will find the best suitable parent view which will contain the Toast element.
        /// </para>
        /// </summary>
        /// <remarks>The snackbar is not displayed directly, you have to call <see cref="PopupNotification{T}.Show"/>.</remarks>
        /// <param name="referenceView">An arbitrary <see cref="VisualElement"/> which is currently present in the UI panel.</param>
        /// <param name="text">The raw message or Localization dictionary key for the message to be displayed inside
        /// the <see cref="Toast"/>.</param>
        /// <param name="duration">A duration enum value.</param>
        /// <returns>The <see cref="Toast"/> instance, if no exception has occured.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="referenceView"/> is null.</exception>
        public static Toast Build(VisualElement referenceView, string text, NotificationDuration duration)
        {
            if (referenceView == null)
                throw new ArgumentNullException(nameof(referenceView));

            var bar = new Toast(referenceView, new ToastVisualElement()).SetText(text).SetDuration(duration);
            return bar;
        }

        /// <summary>
        /// Set a new value for the <see cref="icon"/> property.
        /// </summary>
        /// <param name="iconName">The name of the icon.</param>
        /// <returns>The <see cref="Toast"/> to continuously build the element.</returns>
        public Toast SetIcon(string iconName)
        {
            toast.icon = iconName;
            return this;
        }

        /// <summary>
        /// Set the styling used by the bar.
        /// </summary>
        /// <param name="notificationStyle">A notification style enum value. See <see cref="NotificationStyle"/> for more information.</param>
        /// <returns>The <see cref="Toast"/> to continuously build the element.</returns>
        public Toast SetStyle(NotificationStyle notificationStyle)
        {
            toast.notificationStyle = notificationStyle;
            return this;
        }

        /// <summary>
        /// Update the text in the <see cref="Toast"/>.
        /// </summary>
        /// <param name="txt"> The raw message or Localization dictionary key for the message to be displayed.</param>
        /// <returns>The <see cref="Toast"/> to continuously build the element.</returns>
        public Toast SetText(string txt)
        {
            toast.text = txt;
            return this;
        }

        /// <inheritdoc />
        protected override void InvokeShownEventHandlers()
        {
            base.InvokeShownEventHandlers();
            toast.actionTriggered += OnActionTriggered;
        }

        /// <inheritdoc />
        protected override void HideView(DismissType reason)
        {
            toast.actionTriggered -= OnActionTriggered;
            base.HideView(reason);
        }

        void OnActionTriggered(ToastActionItem actionItem)
        {
            actionItem.callback?.Invoke(this);
            if (actionItem.autoDismiss)
                Dismiss();
        }
    }

    /// <summary>
    /// The Toast UI Element.
    /// </summary>
    sealed partial class ToastVisualElement : VisualElement
    {
        public event Action<ToastActionItem> actionTriggered;

        public const string ussClassName = "appui-toast";

        public const string containerUssClassName = ussClassName + "-container";

        [EnumName("GetNotificationStyleUssClassName", typeof(NotificationStyle))]
        public const string variantUssClassName = ussClassName + "--";

        public const string messageUssClassName = ussClassName + "__message";

        public const string iconUssClassName = ussClassName + "__icon";

        public const string dividerUssClassName = ussClassName + "__divider";

        public const string actionContainerUssClassName = ussClassName + "__actioncontainer";

        public const string actionUssClassName = ussClassName + "__action";

        readonly ExVisualElement m_ToastElement;

        readonly VisualElement m_ActionContainer;

        readonly Dictionary<int, ToastActionItem> m_Actions = new Dictionary<int, ToastActionItem>();

        readonly Dictionary<int, Pressable> m_ActionPressableManipulators = new Dictionary<int, Pressable>();

        readonly Divider m_Divider;

        readonly Icon m_Icon;

        NotificationStyle m_Style;

        readonly LocalizedTextElement m_TextElement;

        public ToastVisualElement()
        {
            pickingMode = PickingMode.Ignore;
            this.EnableDynamicTransform(true);
            AddToClassList(containerUssClassName);

            m_ToastElement = new ExVisualElement
            {
                name = ussClassName,
                pickingMode = PickingMode.Position,
                passMask = ExVisualElement.Passes.Clear | ExVisualElement.Passes.OutsetShadows
            };
            m_ToastElement.AddToClassList(ussClassName);
            hierarchy.Add(m_ToastElement);

            m_Icon = new Icon { name = iconUssClassName };
            m_Icon.AddToClassList(iconUssClassName);
            m_ToastElement.hierarchy.Add(m_Icon);

            m_TextElement = new LocalizedTextElement { name = messageUssClassName };
            m_TextElement.AddToClassList(messageUssClassName);
            m_ToastElement.hierarchy.Add(m_TextElement);

            m_Divider = new Divider
            {
                name = dividerUssClassName,
                size = Size.M, spacing = Spacing.L,
                direction = Direction.Vertical
            };
            m_Divider.AddToClassList(dividerUssClassName);
            m_ToastElement.hierarchy.Add(m_Divider);

            m_ActionContainer = new VisualElement { name = actionContainerUssClassName };
            m_ActionContainer.AddToClassList(actionContainerUssClassName);
            m_ToastElement.hierarchy.Add(m_ActionContainer);

            notificationStyle = NotificationStyle.Default;
            text = "";
            icon = null;
            RefreshActionContainer();
        }

        public string icon
        {
            get => m_Icon.iconName;
            set
            {
                m_Icon.iconName = value;
                m_Icon.EnableInClassList(Styles.hiddenUssClassName, string.IsNullOrEmpty(m_Icon.iconName));
            }
        }

        public string text
        {
            get => m_TextElement.text;
            set => m_TextElement.text = value;
        }

        public AnimationMode animationMode { get; set; } = AnimationMode.Fade;

        public NotificationStyle notificationStyle
        {
            get => m_Style;

            set
            {
                m_ToastElement.RemoveFromClassList(GetNotificationStyleUssClassName(m_Style));
                m_Style = value;
                m_ToastElement.AddToClassList(GetNotificationStyleUssClassName(m_Style));
            }
        }

        void OnActionTriggered(EventBase evt)
        {
            if (evt.target is VisualElement {userData: ToastActionItem actionItem})
                actionTriggered?.Invoke(actionItem);
        }

        void RefreshActionContainer()
        {
            var noActions = m_Actions.Count == 0;

            foreach (var pressable in m_ActionPressableManipulators.Values)
            {
                pressable.clickedWithEventInfo -= OnActionTriggered;
            }

            m_ActionPressableManipulators.Clear();
            m_ActionContainer.Clear();

            foreach (var actionItem in m_Actions.Values)
            {
                var actionButton = new LocalizedTextElement
                {
                    name = actionUssClassName,
                    pickingMode = PickingMode.Position,
                    focusable = true,
                    userData = actionItem,
                    text = actionItem.text
                };
                actionButton.AddToClassList(actionUssClassName);
                var pressable = new Pressable();
                pressable.clickedWithEventInfo += OnActionTriggered;
                actionButton.AddManipulator(pressable);
                m_ActionPressableManipulators[actionItem.key] = pressable;
                m_ActionContainer.Add(actionButton);
            }

            m_Divider.EnableInClassList(Styles.hiddenUssClassName, noActions);
            m_ActionContainer.EnableInClassList(Styles.hiddenUssClassName, noActions);
        }

        public void AddAction(int key, ToastActionItem actionItem)
        {
            m_Actions[key] = actionItem;
            RefreshActionContainer();
        }

        public void RemoveAction(int key)
        {
            if (m_Actions.ContainsKey(key))
            {
                m_Actions.Remove(key);
                RefreshActionContainer();
            }
        }
    }
}
