namespace Portkey.Core
{
    public class Context
    {
        public string ClientId { get; private set; }
        public string RequestId { get; private set; }
        
        public Context(string clientId, string requestId)
        {
            ClientId = clientId;
            RequestId = requestId;
        }
    }
}