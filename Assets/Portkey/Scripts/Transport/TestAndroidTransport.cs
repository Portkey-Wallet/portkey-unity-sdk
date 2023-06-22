using Portkey.Core;
using Portkey.Transport;
using UnityEngine;
using UnityEngine.Serialization;

public class TestAndroidTransport : MonoBehaviour
{
    [SerializeReference]
    private TransportConfig transportConfig;

    public void Send()
    {
        transportConfig?.Send("portkey.did://test.portkey.finance/login?type=login&chainType=aelf");
    }
}
