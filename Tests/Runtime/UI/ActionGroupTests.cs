using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Unity.AppUI.UI;
using UnityEngine.UIElements;
using System.Linq;
using UnityEngine;
using UnityEngine.TestTools;
using Object = UnityEngine.Object;

namespace Unity.AppUI.Tests.UI
{
    [TestFixture]
    [TestOf(typeof(ActionGroup))]
    class ActionGroupTests : VisualElementTests<ActionGroup>
    {
        protected override string mainUssClassName => ActionGroup.ussClassName;

        protected override IEnumerable<string> uxmlTestCases => new[]
        {
            @"<appui:ActionGroup/>",
            @"<appui:ActionGroup quiet=""false"" compact=""false"">
                <appui:ActionButton icon=""info""/>
                <appui:ActionButton icon=""info""/>
                <appui:ActionButton icon=""info""/>
            </appui:ActionGroup>",
            @"<appui:ActionGroup selection-type=""Single"" allow-no-selection=""false"" quiet=""false"" compact=""true"" direction=""Horizontal"" justified=""true"" close-on-selection=""true"">
                <appui:ActionButton icon=""info""/>
                <appui:ActionButton icon=""info""/>
                <appui:ActionButton icon=""info""/>
            </appui:ActionGroup>",
            @"<appui:ActionGroup selection-type=""Multiple"" allow-no-selection=""true"" quiet=""false"" compact=""false"" direction=""Vertical"" justified=""false"" close-on-selection=""false"">
                <appui:ActionButton icon=""info""/>
                <appui:ActionButton icon=""info""/>
                <appui:ActionButton icon=""info""/>
            </appui:ActionGroup>",
        };

        [UnityTest]
        public IEnumerator HandleSelectionByInteraction()
        {
            var doc = Utils.ConstructTestUI();

            var actionGroup = new ActionGroup();
            doc.rootVisualElement.Add(actionGroup);

            Assert.AreEqual(0, actionGroup.childCount);
            Assert.AreEqual(SelectionType.None, actionGroup.selectionType);
            Assert.IsTrue(actionGroup.allowNoSelection,
                "ActionGroup should allow no selection by default.");

            actionGroup.selectionType = SelectionType.Single;

            Assert.AreEqual(SelectionType.Single, actionGroup.selectionType);
            Assert.AreEqual(0, actionGroup.childCount);
            Assert.AreEqual(0, actionGroup.selectedIndices.Count());

            var button1 = new ActionButton();
            var button2 = new ActionButton();
            var button3 = new ActionButton();

            actionGroup.Add(button1);
            actionGroup.Add(button2);
            actionGroup.Add(button3);

            yield return null;

            Assert.AreEqual(3, actionGroup.childCount);
            Assert.AreEqual(0, actionGroup.selectedIndices.Count(),
                "ActionGroup should not have any selected indices by default since allowNoSelection is true.");

            actionGroup.allowNoSelection = false;

            Assert.IsFalse(actionGroup.allowNoSelection);
            Assert.AreEqual(1, actionGroup.selectedIndices.Count(),
                "ActionGroup should have a selected index when allowNoSelection is false.");
            Assert.AreEqual(0, actionGroup.selectedIndices.First());

            var systemEvent = new Event
            {
                type = EventType.MouseUp,
                button = 0,
                clickCount = 1,
                mousePosition = Vector2.zero
            };
            using var upEvent = PointerUpEvent.GetPooled(systemEvent);
            button1.clickable.SimulateSingleClickInternal(upEvent);

            yield return null;

            Assert.AreEqual(1, actionGroup.selectedIndices.Count(),
                "The selection should not change when clicking on the selected item.");
            Assert.AreEqual(0, actionGroup.selectedIndices.First());

            button2.clickable.SimulateSingleClickInternal(upEvent);

            yield return null;

            Assert.AreEqual(1, actionGroup.selectedIndices.Count());
            Assert.AreEqual(1, actionGroup.selectedIndices.First(),
                "The selection should change when clicking on a different item.");

            actionGroup.allowNoSelection = true;

            Assert.IsTrue(actionGroup.allowNoSelection);
            Assert.AreEqual(1, actionGroup.selectedIndices.Count());
            Assert.AreEqual(1, actionGroup.selectedIndices.First());

            button2.clickable.SimulateSingleClickInternal(upEvent);

            yield return null;

            Assert.AreEqual(0, actionGroup.selectedIndices.Count());

            actionGroup.selectionType = SelectionType.Multiple;

            Assert.AreEqual(SelectionType.Multiple, actionGroup.selectionType);
            Assert.AreEqual(0, actionGroup.selectedIndices.Count());

            button1.clickable.SimulateSingleClickInternal(upEvent);

            yield return null;

            Assert.AreEqual(1, actionGroup.selectedIndices.Count());
            Assert.AreEqual(0, actionGroup.selectedIndices.First());

            button2.clickable.SimulateSingleClickInternal(upEvent);

            yield return null;

            Assert.AreEqual(2, actionGroup.selectedIndices.Count());
            Assert.AreEqual(0, actionGroup.selectedIndices.First());
            Assert.AreEqual(1, actionGroup.selectedIndices.Last());

            actionGroup.selectionType = SelectionType.None;

            Assert.AreEqual(SelectionType.None, actionGroup.selectionType);
            Assert.AreEqual(0, actionGroup.selectedIndices.Count(),
                "ActionGroup should not have any selected indices when selectionType is None.");

            Object.Destroy(doc);
        }

