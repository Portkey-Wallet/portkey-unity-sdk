namespace Portkey.Core
{
    public class VerifyCodeResult
    {
        public VerificationDoc verificationDoc = null;
        public string signature = null;
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