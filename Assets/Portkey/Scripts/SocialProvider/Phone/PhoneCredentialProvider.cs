using System;
using System.Collections;
using Portkey.Core;
using Portkey.Core.Captcha;

namespace Portkey.SocialProvider
{
    public class PhoneCredentialProvider : CodeCredentialProviderBase<PhoneCredential>
    {
        public PhoneCredentialProvider(IVerifyCodeLogin phoneLogin, IInternalAuthMessage message, IVerifierService verifierService, ICaptcha captcha) : base(phoneLogin, message, verifierService, captcha)
        {
        }
        
        public IEnumerator Get(PhoneNumber phoneNumber, SuccessCallback<PhoneCredential> successCallback, string chainId = null, string verifierId = null, OperationTypeEnum operationType = OperationTypeEnum.register)
        {
            chainId ??= _message.ChainId;
            yield return GetCredential(phoneNumber.String, successCallback, chainId, verifierId, operationType);
        }

        // TODO: WIP
        public ICredential Get(PhoneNumber phoneNumber, string verificationCode)
        {
            if(_codeLogin.VerifierId == null)
            {
                throw new Exception("Please call DID.AuthService.PhoneCredentialProvider.SendCode first!");
            }
            if(verificationCode == null)
            {
                throw new Exception("Please input verification code!");
            }
            
            return new PhoneCredential(phoneNumber, verificationCode, _message.ChainId, _codeLogin.VerifierId);
        }

        public override AccountType AccountType => AccountType.Phone;

        protected override PhoneCredential CreateCredential(string guardianId, string verifierId, string chainId, string code)
        {
            return new PhoneCredential(PhoneNumber.Parse(guardianId), code, chainId, verifierId);
        }
    }
}