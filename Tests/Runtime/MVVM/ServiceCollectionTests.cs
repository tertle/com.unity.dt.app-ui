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
        class AlternateServiceTest : IServiceTest { }

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

        [Test]
        public void ServiceDescriptor_When_ShouldCreateConditionalDescriptor()
        {
            var originalDescriptor = ServiceDescriptor.Singleton(typeof(IServiceTest), typeof(ServiceTest));
            Assert.IsNull(originalDescriptor.condition);

            var conditionalDescriptor = originalDescriptor.When(ctx => ctx.RequestingType == typeof(ServiceTest));

            Assert.IsNotNull(conditionalDescriptor.condition);
            Assert.AreEqual(originalDescriptor.serviceType, conditionalDescriptor.serviceType);
            Assert.AreEqual(originalDescriptor.implementationType, conditionalDescriptor.implementationType);
            Assert.AreEqual(originalDescriptor.lifetime, conditionalDescriptor.lifetime);
        }

        [Test]
        public void ServiceCollection_AddSingletonWhen_ShouldAddConditionalService()
        {
            var services = new ServiceCollection();

            services.AddSingletonWhen(typeof(IServiceTest), typeof(ServiceTest),
                ctx => ctx.RequestingType == typeof(ServiceTest));

            Assert.AreEqual(1, services.Count);
            var descriptor = services[0];

            Assert.AreEqual(typeof(IServiceTest), descriptor.serviceType);
            Assert.AreEqual(typeof(ServiceTest), descriptor.implementationType);
            Assert.AreEqual(ServiceLifetime.Singleton, descriptor.lifetime);
            Assert.IsNotNull(descriptor.condition);
        }

        [Test]
        public void ServiceCollection_AddScopedWhen_ShouldAddConditionalService()
        {
            var services = new ServiceCollection();

            services.AddScopedWhen(typeof(IServiceTest), typeof(ServiceTest),
                ctx => ctx.IsScoped);

            Assert.AreEqual(1, services.Count);
            var descriptor = services[0];

            Assert.AreEqual(ServiceLifetime.Scoped, descriptor.lifetime);
            Assert.IsNotNull(descriptor.condition);
        }

        [Test]
        public void ServiceCollection_AddTransientWhen_ShouldAddConditionalService()
        {
            var services = new ServiceCollection();

            services.AddTransientWhen(typeof(IServiceTest), typeof(ServiceTest),
                ctx => ctx.ServiceType == typeof(IServiceTest));

            Assert.AreEqual(1, services.Count);
            var descriptor = services[0];

            Assert.AreEqual(ServiceLifetime.Transient, descriptor.lifetime);
            Assert.IsNotNull(descriptor.condition);
        }

        [Test]
        public void ServiceDescriptor_ExtensionWhen_ShouldCreateConditionalDescriptor()
        {
            var services = new ServiceCollection();
            var descriptor = ServiceDescriptor.Singleton(typeof(IServiceTest), typeof(ServiceTest));

            var conditionalDescriptor = descriptor.When(ctx => ctx.RequestingType != null);
            services.Add(conditionalDescriptor);

            Assert.AreEqual(1, services.Count);
            Assert.IsNotNull(services[0].condition);
        }
    }
}
