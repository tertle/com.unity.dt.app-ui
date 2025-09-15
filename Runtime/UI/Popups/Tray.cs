using System;
using Unity.AppUI.Core;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UIElements.Experimental;

namespace Unity.AppUI.UI
{
    /// <summary>
    /// The position of the Tray.
    /// </summary>
    public enum TrayPosition
    {
        /// <summary>
        /// The Tray is displayed on the left side of the screen.
        /// </summary>
        Left,
        /// <summary>
        /// The Tray is displayed on the right side of the screen.
        /// </summary>
        Right,
        /// <summary>
        /// The Tray is displayed at the bottom of the screen.
        /// </summary>
        Bottom
    }

    /// <summary>
    /// The Tray Popup class.
    /// </summary>
    public sealed class Tray : Popup<Tray>
    {
        const int k_TraySlideInDurationMs = 125;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="referenceView"> The element used as context provider for the Tray.</param>
        /// <param name="view">The Tray visual element itself.</param>
        Tray(VisualElement referenceView, TrayVisualElement view)
            : base(referenceView, view)
        {
            keyboardDismissEnabled = true;
        }

        TrayVisualElement tray => (TrayVisualElement)view;

        void OnTrayClicked(ClickEvent evt)
        {
            var insideTray = tray.trayElement.ContainsPoint(tray.trayElement.WorldToLocal(evt.position));
            if (!insideTray)
                Dismiss(DismissType.OutOfBounds);
        }

        /// <inheritdoc cref="Popup.GetFocusableElement"/>
        protected override VisualElement GetFocusableElement()
        {
            return tray.trayElement;
        }

        /// <summary>
        /// Dismiss the <see cref="Popup"/>.
        /// </summary>
        /// <param name="reason">Why the element has been dismissed.</param>
        /// <returns>True if the element has been dismissed.</returns>
        protected override bool ShouldDismiss(DismissType reason)
        {
            return true;
        }

        /// <inheritdoc cref="Popup.ShouldAnimate"/>
        protected override bool ShouldAnimate()
        {
            return true;
        }

        /// <inheritdoc />
        protected override void OnLayoutReadyToAnimateIn()
        {
            base.OnLayoutReadyToAnimateIn();
            var fromValue = tray.position switch
            {
                TrayPosition.Left => -view.parent.resolvedStyle.width,
                TrayPosition.Right => -view.parent.resolvedStyle.width,
                TrayPosition.Bottom => -view.parent.resolvedStyle.height,
                _ => throw new ArgumentOutOfRangeException(nameof(tray.position), tray.position, "Unknown Tray position")
            };
            switch (tray.position)
            {
                case TrayPosition.Left:
                    tray.trayElement.style.left = fromValue;
                    break;
                case TrayPosition.Right:
                    tray.trayElement.style.right = fromValue;
                    break;
                case TrayPosition.Bottom:
                    tray.trayElement.style.bottom = fromValue;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(tray.position), tray.position, "Unknown Tray position");
            }
        }

        /// <inheritdoc cref="Popup.AnimateViewIn"/>
        protected override void AnimateViewIn()
        {
            var fromValue = tray.position switch
            {
                TrayPosition.Left => -tray.trayElement.layout.width,
                TrayPosition.Right => -tray.trayElement.layout.width,
                TrayPosition.Bottom => -tray.trayElement.layout.height,
                _ => throw new ArgumentOutOfRangeException(nameof(tray.position), tray.position, "Unknown Tray position")
            };
            const float toValue = 0f;
            Action<VisualElement, float> interpolation = tray.position switch
            {
                TrayPosition.Left => (element, f) => element.style.left = f,
                TrayPosition.Right => (element, f) => element.style.right = f,
                TrayPosition.Bottom => (element, f) => element.style.bottom = f,
                _ => throw new ArgumentOutOfRangeException(nameof(tray.position), tray.position, "Unknown Tray position")
            };
            tray.trayElement.experimental.animation
                .Start(fromValue, toValue, k_TraySlideInDurationMs, interpolation)
                .Ease(Easing.OutQuad)
                .OnCompleted(m_InvokeShownAction).Start();
            tray.draggedOff += OnTrayDraggedOff;
        }

