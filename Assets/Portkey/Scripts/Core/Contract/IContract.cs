using System.Collections;

namespace Portkey.Core
{
    public class SendOptions
    {
        public string From { get; private set; }
        public string GasPrice { get; private set; }
        public int Gas { get; private set; }
        public int Value { get; private set; }
        public int Nonce { get; private set; }
        public string OnMethod { get; private set; }
        
        public SendOptions(string onMethod, string from = null, string gasPrice = null, int gas = -1, int value = -1, int nonce = -1)
        {
            From = from;
            GasPrice = gasPrice;
            Gas = gas;
            Value = value;
            Nonce = nonce;
            OnMethod = onMethod;
        }
    }

    public interface IContract
    {
        IEnumerator CallViewMethod(string methodName, object[] paramOptions, IHttp.successCallback successCallback, IHttp.errorCallback errorCallback);
        IEnumerator CallSendMethod(string methodName, object[] paramOptions, SendOptions sendOptions, IHttp.successCallback successCallback, IHttp.errorCallback errorCallback);
        IEnumerator EncodeTx(string methodName, object[] paramOptions, IHttp.successCallback successCallback, IHttp.errorCallback errorCallback);
    }
}