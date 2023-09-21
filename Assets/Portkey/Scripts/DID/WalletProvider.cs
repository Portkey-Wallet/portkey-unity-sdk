using BIP39Wallet;
using Portkey.Core;
using KeyPair = Portkey.Core.KeyPair;

namespace Portkey.DID
{
    public class WalletProvider : IWalletProvider
    {
        private readonly WalletFactory _walletFactory = new AElfWalletFactory();
        private readonly IEncryption _encryption;
        
        public WalletProvider(IEncryption encryption)
        {
            _encryption = encryption;
        }

        public IWallet CreateFromEncryptedPrivateKey(byte[] encryptedPrivateKey, string password)
        {
            var privateKey = _encryption.Decrypt(encryptedPrivateKey, password);
            return CreateFromPrivateKey(privateKey);
        }

        public IWallet CreateFromPrivateKey(string privateKey)
        {
            var newPrivateKey = PrivateKey.Parse(privateKey);
            
            // change to our own wrapper class
            var keyPair = new KeyPair(newPrivateKey);
            
            return new AElfWallet(keyPair, _encryption);
        }

        public IWallet Create()
        {
            var privateKey = _walletFactory.Create().PrivateKey;
            
            // change to our own wrapper class
            var keyPair = new KeyPair(privateKey);

            return new AElfWallet(keyPair, _encryption);
        }
    }
}