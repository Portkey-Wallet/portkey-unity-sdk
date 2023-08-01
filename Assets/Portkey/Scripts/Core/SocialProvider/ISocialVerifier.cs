namespace Portkey.Core
{
    public class VerifyAccessTokenParam
    {
        public string verifierId;
        public string accessToken;
        public string chainId;
    }
    
    public class VerificationDoc
    {
        public string verifierId;
        public string type;
        public string identifierHash;
        public string verificationTime;
        public string verifierAddress;
        public string salt;
    }
    
    public delegate void AuthCallback(VerificationDoc verificationDoc, string accessToken, VerifyVerificationCodeResult verificationResult);
    
    public interface ISocialVerifier
    {
        void AuthenticateIfAccessTokenExpired(VerifyAccessTokenParam param, AuthCallback successCallback, ErrorCallback errorCallback);
    }
}