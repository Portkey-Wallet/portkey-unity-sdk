using Portkey.SocialProvider;

namespace Portkey.Core
{
    public class VerifyCodeResult
    {
        public VerificationDoc VerificationDoc { get; private set; }
        public string Signature { get; private set; }

        public VerifyCodeResult(VerifyVerificationCodeResult verificationResult, string verifierId)
        {
            VerificationDoc = LoginHelper.ProcessVerificationDoc(verificationResult.verificationDoc, verifierId);
            Signature = verificationResult.signature;
        }
    }
    
    public class VerificationDoc
    {
        public string verifierId;
        public string type;
        public string identifierHash;
        public string verificationTime;
        public string verifierAddress;
        public string salt;
        public string toString;
    }
}