using System.Collections;
using Portkey.Core;
using Portkey.Network;

namespace Portkey.DID
{
    //create a class inheriting from ServiceBase<FetchJsonHttp> and implementing IPortkeySocialService
    //this class will be used to make requests to the Portkey Social API
    public class PortkeySocialService : ServiceBase<IHttp>, IPortkeySocialService
    {
        public PortkeySocialService(IHttp http) : base(http)
        {
        }

        public IEnumerator GetVerificationCode(SendVerificationCodeRequestParams requestParams, IVerificationService.SuccessCallback<SendVerificationCodeResult> successCallback,
            IVerificationService.ErrorCallback errorCallback)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerator VerifyVerificationCode(VerifyVerificationCodeParams requestParams, IVerificationService.SuccessCallback<VerifyVerificationCodeResult> successCallback,
            IVerificationService.ErrorCallback errorCallback)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerator sendAppleUserExtraInfo(SendAppleUserExtraInfoParams requestParams, IVerificationService.SuccessCallback<SendAppleUserExtraInfoResult> successCallback,
            IVerificationService.ErrorCallback errorCallback)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerator verifyGoogleToken(VerifyGoogleTokenParams requestParams, IVerificationService.SuccessCallback<VerifyVerificationCodeResult> successCallback,
            IVerificationService.ErrorCallback errorCallback)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerator verifyAppleToken(VerifyAppleTokenParams requestParams, IVerificationService.SuccessCallback<VerifyVerificationCodeResult> successCallback,
            IVerificationService.ErrorCallback errorCallback)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerator GetRegisterStatus(string id, QueryOptions queryOptions, ISearchService.SuccessCallback<RegisterStatusResult> successCallback,
            ISearchService.ErrorCallback errorCallback)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerator GetRecoverStatus(string id, QueryOptions queryOptions, ISearchService.SuccessCallback<RecoverStatusResult> successCallback,
            ISearchService.ErrorCallback errorCallback)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerator GetChainsInfo(ISearchService.SuccessCallback<ChainInfo[]> successCallback, ISearchService.ErrorCallback errorCallback)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerator GetCAHolderInfo(string authorization, string caHash, ISearchService.SuccessCallback<CAHolderInfo> successCallback,
            ISearchService.ErrorCallback errorCallback)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerator Register(RegisterParams requestParams, IPortkeySocialService.SuccessCallback<RegisterResult> successCallback, IPortkeySocialService.ErrorCallback errorCallback)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerator Recovery(RecoveryParams requestParams, IPortkeySocialService.SuccessCallback<RecoveryResult> successCallback, IPortkeySocialService.ErrorCallback errorCallback)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerator GetHolderInfo(GetHolderInfoParams requestParams, IPortkeySocialService.SuccessCallback<IHolderInfo> successCallback,
            IPortkeySocialService.ErrorCallback errorCallback)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerator GetHolderInfoByManager(GetCAHolderByManagerParams requestParams, IPortkeySocialService.SuccessCallback<GetCAHolderByManagerResult> successCallback,
            IPortkeySocialService.ErrorCallback errorCallback)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerator GetRegisterInfo(GetRegisterInfoParams requestParams, IPortkeySocialService.SuccessCallback<RegisterInfo> successCallback,
            IPortkeySocialService.ErrorCallback errorCallback)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerator CheckGoogleRecaptcha(CheckGoogleRecaptchaParams requestParams, IPortkeySocialService.SuccessCallback<bool> successCallback,
            IPortkeySocialService.ErrorCallback errorCallback)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerator GetPhoneCountryCode(IPortkeySocialService.SuccessCallback<ICountryItem[]> successCallback, IPortkeySocialService.ErrorCallback errorCallback)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerator GetPhoneCountryCodeWithLocal(IPortkeySocialService.SuccessCallback<IPhoneCountryCodeResult> successCallback, IPortkeySocialService.ErrorCallback errorCallback)
        {
            throw new System.NotImplementedException();
        }
    }
}
