using System;
using NUnit.Framework;
using Unity.AppUI.Core;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using UnityEngine;

namespace Unity.AppUI.Editor.Tests
{
    [TestFixture]
    [TestOf(typeof(OptionalPropertyDrawer<,>))]
    class OptionalPropertyDrawerTests
    {
        [Serializable]
        class IntObject : ScriptableObject
        {
            [SerializeField]
            public Optional<int> value;
        }

        [Test]
        public void OptionalPropertyDrawer_CreatesPropertyGUI_ShouldNotThrow()
        {
            var drawer = new OptionalPropertyDrawer<int, IntegerField>();
            var obj = ScriptableObject.CreateInstance<IntObject>();
            var serializedObject = new SerializedObject(obj);
            Assert.DoesNotThrow(() => drawer.CreatePropertyGUI(serializedObject.FindProperty("value")));
            Assert.DoesNotThrow(() => drawer.SetValueInternal(42));
        }
    }

    [TestFixture]
    [TestOf(typeof(OptionalEnumPropertyDrawer<>))]
    class OptionalEnumPropertyDrawerTests
    {
        [Serializable]
        class EnumObject : ScriptableObject
        {
            [SerializeField]
            public OptionalEnum<Dir> value;
        }

        [Test]
        public void OptionalEnumPropertyDrawer_CreatesPropertyGUI_ShouldNotThrow()
        {
            var drawer = new OptionalEnumPropertyDrawer<Dir>();
            var obj = ScriptableObject.CreateInstance<EnumObject>();
            var serializedObject = new SerializedObject(obj);
            VisualElement field = null;
            Assert.DoesNotThrow(() => field = drawer.CreatePropertyGUI(serializedObject.FindProperty("value")));
            Assert.IsNotNull(field);

            var checkbox = field.Q<Toggle>();
            var enumField = field.Q<EnumField>();

            Assert.IsNotNull(checkbox);
            Assert.IsNotNull(enumField);

            Assert.DoesNotThrow(() => checkbox.value = true);
            Assert.DoesNotThrow(() => enumField.value = Dir.Rtl);
        }
    }

    [TestFixture]
    [TestOf(typeof(OptionalIntDrawer))]
    class OptionalIntDrawerTests
    {
        [Serializable]
        class IntObject : ScriptableObject
        {
            [SerializeField]
            public Optional<int> value;
        }

        [Test]
        public void OptionalIntDrawer_CreatesPropertyGUI_ShouldNotThrow()
        {
            var drawer = new OptionalIntDrawer();
            var obj = ScriptableObject.CreateInstance<IntObject>();
            var serializedObject = new SerializedObject(obj);
            Assert.DoesNotThrow(() => drawer.CreatePropertyGUI(serializedObject.FindProperty("value")));
            Assert.DoesNotThrow(() => drawer.SetValueInternal(42));
        }
    }

    [TestFixture]
    [TestOf(typeof(OptionalLongDrawer))]
    class OptionalLongDrawerTests
    {
        [Serializable]
        class LongObject : ScriptableObject
        {
            [SerializeField]
            public Optional<long> value;
        }

        [Test]
        public void OptionalLongDrawer_CreatesPropertyGUI_ShouldNotThrow()
        {
            var drawer = new OptionalLongDrawer();
            var obj = ScriptableObject.CreateInstance<LongObject>();
            var serializedObject = new SerializedObject(obj);
            Assert.DoesNotThrow(() => drawer.CreatePropertyGUI(serializedObject.FindProperty("value")));
            Assert.DoesNotThrow(() => drawer.SetValueInternal(42));
        }
    }

    [TestFixture]
    [TestOf(typeof(OptionalStringDrawer))]
    class OptionalStringDrawerTests
    {
        [Serializable]
        class StringObject : ScriptableObject
        {
            [SerializeField]
            public Optional<string> value;
        }

        [Test]
        public void OptionalStringDrawer_CreatesPropertyGUI_ShouldNotThrow()
        {
            var drawer = new OptionalStringDrawer();
            var obj = ScriptableObject.CreateInstance<StringObject>();
            var serializedObject = new SerializedObject(obj);
            Assert.DoesNotThrow(() => drawer.CreatePropertyGUI(serializedObject.FindProperty("value")));
            Assert.DoesNotThrow(() => drawer.SetValueInternal("42"));
        }
    }

    [TestFixture]
    [TestOf(typeof(OptionalScaleDrawer))]
    class OptionalScaleDrawerTests
    {
        [Serializable]
        class ScaleObject : ScriptableObject
        {
            [SerializeField]
            public Optional<string> value;
        }

        [Test]
        public void OptionalScaleDrawer_CreatesPropertyGUI_ShouldNotThrow()
        {
            var drawer = new OptionalScaleDrawer();
            var obj = ScriptableObject.CreateInstance<ScaleObject>();
            var serializedObject = new SerializedObject(obj);
            Assert.DoesNotThrow(() => drawer.CreatePropertyGUI(serializedObject.FindProperty("value")));
            Assert.DoesNotThrow(() => drawer.SetValueInternal("medium"));
        }
    }

