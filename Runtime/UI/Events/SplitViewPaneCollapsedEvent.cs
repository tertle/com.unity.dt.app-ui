using Unity.AppUI.Bridge;
using UnityEngine.UIElements;
using EventPropagation = Unity.AppUI.Bridge.EventBaseExtensionsBridge.EventPropagation;

namespace Unity.AppUI.UI
{
    /// <summary>
    /// Event for when the pane of a SplitView is collapsed or expanded.
    /// </summary>
    public class SplitViewPaneCollapsedEvent : EventBase<SplitViewPaneCollapsedEvent>
    {
        /// <summary>
        /// <para>Whether the pane is collapsed or expanded.</para>
        /// <para>True if the pane is collapsed, false if it is expanded.</para>
        /// </summary>
        public bool collapsed { get; internal set; }

        /// <summary>
        /// The index of the pane that was collapsed or expanded.
        /// </summary>
        public int index { get; internal set; }

        /// <summary>
        /// Resets all event members to their initial values.
        /// </summary>
        protected override void Init()
        {
            base.Init();
            LocalInit();
        }

        void LocalInit()
        {
            this.SetPropagation(EventPropagation.Bubbles | EventPropagation.TricklesDown
#if !UNITY_2023_2_OR_NEWER
                | EventPropagation.Cancellable
#endif
            );
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public SplitViewPaneCollapsedEvent() => LocalInit();
    }
}
