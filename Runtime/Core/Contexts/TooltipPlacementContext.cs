using System;
using Unity.AppUI.UI;

namespace Unity.AppUI.Core
{
    /// <summary>
    /// The preferred placement of a tooltip in the current context.
    /// </summary>
    /// <param name="placement"> The preferred placement of a tooltip. </param>
    public record TooltipPlacementContext(PopoverPlacement placement) : IContext
    {
        /// <summary>
        /// The preferred placement of a tooltip.
        /// </summary>
        public PopoverPlacement placement { get; } = placement;
    }
}
