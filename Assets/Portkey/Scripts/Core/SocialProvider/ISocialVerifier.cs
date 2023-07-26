namespace Portkey.Core
{
    public class VerifyAccessTokenParam
    {
        public string verifierId;
        public string accessToken;
        public string chainId;
    }
    
    public delegate void AuthCallback(string verifierId, VerifyVerificationCodeResult verificationResult);
    
    public interface ISocialVerifier
    {
        void AuthenticateIfAccessTokenExpired(VerifyAccessTokenParam param, AuthCallback successCallback, ErrorCallback errorCallback);
    }
}