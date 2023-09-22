using Portkey.Core;
using UnityEngine;

namespace Portkey.Transport
{
    /// <summary>
    /// PortkeyTransportConfig is a config class for different platforms dealing with app to app communication.
    /// </summary>
    [CreateAssetMenu(fileName = "PortkeyTransportConfig", menuName = "Portkey/Transport/PortkeyTransportConfig", order = 0)]
    public class PortkeyTransportConfig : TransportConfig
    {
        /// <summary>
        /// Android transport class to support android deeplinks.
        /// </summary>
        [SerializeField]
        private AndroidTransport androidTransport;

        /// <summary>
        /// Android transport class to support android deeplinks.
        /// </summary>
        [SerializeField]
        private IOSTransport iosTransport;

        public override void Send(string url)
        {
            //the correct transport to call base on the platform
#if UNITY_ANDROID
            if (androidTransport != null)
            {
                androidTransport.Send(url);
            }
#elif UNITY_IOS
            if (iosTransport != null)
            {
                iosTransport.Send(url);
            }
#endif
        }
    }
}