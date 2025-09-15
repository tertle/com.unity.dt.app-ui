using System;
using NUnit.Framework;
using Unity.AppUI.Core;
using UnityEngine;

namespace Unity.AppUI.Tests.Core
{
    [TestFixture]
    [TestOf(typeof(Platform))]
    public class PlatformTests
    {
        static byte[] GetDataForType(PasteboardType type)
        {
            switch (type)
            {
                case PasteboardType.Text:
                    return System.Text.Encoding.UTF8.GetBytes("Hello, World!");
                case PasteboardType.PNG:
                    var tex = new Texture2D(1, 1);
                    tex.SetPixel(0, 0, Color.red);
                    tex.Apply();
                    return tex.EncodeToPNG();
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        static void CheckPasteboardData(PasteboardType type, byte[] data)
        {
            switch (type)
            {
                case PasteboardType.Text:
                    Assert.AreEqual("Hello, World!", data);
                    break;
                case PasteboardType.PNG:
                    var tex = new Texture2D(1, 1);
                    tex.LoadImage(data);
                    Assert.AreEqual(Color.red, tex.GetPixel(0, 0));
                    Assert.AreEqual(1, tex.width);
                    Assert.AreEqual(1, tex.height);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        [Test]
        [TestCase(PasteboardType.Text)]
        [TestCase(PasteboardType.PNG)]
        public void CanGetAndSetPasteboardData(PasteboardType type)
        {
            // create data and set it to the pasteboard
            var data = GetDataForType(type);
            Assert.DoesNotThrow(() => Platform.SetPasteboardData(type, data));

            // check if the pasteboard has data
            var hasData = Platform.HasPasteboardData(type);
            Assert.IsTrue(hasData);

            // get the data from the pasteboard and check if the data is the same as the original data
            var pasteboardData = Platform.GetPasteboardData(type);
            CheckPasteboardData(type, pasteboardData);
        }
    }
}
