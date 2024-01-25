using System;
using System.Collections;
using System.Collections.Generic;

namespace Portkey.Core
{
    [Serializable]
    public class SendVerificationCodeParams
    {
        public string type;
        public string guardianIdentifier;
        public string verifierId;
        public string chainId;
        public int operationType;
        public int platformType;
    }

    [Serializable]
    public class SendVerificationCodeRequestParams
    {
        public SendVerificationCodeParams body;
        public Dictionary<string, string> headers;
    }

    [Serializable]
    public class SendVerificationCodeResult
    {
        public string verifierSessionId;
    }
    
    [Serializable]
    public class VerifyVerificationCodeParams
    {
        public string verifierSessionId;
        public string verificationCode;
        public string guardianIdentifier;
        public string verifierId;
        public string chainId;
        public OperationTypeEnum operationType;
    }

    [Serializable]
    public class VerifyVerificationCodeResult
    {
        public string verificationDoc;
        public string signature;
    }
    
    [Serializable]
    public class FullName
    {
        public string firstName;
        public string lastName;
    }
    
    [Serializable]
    public class UserInfo
    {
        public FullName name;
        public string email;
    }

    [Serializable]
    public class SendAppleUserExtraInfoParams
    {
        public string identityToken;
        public UserInfo userInfo;
    }
    
    [Serializable]
    public class VerifyGoogleTokenParams
    {
        public string accessToken;
        public string verifierId;
        public string chainId;
        public int operationType;
    }
    
    [Serializable]
    public class VerifyTelegramTokenParams
    {
        public string accessToken;
        public string verifierId;
        public string chainId;
        public int operationType;
    }

    [Serializable]
    public class VerifyAppleTokenParams
    {
        public string accessToken;
        public string verifierId;
        public string chainId;
        public int operationType;
    }
    
    [Serializable]
    public class SendAppleUserExtraInfoResult
    {
        public string userId;
    }
    
    public class VerifierServerResult
    {
        public string id;
        public string name;
        public string imageUrl;
    }

    /// <summary>
    /// Verification service interface for all verification services to implement.
    /// </summary>
    public interface IVerificationService
    {
        public IEnumerator GetVerificationCode(SendVerificationCodeRequestParams requestParams, SuccessCallback<SendVerificationCodeResult> successCallback, ErrorCallback errorCallback);
        public IEnumerator VerifyVerificationCode(VerifyVerificationCodeParams requestParams, SuccessCallback<VerifyVerificationCodeResult> successCallback, ErrorCallback errorCallback);
        public IEnumerator SendAppleUserExtraInfo(SendAppleUserExtraInfoParams requestParams, SuccessCallback<SendAppleUserExtraInfoResult> successCallback, ErrorCallback errorCallback);
        public IEnumerator VerifyGoogleToken(VerifyGoogleTokenParams requestParams, SuccessCallback<VerifyVerificationCodeResult> successCallback, ErrorCallback errorCallback);
        public IEnumerator VerifyTelegramToken(VerifyTelegramTokenParams requestParams, SuccessCallback<VerifyVerificationCodeResult> successCallback, ErrorCallback errorCallback);
        public IEnumerator VerifyAppleToken(VerifyAppleTokenParams requestParams, SuccessCallback<VerifyVerificationCodeResult> successCallback, ErrorCallback errorCallback);
        public IEnumerator GetVerifierServer(string chainId, SuccessCallback<VerifierServerResult> successCallback, ErrorCallback errorCallback);
        // TODO: to confirm with Sally if captcha will be needed
        // public IEnumerator CheckGoogleRecaptcha(CheckGoogleRecaptchaParams requestParams, SuccessCallback<bool> successCallback, ErrorCallback errorCallback);
    }
}
