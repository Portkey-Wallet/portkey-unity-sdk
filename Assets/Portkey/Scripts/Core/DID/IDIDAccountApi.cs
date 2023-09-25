using System.Collections;

namespace Portkey.Core
{
    using ScanLoginParam = EditManagerParams;
    
    public class LoginResult
    {
        public RecoverStatusResult Status { get; private set; }
        public string SessionId { get; private set; }
        
        public LoginResult(RecoverStatusResult status, string sessionId)
        {
            Status = status;
            SessionId = sessionId;
        }
    }

    public class RegisterResult
    {
        public RegisterStatusResult Status { get; private set; }
        public string SessionId { get; private set; }

        public RegisterResult(RegisterStatusResult status, string sessionId)
        {
            Status = status;
            SessionId = sessionId;
        }
    }

    public class VerifierItem
    {
        public string id;
        public string name;
        public string imageUrl;
        public string[] endPoints;
        public string[] verifierAddresses;
    }
    
    /// <summary>
    /// Interface for the calling of Portkey DID Backend API.
    /// </summary>
    public interface IDIDAccountApi
    {
        /// <summary>
        /// A coroutine for logging in with scan.
        /// </summary>
        /// <param name="param">Provides chain ID, caHash and manager info.</param>
        /// <param name="successCallback">The callback function when user is successfully logged in.</param>
        /// <param name="errorCallback">The callback function when there is an error.</param>
        IEnumerator Login(ScanLoginParam param, SuccessCallback<bool> successCallback, ErrorCallback errorCallback);
        /// <summary>
        /// A coroutine for logging in.
        /// </summary>
        /// <param name="param">Account parameters such as the logged in guardian ID and the list of approved guardians.</param>
        /// <param name="successCallback">The callback function when user is successfully logged in.</param>
        /// <param name="errorCallback">The callback function when there is an error.</param>
        /// <returns></returns>
        IEnumerator Login(AccountLoginParams param, SuccessCallback<LoginResult> successCallback, ErrorCallback errorCallback);
        /// <summary>
        /// A coroutine for logging out.
        /// </summary>
        /// <param name="param">To specify parameters to log out the account on a specific chain.</param>
        /// <param name="successCallback">The callback function when user is successfully logged out.</param>
        /// <param name="errorCallback">The callback function when there is an error.</param>
        /// <returns></returns>
        IEnumerator Logout(EditManagerParams param, SuccessCallback<bool> successCallback, ErrorCallback errorCallback);
        IEnumerator GetLoginStatus(string chainId, string sessionId, SuccessCallback<RecoverStatusResult> successCallback, ErrorCallback errorCallback);
        /// <summary>
        /// A coroutine for signing up a new account.
        /// </summary>
        /// <param name="param">Parameters including guardian information and account type etc.</param>
        /// <param name="successCallback">The callback function when user is successfully signed up and logged in.</param>
        /// <param name="errorCallback">The callback function when there is an error.</param>
        /// <returns></returns>
        IEnumerator Register(RegisterParams param, SuccessCallback<RegisterResult> successCallback, ErrorCallback errorCallback);
        IEnumerator GetRegisterStatus(string chainId, string sessionId, SuccessCallback<RegisterStatusResult> successCallback, ErrorCallback errorCallback);
        IEnumerator GetHolderInfo(GetHolderInfoParams param, SuccessCallback<IHolderInfo> successCallback, ErrorCallback errorCallback);
        IEnumerator GetHolderInfo(GetHolderInfoByManagerParams param, SuccessCallback<CaHolderWithGuardian> successCallback, ErrorCallback errorCallback);
        /// <summary>
        /// Get holder info through contract using information such as contract address from account info.
        /// </summary>
        /// <param name="param">Account information such as caHash and chainId.</param>
        /// <param name="successCallback">The callback function when user is successful in getting back holder's info.</param>
        /// <param name="errorCallback">The callback function when there is an error.</param>
        /// <returns></returns>
        IEnumerator GetHolderInfoByContract(GetHolderInfoParams param, SuccessCallback<IHolderInfo> successCallback, ErrorCallback errorCallback);
        IEnumerator GetVerifierServers(string chainId, SuccessCallback<VerifierItem[]> successCallback, ErrorCallback errorCallback);
        IEnumerator GetCAHolderInfo(string chainId, SuccessCallback<CAHolderInfo> successCallback, ErrorCallback errorCallback);
        void Reset();
        /// <summary>
        /// To get the wallet information such as Address and Private Key of the management wallet which can be used for signing in place of the contract account.
        /// </summary>
        /// <returns>BlockchainWallet object that contains private key and address.</returns>
        IWallet GetWallet();
        bool IsLoggedIn();
    }
}