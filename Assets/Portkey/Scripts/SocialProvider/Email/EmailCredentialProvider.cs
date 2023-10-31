using System;
using System.Collections;
using Portkey.Core;
using Portkey.Core.Captcha;

namespace Portkey.SocialProvider
{
    public class EmailCredentialProvider : CodeCredentialProviderBase<EmailCredential>
    {
        public EmailCredentialProvider(IVerifyCodeLogin emailLogin, IInternalAuthMessage message, IVerifierService verifierService, ICaptcha captcha) : base(emailLogin, message, verifierService, captcha)
        {
        }
        
        public IEnumerator Get(EmailAddress emailAddress, SuccessCallback<EmailCredential> successCallback, string chainId = null, string verifierId = null, OperationTypeEnum operationType = OperationTypeEnum.register)
        {
            chainId ??= _message.ChainId;
            yield return GetCredential(emailAddress.String, successCallback, chainId, verifierId, operationType);
        }
        
        // TODO: WIP
        public ICredential Get(PhoneNumber phoneNumber, string verificationCode)
        {
            if(_codeLogin.VerifierId == null)
            {
                throw new Exception("Please call DID.AuthService.EmailCredentialProvider.SendCode first!");
            }
            if(verificationCode == null)
            {
                throw new Exception("Please input verification code!");
            }
            
            return new PhoneCredential(phoneNumber, verificationCode, _message.ChainId, _codeLogin.VerifierId);
        }

        public override AccountType AccountType => AccountType.Email;

        protected override EmailCredential CreateCredential(string guardianId, string verifierId, string chainId, string code)
        {
            return new EmailCredential(EmailAddress.Parse(guardianId), code, chainId, verifierId);
        }
    }
}