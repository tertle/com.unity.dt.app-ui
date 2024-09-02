using System;
using System.Collections.Generic;
using NUnit.Framework;
using Unity.AppUI.Undo;

namespace Unity.AppUI.Tests.Undo
{
    [TestFixture]
    class UndoTests
    {
        [Test]
        public void CanUndoAndRedo()
        {
            var undoStack = new UndoStack();

            var indexEventInvoked = false;

            Assert.IsFalse(undoStack.canUndo);

            Assert.IsFalse(undoStack.canRedo);

            Assert.AreEqual(-1, undoStack.index);

            Assert.AreEqual(0, undoStack.count);

            undoStack.Push(new TestCommand("Test 1"));

            undoStack.indexChanged += (index) => indexEventInvoked = true;

            undoStack.Push(new TestCommand("Test 2"));

            Assert.IsTrue(indexEventInvoked);

            var command3 = new TestCommand("Test 3");

            undoStack.Push(command3);

            Assert.IsNull(undoStack[-1]);

            Assert.AreEqual(command3.name, undoStack[2].name);

            Assert.AreEqual(command3, undoStack.lastCommand);

            var commands = new List<UndoCommand>(undoStack.commands);

            Assert.AreEqual(command3, commands[2]);

            undoStack.Redo();

            undoStack.Undo();

            Assert.AreEqual(command3.name, undoStack[2].name);

            Assert.IsTrue(undoStack.canUndo);

            Assert.IsTrue(undoStack.canRedo);

            Assert.AreEqual(1, undoStack.index);

            Assert.AreEqual(3, undoStack.count);

            undoStack.Undo();

            Assert.IsTrue(undoStack.canUndo);

            Assert.IsTrue(undoStack.canRedo);

            Assert.AreEqual(0, undoStack.index);

            Assert.AreEqual(3, undoStack.count);

            undoStack.Undo();

            Assert.IsFalse(undoStack.canUndo);

            Assert.IsTrue(undoStack.canRedo);

            Assert.AreEqual(-1, undoStack.index);

            Assert.AreEqual(3, undoStack.count);

            undoStack.Redo();

            Assert.IsTrue(undoStack.canUndo);

            Assert.IsTrue(undoStack.canRedo);

            Assert.AreEqual(0, undoStack.index);

            Assert.AreEqual(3, undoStack.count);

            undoStack.Redo();

            Assert.IsTrue(undoStack.canUndo);

            Assert.IsTrue(undoStack.canRedo);

            Assert.AreEqual(1, undoStack.index);

            Assert.AreEqual(3, undoStack.count);

            undoStack.Redo();

            Assert.IsTrue(undoStack.canUndo);

            Assert.IsFalse(undoStack.canRedo);

            Assert.AreEqual(2, undoStack.index);

            Assert.AreEqual(3, undoStack.count);

            undoStack.Clear();

            Assert.IsFalse(undoStack.canUndo);

            Assert.IsFalse(undoStack.canRedo);

            Assert.AreEqual(-1, undoStack.index);

            Assert.AreEqual(0, undoStack.count);

            undoStack.Undo();

            Assert.IsFalse(undoStack.canUndo);
        }

        [Test]
        public void CanUndoAndRedoMacroCommand()
        {
            var undoStack = new UndoStack();

            undoStack.BeginMacro("Macro 1");

            undoStack.Push(new TestCommand("Test 1"));

            undoStack.Push(new TestCommand("Test 2"));

            undoStack.EndMacro();

            undoStack.BeginMacro("Macro 2");

            undoStack.Push(new TestCommand("Test 3"));

            var test4 = new TestCommand("Test 4");

            undoStack.Push(test4);

            undoStack.EndMacro();

            Assert.AreEqual(2, undoStack.count);

            Assert.AreEqual(1, undoStack.index);

            Assert.AreEqual("Macro 1", undoStack[0].name);

            Assert.AreEqual("Macro 2", undoStack[1].name);

            Assert.IsFalse(test4.hasUndone);

            Assert.IsFalse(test4.hasRedone);

            undoStack.Undo();

            Assert.AreEqual(2, undoStack.count);

            Assert.AreEqual(0, undoStack.index);

            Assert.AreEqual("Macro 1", undoStack[0].name);

            Assert.IsTrue(test4.hasUndone);

            Assert.IsFalse(test4.hasRedone);

            undoStack.Redo();

            Assert.AreEqual(2, undoStack.count);

            Assert.AreEqual(1, undoStack.index);

            Assert.AreEqual("Macro 2", undoStack[1].name);

            Assert.IsTrue(test4.hasRedone);
        }

