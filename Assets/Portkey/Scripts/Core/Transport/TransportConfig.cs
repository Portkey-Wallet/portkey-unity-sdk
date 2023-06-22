using UnityEngine;

namespace Portkey.Core
{
    /// <summary>
    /// Config class for transport layer dealing with app to app communication.
    /// </summary>
    public abstract class TransportConfig : ScriptableObject, ITransport
    {
        public abstract void Send(string url);
    }
}