    [TestFixture]
    [TestOf(typeof(OptionalThemeDrawer))]
    class OptionalThemeDrawerTests
    {
        [Serializable]
        class ThemeObject : ScriptableObject
        {
            [SerializeField]
            public Optional<string> value;
        }

        [Test]
        public void OptionalThemeDrawer_CreatesPropertyGUI_ShouldNotThrow()
        {
            var drawer = new OptionalThemeDrawer();
            var obj = ScriptableObject.CreateInstance<ThemeObject>();
            var serializedObject = new SerializedObject(obj);
            Assert.DoesNotThrow(() => drawer.CreatePropertyGUI(serializedObject.FindProperty("value")));
            Assert.DoesNotThrow(() => drawer.SetValueInternal("dark"));
        }
    }

    [TestFixture]
    [TestOf(typeof(OptionalDirDrawer))]
    class OptionalDirDrawerTests
    {
        [Serializable]
        class DirObject : ScriptableObject
        {
            [SerializeField]
            public Optional<Dir> value;
        }

        [Test]
        public void OptionalDirDrawer_CreatesPropertyGUI_ShouldNotThrow()
        {
            var drawer = new OptionalDirDrawer();
            var obj = ScriptableObject.CreateInstance<DirObject>();
            var serializedObject = new SerializedObject(obj);
            Assert.DoesNotThrow(() => drawer.CreatePropertyGUI(serializedObject.FindProperty("value")));
        }
    }

    [TestFixture]
    [TestOf(typeof(OptionalPreferredTooltipPlacementDrawer))]
    class OptionalPreferredTooltipPlacementDrawerTests
    {
        [Serializable]
        class PlacementObject : ScriptableObject
        {
            [SerializeField]
            public OptionalEnum<UI.PopoverPlacement> value;
        }

        [Test]
        public void OptionalPreferredTooltipPlacementDrawer_CreatesPropertyGUI_ShouldNotThrow()
        {
            var drawer = new OptionalPreferredTooltipPlacementDrawer();
            var obj = ScriptableObject.CreateInstance<PlacementObject>();
            var serializedObject = new SerializedObject(obj);
            Assert.DoesNotThrow(() => drawer.CreatePropertyGUI(serializedObject.FindProperty("value")));
        }
    }

    [TestFixture]
    [TestOf(typeof(OptionalColorDrawer))]
    class OptionalColorDrawerTests
    {
        [Serializable]
        class ColorObject : ScriptableObject
        {
            [SerializeField]
            public Optional<Color> value;
        }

        [Test]
        public void OptionalColorDrawer_CreatesPropertyGUI_ShouldNotThrow()
        {
            var drawer = new OptionalColorDrawer();
            var obj = ScriptableObject.CreateInstance<ColorObject>();
            var serializedObject = new SerializedObject(obj);
            Assert.DoesNotThrow(() => drawer.CreatePropertyGUI(serializedObject.FindProperty("value")));
            Assert.DoesNotThrow(() => drawer.SetValueInternal(Color.red));
        }
    }

    [TestFixture]
    [TestOf(typeof(OptionalRectDrawer))]
    class OptionalRectDrawerTests
    {
        [Serializable]
        class RectObject : ScriptableObject
        {
            [SerializeField]
            public Optional<Rect> value;
        }

        [Test]
        public void OptionalRectDrawer_CreatesPropertyGUI_ShouldNotThrow()
        {
            var drawer = new OptionalRectDrawer();
            var obj = ScriptableObject.CreateInstance<RectObject>();
            var serializedObject = new SerializedObject(obj);
            Assert.DoesNotThrow(() => drawer.CreatePropertyGUI(serializedObject.FindProperty("value")));
            Assert.DoesNotThrow(() => drawer.SetValueInternal(new Rect(0, 0, 100, 100)));
        }
    }

    [TestFixture]
    [TestOf(typeof(OptionalFloatDrawer))]
    class OptionalFloatDrawerTests
    {
        [Serializable]
        class FloatObject : ScriptableObject
        {
            [SerializeField]
            public Optional<float> value;
        }

        [Test]
        public void OptionalFloatDrawer_CreatesPropertyGUI_ShouldNotThrow()
        {
            var drawer = new OptionalFloatDrawer();
            var obj = ScriptableObject.CreateInstance<FloatObject>();
            var serializedObject = new SerializedObject(obj);
            Assert.DoesNotThrow(() => drawer.CreatePropertyGUI(serializedObject.FindProperty("value")));
            Assert.DoesNotThrow(() => drawer.SetValueInternal(42.0f));
        }
    }

    [TestFixture]
    [TestOf(typeof(OptionalDoubleDrawer))]
    class OptionalDoubleDrawerTests
    {
        [Serializable]
        class DoubleObject : ScriptableObject
        {
            [SerializeField]
            public Optional<double> value;
        }

        [Test]
        public void OptionalDoubleDrawer_CreatesPropertyGUI_ShouldNotThrow()
        {
            var drawer = new OptionalDoubleDrawer();
            var obj = ScriptableObject.CreateInstance<DoubleObject>();
            var serializedObject = new SerializedObject(obj);
            Assert.DoesNotThrow(() => drawer.CreatePropertyGUI(serializedObject.FindProperty("value")));
            Assert.DoesNotThrow(() => drawer.SetValueInternal(42.0));
        }
    }
}
