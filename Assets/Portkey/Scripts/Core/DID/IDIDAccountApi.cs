using System.Collections;

namespace Portkey.Core
{
    using ScanLoginParam = EditManagerParams;
    
    public class LoginResult
    {
        public RecoverStatusResult Status { get; private set; }
        public string SessionId { get; private set; }
        
        LoginResult(RecoverStatusResult status, string sessionId)
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
        public bool Login(ScanLoginParam param);
        public LoginResult Login(AccountLoginParams param);
        public bool Logout(EditManagerParams param);
        public RecoverStatusResult GetLoginStatus(string chainId, string sessionId);
        public RegisterResult Register(RegisterParams param);
        RegisterStatusResult GetRegisterStatus(string chainId, string sessionId);
        public GetCAHolderByManagerResult GetHolderInfo(GetHolderInfoParams param);
        VerifierItem[] GetVerifierServers(string chainId);
        CAHolderInfo GetCAHolderInfo(string chainId);
        /// <summary>
        /// For adding a manager account to the DID.
        /// </summary>
        /// <param name="editManagerParams">Parameters for adding manager account.</param>
        public IEnumerator AddManager(EditManagerParams editManagerParams, IHttp.successCallback successCallback, ErrorCallback errorCallback);
        /// <summary>
        /// For removing a manager account to the DID.
        /// </summary>
        /// <param name="editManagerParams">Parameters for removing manager account.</param>
        public IEnumerator RemoveManager(EditManagerParams editManagerParams, IHttp.successCallback successCallback, ErrorCallback errorCallback);
    }
}