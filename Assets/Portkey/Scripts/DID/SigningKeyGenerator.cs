using AElf.HdWallet;
using Portkey.Core;
using KeyPair = Portkey.Core.KeyPair;

namespace Portkey.DID
{
    public class SigningKeyGenerator : ISigningKeyGenerator
    {
        private readonly WalletFactory _walletFactory = new AElfWalletFactory();
        private readonly IEncryption _encryption;
        
        public SigningKeyGenerator(IEncryption encryption)
        {
            _encryption = encryption;
        }

        public ISigningKey CreateFromEncryptedPrivateKey(byte[] encryptedPrivateKey, string password)
        {
            var privateKey = _encryption.Decrypt(encryptedPrivateKey, password);
            return CreateFromPrivateKey(privateKey);
        }

        public ISigningKey CreateFromPrivateKey(string privateKey)
        {
            var newPrivateKey = PrivateKey.Parse(privateKey);
            
            // change to our own wrapper class
            var keyPair = new KeyPair(newPrivateKey);
            
            return new AElfSigningKey(keyPair, _encryption);
        }

        public ISigningKey Create()
        {
            var privateKey = _walletFactory.Create().PrivateKey;
            
            // change to our own wrapper class
            var keyPair = new KeyPair(privateKey);

            return new AElfSigningKey(keyPair, _encryption);
        }
    }
}