namespace Portkey.Core
{
    public delegate void SuccessCallback<T>(T result);
    public delegate void ErrorCallback(string error);
}