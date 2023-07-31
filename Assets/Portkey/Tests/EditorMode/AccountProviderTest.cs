using NUnit.Framework;
using Portkey.Core;
using Portkey.DID;

namespace Portkey.Test
{
    public class AccountProviderTest
    {
        private const string PRIVATE_KEY = "83829798ac92d428ed13b29fe60ace1a7a10e7a347bdc9f23a85615339068f1c";
        private const string ADDRESS = "TrmPcaqbqmbrztv6iJuN4zeuDmQxvjF5ujbvDDQo9Q1B4ye2T";
        private const string PUBLIC_KEY = "045ab0516fda4eeb504ad8d7ce8a2a24e5af6004afa9ff3ee26f2c697e334be48c31597e1905711a6aa749fc475787000b5d6260bcf0d457f23c60aa60bb6c8602";
        
        private const string ANOTHER_PRIVATE_KEY = "03bd0cea9730bcfc8045248fd7f4841ea19315995c44801a3dfede0ca872f808";
        
        [Test]
        public void CreateTest()
        {
            IAccountProvider<WalletAccount> accountProvider = new AccountProvider();
            var account1 = accountProvider.CreateAccount();
            var account2 = accountProvider.CreateAccount();
            
            Assert.AreEqual(account1.KeyPair.PrivateKey, account2.KeyPair.PrivateKey);
        }
        
        [Test]
        public void GetAccountFromPrivateKeyTest()
        {
            IAccountProvider<WalletAccount> accountProvider = new AccountProvider();
            var account = accountProvider.GetAccountFromPrivateKey(PRIVATE_KEY);
            
            Assert.AreEqual(PRIVATE_KEY, account.PrivateKey);
            Assert.AreEqual(PUBLIC_KEY, account.PublicKey);
        }
        
        [Test]
        public void GetAccountFromPrivateKeyFailTest()
        {
            IAccountProvider<WalletAccount> accountProvider = new AccountProvider();
            var account = accountProvider.GetAccountFromPrivateKey(ANOTHER_PRIVATE_KEY);
            
            Assert.AreNotEqual(PRIVATE_KEY, account.PrivateKey);
        }
    }
}