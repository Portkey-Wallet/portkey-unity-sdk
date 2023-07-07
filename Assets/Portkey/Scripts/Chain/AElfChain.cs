using AElf.Client;
using Portkey.Core;

namespace Portkey.Chain
{
    public class AElfChain : IChain
    {
        public AElfClient Client { get; protected set; }

        public AElfChain(string rpcUrl)
        {
            Client = new AElfClient(rpcUrl);
        }
    }
}