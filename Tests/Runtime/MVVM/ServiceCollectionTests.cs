using System.Linq;
using NUnit.Framework;
using Unity.AppUI.MVVM;

namespace Unity.AppUI.Tests.MVVM
{
    [TestFixture]
    [TestOf(typeof(ServiceCollection))]
    class ServiceCollectionTests
    {
        interface IServiceTest { }
        class ServiceTest : IServiceTest { }

        [Test]
        public void ServiceCollection_ShouldContainServices()
        {
            var services = new ServiceCollection();

            var serviceDescriptor =
                new ServiceDescriptor(
                    typeof(IServiceTest),
                    typeof(ServiceTest),
                    ServiceLifetime.Singleton);

            var serviceDescriptor2 =
                new ServiceDescriptor(
                    typeof(IServiceTest),
                    typeof(ServiceTest),
                    ServiceLifetime.Transient);

            services.Add(serviceDescriptor2);
            services.Insert(0, serviceDescriptor);

            Assert.AreEqual(2, services.Count);

            Assert.AreEqual(serviceDescriptor, services[0]);
            Assert.AreEqual(serviceDescriptor2, services[1]);

            Assert.AreEqual(0, services.IndexOf(serviceDescriptor));
            Assert.AreEqual(1, services.IndexOf(serviceDescriptor2));

            Assert.IsFalse(services.IsReadOnly);

            var array = new ServiceDescriptor[2];
            Assert.DoesNotThrow(() => services.CopyTo(array, 0));

            foreach (var desc in services)
            {
                Assert.IsTrue(array.Contains(desc));
            }

            services.Remove(serviceDescriptor);
            Assert.AreEqual(1, services.Count);

            services.RemoveAt(0);
            Assert.AreEqual(0, services.Count);

            services.Clear();
            Assert.AreEqual(0, services.Count);
        }
    }
}
