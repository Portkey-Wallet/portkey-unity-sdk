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
        public string Id { get; private set; }
        public string Name { get; private set; }
        public string ImageUrl { get; private set; }
        public string[] EndPoints { get; private set; }
        public string[] VerifierAddresses { get; private set; }
        
        public VerifierItem(string id, string name, string imageUrl, string[] endPoints, string[] verifierAddresses)
        {
            Id = id;
            Name = name;
            ImageUrl = imageUrl;
            EndPoints = endPoints;
            VerifierAddresses = verifierAddresses;
        }
    }
    
    public interface IDIDAccountMethods
    {
        /// <summary>
        /// For logging in with scan.
        /// </summary>
        /// <param name="param">Provides chain ID, caHash and manager info.</param>
        /// <returns>True if able to login, false otherwise.</returns>
        public IEnumerator Login(ScanLoginParam param, SuccessCallback<bool> successCallback, ErrorCallback errorCallback);
        public IEnumerator Login(AccountLoginParams param, SuccessCallback<LoginResult> successCallback, ErrorCallback errorCallback);
        public bool Logout(EditManagerParams param);
        public RecoverStatusResult GetLoginStatus(string chainId, string sessionId);
        IEnumerator Register(RegisterParams param, SuccessCallback<RegisterResult> successCallback, ErrorCallback errorCallback);
        IEnumerator GetRegisterStatus(string chainId, string sessionId, SuccessCallback<RegisterStatusResult> successCallback, ErrorCallback errorCallback);
        IEnumerator GetHolderInfo(GetHolderInfoParams param, SuccessCallback<IHolderInfo> successCallback, ErrorCallback errorCallback);
        IEnumerator GetHolderInfo(GetHolderInfoByManagerParams param, SuccessCallback<CaHolderWithGuardian> successCallback, ErrorCallback errorCallback);
        VerifierItem[] GetVerifierServers(string chainId);
        public IEnumerator GetCAHolderInfo(string chainId, SuccessCallback<CAHolderInfo> successCallback, ErrorCallback errorCallback);
    }
}