using Portkey.Core;

namespace Portkey.DID
{
    public class AccountProvider : IAccountProvider<WalletAccount>
    {
        public WalletAccount GetAccountFromPrivateKey(string privateKey)
        {
            
            throw new System.NotImplementedException();
        }

        public WalletAccount CreateAccount()
        {
            throw new System.NotImplementedException();
        }

        public WalletAccount Decrypt(string keyStore, string password)
        {
            throw new System.NotImplementedException();
        }
    }
}