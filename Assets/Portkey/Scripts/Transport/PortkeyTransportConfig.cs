using Portkey.Core;
using UnityEngine;

namespace Portkey.Transport
{
    [CreateAssetMenu(fileName = "PortkeyTransportConfig", menuName = "Portkey/Transport/PortkeyTransportConfig", order = 0)]
    public class PortkeyTransportConfig : TransportConfig
    {
#if UNITY_ANDROID || UNITY_EDITOR
        [SerializeField]
        private AndroidTransport androidTransport;
#endif

        public override void Send(string url)
        {
#if UNITY_ANDROID
            if (androidTransport != null)
            {
                androidTransport.Send(url);
            }
#endif
        }
    }
}