using UnityEngine;

namespace Portkey.Core
{
    public abstract class Transport : ScriptableObject, ITransport
    {
        public void Send(string url)
        {
            if (TrySend(url))
            {
                return;
            }
            OpenDownloadLink();
        }
            
        protected abstract bool TrySend(string url);
        protected abstract void OpenDownloadLink();
    }
}