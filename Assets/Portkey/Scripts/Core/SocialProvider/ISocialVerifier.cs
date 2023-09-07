namespace Portkey.Core
{
    public class VerifyAccessTokenParam
    {
        public string verifierId;
        public string accessToken;
        public string chainId;
    }

    public delegate void AuthCallback(VerifyCodeResult verificationDoc, string accessToken);
    
    public interface ISocialVerifier
    {
        void AuthenticateIfAccessTokenExpired(VerifyAccessTokenParam param, AuthCallback successCallback, SuccessCallback<bool> startLoadCallback, ErrorCallback errorCallback);
    }
}