using NUnit.Framework;
using UnityEngine;
using Portkey.Core;
using Portkey.Storage;

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
            string value = storage.GetItem(KEY);
            Assert.AreEqual(STRING_TO_CHECK, value);
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
            int value = storage.GetItem(KEY);
            Assert.AreEqual(INT_TO_CHECK, value);
        }
        
        /// <summary>
        /// Test that TestPersistentLocalStorage is able to store and retrieve data persistently.
        /// </summary>
        [Test]
        public void TestPersistentLocalStorage()
        {
            const string FILENAME = "temp.txt";
            
            IStorageSuite<string> storage = new PersistentLocalStorage(Application.dataPath);
            storage.SetItem(FILENAME, STRING_TO_CHECK);
            string value = storage.GetItem(FILENAME);
            Assert.AreEqual(STRING_TO_CHECK, value);
            
            //clean up
            storage.RemoveItem(FILENAME);
        }
        
        /// <summary>
        /// Test that TestPersistentLocalStorage is able to remove data.
        /// </summary>
        [Test]
        public void TestPersistentLocalStorageRemove()
        {
            const string FILENAME = "temp.txt";
            
            IStorageSuite<string> storage = new PersistentLocalStorage(Application.dataPath);
            storage.SetItem(FILENAME, STRING_TO_CHECK);

            //clean up
            storage.RemoveItem(FILENAME);
            
            string value = storage.GetItem(FILENAME);
            Assert.AreEqual(null, value);
        }
    }
}
