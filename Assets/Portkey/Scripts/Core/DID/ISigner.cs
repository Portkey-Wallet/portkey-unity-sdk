using System.Collections;
using AElf.Types;

namespace Portkey.Core
{
    public interface ISigner
    {
        public IEnumerator SignTransaction(Transaction transaction, SuccessCallback<Transaction> successCallback, ErrorCallback errorCallback);
        public IEnumerator Sign(string data, SuccessCallback<byte[]> successCallback, ErrorCallback errorCallback);
    }
}