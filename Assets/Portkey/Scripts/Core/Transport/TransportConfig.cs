using Portkey.Transport;
using UnityEngine;

namespace Portkey.Core
{
    /// <summary>
    /// Config class for transport layer dealing with app to app communication.
    /// </summary>
    [CreateAssetMenu(fileName = "TransportConfig", menuName = "Portkey/Transport/TransportConfig", order = 0)]
    public class TransportConfig : ScriptableObject, ITransport
    {
        [SerializeField]
        private string urlScheme = "portkey.did://";

        [SerializeField] 
        private string iconUrl = "https://aelf.io/favicon.ico";
        
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
        
        public string IconURL => iconUrl;

        public void Send(string uri)
        {
            var url = urlScheme + uri;
            
            Debugger.Log(uri);
            
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