        void OnTrayDraggedOff()
        {
            Dismiss(DismissType.Manual);
        }

        /// <inheritdoc cref="Popup.AnimateViewOut"/>
        protected override void AnimateViewOut(DismissType reason)
        {
            var fromValue = tray.position switch
            {
                TrayPosition.Left => tray.trayElement.layout.xMin,
                TrayPosition.Right => tray.layout.width - tray.trayElement.layout.xMax,
                TrayPosition.Bottom => tray.layout.height - tray.trayElement.layout.yMax,
                _ => throw new ArgumentOutOfRangeException(nameof(tray.position), tray.position, "Unknown Tray position")
            };
            var toValue = tray.position switch
            {
                TrayPosition.Left => -tray.trayElement.layout.width,
                TrayPosition.Right => -tray.trayElement.layout.width,
                TrayPosition.Bottom => -tray.trayElement.layout.height,
                _ => throw new ArgumentOutOfRangeException(nameof(tray.position), tray.position, "Unknown Tray position")
            };
            Action<VisualElement, float> interpolation = tray.position switch
            {
                TrayPosition.Left => (element, f) => element.style.left = f,
                TrayPosition.Right => (element, f) => element.style.right = f,
                TrayPosition.Bottom => (element, f) => element.style.bottom = f,
                _ => throw new ArgumentOutOfRangeException(nameof(tray.position), tray.position, "Unknown Tray position")
            };
            var duration = Mathf.FloorToInt(Mathf.Abs(fromValue - toValue) / -toValue * k_TraySlideInDurationMs);
            tray.trayElement.experimental.animation
                .Start(fromValue, toValue, duration, interpolation)
                .OnCompleted(() =>
            {
                view.visible = false;
                InvokeDismissedEventHandlers(reason);
            }).Ease(Easing.OutQuad).Start();
        }

        /// <inheritdoc />
        protected override void InvokeShownEventHandlers()
        {
            base.InvokeShownEventHandlers();
            view.RegisterCallback<ClickEvent>(OnTrayClicked);
        }

        /// <inheritdoc />
        protected override void HideView(DismissType reason)
        {
            tray.trayElement.UnregisterCallback<ClickEvent>(OnTrayClicked);
            base.HideView(reason);
        }

        /// <summary>
        /// Build a new <see cref="Tray"/> component.
        /// </summary>
        /// <param name="referenceView">An arbitrary UI element inside the UI panel.</param>
        /// <param name="content">The content to display inside this <see cref="Tray"/>.</param>
        /// <returns>The <see cref="Tray"/> instance.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="referenceView"/> is null.</exception>
        public static Tray Build(VisualElement referenceView, VisualElement content)
        {
            if (referenceView == null)
                throw new ArgumentNullException(nameof(referenceView));

            return new Tray(referenceView, new TrayVisualElement(content))
                .SetLastFocusedElement(referenceView);
        }

        /// <summary>
        /// Build a new <see cref="Tray"/> component.
        /// </summary>
        /// <param name="position"> The position of the tray.</param>
        /// <returns> The <see cref="Tray"/> instance.</returns>
        public Tray SetPosition(TrayPosition position)
        {
            tray.position = position;
            return this;
        }

        /// <summary>
        /// Set the handle visibility.
        /// </summary>
        /// <param name="value"> The handle visibility.</param>
        /// <returns> The <see cref="Tray"/> instance.</returns>
        public Tray SetHandleVisible(bool value)
        {
            tray.showHandle = value;
            return this;
        }

        /// <summary>
        /// Set the transition duration.
        /// </summary>
        /// <param name="durationMs"> The transition duration in milliseconds.</param>
        /// <returns> The <see cref="Tray"/> instance.</returns>
        public Tray SetTransitionDuration(int durationMs)
        {
            tray.transitionDurationMs = durationMs;
            return this;
        }

