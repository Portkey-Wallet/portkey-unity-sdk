using Portkey.Core;
using Portkey.Transport;
using UnityEngine;

public class TestAndroidTransport : MonoBehaviour
{
    [SerializeField]
    private ITransport _transport = null;

    public void Send()
    {
        _transport?.Send("portkey.did://test.portkey.finance/login?type=login&chainType=aelf");
    }
}
