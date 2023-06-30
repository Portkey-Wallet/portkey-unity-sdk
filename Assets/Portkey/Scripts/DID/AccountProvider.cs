using Portkey.Core;

namespace Portkey.DID
{
    public class AccountProvider : IAccountProvider<AccountBase>
    {
        public AccountBase GetAccountFromPrivateKey(string privateKey)
        {
            
            throw new System.NotImplementedException();
        }

        public AccountBase CreateAccount()
        {
            throw new System.NotImplementedException();
        }

        public AccountBase Decrypt(string keyStore, string password)
        {
            throw new System.NotImplementedException();
        }
    }
}