using UnityEngine;

namespace Portkey.Core
{
    /// <summary>
    /// Transport is a ScriptableObject that can be used to send a url to any app.
    /// Supports opening download link corresponding to an app if it is not installed.
    /// </summary>
    public abstract class Transport : ScriptableObject, ITransport
    {
        /// <summary>
        /// Value to be used as the download link if the app is not installed.
        /// </summary>
        [SerializeField]
        private string downloadLink;
        
        /// <summary>
        /// Send the url to an app if the app is installed, otherwise open the download link corresponding to the app.
        /// </summary>
        /// <param name="url">The url scheme to send.</param>
        public void Send(string url)
        {
            if (TrySend(url))
            {
                return;
            }
            OpenDownloadLink(this.downloadLink);
        }
        
        /// <summary>
        /// Tries to send the url to an app.
        /// </summary>
        /// <param name="url">The url to make the request to.</param>
        /// <returns>True if the url was sent to an app, false otherwise.</returns>
        protected abstract bool TrySend(string url);
        
        /// <summary>
        /// Open the download link corresponding to the app.
        /// </summary>
        /// <param name="downloadLink">The download link to open.</param>
        protected abstract void OpenDownloadLink(string downloadLink);
    }
}