using System.Collections;
using Portkey.Core;

namespace Portkey.Test
{
    public class PortkeySocialServiceMock : IPortkeySocialService
    {
        public IEnumerator GetVerificationCode(SendVerificationCodeRequestParams requestParams, SuccessCallback<SendVerificationCodeResult> successCallback,
            ErrorCallback errorCallback)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerator VerifyVerificationCode(VerifyVerificationCodeParams requestParams, SuccessCallback<VerifyVerificationCodeResult> successCallback,
            ErrorCallback errorCallback)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerator SendAppleUserExtraInfo(SendAppleUserExtraInfoParams requestParams, SuccessCallback<SendAppleUserExtraInfoResult> successCallback,
            ErrorCallback errorCallback)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerator VerifyGoogleToken(VerifyGoogleTokenParams requestParams, SuccessCallback<VerifyVerificationCodeResult> successCallback,
            ErrorCallback errorCallback)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerator VerifyAppleToken(VerifyAppleTokenParams requestParams, SuccessCallback<VerifyVerificationCodeResult> successCallback,
            ErrorCallback errorCallback)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerator GetVerifierServer(string chainId, SuccessCallback<VerifierServerResult> successCallback, ErrorCallback errorCallback)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerator GetRegisterStatus(string sessionId, QueryOptions queryOptions, SuccessCallback<RegisterStatusResult> successCallback,
            ErrorCallback errorCallback)
        {
            successCallback(new RegisterStatusResult
            {
                caAddress = "caAddress_mock",
                caHash = "caHash_mock",
                registerStatus = "pass"
            });
            yield break;
        }

        public IEnumerator GetRecoverStatus(string sessionId, QueryOptions queryOptions, SuccessCallback<RecoverStatusResult> successCallback,
            ErrorCallback errorCallback)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerator GetChainsInfo(SuccessCallback<ArrayWrapper<ChainInfo>> successCallback, ErrorCallback errorCallback)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerator GetCAHolderInfo(string authorization, string caHash, SuccessCallback<CAHolderInfo> successCallback,
            ErrorCallback errorCallback)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerator Register(RegisterParams requestParams, SuccessCallback<SessionIdResult> successCallback, ErrorCallback errorCallback)
        {
            successCallback(new SessionIdResult
            {
                sessionId = "sessionId_mock"
            });
            yield break;
        }

        public IEnumerator Recovery(RecoveryParams requestParams, SuccessCallback<SessionIdResult> successCallback, ErrorCallback errorCallback)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerator GetHolderInfo(GetHolderInfoParams requestParams, SuccessCallback<IHolderInfo> successCallback,
            ErrorCallback errorCallback)
        {
            successCallback(new IHolderInfo
            {
                caAddress = "caAddress_mock",
                caHash = "caHash_mock"
            });
            yield break;
        }

        public IEnumerator GetHolderInfoByManager(GetCAHolderByManagerParams requestParams, SuccessCallback<GetCAHolderByManagerResult> successCallback,
            ErrorCallback errorCallback)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerator GetRegisterInfo(GetRegisterInfoParams requestParams, SuccessCallback<RegisterInfo> successCallback,
            ErrorCallback errorCallback)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerator CheckGoogleRecaptcha(CheckGoogleRecaptchaParams requestParams, SuccessCallback<bool> successCallback,
            ErrorCallback errorCallback)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerator GetPhoneCountryCodeWithLocal(SuccessCallback<IPhoneCountryCodeResult> successCallback, ErrorCallback errorCallback)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerator IsAccountDeletionPossible(ConnectToken connectToken, SuccessCallback<bool> successCallback,
            ErrorCallback errorCallback)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerator ValidateAccountDeletion(ConnectToken connectToken, SuccessCallback<AccountDeletionValidationResult> successCallback,
            ErrorCallback errorCallback)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerator DeleteAccount(ConnectToken connectToken, string appleToken, SuccessCallback<bool> successCallback,
            ErrorCallback errorCallback)
        {
            throw new System.NotImplementedException();
        }
    }
}