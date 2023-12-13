using System;
using System.Collections;
using System.Collections.Generic;

namespace Portkey.Core
{
    public class ExtraData
    {
        public string transactionTime;
        public string deviceInfo;
        public string version = "2.0.0";
    }
    
    [Serializable]
    public class Context 
    {
        public string clientId = null;
        public string requestId = null;
    }

    [Serializable]
    public class ApprovedGuardian
    {
        public AccountType type;
        public string identifier = null;
        public string verifierId = null;
        public string verificationDoc = null;
        public string signature = null;
    }

    [Serializable]
    public class RegisterParams
    {
        public AccountType type;
        public string loginGuardianIdentifier = null;
        public string manager = null;
        public string extraData = null;
        public string chainId = null;
        public string verifierId = null;
        public string verificationDoc = null;
        public string signature = null;
        public Context context = null;
    }

    [Serializable]
    public class SessionIdResult
    {
        public string sessionId;
    }

    [Serializable]
    public class RecoveryParams
    {
        public string loginGuardianIdentifier = null;
        public string manager = null;
        public ApprovedGuardian[] guardiansApproved = null;
        public string extraData = null;
        public string chainId = null;
        public Context context = null;
    }

    [Serializable]
    public class GetCAHolderByManagerParams
    {
        public string manager = null;
        public string chainId = null;
    }

    [Serializable]
    public class GetCAHolderByManagerResult
    {
        public IList<CaHolderWithGuardian> caHolders = null;
    }

    [Serializable]
    public class GuardianDto
    {
        public string guardianIdentifier = null;
        public string identifierHash = null;
        public bool isLoginGuardian = true;
        public string salt = null;
        public AccountType type;
        public string verifierId = null;
        public string thirdPartyEmail = null;
        public string isPrivate = null;
        public string firstName = null;
        public string lastName = null;
    }


    [Serializable]
    public class Manager
    {
        public string address = null; //aelf.Address
        public string extraData = null;
    }
    
    [Serializable]
    public class IHolderInfo
    {
        public string caAddress = null;
        public string caHash = null;
        public GuardianList guardianList = null;
        public Manager[] managerInfos = null;
    }

    [Serializable]
    public class GuardianList
    {
        public GuardianDto[] guardians = null;
    }
    
    public class GetHolderInfoByManagerParams
    {
        public string manager = null;
        public string chainId = null;
    }
    
    [Serializable]
    public class GetHolderInfoParams
    {
        public string chainId = null;
        public string caHash = null;
        public string guardianIdentifier = null;
    }

    [Serializable]
    public class GetRegisterInfoParams
    {
        public string loginGuardianIdentifier = null;
        public string caHash = null;
    }

    [Serializable]
    public class CheckGoogleRecaptchaParams
    {
        public RecaptchaType operationType;
    };

    [Serializable]
    public class RegisterInfo
    {
        public string originChainId = null;
    }

    [Serializable]
    public class ICountryItem
    {
        public string country = null;
        public string code = null;
        public string iso = null;
    }

    [Serializable]
    public class IPhoneCountryCodeResult
    {
        public ICountryItem[] data = null;
        public ICountryItem locateData = null;
    }
    
    /// <summary>
    /// Social service interface for all social services to implement.
    /// </summary>
    public interface IPortkeySocialService : IVerificationService, ISearchService, IAccountService
    {
        public static readonly string UNREGISTERED_CODE = "3002";
        
        /// <summary>
        /// Coroutine to register a new account.
        /// </summary>
        /// <param name="requestParams">Parameters for registration.</param>
        IEnumerator Register(RegisterParams requestParams, SuccessCallback<SessionIdResult> successCallback, ErrorCallback errorCallback);
        /// <summary>
        /// Coroutine to recover an existing account.
        /// </summary>
        /// <param name="requestParams">Parameters for recovery.</param>
        IEnumerator Recovery(RecoveryParams requestParams, SuccessCallback<SessionIdResult> successCallback, ErrorCallback errorCallback);
        /// <summary>
        /// Get holder info by guardian info, chainId and caHash.
        /// </summary>
        IEnumerator GetHolderInfo(GetHolderInfoParams requestParams, SuccessCallback<IHolderInfo> successCallback, ErrorCallback errorCallback);
        /// <summary>
        /// Get holder info by account manager.
        /// </summary>
        IEnumerator GetHolderInfoByManager(GetCAHolderByManagerParams requestParams, SuccessCallback<GetCAHolderByManagerResult> successCallback, ErrorCallback errorCallback);
        /// <summary>
        /// Get registration info.
        /// </summary>
        IEnumerator GetRegisterInfo(GetRegisterInfoParams requestParams, SuccessCallback<RegisterInfo> successCallback, ErrorCallback errorCallback);
        /// <summary>
        /// Check google recaptcha result.
        /// </summary>
        IEnumerator CheckGoogleRecaptcha(CheckGoogleRecaptchaParams requestParams, SuccessCallback<bool> successCallback, ErrorCallback errorCallback);
        /// <summary>
        /// Get phone country code with local.
        /// </summary>
        IEnumerator GetPhoneCountryCodeWithLocal(SuccessCallback<IPhoneCountryCodeResult> successCallback, ErrorCallback errorCallback);
    }
}