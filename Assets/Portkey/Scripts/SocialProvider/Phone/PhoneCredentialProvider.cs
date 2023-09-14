using System.Collections;
using Portkey.Core;

namespace Portkey.SocialProvider
{
    public class PhoneCredentialProvider
    {
        private IPortkeySocialService _portkeySocialService;
        public PhoneCredentialProvider(IPortkeySocialService portkeySocialService)
        {
            _portkeySocialService = portkeySocialService;
        }
        
        public IEnumerator SendVerificationCode (SendCodeParams param, SuccessCallback<string> successCallback, ErrorCallback errorCallback)
        {
            yield return _portkeySocialService.SendCode(param, successCallback, errorCallback);
        }
        
        public PhoneCredential Get(PhoneNumber phoneNumber, string verificationCode)
        {
            return new PhoneCredential(phoneNumber, verificationCode);
        }
    }
}