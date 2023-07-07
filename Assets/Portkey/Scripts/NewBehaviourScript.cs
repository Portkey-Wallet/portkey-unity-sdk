using System.Collections;
using System.Collections.Generic;
using Google.Protobuf.WellKnownTypes;
using Portkey.Contract;
using Portkey.Contracts.CA;
using Portkey.Core;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

        Request();
    }

    private async void Request()
    {
        /*
        IContract contract = new ContractBasic();

        var result = await contract.CallTransactionAsync<GetVerifierServersOutput>("", 
            "GetVerifierServers", new Empty());
        
        Debugger.Log(result.VerifierServers[0]);*/
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
