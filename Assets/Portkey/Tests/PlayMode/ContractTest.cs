using System.Collections;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using NUnit.Framework;
using Portkey.Contract;
using Portkey.Contracts.CA;
using Portkey.Core;
using UnityEngine;
using UnityEngine.TestTools;

namespace Portkey.Test
{
    public class ContractTest
    {
        [Test]
        public void ContractGetVerifierServersTest()
        {
            IContract contract = new AElfContractBasic();

            var awaiter = contract.CallTransactionAsync<GetVerifierServersOutput>("", 
                "GetVerifierServers", new Empty()).GetAwaiter();
            
            while(awaiter.IsCompleted == false)
            {
                // do nothing, only time out if it takes too long
                if (Time.realtimeSinceStartup > 20.0f)
                {
                    break;
                }
                Debug.Log(Time.realtimeSinceStartup);
            }
            
            var result = (GetVerifierServersOutput) awaiter.GetResult();
            
            Assert.AreNotEqual(null, result.VerifierServers[0]);
        }
    }
}
