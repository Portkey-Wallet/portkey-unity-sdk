namespace Portkey.Core
{
    public class VerifyAccessTokenParam
    {
        public string verifierId;
        public string accessToken;
        public string chainId;
        public int operationType;
    }

    public delegate void AuthCallback(VerifyCodeResult verificationDoc, string accessToken);
    
    public interface ISocialVerifier
    {
        void AuthenticateIfAccessTokenExpired(VerifyAccessTokenParam param, AuthCallback successCallback, ErrorCallback errorCallback);
    }
}