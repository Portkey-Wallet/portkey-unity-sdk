using System.Collections.Generic;
using Portkey.Core;
using UnityEngine;

namespace Portkey
{
    [CreateAssetMenu(fileName = "PortkeyTransport", menuName = "Portkey/Transport/PortkeyTransport", order = 0)]
    public class PortkeyTransport : ScriptableObject
    {
        [SerializeField]
        private Dictionary<string, ITransport> _transports;

        public void Send(string key, string url)
        {
            if(_transports.TryGetValue(key, out ITransport transport))
            {
                transport.Send(url);
            }
        }
    }
}