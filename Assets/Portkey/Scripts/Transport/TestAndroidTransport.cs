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
        if (transportConfig != null)
        {
            transportConfig.Send("portkey.did://test.portkey.finance/login?type=login&chainType=aelf");
        }
    }
}
