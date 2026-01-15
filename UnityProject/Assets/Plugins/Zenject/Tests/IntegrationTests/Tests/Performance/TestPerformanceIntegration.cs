using System.Collections;
using ModestTree;
using UnityEngine.TestTools;
using Zenject.Tests.Performance;

namespace Zenject.Tests.Performance
{
    public class TestPerformanceIntegration : ZenjectIntegrationTestFixture
    {
        class SimpleService
        {
            public int Value = 42;
        }

        class DependentService
        {
            public SimpleService Simple { get; private set; }

            public DependentService(SimpleService simple)
            {
                Simple = simple;
            }
        }

        class ComplexService
        {
            public SimpleService Simple { get; private set; }
            public DependentService Dependent { get; private set; }

            public ComplexService(SimpleService simple, DependentService dependent)
            {
                Simple = simple;
                Dependent = dependent;
            }
        }

        [UnityTest]
        public IEnumerator TestResolveAfterOptimization()
        {
            PreInstall();

            Container.Bind<SimpleService>().AsSingle();
            Container.Bind<DependentService>().AsSingle();
            Container.Bind<ComplexService>().AsSingle();

            PostInstall();

            // Resolve multiple times to test cache and optimized paths
            for (int i = 0; i < 10; i++)
            {
                var simple = Container.Resolve<SimpleService>();
                Assert.IsNotNull(simple);
                Assert.IsEqual(simple.Value, 42);

                var dependent = Container.Resolve<DependentService>();
                Assert.IsNotNull(dependent);
                Assert.IsNotNull(dependent.Simple);

                var complex = Container.Resolve<ComplexService>();
                Assert.IsNotNull(complex);
                Assert.IsNotNull(complex.Simple);
                Assert.IsNotNull(complex.Dependent);

                // Verify references are correct for singletons
                Assert.That(ReferenceEquals(simple, dependent.Simple));
                Assert.That(ReferenceEquals(simple, complex.Simple));
                Assert.That(ReferenceEquals(dependent, complex.Dependent));
            }

            yield break;
        }

        [UnityTest]
        public IEnumerator TestResolveAllAfterOptimization()
        {
            PreInstall();

            Container.Bind<SimpleService>().AsTransient().WithId("first");
            Container.Bind<SimpleService>().AsTransient().WithId("second");
            Container.Bind<SimpleService>().AsTransient().WithId("third");

            PostInstall();

            var instances = Container.ResolveAll<SimpleService>();
            
            Assert.IsEqual(instances.Count, 3);
            for (int i = 0; i < instances.Count; i++)
            {
                Assert.IsNotNull(instances[i]);
                Assert.IsEqual(instances[i].Value, 42);
            }

            yield break;
        }

        [UnityTest]
        public IEnumerator TestTransientResolvesAfterOptimization()
        {
            PreInstall();

            Container.Bind<SimpleService>().AsTransient();

            PostInstall();

            var instances = new System.Collections.Generic.List<SimpleService>();
            
            for (int i = 0; i < 10; i++)
            {
                var instance = Container.Resolve<SimpleService>();
                Assert.IsNotNull(instance);
                instances.Add(instance);
            }

            // Verify all instances are unique (transient)
            for (int i = 0; i < instances.Count; i++)
            {
                for (int j = i + 1; j < instances.Count; j++)
                {
                    Assert.That(!ReferenceEquals(instances[i], instances[j]),
                        "Transient instances should be unique");
                }
            }

            yield break;
        }

        [UnityTest]
        public IEnumerator TestResolveTypeAllAfterOptimization()
        {
            PreInstall();

            Container.Bind<SimpleService>().AsTransient().WithId("first");
            Container.Bind<SimpleService>().AsTransient().WithId("second");

            PostInstall();

            using (var context = ZenPools.SpawnInjectContext(Container, typeof(SimpleService)))
            {
                var types = Container.ResolveTypeAll(context);
                
                Assert.That(types.Count > 0);
                foreach (var type in types)
                {
                    Assert.IsNotNull(type);
                    Assert.IsEqual(type, typeof(SimpleService));
                }
            }

            yield break;
        }
    }
}
