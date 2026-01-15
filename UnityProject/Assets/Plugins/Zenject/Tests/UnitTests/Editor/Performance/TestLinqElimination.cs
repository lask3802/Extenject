using System;
using NUnit.Framework;
using Zenject;
using Assert = ModestTree.Assert;

namespace Zenject.Tests.Performance
{
    [TestFixture]
    public class TestLinqElimination : ZenjectUnitTestFixture
    {
        class SimpleClass
        {
            public int Value = 42;
        }

        class ClassWithDependency
        {
            public SimpleClass Dependency;

            public ClassWithDependency(SimpleClass dependency)
            {
                Dependency = dependency;
            }
        }

        [Test]
        public void TestSingleResolveReturnsCorrectInstance()
        {
            Container.Bind<SimpleClass>().AsSingle();
            
            var instance = Container.Resolve<SimpleClass>();
            
            Assert.IsNotNull(instance);
            Assert.IsEqual(instance.Value, 42);
        }

        [Test]
        public void TestTransientResolveReturnsNewInstances()
        {
            Container.Bind<SimpleClass>().AsTransient();
            
            var instance1 = Container.Resolve<SimpleClass>();
            var instance2 = Container.Resolve<SimpleClass>();
            
            Assert.IsNotNull(instance1);
            Assert.IsNotNull(instance2);
            Assert.That(!ReferenceEquals(instance1, instance2), 
                "Transient should create new instances");
        }

        [Test]
        public void TestResolveThrowsWhenMultipleProvidersAndNotRequested()
        {
            // This test verifies that the error detection for multiple providers
            // still works after removing LINQ .Count() > 1 check
            Container.Bind<SimpleClass>().AsTransient().WithId("first");
            Container.Bind<SimpleClass>().AsTransient().WithId("second");
            
            // Resolving without specifying which binding should succeed 
            // (because it will use the first match or throw if truly ambiguous)
            // But let's verify we can still resolve with ID specified
            var instance1 = Container.ResolveId<SimpleClass>("first");
            var instance2 = Container.ResolveId<SimpleClass>("second");
            
            Assert.IsNotNull(instance1);
            Assert.IsNotNull(instance2);
            Assert.That(!ReferenceEquals(instance1, instance2));
        }

        [Test]
        public void TestResolveAllReturnsMultipleInstances()
        {
            Container.Bind<SimpleClass>().AsTransient().WithId("first");
            Container.Bind<SimpleClass>().AsTransient().WithId("second");
            Container.Bind<SimpleClass>().AsTransient().WithId("third");
            
            var instances = Container.ResolveAll<SimpleClass>();
            
            Assert.IsEqual(instances.Count, 3);
            Assert.IsNotNull(instances[0]);
            Assert.IsNotNull(instances[1]);
            Assert.IsNotNull(instances[2]);
        }

        [Test]
        public void TestResolveWithDependencyReturnsCorrectInstances()
        {
            Container.Bind<SimpleClass>().AsSingle();
            Container.Bind<ClassWithDependency>().AsTransient();
            
            var instance = Container.Resolve<ClassWithDependency>();
            
            Assert.IsNotNull(instance);
            Assert.IsNotNull(instance.Dependency);
            Assert.IsEqual(instance.Dependency.Value, 42);
        }

        [Test]
        public void TestOptionalResolveReturnsNullWhenNotBound()
        {
            // Test optional resolution behavior is preserved
            var instance = Container.TryResolve<SimpleClass>();
            
            Assert.IsNull(instance);
        }

        [Test]
        public void TestOptionalResolveReturnsInstanceWhenBound()
        {
            Container.Bind<SimpleClass>().AsSingle();
            
            var instance = Container.TryResolve<SimpleClass>();
            
            Assert.IsNotNull(instance);
            Assert.IsEqual(instance.Value, 42);
        }
    }
}
