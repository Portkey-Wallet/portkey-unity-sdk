using System.Collections;

namespace Portkey.Core
{
    public class SendCodeParams
    {
        public string guardianId;
        public string verifierId;
        public string chainId;
        public OperationTypeEnum operationType;
    }
    
    public interface IVerifyCodeLogin
    {
        AccountType AccountType { get; }
        string VerifierId { get; }
        IEnumerator SendCode(SendCodeParams param, SuccessCallback<string> successCallback, ErrorCallback errorCallback);
        IEnumerator VerifyCode(ICodeCredential credential, SuccessCallback<VerifyCodeResult> successCallback, ErrorCallback errorCallback);
    }
}