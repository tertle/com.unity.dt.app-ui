using System;
using NUnit.Framework;
using Unity.AppUI.Core;
using UnityEditor;
using UnityEngine;

namespace Unity.AppUI.Editor.Tests
{
    [TestFixture]
    [TestOf(typeof(DirDrawer))]
    class DirDrawerTests
    {
        [Serializable]
        class DirObject : ScriptableObject
        {
            [SerializeField]
            public Dir value;
        }

        [Test]
        public void DirDrawer_CreatesPropertyGUI_ShouldNotThrow()
        {
            var drawer = new DirDrawer();
            var obj = ScriptableObject.CreateInstance<DirObject>();
            var serializedObject = new SerializedObject(obj);
            Assert.DoesNotThrow(() => drawer.CreatePropertyGUI(serializedObject.FindProperty("value")));
        }
    }

    [TestFixture]
    [TestOf(typeof(ThemeDrawer))]
    class ThemeDrawerTests
    {
        [Serializable]
        class ThemeObject : ScriptableObject
        {
            [SerializeField]
            public string value;
        }

        [Test]
        public void ThemeDrawer_CreatesPropertyGUI_ShouldNotThrow()
        {
            var drawer = new ThemeDrawer();
            var obj = ScriptableObject.CreateInstance<ThemeObject>();
            var serializedObject = new SerializedObject(obj);
            Assert.DoesNotThrow(() => drawer.CreatePropertyGUI(serializedObject.FindProperty("value")));
        }
    }

    [TestFixture]
    [TestOf(typeof(ScaleDrawer))]
    class ScaleDrawerTests
    {
        [Serializable]
        class ScaleObject : ScriptableObject
        {
            [SerializeField]
            public string value;
        }

        [Test]
        public void ScaleDrawer_CreatesPropertyGUI_ShouldNotThrow()
        {
            var drawer = new ScaleDrawer();
            var obj = ScriptableObject.CreateInstance<ScaleObject>();
            var serializedObject = new SerializedObject(obj);
            Assert.DoesNotThrow(() => drawer.CreatePropertyGUI(serializedObject.FindProperty("value")));
        }
    }
}
