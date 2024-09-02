using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.AppUI.UI
{
    /// <summary>
    /// Utility class to compute the position of a popup based on the anchor's position and size.
    /// </summary>
    public static class AnchorPopupUtils
    {
        static void CrossSnapHorizontally(ref PositionResult result, Rect screenRect, Rect elementRect, bool shouldCrossSnap)
        {
            if (!shouldCrossSnap)
                return;

            if (elementRect.width < screenRect.width)
            {
                if (result.left + result.marginLeft < screenRect.xMin)
                {
                    result.left = -result.marginLeft;
                }
                else if (result.left + result.marginLeft + elementRect.width > screenRect.width)
                {
                    var tmpLeft = screenRect.width - elementRect.width - result.marginLeft;
                    if (tmpLeft >= screenRect.xMin)
                        result.left = tmpLeft;
                }
            }
        }

        static void CrossSnapVertically(ref PositionResult result, Rect screenRect, Rect elementRect, bool shouldCrossSnap)
        {
            if (!shouldCrossSnap)
                return;

            if (elementRect.height < screenRect.height)
            {
                if (result.top + result.marginTop < screenRect.yMin)
                {
                    result.top = -result.marginTop;
                }
                else if (result.top + result.marginTop + elementRect.height > screenRect.height)
                {
                    var tmpTop = screenRect.height - elementRect.height - result.marginTop;
                    if (tmpTop >= screenRect.yMin)
                        result.top = tmpTop;
                }
            }
        }

        static void ComputePositionBottom(Rect screenRect, Rect elementRect, Rect anchorRect, PositionOptions options, ref PositionResult result)
        {
            var bottomSideTop = anchorRect.yMax;
            var topSideTop = anchorRect.yMin - elementRect.height;
            var bottomSideSpace = screenRect.height - anchorRect.yMax;
            var topSideSpace = anchorRect.yMin;

            if (options.shouldFlip &&
                bottomSideTop + elementRect.height + options.offset > screenRect.height &&
                bottomSideSpace < topSideSpace)
            {
                result.top = topSideTop;
                result.marginTop = -options.offset;
                result.finalPlacement = options.favoritePlacement switch
                {
                    PopoverPlacement.Bottom => PopoverPlacement.Top,
                    PopoverPlacement.BottomEnd => PopoverPlacement.TopEnd,
                    PopoverPlacement.BottomStart => PopoverPlacement.TopStart,
                    PopoverPlacement.BottomLeft => PopoverPlacement.TopLeft,
                    PopoverPlacement.BottomRight => PopoverPlacement.TopRight,
                    _ => options.favoritePlacement
                };
            }
            else
            {
                result.top = bottomSideTop;
                result.marginTop = options.offset;
            }
        }

        static void ComputePositionTop(Rect screenRect, Rect elementRect, Rect anchorRect, PositionOptions options, ref PositionResult result)
        {
            var bottomSideTop = anchorRect.yMax;
            var topSideTop = anchorRect.yMin - elementRect.height;
            var bottomSideSpace = screenRect.height - anchorRect.yMax;
            var topSideSpace = anchorRect.yMin - elementRect.yMin;

            if (options.shouldFlip &&
                topSideTop - options.offset < screenRect.yMin &&
                topSideSpace < bottomSideSpace)
            {
                result.top = bottomSideTop;
                result.marginTop = options.offset;
                result.finalPlacement = options.favoritePlacement switch
                {
                    PopoverPlacement.Top => PopoverPlacement.Bottom,
                    PopoverPlacement.TopEnd => PopoverPlacement.BottomEnd,
                    PopoverPlacement.TopStart => PopoverPlacement.BottomStart,
                    PopoverPlacement.TopLeft => PopoverPlacement.BottomLeft,
                    PopoverPlacement.TopRight => PopoverPlacement.BottomRight,
                    _ => options.favoritePlacement
                };
            }
            else
            {
                result.top = topSideTop;
                result.marginTop = -options.offset;
            }
        }

        static void ComputePositionLeft(Rect screenRect, Rect elementRect, Rect anchorRect, PositionOptions options, ref PositionResult result)
        {
            var leftSideLeft = anchorRect.xMin - elementRect.width;
            var rightSideLeft = anchorRect.xMax;
            var leftSideSpace = anchorRect.xMin - screenRect.xMin;
            var rightSideSpace = screenRect.width - anchorRect.xMax;

            if (options.shouldFlip &&
                leftSideLeft - options.offset < screenRect.xMin &&
                leftSideSpace < rightSideSpace)
            {
                result.left = rightSideLeft;
                result.marginLeft = options.offset;
                result.finalPlacement = options.favoritePlacement switch
                {
                    PopoverPlacement.Left => PopoverPlacement.Right,
                    PopoverPlacement.Start => PopoverPlacement.End,
                    PopoverPlacement.LeftTop => PopoverPlacement.RightTop,
                    PopoverPlacement.StartTop => PopoverPlacement.EndTop,
                    PopoverPlacement.LeftBottom => PopoverPlacement.RightBottom,
                    PopoverPlacement.StartBottom => PopoverPlacement.EndBottom,
                    _ => options.favoritePlacement
                };
            }
            else
            {
                result.left = leftSideLeft;
                result.marginLeft = -options.offset;
            }
        }

        static void ComputePositionRight(Rect screenRect, Rect elementRect, Rect anchorRect, PositionOptions options, ref PositionResult result)
        {
            var leftSideLeft = anchorRect.xMin - elementRect.width;
            var rightSideLeft = anchorRect.xMax;
            var leftSideSpace = anchorRect.xMin - screenRect.xMin;
            var rightSideSpace = screenRect.width - anchorRect.xMax;

            if (options.shouldFlip &&
                rightSideLeft + elementRect.width + options.offset > screenRect.xMax &&
                rightSideSpace < leftSideSpace)
            {
                result.left = leftSideLeft;
                result.marginLeft = -options.offset;
                result.finalPlacement = options.favoritePlacement switch
                {
                    PopoverPlacement.Right => PopoverPlacement.Left,
                    PopoverPlacement.End => PopoverPlacement.Start,
                    PopoverPlacement.RightTop => PopoverPlacement.LeftTop,
                    PopoverPlacement.EndTop => PopoverPlacement.StartTop,
                    PopoverPlacement.RightBottom => PopoverPlacement.LeftBottom,
                    PopoverPlacement.EndBottom => PopoverPlacement.StartBottom,
                    _ => options.favoritePlacement
                };
            }
            else
            {
                result.left = rightSideLeft;
                result.marginLeft = options.offset;
            }
        }

        /// <summary>
        /// This method will return the possible position of UI element based on a specific context.
        /// </summary>
        /// <param name="element">The element which needs to be positioned</param>
        /// <param name="anchor">The element used as an anchor for the element.</param>
        /// <param name="container">The element used as container.</param>
        /// <param name="options"> The options used to compute the position.</param>
        /// <returns>The computed position.</returns>
        /// <exception cref="ArgumentOutOfRangeException">The provided `favoritePlacement` value is invalid.</exception>
        public static PositionResult ComputePosition(VisualElement element, VisualElement anchor, VisualElement container, PositionOptions options)
        {
            var result = new PositionResult();
            result.finalPlacement = options.favoritePlacement;

            if (container == null)
                return result;

            var anchorRect = anchor.worldBound;
            anchorRect.x -= container.worldBound.x;
            anchorRect.y -= container.worldBound.y;
            var screenRect = new Rect(Vector2.zero, container.worldBound.size);
            var elementRect = element.worldBound;
            var halfHorizontalDeltaWidth = (elementRect.width - anchorRect.width) * 0.5f;
            var halfVerticalDeltaWidth = (elementRect.height - anchorRect.height) * 0.5f;

            if (float.IsNaN(halfHorizontalDeltaWidth) || float.IsNaN(halfVerticalDeltaWidth))
                return result;

            switch (options.favoritePlacement)
            {
                case PopoverPlacement.Bottom:
                    result.left = anchorRect.x - halfHorizontalDeltaWidth;
                    result.marginLeft = options.crossOffset;
                    ComputePositionBottom(screenRect, elementRect, anchorRect, options, ref result);
                    CrossSnapHorizontally(ref result, screenRect, elementRect, options.crossSnap);
                    break;
                case PopoverPlacement.BottomLeft:
                case PopoverPlacement.BottomStart:
                    result.left = anchorRect.x;
                    result.marginLeft = options.crossOffset;
                    ComputePositionBottom(screenRect, elementRect, anchorRect, options, ref result);
                    CrossSnapHorizontally(ref result, screenRect, elementRect, options.crossSnap);
                    break;
                case PopoverPlacement.BottomRight:
                case PopoverPlacement.BottomEnd:
                    result.left = anchorRect.xMax - elementRect.width;
                    result.marginLeft = -options.crossOffset;
                    ComputePositionBottom(screenRect, elementRect, anchorRect, options, ref result);
                    CrossSnapHorizontally(ref result, screenRect, elementRect, options.crossSnap);
                    break;
                case PopoverPlacement.Top:
                    result.left = anchorRect.x - halfHorizontalDeltaWidth;
                    result.marginLeft = options.crossOffset;
                    ComputePositionTop(screenRect, elementRect, anchorRect, options, ref result);
                    CrossSnapHorizontally(ref result, screenRect, elementRect, options.crossSnap);
                    break;
                case PopoverPlacement.TopLeft:
                case PopoverPlacement.TopStart:
                    result.left = anchorRect.x;
                    result.marginLeft = options.crossOffset;
                    ComputePositionTop(screenRect, elementRect, anchorRect, options, ref result);
                    CrossSnapHorizontally(ref result, screenRect, elementRect, options.crossSnap);
                    break;
                case PopoverPlacement.TopRight:
                case PopoverPlacement.TopEnd:
                    result.left = anchorRect.xMax - elementRect.width;
                    result.marginLeft = -options.crossOffset;
                    ComputePositionTop(screenRect, elementRect, anchorRect, options, ref result);
                    CrossSnapHorizontally(ref result, screenRect, elementRect, options.crossSnap);
                    break;
                case PopoverPlacement.Left:
                case PopoverPlacement.Start:
                    result.top = anchorRect.yMin - halfVerticalDeltaWidth;
                    result.marginTop = options.crossOffset;
                    ComputePositionLeft(screenRect, elementRect, anchorRect, options, ref result);
                    CrossSnapVertically(ref result, screenRect, elementRect, options.crossSnap);
                    break;
                case PopoverPlacement.LeftTop:
                case PopoverPlacement.StartTop:
                    result.top = anchorRect.yMin;
                    result.marginTop = options.crossOffset;
                    ComputePositionLeft(screenRect, elementRect, anchorRect, options, ref result);
                    CrossSnapVertically(ref result, screenRect, elementRect, options.crossSnap);
                    break;
                case PopoverPlacement.LeftBottom:
                case PopoverPlacement.StartBottom:
                    result.top = anchorRect.yMax - elementRect.height;
                    result.marginTop = -options.crossOffset;
                    ComputePositionLeft(screenRect, elementRect, anchorRect, options, ref result);
                    CrossSnapVertically(ref result, screenRect, elementRect, options.crossSnap);
                    break;
                case PopoverPlacement.Right:
                case PopoverPlacement.End:
                    result.top = anchorRect.yMin - halfVerticalDeltaWidth;
                    result.marginTop = options.crossOffset;
                    ComputePositionRight(screenRect, elementRect, anchorRect, options, ref result);
                    CrossSnapVertically(ref result, screenRect, elementRect, options.crossSnap);
                    break;
                case PopoverPlacement.RightTop:
                case PopoverPlacement.EndTop:
                    result.top = anchorRect.yMin;
                    result.marginTop = options.crossOffset;
                    ComputePositionRight(screenRect, elementRect, anchorRect, options, ref result);
                    CrossSnapVertically(ref result, screenRect, elementRect, options.crossSnap);
                    break;
                case PopoverPlacement.RightBottom:
                case PopoverPlacement.EndBottom:
                    result.top = anchorRect.yMax - elementRect.height;
                    result.marginTop = -options.crossOffset;
                    ComputePositionRight(screenRect, elementRect, anchorRect, options, ref result);
                    CrossSnapVertically(ref result, screenRect, elementRect, options.crossSnap);
                    break;
                case PopoverPlacement.InsideTopStart:
                    result.top = anchorRect.yMin;
                    result.marginTop = options.offset;
                    result.left = anchorRect.xMin;
                    result.marginLeft = options.crossOffset;
                    break;
                case PopoverPlacement.InsideTopEnd:
                    result.top = anchorRect.yMin;
                    result.marginTop = options.offset;
                    result.left = anchorRect.xMax - elementRect.width;
                    result.marginLeft = -options.crossOffset;
                    break;
                case PopoverPlacement.InsideTop:
                    result.top = anchorRect.yMin;
                    result.marginTop = options.offset;
                    result.left = anchorRect.center.x - elementRect.width * 0.5f;
                    result.marginLeft = options.crossOffset;
                    break;
                case PopoverPlacement.InsideBottomStart:
                    result.top = anchorRect.yMax - elementRect.height;
                    result.marginTop = -options.offset;
                    result.left = anchorRect.xMin;
                    result.marginLeft = options.crossOffset;
                    break;
                case PopoverPlacement.InsideBottomEnd:
                    result.top = anchorRect.yMax - elementRect.height;
                    result.marginTop = -options.offset;
                    result.left = anchorRect.xMax - elementRect.width;
                    result.marginLeft = -options.crossOffset;
                    break;
                case PopoverPlacement.InsideBottom:
                    result.top = anchorRect.yMax - elementRect.height;
                    result.marginTop = -options.offset;
                    result.left = anchorRect.center.x - elementRect.width * 0.5f;
                    result.marginLeft = options.crossOffset;
                    break;
                case PopoverPlacement.InsideStart:
                    result.top = anchorRect.center.y - elementRect.height * 0.5f;
                    result.marginTop = options.crossOffset;
                    result.left = anchorRect.xMin;
                    result.marginLeft = options.offset;
                    break;
                case PopoverPlacement.InsideEnd:
                    result.top = anchorRect.center.y - elementRect.height * 0.5f;
                    result.marginTop = options.crossOffset;
                    result.left = anchorRect.xMax - elementRect.width;
                    result.marginLeft = -options.offset;
                    break;
                case PopoverPlacement.InsideCenter:
                    result.top = anchorRect.center.y - elementRect.height * 0.5f;
                    result.marginTop = options.crossOffset;
                    result.left = anchorRect.center.x - elementRect.width * 0.5f;
                    result.marginLeft = options.offset;
                    break;
                default:
                    break;
            }

            const float tipHalfSize = 6;
            const float autoLength = -1;
            const float popoverPadding = 12;

            // compute tip/arrow placement
            switch (result.finalPlacement)
            {
                case PopoverPlacement.Bottom:
                case PopoverPlacement.BottomLeft:
                case PopoverPlacement.BottomRight:
                case PopoverPlacement.BottomStart:
                case PopoverPlacement.BottomEnd:
                    result.tipTop = tipHalfSize;
                    result.tipBottom = autoLength;
                    result.tipLeft = Mathf.Clamp(anchorRect.center.x - (result.left + result.marginLeft), popoverPadding * 2, elementRect.width - popoverPadding * 2);
                    result.tipRight = autoLength;
                    break;
                case PopoverPlacement.Top:
                case PopoverPlacement.TopLeft:
                case PopoverPlacement.TopRight:
                case PopoverPlacement.TopStart:
                case PopoverPlacement.TopEnd:
                    result.tipTop = autoLength;
                    result.tipBottom = tipHalfSize;
                    result.tipLeft = Mathf.Clamp(anchorRect.center.x - (result.left + result.marginLeft), popoverPadding * 2, elementRect.width - popoverPadding * 2);
                    result.tipRight = autoLength;
                    break;
                case PopoverPlacement.Left:
                case PopoverPlacement.LeftTop:
                case PopoverPlacement.LeftBottom:
                case PopoverPlacement.Start:
                case PopoverPlacement.StartTop:
                case PopoverPlacement.StartBottom:
                    result.tipTop = Mathf.Clamp(anchorRect.center.y - (result.top + result.marginTop), popoverPadding * 2, elementRect.height - popoverPadding * 2);
                    result.tipBottom = autoLength;
                    result.tipLeft = autoLength;
                    result.tipRight = tipHalfSize;
                    break;
                case PopoverPlacement.Right:
                case PopoverPlacement.RightTop:
                case PopoverPlacement.RightBottom:
                case PopoverPlacement.End:
                case PopoverPlacement.EndTop:
                case PopoverPlacement.EndBottom:
                    result.tipTop = Mathf.Clamp(anchorRect.center.y - (result.top + result.marginTop), popoverPadding * 2, elementRect.height - popoverPadding * 2);
                    result.tipBottom = autoLength;
                    result.tipLeft = tipHalfSize;
                    result.tipRight = autoLength;
                    break;
                default:
                    break;
            }

            return result;
        }

    }
}