        [UnityTest]
        public IEnumerator HandleSelectionByCode()
        {
            var doc = Utils.ConstructTestUI();
            var actionGroup = new ActionGroup();
            doc.rootVisualElement.Add(actionGroup);

            var button1 = new ActionButton();
            var button2 = new ActionButton();
            var button3 = new ActionButton();

            actionGroup.getItemId = (int index) => index;
            actionGroup.selectionType = SelectionType.Multiple;

            actionGroup.Add(button1);
            actionGroup.Add(button2);
            actionGroup.Add(button3);

            yield return null;

            Assert.AreEqual(0, actionGroup.selectedIndices.Count());

            var selectionChangedCount = 0;
            var newIndexSelection = new List<int>();
            var newIdSelection = new List<int>();

            void OnSelectionChanged(IEnumerable<int> indices)
            {
                selectionChangedCount++;
                newIndexSelection.Clear();
                newIndexSelection.AddRange(indices);
                newIdSelection.Clear();
                newIdSelection.AddRange(indices.Select(actionGroup.getItemId));
            }

            actionGroup.selectionChanged += OnSelectionChanged;

            Assert.AreEqual(0, selectionChangedCount);
            Assert.AreEqual(0, newIndexSelection.Count);
            Assert.AreEqual(0, newIdSelection.Count);
            Assert.IsFalse(button1.selected);
            Assert.IsFalse(button2.selected);
            Assert.IsFalse(button3.selected);
            Assert.AreEqual(0, actionGroup.selectedIndices.Count());
            Assert.AreEqual(0, actionGroup.selectedIds.Count());

            actionGroup.SetSelection(new List<int> {0});

            Assert.AreEqual(1, selectionChangedCount);
            Assert.AreEqual(1, newIndexSelection.Count);
            Assert.AreEqual(0, newIndexSelection.First());
            Assert.AreEqual(1, newIdSelection.Count);
            Assert.AreEqual(0, newIdSelection.First());
            Assert.IsTrue(button1.selected);
            Assert.IsFalse(button2.selected);
            Assert.IsFalse(button3.selected);
            Assert.AreEqual(1, actionGroup.selectedIndices.Count());
            Assert.AreEqual(0, actionGroup.selectedIndices.First());
            Assert.AreEqual(1, actionGroup.selectedIds.Count());
            Assert.AreEqual(0, actionGroup.selectedIds.First());

            actionGroup.SetSelection(new List<int> {0, 1});

            Assert.AreEqual(2, selectionChangedCount);
            Assert.AreEqual(2, newIndexSelection.Count);
            Assert.AreEqual(0, newIndexSelection.First());
            Assert.AreEqual(1, newIndexSelection.Last());
            Assert.AreEqual(2, newIdSelection.Count);
            Assert.AreEqual(0, newIdSelection.First());
            Assert.AreEqual(1, newIdSelection.Last());
            Assert.IsTrue(button1.selected);
            Assert.IsTrue(button2.selected);
            Assert.IsFalse(button3.selected);
            Assert.AreEqual(2, actionGroup.selectedIndices.Count());
            Assert.AreEqual(0, actionGroup.selectedIndices.First());
            Assert.AreEqual(1, actionGroup.selectedIndices.Last());
            Assert.AreEqual(2, actionGroup.selectedIds.Count());
            Assert.AreEqual(0, actionGroup.selectedIds.First());
            Assert.AreEqual(1, actionGroup.selectedIds.Last());

            actionGroup.SetSelectionWithoutNotify(new List<int> {0, 1, 2});

            Assert.AreEqual(2, selectionChangedCount,
                "SetSelectionWithoutNotify should not trigger selectionChanged event.");
            Assert.AreEqual(2, newIndexSelection.Count,
                "SetSelectionWithoutNotify should not trigger selectionChanged event.");
            Assert.AreEqual(0, newIndexSelection.First());
            Assert.AreEqual(1, newIndexSelection.Last());
            Assert.AreEqual(2, newIdSelection.Count);
            Assert.AreEqual(0, newIdSelection.First());
            Assert.AreEqual(1, newIdSelection.Last());
            Assert.IsTrue(button1.selected);
            Assert.IsTrue(button2.selected);
            Assert.IsTrue(button3.selected);
            Assert.AreEqual(3, actionGroup.selectedIndices.Count());
            Assert.AreEqual(0, actionGroup.selectedIndices.First());
            Assert.AreEqual(1, actionGroup.selectedIndices.ElementAt(1));
            Assert.AreEqual(2, actionGroup.selectedIndices.Last());
            Assert.AreEqual(3, actionGroup.selectedIds.Count());
            Assert.AreEqual(0, actionGroup.selectedIds.First());
            Assert.AreEqual(1, actionGroup.selectedIds.ElementAt(1));
            Assert.AreEqual(2, actionGroup.selectedIds.Last());

            actionGroup.SetSelection(new List<int> {0, 1, 2});

            Assert.AreEqual(2, selectionChangedCount,
                "SetSelection should not trigger selectionChanged event when selection is unchanged.");
            Assert.AreEqual(2, newIndexSelection.Count,
                "SetSelection should not trigger selectionChanged event when selection is unchanged.");
            Assert.AreEqual(0, newIndexSelection.First());
            Assert.AreEqual(1, newIndexSelection.Last());
            Assert.AreEqual(2, newIdSelection.Count);
            Assert.AreEqual(0, newIdSelection.First());
            Assert.AreEqual(1, newIdSelection.Last());
            Assert.IsTrue(button1.selected);
            Assert.IsTrue(button2.selected);
            Assert.IsTrue(button3.selected);
            Assert.AreEqual(3, actionGroup.selectedIndices.Count());
            Assert.AreEqual(0, actionGroup.selectedIndices.First());
            Assert.AreEqual(1, actionGroup.selectedIndices.ElementAt(1));
            Assert.AreEqual(2, actionGroup.selectedIndices.Last());
            Assert.AreEqual(3, actionGroup.selectedIds.Count());
            Assert.AreEqual(0, actionGroup.selectedIds.First());
            Assert.AreEqual(1, actionGroup.selectedIds.ElementAt(1));
            Assert.AreEqual(2, actionGroup.selectedIds.Last());

            actionGroup.ClearSelection();

            Assert.AreEqual(3, selectionChangedCount);
            Assert.AreEqual(0, newIndexSelection.Count);
            Assert.AreEqual(0, newIdSelection.Count);
            Assert.IsFalse(button1.selected);
            Assert.IsFalse(button2.selected);
            Assert.IsFalse(button3.selected);
            Assert.AreEqual(0, actionGroup.selectedIndices.Count());
            Assert.AreEqual(0, actionGroup.selectedIds.Count());

            actionGroup.SetSelection(new List<int> {0, 1, 2});
            actionGroup.ClearSelectionWithoutNotify();

            Assert.AreEqual(4, selectionChangedCount);
            Assert.AreEqual(3, newIndexSelection.Count);
            Assert.AreEqual(3, newIdSelection.Count);
            Assert.IsFalse(button1.selected);
            Assert.IsFalse(button2.selected);
            Assert.IsFalse(button3.selected);
            Assert.AreEqual(0, actionGroup.selectedIndices.Count());
            Assert.AreEqual(0, actionGroup.selectedIds.Count());

            Object.Destroy(doc);
        }

