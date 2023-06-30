using Portkey.Core;

namespace Portkey.DID
{
    public class WalletAccount : IAccountMethods
    {
        public Signature SignTransaction(string transaction)
        {
            throw new System.NotImplementedException();
        }

        public byte[] Sign(string data)
        {
            throw new System.NotImplementedException();
        }

        public KeyStore Encrypt(string password, string options = null)
        {
            throw new System.NotImplementedException();
        }
    }
}