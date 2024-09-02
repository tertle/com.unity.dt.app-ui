using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.AppUI.UI
{
    /// <summary>
    /// Extension methods for <see cref="Background"/> objects.
    /// </summary>
    public static class BackgroundExtensions
    {
#if !UNITY_2023_2_OR_NEWER
        /// <summary>
        /// Gets the selected image from the background.
        /// </summary>
        /// <param name="bg"> The background to get the image from. </param>
        /// <returns> The selected image. </returns>
        public static Object GetSelectedImage(this Background bg)
        {
            if (bg == default)
                return (UnityEngine.Object) null;

            if ((UnityEngine.Object) bg.texture != (UnityEngine.Object) null)
                return (UnityEngine.Object) bg.texture;
            if ((UnityEngine.Object) bg.sprite != (UnityEngine.Object) null)
                return (UnityEngine.Object) bg.sprite;
            if ((UnityEngine.Object) bg.renderTexture != (UnityEngine.Object) null)
                return (UnityEngine.Object) bg.renderTexture;
            return (UnityEngine.Object) bg.vectorImage != (UnityEngine.Object) null ? (UnityEngine.Object) bg.vectorImage : (UnityEngine.Object) null;
        }
#endif

        /// <summary>
        /// Creates a new <see cref="Background"/> from a Unity image asset.
        /// </summary>
        /// <param name="obj"> The image asset to create the background from. </param>
        /// <returns> The created background. </returns>
        /// <remarks>
        /// The image asset can be a <see cref="Texture2D"/>, <see cref="Sprite"/>, <see cref="RenderTexture"/> or <see cref="VectorImage"/>.
        /// </remarks>
        public static Background FromObject(Object obj)
        {
            return obj switch
            {
                Texture2D tex => Background.FromTexture2D(tex),
                Sprite sprite => Background.FromSprite(sprite),
                RenderTexture rt => Background.FromRenderTexture(rt),
                VectorImage vi => Background.FromVectorImage(vi),
                _ => new Background()
            };
        }
    }
}
