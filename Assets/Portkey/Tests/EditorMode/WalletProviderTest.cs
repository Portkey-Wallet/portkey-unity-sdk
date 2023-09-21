using NUnit.Framework;
using Portkey.Core;
using Portkey.DID;
using Portkey.Encryption;

namespace Portkey.Test
{
    public class WalletProviderTest
    {
        private const string PRIVATE_KEY = "83829798ac92d428ed13b29fe60ace1a7a10e7a347bdc9f23a85615339068f1c";
        private const string ADDRESS = "TrmPcaqbqmbrztv6iJuN4zeuDmQxvjF5ujbvDDQo9Q1B4ye2T";
        private const string PUBLIC_KEY = "045ab0516fda4eeb504ad8d7ce8a2a24e5af6004afa9ff3ee26f2c697e334be48c31597e1905711a6aa749fc475787000b5d6260bcf0d457f23c60aa60bb6c8602";
        
        private const string ANOTHER_PRIVATE_KEY = "03bd0cea9730bcfc8045248fd7f4841ea19315995c44801a3dfede0ca872f808";

        private const string PASSWORD = "password";
        
        private IEncryption _encryption = new AESEncryption();

        [Test]
        public void CreateTest()
        {
            IWalletProvider walletProvider = new WalletProvider(_encryption);
            var account = walletProvider.Create();
            
            Assert.IsNotNull(account.PublicKey);
        }
        
        [Test]
        public void GetAccountFromPrivateKeyTest()
        {
            IWalletProvider walletProvider = new WalletProvider(_encryption);
            var account = walletProvider.CreateFromPrivateKey(PRIVATE_KEY);
            
            Assert.AreEqual(ADDRESS, account.Address);
            Assert.AreEqual(PUBLIC_KEY, account.PublicKey);
        }
        
        [Test]
        public void GetAccountFromEncryptedPrivateKeyTest()
        {
            IWalletProvider walletProvider = new WalletProvider(_encryption);
            var account = walletProvider.CreateFromPrivateKey(PRIVATE_KEY);
            var encryptedPrivateKey = account.Encrypt(PASSWORD);
            var accountFromEncryptedPrivateKey = walletProvider.CreateFromEncryptedPrivateKey(encryptedPrivateKey, PASSWORD);
            
            Assert.AreEqual(account.Address, accountFromEncryptedPrivateKey.Address);
            Assert.AreEqual(account.PublicKey, accountFromEncryptedPrivateKey.PublicKey);
        }
        
        [Test]
        public void GetAccountFromPrivateKeyFailTest()
        {
            IWalletProvider walletProvider = new WalletProvider(_encryption);
            var account = walletProvider.CreateFromPrivateKey(ANOTHER_PRIVATE_KEY);
            
            Assert.AreNotEqual(PUBLIC_KEY, account.PublicKey);
        }
    }
}