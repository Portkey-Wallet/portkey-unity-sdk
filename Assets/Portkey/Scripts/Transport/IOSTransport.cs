using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Serialization;

namespace Portkey.Transport
{
    /// <summary>
    /// Class that implements iOS transport to support iOS schemes.
    /// </summary>
    [CreateAssetMenu(fileName = "IOSTransport", menuName = "Portkey/Transport/IOSTransport")]
    public class IOSTransport : Core.Transport
    {
        /// <summary>
        /// iOS package name.
        /// </summary>
        [SerializeField] private string urlScheme;

#if UNITY_IOS
        [DllImport("__Internal")]
        private static extern void InitilizeAppCheck();

        [DllImport("__Internal")]
        private static extern bool CheckApp(string URL);
#endif
        
        public string URLScheme => urlScheme;
        
        protected override bool TrySend(string url)
        {
#if UNITY_IOS
            InitilizeAppCheck();

            if (!CheckApp(urlScheme))
            {
                return false;
            }

            Application.OpenURL(url);
            return true;
#else
            return false;
#endif
        }

        protected override void OpenDownloadLink(string downloadLink)
        {
#if UNITY_IOS
            Application.OpenURL(downloadLink);
#endif
        }
    }
}
