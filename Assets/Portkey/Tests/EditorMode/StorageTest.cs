using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Portkey.Core;
using Portkey.Storage;
using UnityEditor.VersionControl;

namespace Portkey.Test
{
    /// <summary>
    /// Storage test for testing all storage related classes.
    /// </summary>
    /// <remarks>
    /// Check NonPersistentStorageMock.cs and NonPersistentStorage.cs for implementation details.
    /// </remarks>
    public class StorageTest
    {
        private const int INT_TO_CHECK = 89;
        private const string STRING_TO_CHECK = "lorem ipsum";
        
        /// <summary>
        /// Test that NonPersistentStorage is able to store and retrieve data.
        /// </summary>
        [Test]
        public void TestNonPersistentStorageStore()
        {
            const string KEY = "testSave";
            
            IStorageSuite<string> storage = new NonPersistentStorage<string>();
            storage.SetItem(KEY, STRING_TO_CHECK);
            Task<string> value = storage.GetItem(KEY);
            Assert.AreEqual(STRING_TO_CHECK, value.Result);
        }
        
        /// <summary>
        /// Test that TestNonPersistentStorage is able to store and retrieve data.
        /// </summary>
        [Test]
        public void TestDifferentStorage()
        {
            const string KEY = "MyKey";
            
            IStorageSuite<int> storage = new NonPersistentStorageMock<int>("MyCuteLilStorage");
            storage.SetItem(KEY, INT_TO_CHECK);
            Task<int> value = storage.GetItem(KEY);
            Assert.AreEqual(INT_TO_CHECK, value.Result);
        }
    }
}
