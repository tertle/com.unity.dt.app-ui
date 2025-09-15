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

        [Test]
        public void ServiceProvider_ShouldCreateScope()
        {
            var services = new ServiceCollection();
            services.AddSingleton<ServiceWithValidConstructor>();
            services.AddSingleton<ServiceWithValidConstructor2>();
            services.AddScoped<ServiceWithAttributes>();
            var serviceProvider = services.BuildServiceProvider();

            var service1 = serviceProvider.GetService<ServiceWithValidConstructor>();
            Assert.IsNotNull(service1);
            var service2 = serviceProvider.GetService<ServiceWithValidConstructor2>();
            Assert.IsNotNull(service2);

            var service3 = serviceProvider.GetService<ServiceWithAttributes>();
            Assert.IsNotNull(service3);

            using (var scope = serviceProvider.CreateScope())
            {
                var scoped1 = scope.ServiceProvider.GetService<ServiceWithValidConstructor>();
                Assert.IsNotNull(scoped1);
                Assert.AreSame(service1, scoped1);

                var scoped2 = scope.ServiceProvider.GetService<ServiceWithValidConstructor2>();
                Assert.IsNotNull(scoped2);
                Assert.AreSame(service2, scoped2);

                var scoped3 = scope.ServiceProvider.GetService<ServiceWithAttributes>();
                Assert.IsNotNull(scoped3);
                Assert.IsNotNull(scoped3.dependency1);
                Assert.IsNotNull(scoped3.dependency2);
                Assert.IsNotNull(scoped3.dependency2.dep);
                Assert.AreEqual(1, scoped3.onDependenciesInjectedCalled);

                Assert.AreNotSame(service3, scoped3);
            }
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

        [Test]
        public void ServiceProvider_ConditionalResolution_ShouldResolveBasedOnContext()
        {
            var services = new ServiceCollection();

            // Add conditional services for ITestInterface
            services.AddSingletonWhen(typeof(ITestInterface), typeof(TestImplementation1),
                ctx => ctx.RequestingType == typeof(ServiceConsumer1));

            services.AddSingletonWhen(typeof(ITestInterface), typeof(TestImplementation2),
                ctx => ctx.RequestingType == typeof(ServiceConsumer2));

            services.AddSingleton<ServiceConsumer1>();
            services.AddSingleton<ServiceConsumer2>();

            var provider = services.BuildServiceProvider();

            var consumer1 = provider.GetService<ServiceConsumer1>();
            var consumer2 = provider.GetService<ServiceConsumer2>();

            Assert.IsNotNull(consumer1);
            Assert.IsNotNull(consumer2);
            Assert.IsNotNull(consumer1.dependency);
            Assert.IsNotNull(consumer2.dependency);

            Assert.IsInstanceOf<TestImplementation1>(consumer1.dependency);
            Assert.IsInstanceOf<TestImplementation2>(consumer2.dependency);
        }

        [Test]
        public void ServiceProvider_ConditionalResolution_WithNoMatchingCondition_ShouldThrow()
        {
            var services = new ServiceCollection();

            services.AddSingletonWhen(typeof(ITestInterface), typeof(TestImplementation1),
                ctx => ctx.RequestingType == typeof(ServiceConsumer1));

            services.AddSingleton<ServiceConsumer2>();

            var provider = services.BuildServiceProvider();

            Assert.Throws<System.InvalidOperationException>(() => provider.GetService<ServiceConsumer2>());
        }

        [Test]
        public void ServiceProvider_ConditionalResolution_WithScopeContext_ShouldWork()
        {
            var services = new ServiceCollection();

            services.AddSingletonWhen(typeof(ITestInterface), typeof(TestImplementation1),
                ctx => !ctx.IsScoped);

            services.AddScopedWhen(typeof(ITestInterface), typeof(TestImplementation2),
                ctx => ctx.IsScoped);

            services.AddSingleton<ServiceConsumer1>();
            services.AddScoped<ServiceConsumer2>();

            var provider = services.BuildServiceProvider();

            var rootConsumer = provider.GetService<ServiceConsumer1>();
            Assert.IsInstanceOf<TestImplementation1>(rootConsumer.dependency);

            using var scope = provider.CreateScope();
            var scopedConsumer = scope.ServiceProvider.GetService<ServiceConsumer2>();
            Assert.IsInstanceOf<TestImplementation2>(scopedConsumer.dependency);
        }

        [Test]
        public void ServiceProvider_ConditionalResolution_WithAttributeInjection_ShouldWork()
        {
            var services = new ServiceCollection();

            services.AddSingletonWhen(typeof(ITestInterface), typeof(TestImplementation1),
                ctx => ctx.RequestingType == typeof(ServiceWithConditionalAttributes));

            services.AddSingleton<ServiceWithConditionalAttributes>();

            var provider = services.BuildServiceProvider();
            var service = provider.GetService<ServiceWithConditionalAttributes>();

            Assert.IsNotNull(service);
            Assert.IsNotNull(service.dependency);
            Assert.IsInstanceOf<TestImplementation1>(service.dependency);
        }

        [Test]
        public void ServiceProvider_ConditionalResolution_MultipleConditions_ShouldMatchFirst()
        {
            var services = new ServiceCollection();

            // First condition should match
            services.AddSingletonWhen(typeof(ITestInterface), typeof(TestImplementation1),
                ctx => ctx.ServiceType == typeof(ITestInterface));

            // Second condition would also match but shouldn't be reached
            services.AddSingletonWhen(typeof(ITestInterface), typeof(TestImplementation2),
                ctx => ctx.ServiceType == typeof(ITestInterface));

            services.AddSingleton<ServiceConsumer1>();

            var provider = services.BuildServiceProvider();
            var consumer = provider.GetService<ServiceConsumer1>();

            Assert.IsInstanceOf<TestImplementation1>(consumer.dependency);
        }

        // Test interfaces and implementations for conditional resolution
        public interface ITestInterface { }

        public class TestImplementation1 : ITestInterface { }

        public class TestImplementation2 : ITestInterface { }

        public class ServiceConsumer1
        {
            public ITestInterface dependency { get; }

            public ServiceConsumer1(ITestInterface dep)
            {
                dependency = dep;
            }
        }

        public class ServiceConsumer2
        {
            public ITestInterface dependency { get; }

            public ServiceConsumer2(ITestInterface dep)
            {
                dependency = dep;
            }
        }

        public class ServiceWithConditionalAttributes
        {
            [Service]
            public ITestInterface dependency;
        }
    }
}
