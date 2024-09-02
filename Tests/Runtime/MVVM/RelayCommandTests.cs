using NUnit.Framework;
using Unity.AppUI.MVVM;

namespace Unity.AppUI.Tests.MVVM
{
    [TestFixture]
    [TestOf(typeof(RelayCommand))]
    class RelayCommandTests
    {
        [Test]
        public void CanExecute()
        {
            var command = new RelayCommand(() => { }, () => true);
            Assert.IsTrue(command.CanExecute());

            command = new RelayCommand(() => { });
            Assert.IsTrue(command.CanExecute());

            var canExecute = false;
            var executed = 0;

            command = new RelayCommand(() =>
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
            var command = new RelayCommand<int>(i => { }, i => true);
            Assert.IsTrue(command.CanExecute(1));

            command = new RelayCommand<int>(i => { });
            Assert.IsTrue(command.CanExecute(1));

            var canExecute = false;

            var executed = 0;
            command = new RelayCommand<int>(i =>
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
