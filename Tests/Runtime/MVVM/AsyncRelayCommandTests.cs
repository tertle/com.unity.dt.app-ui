using NUnit.Framework;
using Unity.AppUI.MVVM;

#pragma warning disable 1998

namespace Unity.AppUI.Tests.MVVM
{
    [TestFixture]
    [TestOf(typeof(AsyncRelayCommand))]
    class AsyncRelayCommandTests
    {
        [Test]
        public void CanExecute()
        {
            var command = new AsyncRelayCommand(async () => { }, () => true);
            Assert.IsTrue(command.CanExecute());

            command = new AsyncRelayCommand(async () => { });
            Assert.IsTrue(command.CanExecute());

            var canExecute = false;
            var executed = 0;

            command = new AsyncRelayCommand(async () =>
            {
                executed++;
            }, () => canExecute);
            Assert.IsFalse(command.CanExecute());
            Assert.AreEqual(0, executed);
            command.Execute();
            Assert.AreEqual(0, executed);

            var changed = 0;
            command.CanExecuteChanged += (sender, args) =>
            {
                changed++;
            };

            canExecute = true;
            command.NotifyCanExecuteChanged();

            Assert.AreEqual(1, changed);
            Assert.IsTrue(command.CanExecute());

            command.Execute();
            Assert.AreEqual(1, executed);
        }

        [Test]
        public void CanExecuteWithType()
        {
            var command = new AsyncRelayCommand<int>(async i => { }, i => true);
            Assert.IsTrue(command.CanExecute(1));

            command = new AsyncRelayCommand<int>(async i => { });
            Assert.IsTrue(command.CanExecute(1));

            var canExecute = false;

            var executed = 0;
            command = new AsyncRelayCommand<int>(async i =>
            {
                executed++;
            }, i => canExecute);
            Assert.IsFalse(command.CanExecute(1));
            Assert.AreEqual(0, executed);
            command.Execute(1);
            Assert.AreEqual(0, executed);

            var changed = 0;
            command.CanExecuteChanged += (sender, args) =>
            {
                changed++;
            };

            canExecute = true;
            command.NotifyCanExecuteChanged();

            Assert.AreEqual(1, changed);
            Assert.IsTrue(command.CanExecute((object)1));

            command.Execute((object)1);
            Assert.AreEqual(1, executed);
        }
    }
}

#pragma warning restore 1998
