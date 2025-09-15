using NUnit.Framework;
using Unity.AppUI.MVVM;

namespace Unity.AppUI.Tests.MVVM
{
    [TestFixture]
    [TestOf(typeof(ObservablePropertyAttribute))]
    class ObservablePropertyTests
    {
        [Test]
        public void ObservablePropertyAttribute_CanBeInstantiated()
        {
            var attribute = new ObservablePropertyAttribute();
            Assert.IsNotNull(attribute);
        }

        [Test]
        public void ObservableTestObject_HasProperties()
        {
            var obj = new ObservableTestObject();
            Assert.IsNotNull(obj);
            Assert.AreEqual(0, obj.Count);
            Assert.AreEqual(null, obj.Name);
            Assert.AreEqual(0, obj.Value);
        }

        [Test]
        public void ObservableTestObject_CanSetProperties()
        {
            var obj = new ObservableTestObject();
            obj.Count = 1;
            Assert.AreEqual(1, obj.Count);
            obj.Name = "Test";
            Assert.AreEqual("Test", obj.Name);
            obj.Value = 1.0f;
            Assert.AreEqual(1.0f, obj.Value);
        }
    }

    partial class ObservableTestObject : ObservableObject
    {
        [ObservableProperty]
        [AlsoNotifyChangeFor(nameof(Total))]
        int count;

        [ObservableProperty]
        string mName;

        [ObservableProperty]
        float m_Value;

        public int Total => Count + 1;
    }
}
