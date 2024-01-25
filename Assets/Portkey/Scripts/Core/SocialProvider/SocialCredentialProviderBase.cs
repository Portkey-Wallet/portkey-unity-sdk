using System;
using System.Collections;

namespace Portkey.Core
{
    public abstract class SocialCredentialProviderBase<T> : ICredentialProvider where T : ICredential
    {
        protected readonly IAuthMessage authMessage;
        protected readonly IVerifierService verifierService;
        protected readonly ISocialVerifierProvider socialVerifierProvider;

        protected SocialCredentialProviderBase(IAuthMessage authMessage, IVerifierService verifierService, ISocialVerifierProvider socialVerifierProvider)
        {
            this.authMessage = authMessage;
            this.verifierService = verifierService;
            this.socialVerifierProvider = socialVerifierProvider;
        }
        
        public abstract AccountType AccountType { get; }
        
        public IEnumerator Verify(ICredential credential, SuccessCallback<VerifiedCredential> successCallback, OperationTypeEnum operationType = OperationTypeEnum.register)
        {
            Debugger.Log($"verifying {credential}");
            if (credential is not T cred)
            {
                throw new Exception("Invalid credential type!");
            }
            Debugger.Log($"aurthmsg is {authMessage}");
            var chainId = authMessage.ChainId;
            
            authMessage.Loading(true, "Assigning a verifier on-chain...");
            yield return verifierService.GetVerifierServer(chainId, verifierServer =>
            {
                authMessage.Loading(true, "Loading...");
                var socialVerifier = socialVerifierProvider.GetSocialVerifier(credential.AccountType);
                var param = new VerifyAccessTokenParam
                {
                    verifierId = verifierServer.id,
                    accessToken = credential.SignInToken,
                    chainId = chainId,
                    operationType = (int)operationType
                };
                socialVerifier.AuthenticateIfAccessTokenExpired(param, (result, token) =>
                {
                    successCallback?.Invoke(new VerifiedCredential(credential, chainId, result.verificationDoc, result.signature));
                }, authMessage.Error);
            }, authMessage.Error);
        }
    }
}