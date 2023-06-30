using System;
using System.Collections;

namespace Portkey.Core
{
    [Serializable]
    public class Context 
    {
        public string clientId;
        public string requestId;
    }

    [Serializable]
    public class GuardiansApproved
    {
        public AccountType type;
        public string identifier;
        public string verifierId;
        public string verificationDoc;
        public string signature;
    }

    [Serializable]
    public class RegisterParams
    {
        public AccountType type;
        public string loginGuardianIdentifier;
        public string manager;
        public string extraData;
        public string chainId;
        public string verifierId;
        public string verificationDoc;
        public string signature;
        public Context context;
    }

    [Serializable]
    public class RegisterResult
    {
        public string sessionId;
    }

    [Serializable]
    public class RecoveryParams
    {
        public string loginGuardianIdentifier;
        public string manager;
        public GuardiansApproved[] guardiansApproved;
        public string extraData;
        public string chainId;
        public Context context;
    }

    [Serializable]
    public class RecoveryResult
    {
        public string sessionId;
    }

    [Serializable]
    public class GetCAHolderByManagerParams
    {
        public string manager;
        public string chainId;
    }

    [Serializable]
    public class GetCAHolderByManagerResult
    {
        //CaHolderWithGuardian[] caHolders;
    }

    [Serializable]
    public class Guardian
    {
        public string guardianIdentifier;
        public string identifierHash;
        public bool isLoginGuardian = true;
        public string salt;
        public AccountType type;
        public string verifierId;
    }
    
    [Serializable]
    public class Manager
    {
        public string address; //aelf.Address
        public string extraData;
    }
    
    [Serializable]
    public class IHolderInfo
    {
        public string caAddress;
        public string caHash;
        public Guardian[] guardianList;
        public Manager[] managerInfos;
    }
    
    [Serializable]
    public class GetHolderInfoParams
    {
        public string chainId;
        public string caHash;
        public string guardianIdentifier;
    }

    [Serializable]
    public class GetRegisterInfoParams
    {
        public string loginGuardianIdentifier;
        public string caHash;
    }

    [Serializable]
    public class CheckGoogleRecaptchaParams
    {
        public RecaptchaType operationType;
    };

    [Serializable]
    public class RegisterInfo
    {
        string originChainId;
    }

    [Serializable]
    public class ICountryItem
    {
        public string country;
        public string code;
        public string iso;
    }

    [Serializable]
    public class IPhoneCountryCodeResult
    {
        public ICountryItem[] data;
        public ICountryItem locateData;
    }
    
    /// <summary>
    /// Social service interface for all social services to implement.
    /// </summary>
    public interface IPortkeySocialService : IVerificationService, ISearchService
    {
        public IEnumerator Register(RegisterParams requestParams, SuccessCallback<RegisterResult> successCallback, ErrorCallback errorCallback);
        public IEnumerator Recovery(RecoveryParams requestParams, SuccessCallback<RecoveryResult> successCallback, ErrorCallback errorCallback);
        public IEnumerator GetHolderInfo(GetHolderInfoParams requestParams, SuccessCallback<IHolderInfo> successCallback, ErrorCallback errorCallback);
        public IEnumerator GetHolderInfoByManager(GetCAHolderByManagerParams requestParams, SuccessCallback<GetCAHolderByManagerResult> successCallback, ErrorCallback errorCallback);
        public IEnumerator GetRegisterInfo(GetRegisterInfoParams requestParams, SuccessCallback<RegisterInfo> successCallback, ErrorCallback errorCallback);
        public IEnumerator CheckGoogleRecaptcha(CheckGoogleRecaptchaParams requestParams, SuccessCallback<bool> successCallback, ErrorCallback errorCallback);
        public IEnumerator GetPhoneCountryCode(SuccessCallback<ICountryItem[]> successCallback, ErrorCallback errorCallback);
        public IEnumerator GetPhoneCountryCodeWithLocal(SuccessCallback<IPhoneCountryCodeResult> successCallback, ErrorCallback errorCallback);
    }
}