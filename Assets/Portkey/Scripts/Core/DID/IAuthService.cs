using Portkey.SocialProvider;

namespace Portkey.Core
{
    public interface IAuthService
    {
        EmailLogin Email { get; }
        PhoneLogin Phone { get; }
        void HasGuardian(string identifier, AccountType accountType, string token, SuccessCallback<GuardianIdentifierInfo> successCallback, ErrorCallback errorCallback);
    }
}