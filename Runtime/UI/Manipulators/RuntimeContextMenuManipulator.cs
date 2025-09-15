using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.AppUI.UI
{
    /// <summary>
    /// A manipulator that handles context menu interactions for UI elements.
    /// </summary>
    /// <remarks>
    /// It uses the legacy <see cref="ContextualMenuManipulator"/> for context menu handling in the Editor,
    /// and a custom implementation for runtime.
    /// </remarks>
    public class RuntimeContextMenuManipulator : PointerManipulator
    {
        bool m_Initialized;

        readonly Action<RuntimeContextMenuEvent> m_RuntimeBuilder;

        /// <summary>
        /// Creates a new instance of the <see cref="RuntimeContextMenuManipulator"/> class.
        /// </summary>
        /// <param name="runtimeBuilder"> The action that builds the context menu at runtime.</param>
        public RuntimeContextMenuManipulator(Action<RuntimeContextMenuEvent> runtimeBuilder)
        {
            m_RuntimeBuilder = runtimeBuilder;
        }

        /// <inheritdoc />
        protected override void RegisterCallbacksOnTarget()
        {
            target.RegisterCallback<AttachToPanelEvent>(OnAttachedToPanel);
            target.RegisterCallback<DetachFromPanelEvent>(OnDetachedFromPanel);
            if (target.panel != null)
                OnAttachedToPanel(null);
        }

        void OnAttachedToPanel(AttachToPanelEvent evt)
        {
            var isEditor = target.panel.contextType == ContextType.Editor;
            if (!isEditor)
            {
                target.RegisterCallback<PointerUpEvent>(OnPointerUp);
                target.RegisterCallback<KeyUpEvent>(OnKeyUp);
                m_Initialized = true;
            }
        }

        void OnDetachedFromPanel(DetachFromPanelEvent evt)
        {
            if (m_Initialized)
            {
                target.UnregisterCallback<PointerUpEvent>(OnPointerUp);
                target.UnregisterCallback<KeyUpEvent>(OnKeyUp);
            }
            m_Initialized = false;
        }

        void OnPointerUp(PointerUpEvent evt)
        {
            if (evt.button != (int)MouseButton.RightMouse)
                return;

            evt.StopPropagation();
            var e = RuntimeContextMenuEvent.GetPooled();
            e.target = target;
            e.menuBuilder = MenuBuilder.Build(target, new Menu());
            e.localPosition = evt.localPosition;
            m_RuntimeBuilder.Invoke(e);
            target.SendEvent(e);
            e.menuBuilder.Show();
        }

        void OnKeyUp(KeyUpEvent evt)
        {
            if (evt.keyCode != KeyCode.Menu)
                return;

            evt.StopPropagation();
            var e = RuntimeContextMenuEvent.GetPooled();
            e.target = target;
            e.menuBuilder = MenuBuilder.Build(target, new Menu());
            e.localPosition = new Vector2(16, 16); // Default position, can be adjusted
            m_RuntimeBuilder.Invoke(e);
            target.SendEvent(e);
            e.menuBuilder.Show();
        }

        /// <inheritdoc />
        protected override void UnregisterCallbacksFromTarget()
        {
            target.UnregisterCallback<AttachToPanelEvent>(OnAttachedToPanel);
            target.UnregisterCallback<DetachFromPanelEvent>(OnDetachedFromPanel);
            OnDetachedFromPanel(null);
        }
    }
}
