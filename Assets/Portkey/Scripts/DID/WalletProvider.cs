using NBitcoin;
using Portkey.Core;
using KeyPair = Portkey.Core.KeyPair;

namespace Portkey.DID
{
    public class WalletProvider : IWalletProvider
    {
        private string _mnemonic = null;
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
            var newWallet = BIP39Wallet.BIP39Wallet.Wallet.GetWalletByPrivateKey(privateKey);
            
            // change to our own wrapper class
            var keyPair = GetKeyPair(newWallet);
            
            return new AElfWallet(keyPair, _encryption);
        }

        public IWallet Create()
        {
            var newWallet = _mnemonic switch
            {
                null => BIP39Wallet.BIP39Wallet.Wallet.CreateWallet(128, Language.English, null),
                _ => BIP39Wallet.BIP39Wallet.Wallet.GetWalletByMnemonic(_mnemonic)
            };
            _mnemonic = newWallet.Mnemonic;
            
            // change to our own wrapper class
            var keyPair = GetKeyPair(newWallet);

            return new AElfWallet(keyPair, _encryption);
        }

        private static KeyPair GetKeyPair(BIP39Wallet.BIP39Wallet.Wallet.BlockchainWallet newWallet)
        {
            var keyPair = new KeyPair(newWallet.Address, newWallet.PrivateKey, newWallet.PublicKey);
            return keyPair;
        }
    }
}