namespace Portkey.Core
{
    public interface ICredential
    {
        AccountType AccountType { get; }
        SocialInfo SocialInfo { get; }
        string SignInToken { get; }
    }
}