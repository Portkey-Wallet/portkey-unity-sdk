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
    public class StorageTest
    {
        // A Test behaves as an ordinary method
        [Test]
        public void StorageTestSimplePasses()
        {
            IStorageSuite<string> storage = new NonPersistentStorage<string>();
            storage.SetItem("testSave", "100");
            Task<string> value = storage.GetItem("testSave");
            Assert.AreEqual("100", value.Result);
        }
    }
}