        // [Test]
        // public void CanBindItems()
        // {
        //     var actionGroup = new ActionGroup();
        //
        //     Assert.AreEqual(0, actionGroup.childCount,
        //         "ActionGroup must be empty.");
        //
        //     var items = new List<string> { "hello", "world" };
        //
        //     actionGroup.sourceItems = items;
        //
        //     Assert.AreEqual(0, actionGroup.childCount,
        //         "ActionGroup should not create ActionButtons when bound to a sourceItems list without a bindItem callback.");
        //
        //     actionGroup.bindItem = (ActionButton el, int idx) => el.label = items[idx];
        //
        //     Assert.AreEqual(2, actionGroup.childCount,
        //         "ActionGroup should create ActionButtons when bound to a sourceItems list with a bindItem callback.");
        //
        //     actionGroup.unbindItem = (ActionButton el, int idx) => el.label = string.Empty;
        //
        //     Assert.AreEqual(2, actionGroup.childCount,
        //         "ActionGroup should not remove ActionButtons when unbindItem callback is set.");
        //
        //     Assert.Throws<InvalidOperationException>(() =>
        //     {
        //         actionGroup.Add(new ActionButton());
        //     }, "Cannot add items to an ActionGroup that is bound to a sourceItems list.");
        //
        //     actionGroup.sourceItems = null;
        //
        //     Assert.AreEqual(0, actionGroup.childCount,
        //         "ActionGroup should remove ActionButtons when sourceItems is set to null.");
        //
        //     Assert.DoesNotThrow(() =>
        //     {
        //         actionGroup.Add(new ActionButton());
        //     }, "Can add items to an ActionGroup that is not bound to a sourceItems list.");
        // }
    }
}
