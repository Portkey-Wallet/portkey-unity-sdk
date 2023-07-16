using System;
using System.Collections;

namespace Portkey.Core
{
    [Serializable]
    public class CAInfo {
        public string caAddress;
        public string caHash;
    }
    
    [Serializable]
    public class RegisterStatusResult : CAInfo 
    {
        /// <summary>
        /// recoveryStatus can be one of the following: pass, fail, pending
        /// </summary>
        public string registerStatus;
        public string registerMessage;
        
        public bool IsStatusPass()
        {
            return registerStatus == "pass";
        }
    }
    
    [Serializable]
    public class RecoverStatusResult : CAInfo 
    {
        /// <summary>
        /// recoveryStatus can be one of the following: pass, fail, pending
        /// </summary>
        public string recoveryStatus;
        public string recoveryMessage;
    }
    
    [Serializable]
    public class DefaultToken
    {
        public string name;
        public string address;
        public string imageUrl;
        public string symbol;
        public string decimals;
    }
    
    [Serializable]
    public class ChainInfo
    {
        public string chainId;
        public string chainName;
        public string endPoint;
        public string explorerUrl;
        public string caContractAddress;
        public string lastModifyTime;
        public string id;
        public DefaultToken defaultToken;
    }
    
    [Serializable]
    public class QueryOptions
    {
        public long interval;
        public long reCount;
        public long maxCount;
        
        public static readonly QueryOptions DefaultQueryOptions = new QueryOptions
        {
            interval = 500,
            reCount = 0,
            maxCount = 20
        };
    }
    
    [Serializable]
    public class CAHolderInfo
    {
        public string userId;
        public string caAddress;
        public string caHash;
        public string id;
        public string nickName;
    }

    /// <summary>
    /// Service for searching register status, recover status as well as chain and holder info.
    /// </summary>
    public interface ISearchService
    {
        public IEnumerator GetRegisterStatus(string sessionId, QueryOptions queryOptions, SuccessCallback<RegisterStatusResult> successCallback, ErrorCallback errorCallback);
        public IEnumerator GetRecoverStatus(string sessionId, QueryOptions queryOptions, SuccessCallback<RecoverStatusResult> successCallback, ErrorCallback errorCallback);
        public IEnumerator GetChainsInfo(SuccessCallback<ArrayWrapper<ChainInfo>> successCallback, ErrorCallback errorCallback);
        public IEnumerator GetCAHolderInfo(string authorization, string caHash, SuccessCallback<CAHolderInfo> successCallback, ErrorCallback errorCallback);
    }
}
