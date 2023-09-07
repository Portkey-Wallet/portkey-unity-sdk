using System.Collections;

namespace Portkey.Core
{
    public class SendCodeParams
    {
        public string guardianId;
        public string verifierId;
        public string chainId;
    }
    
    public interface IVerifyCodeLogin
    {
        AccountType AccountType { get; }
        IEnumerator SendCode(SendCodeParams param, SuccessCallback<string> successCallback, ErrorCallback errorCallback);
        IEnumerator VerifyCode(string code, SuccessCallback<VerifyVerificationCodeResult> successCallback, ErrorCallback errorCallback);
    }
}