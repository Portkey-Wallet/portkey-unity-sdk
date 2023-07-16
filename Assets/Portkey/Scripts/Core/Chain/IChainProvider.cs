namespace Portkey.Core
{
    public interface IChainProvider
    {
        IChain GetChain(string chainId);
        void SetChainInfo(ChainInfo[] chainInfos);
    }
}