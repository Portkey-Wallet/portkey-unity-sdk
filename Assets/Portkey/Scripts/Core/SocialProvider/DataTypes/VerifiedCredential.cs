namespace Portkey.Core
{
    public class VerifiedCredential : ICredential
    {
        private readonly ICredential _credential;
        public AccountType AccountType => _credential.AccountType;
        public SocialInfo SocialInfo => _credential.SocialInfo;
        public string SignInToken => _credential.SignInToken;
        public VerificationDoc VerificationDoc { get; private set; }
        public string Signature { get; private set; }

        public VerifiedCredential(ICredential credential, VerificationDoc verificationDoc, string signature)
        {
            _credential = credential;
            VerificationDoc = verificationDoc;
            Signature = signature;
        }
    }
}