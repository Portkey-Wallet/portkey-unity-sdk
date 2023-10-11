using System;

namespace Portkey.Core
{
    public interface IPortkeySocialLoginCallback
    {
        Action<string> OnSuccessCallback { get; set; }
        Action<string> OnFailureCallback { get; set; }
        void OnSuccess(string data);
        void OnFailure(string error);
    }
}