using System;
using System.Collections;
using UnityEngine;

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

    public class PortkeyAppLoginResult
    {
        public CaHolderWithGuardian caHolder;
        public ISigningKey managementAccount;
    }
    
    /// <summary>
    /// Interface for the calling of DID Account methods.
    /// </summary>
    public interface IDIDAccountApi
    {
        IEnumerator Login(AccountLoginParams param, SuccessCallback<LoginResult> successCallback, ErrorCallback errorCallback);
        IEnumerator LoginWithPortkeyApp(SuccessCallback<PortkeyAppLoginResult> successCallback, ErrorCallback errorCallback);
        IEnumerator LoginWithPortkeyExtension(SuccessCallback<DIDWalletInfo> successCallback, Action OnDisconnected, ErrorCallback errorCallback);
        IEnumerator LoginWithQRCode(SuccessCallback<Texture2D> qrCodeCallback, SuccessCallback<PortkeyAppLoginResult> successCallback, ErrorCallback errorCallback);
        IEnumerator Logout(EditManagerParams param, SuccessCallback<bool> successCallback, ErrorCallback errorCallback);
        IEnumerator Register(RegisterParams param, SuccessCallback<RegisterResult> successCallback, ErrorCallback errorCallback);
        IEnumerator GetHolderInfo(GetHolderInfoParams param, SuccessCallback<IHolderInfo> successCallback, ErrorCallback errorCallback);
        IEnumerator GetHolderInfo(GetHolderInfoByManagerParams param, SuccessCallback<CaHolderWithGuardian> successCallback, ErrorCallback errorCallback);
        IEnumerator GetHolderInfoByContract(GetHolderInfoParams param, SuccessCallback<IHolderInfo> successCallback, ErrorCallback errorCallback);
        IEnumerator GetCAHolderInfo(string chainId, SuccessCallback<CAHolderInfo> successCallback, ErrorCallback errorCallback);
        ISigningKey GetManagementSigningKey();
        void CancelLoginWithQRCode();
        void CancelLoginWithPortkeyApp();
        bool IsLoggedIn();
        bool Save(string password, string keyName);
        bool Load(string password, string keyName);
    }
}