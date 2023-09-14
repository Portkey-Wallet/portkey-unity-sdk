using System.Collections;
using Portkey.Core;

namespace Portkey.SocialProvider
{
    public class PhoneCredentialProvider : CodeCredentialProviderBase<PhoneCredential>
    {
        public PhoneCredentialProvider(IVerifyCodeLogin phoneLogin, IAuthMessage message, IVerifierService verifierService) : base(phoneLogin, message, verifierService)
        {
        }
        
        public IEnumerator Get(PhoneNumber phoneNumber, SuccessCallback<PhoneCredential> successCallback, string chainId = "AELF", string verifierId = null, OperationTypeEnum operationType = OperationTypeEnum.register)
        {
            yield return GetCredential(phoneNumber.String, successCallback, chainId, verifierId, operationType);
        }

        protected override PhoneCredential CreateCredential(string guardianId, string verifierId, string chainId, string code)
        {
            return new PhoneCredential(PhoneNumber.Parse(guardianId), code, chainId, verifierId);
        }
    }
}