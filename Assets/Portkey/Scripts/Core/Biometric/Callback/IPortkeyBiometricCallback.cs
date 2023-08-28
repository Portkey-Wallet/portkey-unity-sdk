namespace Portkey.Core
{
    public interface IPortkeyBiometricCallback
    {
        IBiometric.SuccessCallback SuccessCallback { get; set; }
        ErrorCallback ErrorCallback { get; set; }
        void OnSuccess(string data);
        void OnFailure(string error);
    }
}