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
        /// For logging in with scan.
        /// </summary>
        /// <param name="param">Provides chain ID, caHash and manager info.</param>
        /// <returns>True if able to login, false otherwise.</returns>
        IEnumerator Login(ScanLoginParam param, SuccessCallback<bool> successCallback, ErrorCallback errorCallback);
        IEnumerator Login(AccountLoginParams param, SuccessCallback<LoginResult> successCallback, ErrorCallback errorCallback);
        IEnumerator Logout(EditManagerParams param, SuccessCallback<bool> successCallback, ErrorCallback errorCallback);
        IEnumerator GetLoginStatus(string chainId, string sessionId, SuccessCallback<RecoverStatusResult> successCallback, ErrorCallback errorCallback);
        IEnumerator Register(RegisterParams param, SuccessCallback<RegisterResult> successCallback, ErrorCallback errorCallback);
        IEnumerator GetRegisterStatus(string chainId, string sessionId, SuccessCallback<RegisterStatusResult> successCallback, ErrorCallback errorCallback);
        IEnumerator GetHolderInfo(GetHolderInfoParams param, SuccessCallback<IHolderInfo> successCallback, ErrorCallback errorCallback);
        IEnumerator GetHolderInfo(GetHolderInfoByManagerParams param, SuccessCallback<CaHolderWithGuardian> successCallback, ErrorCallback errorCallback);
        IEnumerator GetHolderInfoByContract(GetHolderInfoParams param, SuccessCallback<IHolderInfo> successCallback, ErrorCallback errorCallback);
        IEnumerator GetVerifierServers(string chainId, SuccessCallback<VerifierItem[]> successCallback, ErrorCallback errorCallback);
        IEnumerator GetCAHolderInfo(string chainId, SuccessCallback<CAHolderInfo> successCallback, ErrorCallback errorCallback);
        void Reset();
        IWallet GetWallet();
        bool IsLoggedIn();
        /// <summary>
        /// For adding a manager account to the DID.
        /// </summary>
        /// <param name="editManagerParams">Parameters for adding manager account.</param>
        IEnumerator AddManager(EditManagerParams editManagerParams, SuccessCallback<bool> successCallback, ErrorCallback errorCallback);
        /// <summary>
        /// For removing a manager account to the DID.
        /// </summary>
        /// <param name="editManagerParams">Parameters for removing manager account.</param>
        IEnumerator RemoveManager(EditManagerParams editManagerParams, SuccessCallback<bool> successCallback, ErrorCallback errorCallback);
    }
}