        [Test]
        public void CanSetIndex()
        {
            var undoStack = new UndoStack();

            undoStack.Push(new TestCommand("Test 1"));

            undoStack.Push(new TestCommand("Test 2"));

            undoStack.Push(new TestCommand("Test 3"));

            Assert.AreEqual(3, undoStack.count);

            Assert.AreEqual(2, undoStack.index);

            Assert.AreEqual("Test 1", undoStack[0].name);

            Assert.AreEqual("Test 2", undoStack[1].name);

            Assert.AreEqual("Test 3", undoStack[2].name);

            undoStack.index = 1;

            Assert.AreEqual(3, undoStack.count);

            Assert.AreEqual(1, undoStack.index);

            Assert.AreEqual("Test 1", undoStack[0].name);

            Assert.AreEqual("Test 2", undoStack[1].name);

            Assert.AreEqual("Test 3", undoStack[2].name);

            undoStack.index = 0;

            Assert.AreEqual(3, undoStack.count);

            Assert.AreEqual(0, undoStack.index);

            Assert.AreEqual("Test 1", undoStack[0].name);

            Assert.AreEqual("Test 2", undoStack[1].name);

            Assert.AreEqual("Test 3", undoStack[2].name);

            undoStack.index = 2;

            Assert.AreEqual(3, undoStack.count);

            undoStack.index = 2;

            Assert.AreEqual(2, undoStack.index);

            Assert.AreEqual("Test 1", undoStack[0].name);

            Assert.AreEqual("Test 2", undoStack[1].name);

            Assert.AreEqual("Test 3", undoStack[2].name);

            Assert.Throws(typeof(ArgumentOutOfRangeException), () => undoStack.index = -1);

            undoStack.BeginMacro("Macro");

            Assert.Throws(typeof(InvalidOperationException), () => undoStack.index = 1);
        }

        [Test]
        public void CanSetLimit()
        {
            var undoStack = new UndoStack
            {
                undoLimit = 2
            };

            undoStack.Push(new TestCommand("Test 1"));

            undoStack.Push(new TestCommand("Test 2"));

            undoStack.Push(new TestCommand("Test 3"));

            Assert.AreEqual(2, undoStack.count);

            Assert.AreEqual(1, undoStack.index);

            Assert.AreEqual("Test 2", undoStack[0].name);

            Assert.AreEqual("Test 3", undoStack[1].name);

            undoStack.Undo();

            Assert.AreEqual(2, undoStack.count);

            Assert.AreEqual(0, undoStack.index);

            Assert.AreEqual("Test 2", undoStack[0].name);

            Assert.AreEqual("Test 3", undoStack[1].name);

            undoStack.Undo();

            Assert.AreEqual(2, undoStack.count);

            Assert.AreEqual(-1, undoStack.index);

            Assert.AreEqual("Test 2", undoStack[0].name);

            Assert.AreEqual("Test 3", undoStack[1].name);

            undoStack.Undo();

            Assert.AreEqual(2, undoStack.count);

            Assert.AreEqual(-1, undoStack.index);

            Assert.AreEqual("Test 2", undoStack[0].name);

            Assert.AreEqual("Test 3", undoStack[1].name);

            undoStack.Redo();

            Assert.AreEqual(2, undoStack.count);

            Assert.AreEqual(0, undoStack.index);

            Assert.AreEqual("Test 2", undoStack[0].name);

            Assert.AreEqual("Test 3", undoStack[1].name);

            undoStack.Push(new TestCommand("Test 4"));

            Assert.AreEqual(2, undoStack.count);

            Assert.AreEqual(1, undoStack.index);

            Assert.AreEqual("Test 2", undoStack[0].name);

            Assert.AreEqual("Test 4", undoStack[1].name);
        }

