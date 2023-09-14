using System.Collections;
using Portkey.Core;

namespace Portkey.SocialProvider
{
    public class EmailCredentialProvider : CodeCredentialProviderBase<EmailCredential>
    {
        public EmailCredentialProvider(IVerifyCodeLogin emailLogin, IAuthMessage message, IVerifierService verifierService) : base(emailLogin, message, verifierService)
        {
        }
        
        public IEnumerator Get(EmailAddress emailAddress, SuccessCallback<EmailCredential> successCallback, string chainId = "AELF", string verifierId = null, OperationTypeEnum operationType = OperationTypeEnum.register)
        {
            yield return GetCredential(emailAddress.String, successCallback, chainId, verifierId, operationType);
        }

        protected override EmailCredential CreateCredential(string guardianId, string verifierId, string chainId, string code)
        {
            return new EmailCredential(EmailAddress.Parse(guardianId), code, chainId, verifierId);
        }
    }
}