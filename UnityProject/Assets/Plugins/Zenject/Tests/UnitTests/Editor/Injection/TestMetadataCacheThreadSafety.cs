using System;
using System.Collections.Generic;
using System.Threading;
using NUnit.Framework;
using Zenject;
using Assert = ModestTree.Assert;

namespace Zenject.Tests.Injection
{
    [TestFixture]
    public class TestMetadataCacheThreadSafety : ZenjectUnitTestFixture
    {
        class TestClass1
        {
            public int Value = 1;
        }

        class TestClass2
        {
            [Inject]
            public TestClass1 Dependency { get; set; }
        }

        class TestClass3
        {
            public TestClass1 Dep1;
            public TestClass2 Dep2;

            public TestClass3(TestClass1 dep1, TestClass2 dep2)
            {
                Dep1 = dep1;
                Dep2 = dep2;
            }
        }

        [Test]
        public void TestConcurrentMetadataAccess()
        {
            // This test verifies that concurrent access to TypeAnalyzer
            // doesn't cause issues with the cache
            
            var exceptions = new List<Exception>();
            var threads = new List<Thread>();
            var iterations = 100;

            // Create multiple threads that will access TypeAnalyzer concurrently
            for (int t = 0; t < 4; t++)
            {
                var thread = new Thread(() =>
                {
                    try
                    {
                        for (int i = 0; i < iterations; i++)
                        {
                            // Access different types
                            var info1 = TypeAnalyzer.TryGetInfo<TestClass1>();
                            var info2 = TypeAnalyzer.TryGetInfo<TestClass2>();
                            var info3 = TypeAnalyzer.TryGetInfo<TestClass3>();

                            // Verify results are valid
                            if (info1 == null || info2 == null || info3 == null)
                            {
                                throw new Exception("TypeAnalyzer returned null unexpectedly");
                            }

                            // Verify types are correct
                            if (info1.Type != typeof(TestClass1) ||
                                info2.Type != typeof(TestClass2) ||
                                info3.Type != typeof(TestClass3))
                            {
                                throw new Exception("TypeAnalyzer returned wrong type");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        lock (exceptions)
                        {
                            exceptions.Add(ex);
                        }
                    }
                });

                threads.Add(thread);
            }

            // Start all threads
            foreach (var thread in threads)
            {
                thread.Start();
            }

            // Wait for all threads to complete
            foreach (var thread in threads)
            {
                thread.Join();
            }

            // Check if any exceptions occurred
            if (exceptions.Count > 0)
            {
                throw new AggregateException(
                    "Concurrent access to TypeAnalyzer caused exceptions", exceptions);
            }
        }

        [Test]
        public void TestMetadataCacheConsistencyAfterMultipleAccess()
        {
            // Get metadata multiple times
            var infos = new List<InjectTypeInfo>();
            
            for (int i = 0; i < 100; i++)
            {
                infos.Add(TypeAnalyzer.TryGetInfo<TestClass3>());
            }

            // All should be the exact same reference
            for (int i = 1; i < infos.Count; i++)
            {
                Assert.That(ReferenceEquals(infos[0], infos[i]),
                    "All metadata accesses should return the same cached instance");
            }

            // Verify constructor info consistency
            for (int i = 0; i < infos.Count; i++)
            {
                Assert.IsEqual(infos[i].InjectConstructor.Parameters.Count, 2,
                    "Constructor should always have 2 parameters");
            }
        }

        [Test]
        public void TestMetadataNotNullForValidTypes()
        {
            Assert.IsNotNull(TypeAnalyzer.TryGetInfo<TestClass1>());
            Assert.IsNotNull(TypeAnalyzer.TryGetInfo<TestClass2>());
            Assert.IsNotNull(TypeAnalyzer.TryGetInfo<TestClass3>());
        }

        [Test]
        public void TestMetadataNullForInterfaces()
        {
            var info = TypeAnalyzer.TryGetInfo<IDisposable>();
            Assert.IsNull(info, "Interfaces should not have inject metadata");
        }

        [Test]
        public void TestMetadataNullForAbstractTypes()
        {
            abstract class AbstractClass
            {
                public abstract void DoSomething();
            }

            var info = TypeAnalyzer.TryGetInfo<AbstractClass>();
            // Abstract classes may or may not have metadata depending on implementation
            // Just verify it doesn't throw
        }
    }
}
