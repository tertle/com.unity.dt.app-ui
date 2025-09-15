using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.AppUI.UI
{
    /// <summary>
    /// Event sent in a <see cref="VisualElement"/> when a context menu is requested at runtime
    /// if it has a <see cref="RuntimeContextMenuManipulator"/> attached.
    /// </summary>
    public class RuntimeContextMenuEvent : EventBase<RuntimeContextMenuEvent>
    {
        /// <summary>
        /// The menu builder that will be used to build the context menu.
        /// </summary>
        public MenuBuilder menuBuilder { get; internal set; }

        /// <summary>
        /// The local position of the context menu in the target element's coordinate space.
        /// </summary>
        public Vector2 localPosition { get; internal set; }
    }
}
