using System.Collections;

namespace Portkey.Core
{
    public class SendCodeParams
    {
        public string guardianId;
        public string verifierId;
        public string chainId;
        public string captchaToken;
        public OperationTypeEnum operationType;
    }
    
    public class ProcessingInfo
    {
        public string guardianId;
        public string verifierId;
        public string chainId;
    }
    
    public interface IVerifyCodeLogin
    {
        AccountType AccountType { get; }
        bool IsProcessingAccount(string guardianId, out ProcessingInfo processingInfo);
        IEnumerator SendCode(SendCodeParams param, SuccessCallback<string> successCallback, ErrorCallback errorCallback);
        IEnumerator VerifyCode(ICodeCredential credential, OperationTypeEnum operationType, SuccessCallback<VerifyCodeResult> successCallback, ErrorCallback errorCallback);
    }
}