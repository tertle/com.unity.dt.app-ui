using NUnit.Framework;
using Unity.AppUI.UI;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.AppUI.Tests.UI
{
    [TestFixture]
    [TestOf(typeof(BackgroundExtensions))]
    class BackgroundExtensionsTests
    {
        [Test]
        public void FromObject_WhenTexture2D_ReturnsBackgroundFromTexture2D()
        {
            var tex = new Texture2D(1, 1);
            var bg = BackgroundExtensions.FromObject(tex);
            Assert.That(bg.texture, Is.EqualTo(tex));
            Object.DestroyImmediate(tex);
        }

        [Test]
        public void FromObject_WhenSprite_ReturnsBackgroundFromSprite()
        {
            var tex = new Texture2D(1, 1);
            var sprite = Sprite.Create(tex, new Rect(), new Vector2());
            var bg = BackgroundExtensions.FromObject(sprite);
            Assert.That(bg.sprite, Is.EqualTo(sprite));
            Object.DestroyImmediate(sprite);
            Object.DestroyImmediate(tex);
        }

        [Test]
        public void FromObject_WhenRenderTexture_ReturnsBackgroundFromRenderTexture()
        {
            var rt = new RenderTexture(1, 1, 0);
            var bg = BackgroundExtensions.FromObject(rt);
            Assert.That(bg.renderTexture, Is.EqualTo(rt));
            Object.DestroyImmediate(rt);
        }

        [Test]
        public void FromObject_WhenVectorImage_ReturnsBackgroundFromVectorImage()
        {
            var vi = ScriptableObject.CreateInstance<VectorImage>();
            var bg = BackgroundExtensions.FromObject(vi);
            Assert.That(bg.vectorImage, Is.EqualTo(vi));
            Object.DestroyImmediate(vi);
        }

        [Test]
        public void FromObject_WhenNull_ReturnsDefaultBackground()
        {
            var bg = BackgroundExtensions.FromObject(null);
            Assert.That(bg.texture, Is.Null);
            Assert.That(bg.sprite, Is.Null);
            Assert.That(bg.renderTexture, Is.Null);
            Assert.That(bg.vectorImage, Is.Null);
        }

        [Test]
        public void GetSelectedImage_WhenTexture2D_ReturnsTexture2D()
        {
            var tex = new Texture2D(1, 1);
            var bg = new Background { texture = tex };
            Assert.That(bg.GetSelectedImage(), Is.EqualTo(tex));
            Object.DestroyImmediate(tex);
        }

        [Test]
        public void GetSelectedImage_WhenSprite_ReturnsSprite()
        {
            var tex = new Texture2D(1, 1);
            var sprite = Sprite.Create(tex, new Rect(), new Vector2());
            var bg = new Background { sprite = sprite };
            Assert.That(bg.GetSelectedImage(), Is.EqualTo(sprite));
            Object.DestroyImmediate(sprite);
            Object.DestroyImmediate(tex);
        }

        [Test]
        public void GetSelectedImage_WhenRenderTexture_ReturnsRenderTexture()
        {
            var rt = new RenderTexture(1, 1, 0);
            var bg = new Background { renderTexture = rt };
            Assert.That(bg.GetSelectedImage(), Is.EqualTo(rt));
            Object.DestroyImmediate(rt);
        }

        [Test]
        public void GetSelectedImage_WhenVectorImage_ReturnsVectorImage()
        {
            var vi = ScriptableObject.CreateInstance<VectorImage>();
            var bg = new Background { vectorImage = vi };
            Assert.That(bg.GetSelectedImage(), Is.EqualTo(vi));
            Object.DestroyImmediate(vi);
        }

        [Test]
        public void GetSelectedImage_WhenNull_ReturnsNull()
        {
            var bg = new Background();
            Assert.That(bg.GetSelectedImage(), Is.Null);
        }
    }
}
