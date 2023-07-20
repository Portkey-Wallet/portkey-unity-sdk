using System;
using System.Collections;
using System.Collections.Generic;

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
    public class SessionIdResult
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
    public class GetCAHolderByManagerParams
    {
        public string manager;
        public string chainId;
    }

    [Serializable]
    public class GetCAHolderByManagerResult
    {
        public IList<CaHolderWithGuardian> caHolders;
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
        public GuardianList guardianList;
        public Manager[] managerInfos;
    }

    [Serializable]
    public class GuardianList
    {
        public Guardian[] guardians;
    }
    
    [Serializable]
    public class GetHolderInfoByManagerParams : GetHolderInfoParams
    {
        public string manager = null;
    }
    
    [Serializable]
    public class GetHolderInfoParams
    {
        public string chainId;
        public string caHash = null;
        public string guardianIdentifier = null;
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
        public string originChainId;
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
        /// <summary>
        /// Coroutine to register a new account.
        /// </summary>
        /// <param name="requestParams">Parameters for registration.</param>
        public IEnumerator Register(RegisterParams requestParams, SuccessCallback<SessionIdResult> successCallback, ErrorCallback errorCallback);
        /// <summary>
        /// Coroutine to recover an existing account.
        /// </summary>
        /// <param name="requestParams">Parameters for recovery.</param>
        public IEnumerator Recovery(RecoveryParams requestParams, SuccessCallback<SessionIdResult> successCallback, ErrorCallback errorCallback);
        /// <summary>
        /// Get holder info by guardian info, chainId and caHash.
        /// </summary>
        public IEnumerator GetHolderInfo(GetHolderInfoParams requestParams, SuccessCallback<IHolderInfo> successCallback, ErrorCallback errorCallback);
        /// <summary>
        /// Get holder info by account manager.
        /// </summary>
        public IEnumerator GetHolderInfoByManager(GetCAHolderByManagerParams requestParams, SuccessCallback<GetCAHolderByManagerResult> successCallback, ErrorCallback errorCallback);
        /// <summary>
        /// Get registration info.
        /// </summary>
        public IEnumerator GetRegisterInfo(GetRegisterInfoParams requestParams, SuccessCallback<RegisterInfo> successCallback, ErrorCallback errorCallback);
        /// <summary>
        /// Check google recaptcha result.
        /// </summary>
        public IEnumerator CheckGoogleRecaptcha(CheckGoogleRecaptchaParams requestParams, SuccessCallback<bool> successCallback, ErrorCallback errorCallback);
        /// <summary>
        /// Get phone country code.
        /// </summary>
        public IEnumerator GetPhoneCountryCode(SuccessCallback<ICountryItem[]> successCallback, ErrorCallback errorCallback);
        /// <summary>
        /// Get phone country code with local.
        /// </summary>
        public IEnumerator GetPhoneCountryCodeWithLocal(SuccessCallback<IPhoneCountryCodeResult> successCallback, ErrorCallback errorCallback);
    }
}