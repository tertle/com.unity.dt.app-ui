using System.Reflection;
using UnityEngine.UIElements.Experimental;

namespace Unity.AppUI.Bridge
{
    static class ValueAnimationExtensionsBridge
    {
        // We have to use Reflection since the field is private and there is no public API to check if an animation is recycled.
        static readonly PropertyInfo k_Recycled =
            typeof(ValueAnimation<float>).GetProperty("recycled", BindingFlags.Instance | BindingFlags.NonPublic);

        internal static bool IsRecycled(this ValueAnimation<float> animation)
        {
            return (bool)k_Recycled.GetValue(animation);
        }
    }
}
