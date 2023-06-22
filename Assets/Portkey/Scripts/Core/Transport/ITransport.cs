namespace Portkey.Core
{
    public interface ITransport
    {
        void Send(string url);
    }
}