        [Test]
        public void CanSetCleanState()
        {
            var undoStack = new UndoStack();

            undoStack.Push(new TestCommand("Test 1"));

            undoStack.Push(new TestCommand("Test 2"));

            undoStack.Push(new TestCommand("Test 3"));

            var cleanEventInvoked = false;

            undoStack.cleanStateChanged += () =>
            {
                cleanEventInvoked = true;
            };

            Assert.AreEqual(-1, undoStack.cleanIndex);

            undoStack.SetClean();

            Assert.IsTrue(undoStack.isClean);

            undoStack.SetClean();

            Assert.AreEqual(2, undoStack.cleanIndex);

            Assert.IsTrue(cleanEventInvoked);

            undoStack.ResetClean();

            undoStack.SetClean();

            undoStack.Undo();

            Assert.IsFalse(undoStack.isClean);

            undoStack.Redo();

            Assert.IsTrue(undoStack.isClean);

            undoStack.Clear();

            Assert.IsTrue(undoStack.isClean);

            undoStack.Push(new TestCommand("Test 1"));

            undoStack.Push(new TestCommand("Test 2"));

            undoStack.Push(new TestCommand("Test 3"));

            undoStack.Undo();

            Assert.IsFalse(undoStack.isClean);

            undoStack.SetClean();

            Assert.IsTrue(undoStack.isClean);

            undoStack.Redo();

            Assert.IsFalse(undoStack.isClean);

            undoStack.Clear();

            Assert.IsTrue(undoStack.isClean);

            undoStack.ResetClean();

            undoStack.ResetClean();

            Assert.IsTrue(undoStack.isClean);
        }

        [Test]
        public void CanComposeMacro()
        {
            var undoStack = new UndoStack();

            undoStack.Push(new TestCommand("Test 1"));

            undoStack.Push(new TestCommand("Test 2"));

            undoStack.Push(new TestCommand("Test 3"));

            Assert.IsTrue(undoStack.canUndo);

            Assert.IsFalse(undoStack.canRedo);

            undoStack.BeginMacro("Macro");

            Assert.AreEqual("MacroCommand", new MacroCommand("MacroTest").id);

            Assert.AreEqual(3, undoStack.count);

            Assert.AreEqual(2, undoStack.index);

            Assert.IsFalse(undoStack.canUndo);

            Assert.IsFalse(undoStack.canRedo);

            undoStack.Push(new TestCommand("Test 4"));

            undoStack.Push(new TestCommand("Test 5"));

            undoStack.Push(new TestCommand("Test 6"));

            undoStack.EndMacro();

            Assert.AreEqual(4, undoStack.count);

            Assert.AreEqual(3, undoStack.index);

            Assert.AreEqual("Test 5", ((MacroCommand)undoStack.lastCommand)[1].name);

            undoStack.Undo();

            Assert.AreEqual(4, undoStack.count);

            Assert.AreEqual(2, undoStack.index);

            undoStack.Clear();
        }

