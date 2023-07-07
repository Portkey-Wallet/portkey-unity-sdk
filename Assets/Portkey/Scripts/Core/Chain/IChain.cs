using AElf.Client;

namespace Portkey.Core
{
    public interface IChain
    {
        /// <summary>
        /// Property to get AElfClient
        /// </summary>
        AElfClient Client { get; }
    }
}