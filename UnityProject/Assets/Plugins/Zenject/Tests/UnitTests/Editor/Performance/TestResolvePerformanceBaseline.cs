using System;
using System.Diagnostics;
using NUnit.Framework;
using Zenject;
using Assert = ModestTree.Assert;

namespace Zenject.Tests.Performance
{
    [TestFixture]
    public class TestResolvePerformanceBaseline : ZenjectUnitTestFixture
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

        class ClassWithMultipleDependencies
        {
            public SimpleClass Dep1;
            public ClassWithDependency Dep2;

            public ClassWithMultipleDependencies(SimpleClass dep1, ClassWithDependency dep2)
            {
                Dep1 = dep1;
                Dep2 = dep2;
            }
        }

        [Test]
        public void TestSingleResolveBaseline()
        {
            Container.Bind<SimpleClass>().AsSingle();
            
            // Warmup - first resolve may trigger metadata caching
            var warmup = Container.Resolve<SimpleClass>();
            Assert.IsNotNull(warmup);
            
            // Baseline measurement
            var sw = Stopwatch.StartNew();
            var iterations = 1000;
            
            for (int i = 0; i < iterations; i++)
            {
                var instance = Container.Resolve<SimpleClass>();
                Assert.IsNotNull(instance);
            }
            
            sw.Stop();
            
            var avgMs = sw.Elapsed.TotalMilliseconds / iterations;
            UnityEngine.Debug.Log($"[Baseline] Single resolve average: {avgMs:F4}ms per resolve");
            
            // Sanity check - should complete reasonably fast
            Assert.That(sw.Elapsed.TotalSeconds < 5.0, 
                "1000 simple resolves should complete in under 5 seconds");
        }

        [Test]
        public void TestTransientResolveBaseline()
        {
            Container.Bind<SimpleClass>().AsTransient();
            
            // Warmup
            var warmup = Container.Resolve<SimpleClass>();
            Assert.IsNotNull(warmup);
            
            // Count allocations by resolving multiple times
            var sw = Stopwatch.StartNew();
            var iterations = 1000;
            
            for (int i = 0; i < iterations; i++)
            {
                var instance = Container.Resolve<SimpleClass>();
                Assert.IsNotNull(instance);
            }
            
            sw.Stop();
            
            var avgMs = sw.Elapsed.TotalMilliseconds / iterations;
            UnityEngine.Debug.Log($"[Baseline] Transient resolve average: {avgMs:F4}ms per resolve");
            
            Assert.That(sw.Elapsed.TotalSeconds < 5.0);
        }

        [Test]
        public void TestResolveWithDependenciesBaseline()
        {
            Container.Bind<SimpleClass>().AsSingle();
            Container.Bind<ClassWithDependency>().AsTransient();
            
            // Warmup
            var warmup = Container.Resolve<ClassWithDependency>();
            Assert.IsNotNull(warmup);
            Assert.IsNotNull(warmup.Dependency);
            
            // Baseline
            var sw = Stopwatch.StartNew();
            var iterations = 1000;
            
            for (int i = 0; i < iterations; i++)
            {
                var instance = Container.Resolve<ClassWithDependency>();
                Assert.IsNotNull(instance);
                Assert.IsNotNull(instance.Dependency);
            }
            
            sw.Stop();
            
            var avgMs = sw.Elapsed.TotalMilliseconds / iterations;
            UnityEngine.Debug.Log($"[Baseline] Resolve with dependency average: {avgMs:F4}ms per resolve");
            
            Assert.That(sw.Elapsed.TotalSeconds < 5.0);
        }

        [Test]
        public void TestResolveWithMultipleDependenciesBaseline()
        {
            Container.Bind<SimpleClass>().AsSingle();
            Container.Bind<ClassWithDependency>().AsSingle();
            Container.Bind<ClassWithMultipleDependencies>().AsTransient();
            
            // Warmup
            var warmup = Container.Resolve<ClassWithMultipleDependencies>();
            Assert.IsNotNull(warmup);
            
            // Baseline
            var sw = Stopwatch.StartNew();
            var iterations = 1000;
            
            for (int i = 0; i < iterations; i++)
            {
                var instance = Container.Resolve<ClassWithMultipleDependencies>();
                Assert.IsNotNull(instance);
                Assert.IsNotNull(instance.Dep1);
                Assert.IsNotNull(instance.Dep2);
            }
            
            sw.Stop();
            
            var avgMs = sw.Elapsed.TotalMilliseconds / iterations;
            UnityEngine.Debug.Log($"[Baseline] Resolve with multiple dependencies average: {avgMs:F4}ms per resolve");
            
            Assert.That(sw.Elapsed.TotalSeconds < 5.0);
        }

        [Test]
        public void TestResolveAllBaseline()
        {
            Container.Bind<SimpleClass>().AsTransient().WithId("first");
            Container.Bind<SimpleClass>().AsTransient().WithId("second");
            Container.Bind<SimpleClass>().AsTransient().WithId("third");
            
            // Warmup
            var warmup = Container.ResolveAll<SimpleClass>();
            Assert.IsEqual(warmup.Count, 3);
            
            // Baseline
            var sw = Stopwatch.StartNew();
            var iterations = 1000;
            
            for (int i = 0; i < iterations; i++)
            {
                var instances = Container.ResolveAll<SimpleClass>();
                Assert.IsEqual(instances.Count, 3);
            }
            
            sw.Stop();
            
            var avgMs = sw.Elapsed.TotalMilliseconds / iterations;
            UnityEngine.Debug.Log($"[Baseline] ResolveAll (3 instances) average: {avgMs:F4}ms per resolve");
            
            Assert.That(sw.Elapsed.TotalSeconds < 5.0);
        }

        [Test]
        public void TestFirstResolveIncludesMetadataCaching()
        {
            // Define a new type that hasn't been used yet
            class UniqueClass
            {
                public int Value = 123;
            }
            
            Container.Bind<UniqueClass>().AsSingle();
            
            // First resolve includes metadata analysis
            var sw1 = Stopwatch.StartNew();
            var first = Container.Resolve<UniqueClass>();
            sw1.Stop();
            
            Assert.IsNotNull(first);
            
            // Second resolve uses cached metadata
            var sw2 = Stopwatch.StartNew();
            var second = Container.Resolve<UniqueClass>();
            sw2.Stop();
            
            Assert.IsNotNull(second);
            Assert.That(ReferenceEquals(first, second), "AsSingle should return same instance");
            
            UnityEngine.Debug.Log($"[Baseline] First resolve (with caching): {sw1.Elapsed.TotalMilliseconds:F4}ms");
            UnityEngine.Debug.Log($"[Baseline] Second resolve (cached): {sw2.Elapsed.TotalMilliseconds:F4}ms");
            
            // Second resolve should generally be faster or similar
            // (Note: This is a soft assertion for informational purposes)
        }
    }
}