        /// <summary>
        /// The Tray UI Element.
        /// </summary>
        class TrayVisualElement : VisualElement
        {
            public const string ussClassName = "appui-tray";

            public const string leftTrayUssClassName = ussClassName + "--left";

            public const string rightTrayUssClassName = ussClassName + "--right";

            public const string bottomTrayUssClassName = ussClassName + "--bottom";

            public const string handleZoneUssClassName = ussClassName + "__handle-zone";

            public const string handleUssClassName = ussClassName + "__handle";

            public const string trayUssClassName = ussClassName + "__tray";

            public const string containerUssClassName = ussClassName + "__container";

            /// <summary>
            /// Event triggered when the user has dragged almost completely the tray out of the screen.
            /// </summary>
            public event Action draggedOff;

            readonly VisualElement m_Container;

            TrayPosition m_Position;

            readonly Draggable m_Draggable;

            readonly VisualElement m_HandleZone;

            bool m_OnHandleZone;

            public bool showHandle
            {
                get => !m_HandleZone.ClassListContains(Styles.hiddenUssClassName);
                set => m_HandleZone.EnableInClassList(Styles.hiddenUssClassName, !value);
            }

            public int transitionDurationMs { get; set; } = 150;

            public TrayVisualElement(VisualElement content)
            {
                AddToClassList(ussClassName);

                pickingMode = PickingMode.Position;
                focusable = false;

                trayElement = new VisualElement
                {
                    name = trayUssClassName,
                    focusable = true,
                    pickingMode = PickingMode.Position
                };
                trayElement.EnableDynamicTransform(true);
                trayElement.AddToClassList(trayUssClassName);
                m_HandleZone = new VisualElement { name = handleZoneUssClassName, focusable = true, pickingMode = PickingMode.Position };
                m_HandleZone.AddToClassList(handleZoneUssClassName);
                var handle = new VisualElement { name = handleUssClassName, focusable = false, pickingMode = PickingMode.Ignore };
                handle.AddToClassList(handleUssClassName);
                m_Draggable = new Draggable(OnHandleClick, OnHandleDrag, OnHandleUp, OnHandleDown);
                trayElement.AddManipulator(m_Draggable);
                m_Container = new VisualElement { name = containerUssClassName, focusable = false, pickingMode = PickingMode.Ignore };
                m_Container.AddToClassList(containerUssClassName);

                hierarchy.Add(trayElement);
                m_HandleZone.Add(handle);
                trayElement.hierarchy.Add(m_HandleZone);
                trayElement.hierarchy.Add(m_Container);

                m_Container.hierarchy.Add(content);

                position = TrayPosition.Bottom;
                showHandle = true;
            }

            void OnHandleClick()
            {
                // nothing
            }

            void OnHandleDrag(Draggable draggable)
            {
                if (!m_OnHandleZone)
                    return;

                var minPos = m_Position switch
                {
                    TrayPosition.Left => -trayElement.layout.width,
                    TrayPosition.Right => -trayElement.layout.width,
                    TrayPosition.Bottom => -trayElement.layout.height,
                    _ => throw new ArgumentOutOfRangeException()
                };

                const float maxPos = 0f;

                var currentPos = m_Position switch
                {
                    TrayPosition.Left => trayElement.layout.xMin,
                    TrayPosition.Right => layout.width - trayElement.layout.xMax,
                    TrayPosition.Bottom => layout.height - trayElement.layout.yMax,
                    _ => throw new ArgumentOutOfRangeException()
                };

                var newPos = m_Position switch
                {
                    TrayPosition.Left => Mathf.Clamp(currentPos + draggable.deltaPos.x, minPos, maxPos),
                    TrayPosition.Right => Mathf.Clamp(currentPos - draggable.deltaPos.x, minPos, maxPos),
                    TrayPosition.Bottom => Mathf.Clamp(currentPos - draggable.deltaPos.y, minPos, maxPos),
                    _ => throw new ArgumentOutOfRangeException()
                };

                switch (position)
                {
                    case TrayPosition.Left:
                        trayElement.style.left = newPos;
                        break;
                    case TrayPosition.Right:
                        trayElement.style.right = newPos;
                        break;
                    case TrayPosition.Bottom:
                        trayElement.style.bottom = newPos;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(position), position, "Unknown Tray position");
                }
            }

