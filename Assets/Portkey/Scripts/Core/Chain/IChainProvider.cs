using System.Collections;

namespace Portkey.Core
{
    public interface IChainProvider
    {
        /// <summary> The GetAvailableChainIds function returns a list of all the available chain IDs.</summary>        
        ///
        /// <param name="SuccessCallback&lt;string[]&gt; successCallback"> /// this parameter is a callback function that returns the chain ids.
        /// </param>
        /// <param name="ErrorCallback errorCallback"> ///     &lt;para&gt;error callback.&lt;/para&gt;
        /// </param>
        ///
        /// <returns> The array of available chain ids.</returns>
        IEnumerator GetAvailableChainIds(SuccessCallback<string[]> successCallback, ErrorCallback errorCallback);
        /// <summary> The GetChain function is used to retrieve a chain that is available with an input chain ID.</summary>
        ///
        /// <param name="string chainId"> The chain id of the chain to get.</param>
        /// <param name="SuccessCallback&lt;IChain&gt; successCallback"> ///     the callback that will be called when the chain is returned.
        /// </param>
        /// <param name="ErrorCallback errorCallback"> ///     this is the callback that will be called if there is an error.
        /// </param>
        ///
        /// <returns> Returns the chain associated with the requires chain ID.</returns>
        IEnumerator GetChain(string chainId, SuccessCallback<IChain> successCallback, ErrorCallback errorCallback);
    }
}