using System.Collections;
using Portkey.Core;
using Portkey.Utilities;

namespace Portkey.SocialProvider
{
    public class PhoneCredentialProvider
    {
        private IPortkeySocialService _portkeySocialService;
        public PhoneCredentialProvider(IPortkeySocialService portkeySocialService)
        {
            _portkeySocialService = portkeySocialService;
        }
        
        public PhoneCredential Get(PhoneNumber phoneNumber, string verificationCode)
        {
            // TODO: implement sending verification code and waiting for input before constructing PhoneCredential
            /*
            StaticCoroutine.StartCoroutine(Phone.SendCode(param, ret =>
            {
                OnSendVerificationCode?.Invoke();

                StaticCoroutine.StartCoroutine(WaitForInputCode((code) =>
                {
                    var phoneCredential = PhoneCredentialProvider.Get(PhoneNumber.Parse(param.guardianId), code);
                    successCallback(phoneCredential);
                }));
            }, OnError));
            */
            return new PhoneCredential(phoneNumber, verificationCode);
        }
    }
}