            void OnHandleUp(Draggable _)
            {
                if (!m_OnHandleZone)
                    return;

                var fromValue = m_Position switch
                {
                    TrayPosition.Left => trayElement.layout.xMin,
                    TrayPosition.Right => layout.width - trayElement.layout.xMax,
                    TrayPosition.Bottom => layout.height - trayElement.layout.yMax,
                    _ => throw new ArgumentOutOfRangeException()
                };

                var shouldCollapse = m_Position switch
                {
                    TrayPosition.Left => fromValue < -trayElement.layout.width * .25f,
                    TrayPosition.Right => fromValue < -trayElement.layout.width * .25f,
                    TrayPosition.Bottom => fromValue < -trayElement.layout.height * .25f,
                    _ => throw new ArgumentOutOfRangeException()
                };

                var toValue = m_Position switch
                {
                    TrayPosition.Left when shouldCollapse => -trayElement.layout.width,
                    TrayPosition.Right when shouldCollapse => -trayElement.layout.width,
                    TrayPosition.Bottom when shouldCollapse => -trayElement.layout.height,
                    TrayPosition.Left => 0,
                    TrayPosition.Right => 0,
                    TrayPosition.Bottom => 0,
                    _ => throw new ArgumentOutOfRangeException()
                };

                Action<VisualElement, float> interpolation = m_Position switch
                {
                    TrayPosition.Left => (element, f) => element.style.left = f,
                    TrayPosition.Right => (element, f) => element.style.right = f,
                    TrayPosition.Bottom => (element, f) => element.style.bottom = f,
                    _ => throw new ArgumentOutOfRangeException()
                };

                trayElement.experimental.animation.Start(fromValue, toValue, transitionDurationMs, interpolation)
                    .Ease(Easing.OutQuad)
                    .OnCompleted(() =>
                    {
                        if (shouldCollapse)
                            InvokeDraggedOff();
                    })
                    .Start();

                m_OnHandleZone = false;
            }

            void InvokeDraggedOff()
            {
                draggedOff?.Invoke();
            }

            void OnHandleDown(Draggable _)
            {
                if (!showHandle)
                {
                    m_OnHandleZone = false;
                    return;
                }

                m_OnHandleZone = true;
            }

            public VisualElement trayElement { get; }

            public TrayPosition position
            {
                get => m_Position;
                set
                {
                    m_Position = value;
                    EnableInClassList(leftTrayUssClassName, m_Position == TrayPosition.Left);
                    EnableInClassList(rightTrayUssClassName, m_Position == TrayPosition.Right);
                    EnableInClassList(bottomTrayUssClassName, m_Position == TrayPosition.Bottom);

                    trayElement.style.top = new StyleLength(StyleKeyword.Null);
                    switch (position)
                    {
                        case TrayPosition.Left:
                            trayElement.style.right = new StyleLength(StyleKeyword.Null);
                            trayElement.style.bottom = new StyleLength(StyleKeyword.Null);
                            break;
                        case TrayPosition.Right:
                            trayElement.style.left = new StyleLength(StyleKeyword.Null);
                            trayElement.style.bottom = new StyleLength(StyleKeyword.Null);
                            break;
                        case TrayPosition.Bottom:
                            trayElement.style.left = new StyleLength(StyleKeyword.Null);
                            trayElement.style.right = new StyleLength(StyleKeyword.Null);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(position), position, "Unknown Tray position");
                    }

                    m_Draggable.dragDirection = m_Position switch
                    {
                        TrayPosition.Left => Draggable.DragDirection.Horizontal,
                        TrayPosition.Right => Draggable.DragDirection.Horizontal,
                        TrayPosition.Bottom => Draggable.DragDirection.Vertical,
                        _ => throw new ArgumentOutOfRangeException()
                    };
                }
            }

            public override VisualElement contentContainer => m_Container;
        }
    }
}
