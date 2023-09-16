using System.Collections;
using Portkey.Core;

namespace Portkey.SocialProvider
{
    public class PhoneCredentialProvider : CodeCredentialProviderBase<PhoneCredential>
    {
        public PhoneCredentialProvider(IVerifyCodeLogin phoneLogin, IAuthMessage message, IVerifierService verifierService) : base(phoneLogin, message, verifierService)
        {
        }
        
        public IEnumerator Get(PhoneNumber phoneNumber, SuccessCallback<PhoneCredential> successCallback, string chainId = null, string verifierId = null, OperationTypeEnum operationType = OperationTypeEnum.register)
        {
            chainId ??= _message.ChainId;
            yield return GetCredential(phoneNumber.String, successCallback, chainId, verifierId, operationType);
        }

        public override AccountType AccountType => AccountType.Phone;

        protected override PhoneCredential CreateCredential(string guardianId, string verifierId, string chainId, string code)
        {
            return new PhoneCredential(PhoneNumber.Parse(guardianId), code, chainId, verifierId);
        }
    }
}