        [Test]
        public void CanComposeNestedMacros()
        {
            var undoStack = new UndoStack();

            undoStack.Push(new TestCommand("Test 1"));

            undoStack.Push(new TestCommand("Test 2"));

            undoStack.Push(new TestCommand("Test 3"));

            Assert.IsTrue(undoStack.canUndo);

            Assert.IsFalse(undoStack.canRedo);

            undoStack.BeginMacro("Macro 1");

            Assert.AreEqual(3, undoStack.count);

            Assert.AreEqual(2, undoStack.index);

            Assert.IsFalse(undoStack.canUndo);

            Assert.IsFalse(undoStack.canRedo);

            undoStack.Push(new TestCommand("Test 4"));

            undoStack.Push(new TestCommand("Test 5"));

            undoStack.Push(new TestCommand("Test 6"));

            undoStack.BeginMacro("Macro 2");

            Assert.AreEqual(3, undoStack.count);

            Assert.AreEqual(2, undoStack.index);

            Assert.IsFalse(undoStack.canUndo);

            Assert.IsFalse(undoStack.canRedo);

            undoStack.Push(new TestCommand("Test 7"));

            undoStack.Push(new TestCommand("Test 8"));

            undoStack.Push(new TestCommand("Test 9"));

            undoStack.EndMacro();

            Assert.AreEqual(3, undoStack.count);

            Assert.AreEqual(2, undoStack.index);

            Assert.IsFalse(undoStack.canUndo);

            Assert.IsFalse(undoStack.canRedo);

            undoStack.EndMacro();

            Assert.AreEqual(4, undoStack.count);

            Assert.AreEqual(3, undoStack.index);

            Assert.IsTrue(undoStack.canUndo);

            Assert.IsFalse(undoStack.canRedo);

            undoStack.Undo();

            Assert.AreEqual(4, undoStack.count);

            Assert.AreEqual(2, undoStack.index);

            Assert.IsTrue(undoStack.canUndo);

            Assert.IsTrue(undoStack.canRedo);

            Assert.Throws<InvalidOperationException>(() => undoStack.EndMacro());

            undoStack.Redo();
        }

        [Test]
        public void CanMergeCommands()
        {
            var command1 = new TestCommand("Test 1");

            Assert.IsFalse(command1.hasMerged);

            Assert.IsFalse(command1.isObsolete);

            var command2 = new TestCommand("Test 2");

            Assert.IsFalse(command2.hasMerged);

            Assert.IsFalse(command2.isObsolete);

            var command3 = new TestCommand("Test 3");

            Assert.IsFalse(command3.hasMerged);

            Assert.IsFalse(command3.isObsolete);

            command1.MergeWith(command2);

            Assert.IsTrue(command1.hasMerged);

            Assert.IsTrue(command2.isObsolete);

            Assert.IsFalse(command3.hasMerged);

            Assert.IsFalse(command3.isObsolete);

            command1.MergeWith(command3);

            Assert.IsTrue(command1.hasMerged);

            Assert.IsTrue(command3.isObsolete);

            var invalidCommand = new MacroCommand("Invalid");

            Assert.IsFalse(command1.MergeWith(invalidCommand));
        }

        [Test]
        public void MemorySizeIsCorrect()
        {
            var undoStack = new UndoStack { undoLimit = 2 };

            undoStack.Push(new TestCommand("Test 1"));

            undoStack.Push(new TestCommand("Test 2"));

            undoStack.Push(new TestCommand("Test 3"));

            Assert.AreEqual(2, undoStack.memorySize);

            undoStack.Clear();

            Assert.AreEqual(0, undoStack.memorySize);
        }

        [Test]
        public void CantExceedMemorySizeLimit()
        {
            var undoStack = new UndoStack { undoLimit = 0 };

            Assert.Throws<InvalidOperationException>(() =>
            {
                undoStack.Push(new TestCommand("Test 1"));
            });
        }

        class TestCommand : UndoCommand
        {
            internal bool hasUndone;

            internal bool hasRedone;

            internal bool hasMerged;

            public TestCommand(string name)
                : base(name) { }

            public override string id => nameof(TestCommand);

            public override ulong memorySize => 1;

            public override void Undo()
            {
                hasUndone = true;
            }

            public override void Redo()
            {
                hasRedone = true;
            }

            public override void OnFlush()
            {
                hasUndone = false;
                hasRedone = false;
                hasMerged = false;
            }

            public override bool MergeWith(UndoCommand command)
            {
                if (id == command.id)
                {
                    hasMerged = true;
                    command.isObsolete = true;
                    return true;
                }
                return base.MergeWith(command);
            }
        }
    }
}
