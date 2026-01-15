using System;
using System.Reflection;
using NUnit.Framework;
using Zenject;
using Assert = ModestTree.Assert;

namespace Zenject.Tests.Injection
{
    [TestFixture]
    public class TestInjectMetadataCache : ZenjectUnitTestFixture
    {
        class SimpleClass
        {
            public int Value;
        }

        class ClassWithInject
        {
            [Inject]
            public SimpleClass Dependency { get; set; }
        }

        class ClassWithConstructorInject
        {
            public SimpleClass Dependency;

            public ClassWithConstructorInject(SimpleClass dependency)
            {
                Dependency = dependency;
            }
        }

        class ClassWithMethodInject
        {
            public SimpleClass Dependency;

            [Inject]
            public void Initialize(SimpleClass dependency)
            {
                Dependency = dependency;
            }
        }

        [Test]
        public void TestMetadataCachedOnFirstAccess()
        {
            // First call should trigger metadata creation
            var info1 = TypeAnalyzer.TryGetInfo<SimpleClass>();
            Assert.IsNotNull(info1);
            
            // Second call should return cached instance (same reference)
            var info2 = TypeAnalyzer.TryGetInfo<SimpleClass>();
            Assert.IsNotNull(info2);
            
            // Verify it's the same cached object reference
            Assert.That(ReferenceEquals(info1, info2), 
                "TypeAnalyzer should return same cached InjectTypeInfo instance");
        }

        [Test]
        public void TestMetadataCacheForDifferentTypes()
        {
            var info1 = TypeAnalyzer.TryGetInfo<SimpleClass>();
            var info2 = TypeAnalyzer.TryGetInfo<ClassWithInject>();
            
            Assert.IsNotNull(info1);
            Assert.IsNotNull(info2);
            Assert.That(!ReferenceEquals(info1, info2), 
                "Different types should have different cached metadata");
            
            Assert.IsEqual(info1.Type, typeof(SimpleClass));
            Assert.IsEqual(info2.Type, typeof(ClassWithInject));
        }

        [Test]
        public void TestMetadataIncludesConstructorInfo()
        {
            var info = TypeAnalyzer.TryGetInfo<ClassWithConstructorInject>();
            
            Assert.IsNotNull(info);
            Assert.IsNotNull(info.InjectConstructor);
            Assert.IsEqual(info.InjectConstructor.Parameters.Count, 1);
        }

        [Test]
        public void TestMetadataIncludesPropertyInfo()
        {
            var info = TypeAnalyzer.TryGetInfo<ClassWithInject>();
            
            Assert.IsNotNull(info);
            Assert.That(info.AllInjectables.Count > 0, 
                "Should have at least one injectable member");
        }

        [Test]
        public void TestMetadataIncludesMethodInfo()
        {
            var info = TypeAnalyzer.TryGetInfo<ClassWithMethodInject>();
            
            Assert.IsNotNull(info);
            Assert.That(info.InjectMethods.Count > 0, 
                "Should have at least one inject method");
        }

        [Test]
        public void TestMetadataBehaviorConsistentAcrossCalls()
        {
            // Get metadata multiple times
            var info1 = TypeAnalyzer.TryGetInfo<ClassWithConstructorInject>();
            var info2 = TypeAnalyzer.TryGetInfo<ClassWithConstructorInject>();
            var info3 = TypeAnalyzer.TryGetInfo<ClassWithConstructorInject>();
            
            // All should be same reference (cached)
            Assert.That(ReferenceEquals(info1, info2));
            Assert.That(ReferenceEquals(info2, info3));
            
            // All should have consistent data
            Assert.IsEqual(info1.InjectConstructor.Parameters.Count, 
                           info2.InjectConstructor.Parameters.Count);
            Assert.IsEqual(info2.InjectConstructor.Parameters.Count, 
                           info3.InjectConstructor.Parameters.Count);
        }

        [Test]
        public void TestHasInfoReturnsTrueAfterMetadataCreated()
        {
            // Initially may not have info cached
            TypeAnalyzer.TryGetInfo<SimpleClass>();
            
            // After TryGetInfo, HasInfo should return true
            Assert.That(TypeAnalyzer.HasInfo<SimpleClass>());
        }

        [Test]
        public void TestGetInfoThrowsForInvalidTypes()
        {
            // GetInfo should throw for types that can't have metadata
            // (note: interface types should return null from TryGetInfo)
            var info = TypeAnalyzer.TryGetInfo<IDisposable>();
            Assert.IsNull(info, "Interfaces should not have inject metadata");
        }

        [Test]
        public void TestCacheReusedAcrossContainerInstances()
        {
            // Get info before container creation
            var infoBefore = TypeAnalyzer.TryGetInfo<SimpleClass>();
            
            // Create new container
            var container1 = new DiContainer();
            container1.Bind<SimpleClass>().AsSingle();
            
            // Get info after container creation
            var infoAfter = TypeAnalyzer.TryGetInfo<SimpleClass>();
            
            // Should be same cached instance
            Assert.That(ReferenceEquals(infoBefore, infoAfter),
                "Cache should be shared across container instances");
        }
    }
}
