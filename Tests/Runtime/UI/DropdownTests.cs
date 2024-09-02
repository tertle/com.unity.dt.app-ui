using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Unity.AppUI.UI;
using UnityEngine.TestTools;
using UnityEngine.UIElements;

namespace Unity.AppUI.Tests.UI
{
    [TestFixture]
    [TestOf(typeof(Dropdown))]
    class DropdownTests : VisualElementTests<Dropdown>
    {
        protected override string mainUssClassName => Dropdown.ussClassName;

        [UnityTest]
        [Order(10)]
        [TestCase(null, ExpectedResult = null)]
        [TestCase(new int[] {}, ExpectedResult = null)]
        [TestCase(new int[] { 0 }, ExpectedResult = null)]
        public IEnumerator Dropdown_WhenSettingSourceItems_ShouldResetValue(int[] defaultValue)
        {
            m_TestUI.rootVisualElement.Clear();
            var panel = new Panel();
            m_TestUI.rootVisualElement.Add(panel);
            var dropdown = new Dropdown
            {
                defaultValue = defaultValue
            };

            var defaultNonNullValue = defaultValue ?? Array.Empty<int>();
            Assert.AreEqual(defaultValue, dropdown.defaultValue, "Default value should be set");

            var valueChangedNotified = 0;
            dropdown.RegisterValueChangedCallback(OnValueChanged);
            panel.Add(dropdown);

            yield return null;

            Assert.IsEmpty(dropdown.value, "Value should be empty if there are no source items");
            Assert.Zero(valueChangedNotified, "Value changed should not be notified by default");

            dropdown.sourceItems = new[] { "Item 1", "Item 2", "Item 3" };

            yield return null;

            CollectionAssert.AreEquivalent(defaultNonNullValue, dropdown.value, "Value should be set to the default value");
            var expectedNotified = defaultNonNullValue.Length == 0 ? 0 : 1;
            Assert.AreEqual(expectedNotified, valueChangedNotified, "Value changed should br notified when source items are changed " +
                "only if the new value is different than the previous one");

            dropdown.value = new[] {2};

            yield return null;

            CollectionAssert.AreEquivalent(new[] {2}, dropdown.value, "Value should be set to the new value");
            Assert.AreEqual(++expectedNotified, valueChangedNotified, "Value changed should be notified when value is changed");

            dropdown.sourceItems = new[] { "Item 1", "Item 2", "Item 3", "Item 4" };

            yield return null;

            CollectionAssert.AreEquivalent(defaultNonNullValue, dropdown.value, "Value should be set to the default value");
            Assert.AreEqual(++expectedNotified, valueChangedNotified);

            yield break;

            // --------------------------------------------------------------------------------------------

            void OnValueChanged(ChangeEvent<IEnumerable<int>> evt)
            {
                valueChangedNotified++;
            }
        }
    }
}
