namespace Portkey.Core
{
    public interface IPortkeySocialLoginCallback
    {
        ISocialLogin SocialLogin { get; set; }
        void OnSuccess(string data);
        void OnFailure(string error);
    }
}