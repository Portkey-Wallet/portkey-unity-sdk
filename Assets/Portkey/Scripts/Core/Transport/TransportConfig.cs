using UnityEngine;

namespace Portkey.Core
{
    public abstract class TransportConfig : ScriptableObject, ITransport
    {
        public abstract void Send(string url);
    }
}