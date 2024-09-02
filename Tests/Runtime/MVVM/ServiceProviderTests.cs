using NUnit.Framework;
using Unity.AppUI.MVVM;

namespace Unity.AppUI.Tests.MVVM
{
    [TestFixture]
    [TestOf(typeof(ServiceProvider))]
    class ServiceProviderTests
    {
        [Test]
        public void ServiceProvider_ShouldCreateServiceProvider()
        {
            var services = new ServiceCollection();
            var serviceProvider = new ServiceProvider(services);
            Assert.IsNotNull(serviceProvider);
        }

        [Test]
        public void ServiceProvider_WithNullCollection_ShouldThrow()
        {
            Assert.Throws<System.ArgumentNullException>(() => new ServiceProvider(null));
        }

        [Test]
        public void ServiceProvider_ShouldDispose()
        {
            var services = new ServiceCollection();
            var serviceProvider = new ServiceProvider(services);
            serviceProvider.Dispose();
            Assert.IsTrue(serviceProvider.disposed);
            Assert.Throws<System.ObjectDisposedException>(() => serviceProvider.GetService<object>());
        }

        [Test]
        public void ServiceProvider_WithValidService_ShouldRealizeService()
        {
            var services = new ServiceCollection();
            services.AddSingleton<ServiceWithValidConstructor>();
            services.AddSingleton<ServiceWithValidConstructor2>();
            var serviceProvider = new ServiceProvider(services);
            var service = serviceProvider.GetService<ServiceWithValidConstructor>();
            Assert.IsNotNull(service);

            var service2 = serviceProvider.GetService<ServiceWithValidConstructor2>();
            Assert.IsNotNull(service2);
            Assert.IsNotNull(service2.dep);
        }

        [Test]
        public void ServiceProvider_ThatNotContainServiceDesc_ShouldThrow()
        {
            var services = new ServiceCollection();
            var serviceProvider = new ServiceProvider(services);
            Assert.Throws<System.InvalidOperationException>(() => serviceProvider.GetService<ServiceWithValidConstructor>());
        }

        [Test]
        public void ServiceProvider_WithInvalidService_ShouldThrow()
        {
            var services = new ServiceCollection();
            services.AddSingleton<ServiceWithInvalidConstructor>();
            var serviceProvider = new ServiceProvider(services);
            Assert.Throws<System.InvalidOperationException>(() => serviceProvider.GetService<ServiceWithInvalidConstructor>());
        }

        [Test]
        public void ServiceProvider_WithServiceUsingAttributes_ShouldRealizeService()
        {
            var services = new ServiceCollection();
            services.AddSingleton<ServiceWithValidConstructor>();
            services.AddSingleton<ServiceWithValidConstructor2>();
            services.AddSingleton<ServiceWithAttributes>();
            var serviceProvider = new ServiceProvider(services);
            var service = serviceProvider.GetService<ServiceWithAttributes>();
            Assert.IsNotNull(service);
            Assert.IsNotNull(service.dependency1);
            Assert.IsNotNull(service.dependency2);
            Assert.IsNotNull(service.dependency2.dep);
            Assert.AreEqual(1, service.onDependenciesInjectedCalled);
        }

        public class ServiceWithValidConstructor
        {
            public ServiceWithValidConstructor() { }
        }

        public class ServiceWithValidConstructor2
        {
            public ServiceWithValidConstructor dep { get; }

            public ServiceWithValidConstructor2(ServiceWithValidConstructor s)
            {
                dep = s;
            }
        }

        public class ServiceWithInvalidConstructor
        {
            public ServiceWithInvalidConstructor(int i) { }
        }

        public class ServiceWithAttributes : IDependencyInjectionListener
        {
            [Service]
            public ServiceWithValidConstructor dependency1;

            [Service]
            public ServiceWithValidConstructor2 dependency2 { get; private set; }

            public int onDependenciesInjectedCalled { get; private set; }

            public void OnDependenciesInjected()
            {
                onDependenciesInjectedCalled++;
            }
        }
    }
}
