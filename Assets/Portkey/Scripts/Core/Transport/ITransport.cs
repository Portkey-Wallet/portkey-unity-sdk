namespace Portkey.Core
{
    /// <summary>
    /// A simple interface to transport layer calls.
    /// Supports only opening an app with parameters in the form of scheme.
    /// </summary>
    public interface ITransport
    {
        /// <summary>
        /// Sends a url scheme to an app.
        /// </summary>
        /// <param name="url">The url scheme to send.</param>
        void Send(string url);
    }
}