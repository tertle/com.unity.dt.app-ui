using UnityEngine.UIElements;

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
            tricklesDown = true;
            bubbles = true;
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public SplitViewPaneCollapsedEvent() => LocalInit();
    }
}
