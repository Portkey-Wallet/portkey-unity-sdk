using NUnit.Framework;
using Portkey.Core;
using Portkey.DID;

namespace Portkey.Test
{
    public class AccountProviderTest
    {
        private const string PRIVATE_KEY = "03bd0cea9730bcfc8045248fd7f4841ea19315995c44801a3dfede0ca872f808";
        private const string PASSWORD = "123123";

        /*
        [Test]
        public void CreateTest()
        {
            IAccountProvider<WalletAccount> accountProvider = new AccountProvider();
            var account1 = accountProvider.CreateAccount();
            var account2 = accountProvider.CreateAccount();
            
            Assert.AreEqual(account1.Wallet.Mnemonic, account2.Wallet.Mnemonic);
        }
        
        [Test]
        public void GetAccountFromPrivateKeyTest()
        {
            IAccountProvider<WalletAccount> accountProvider = new AccountProvider();
            var account = accountProvider.GetAccountFromPrivateKey(PRIVATE_KEY);
            
            Assert.AreEqual(PRIVATE_KEY, account.PrivateKey);
        }*/
    }
}