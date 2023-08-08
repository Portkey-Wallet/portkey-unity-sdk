namespace Portkey.Core
{
    public interface ISocialProvider
    {
        ISocialLogin GetSocialLogin(AccountType type);
    }
}