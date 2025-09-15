using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Unity.AppUI.UI;
using UnityEngine.TestTools;
using MaskField = Unity.AppUI.UI.MaskField;

namespace Unity.AppUI.Tests.UI
{
    [TestFixture]
    [TestOf(typeof(MaskField))]
    class MaskFieldTests : VisualElementTests<MaskField>
    {
        protected override string mainUssClassName => MaskField.ussClassName;

        protected override IEnumerable<Story> stories
        {
            get
            {
                yield return new Story("Default", _ => new MaskField(
                    Enum.GetValues(typeof(EnumMaskForTest)),
                    (int)(EnumMaskForTest.Value1 | EnumMaskForTest.Value2)));
            }
        }

        protected override bool uxmlConstructable => true;

        protected override IEnumerable<string> uxmlTestCases
        {
            get
            {
                yield return "<appui:MaskField />";
            }
        }

        [UnityTest, Order(3)]
        public IEnumerator MaskField_Value_ShouldBeSettable()
        {
            var t = typeof(EnumMaskForTest);
            var values = Enum.GetValues(t);
            var names = Enum.GetNames(t);

            var doc = Utils.ConstructTestUI();
            var field = new MaskField(
                values,
                0,
                i => names[i],
                i => (int)values.GetValue(i));
            doc.rootVisualElement.Add(field);

            yield return null;

            Assert.AreEqual(0, field.value);
            Assert.AreEqual(0, ((Dropdown)field).value.Count());

            var expected = (int) (EnumMaskForTest.Value1 | EnumMaskForTest.Value2);
            field.value = expected;
            Assert.AreEqual(expected, field.value);
            Assert.AreEqual(3, ((Dropdown)field).value.Count());

            UnityEngine.Object.Destroy(doc);
        }
    }

    [Flags]
    enum EnumMaskForTest
    {
        Value1 = 0x1,
        Value2 = 0x2,
        Value3 = Value1 | Value2
    }
}
