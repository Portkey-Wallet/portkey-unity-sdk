namespace Portkey.Core
{
    public interface ISocialVerifierProvider
    {
        ISocialVerifier GetSocialVerifier(AccountType type);
    }
}