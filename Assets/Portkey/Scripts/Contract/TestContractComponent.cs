using Google.Protobuf.WellKnownTypes;
using Portkey.Chain;
using Portkey.Contract;
using Portkey.Contracts.CA;
using Portkey.Core;
using UnityEngine;

public class TestContractComponent : MonoBehaviour
{
    [SerializeField]
    private PortkeyConfig _config;
    
    // Start is called before the first frame update
    void Start()
    {
        Request();
    }

    private async void Request()
    {
        var testMainChain = _config.ChainInfos["TestMain"];
        IChain aelfChain = new AElfChain(testMainChain.RpcUrl);
        IContract contract = new ContractBasic(aelfChain, testMainChain.ContractInfos["CAContract"].ContractAddress);

        var result = await contract.CallTransactionAsync<GetVerifierServersOutput>("GetVerifierServers", new Empty());
        
        Debugger.Log(result.VerifierServers[0]);
    }
}
