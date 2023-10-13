using System.Collections;

namespace Portkey.Core
{
    public interface IChainProvider
    {
        IEnumerator GetAvailableChainIds(SuccessCallback<string[]> successCallback, ErrorCallback errorCallback);
        IEnumerator GetChain(string chainId, SuccessCallback<IChain> successCallback, ErrorCallback errorCallback);
    }
}