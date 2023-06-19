using System.Collections;
using System.Collections.Generic;
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
        private const string KEY = "testSave";
        
        /// <summary>
        /// Test that NonPersistentStorage is able to store and retrieve data.
        /// </summary>
        [Test]
        public void TestNonPersistentStorageStore()
        {
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
            const string MYKEY = "MyKey";
            
            IStorageSuite<int> storage = new NonPersistentStorageMock<int>("MyCuteLilStorage");
            storage.SetItem(MYKEY, INT_TO_CHECK);
            int value = storage.GetItem(MYKEY);
            Assert.AreEqual(INT_TO_CHECK, value);
        }
        
        /// <summary>
        /// Test that NonPersistentStorage is able to remove data.
        /// </summary>
        [Test]
        public void TestNonPersistentStorageRemove()
        {
            IStorageSuite<string> storage = new NonPersistentStorage<string>();
            storage.SetItem(KEY, STRING_TO_CHECK);
            
            storage.RemoveItem(KEY);
            
            string value = storage.GetItem(KEY);
            Assert.AreEqual(null, value);
        }

        /// <summary>
        /// Test that NonPersistentStorage is able to return the correct value when removing data.
        /// </summary>
        [Test]
        public void TestNonPersistentStorageRemoveReturnValue()
        {
            IStorageSuite<string> storage = new NonPersistentStorage<string>();
            storage.SetItem(KEY, STRING_TO_CHECK);

            string removedValue = storage.RemoveItem(KEY);

            Assert.AreEqual(removedValue, STRING_TO_CHECK);
        }
        
        /// <summary>
        /// Test that NonPersistentStorage is able to return null when removing data that doesn't exist.
        /// </summary>
        [Test]
        public void TestNonPersistentStorageRemoveNothing()
        {
            IStorageSuite<string> storage = new NonPersistentStorage<string>();

            string removedValue = storage.RemoveItem(KEY);

            Assert.AreEqual(removedValue, null);
        }
        
        /// <summary>
        /// Test that NonPersistentStorage is able to return null when getting data that doesn't exist.
        /// </summary>
        [Test]
        public void TestNonPersistentStorageGetFromNothing()
        {
            IStorageSuite<string> storage = new NonPersistentStorage<string>();

            string value = storage.GetItem(KEY);

            Assert.AreEqual(value, null);
        }
        
        /// <summary>
        /// Test that NonPersistentStorage is able to store and retrieve multiple data.
        /// </summary>
        [Test]
        public void TestNonPersistentStorageMultipleItems()
        {
            //3 different keys
            const string KEY1 = "key1";
            const string KEY2 = "key2";
            const string KEY3 = "key3";
            
            IStorageSuite<string> storage = new NonPersistentStorage<string>();
            storage.SetItem(KEY1, "1");
            storage.SetItem(KEY2, "2");
            storage.SetItem(KEY3, "3");

            if(storage.GetItem(KEY1) != null && storage.GetItem(KEY2) != null && storage.GetItem(KEY3) != null)
            {
                Assert.Pass();
            }
            else
            {
                Assert.Fail();
            }
        }
    }
}
