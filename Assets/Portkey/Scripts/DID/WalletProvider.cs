using BIP39Wallet;
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

        public IWallet GetAccountFromEncryptedPrivateKey(byte[] encryptedPrivateKey, string password)
        {
            var privateKey = _encryption.Decrypt(encryptedPrivateKey, password);
            return GetAccountFromPrivateKey(privateKey);
        }

        public IWallet GetAccountFromPrivateKey(string privateKey)
        {
            var wallet = new Wallet();
            var newWallet = wallet.GetWalletByPrivateKey(privateKey);
            
            var keyPair = GetKeyPair(newWallet);
            
            return new AElfWallet(keyPair, _encryption);
        }

        public IWallet CreateAccount()
        {
            var wallet = new Wallet();

            var newWallet = _mnemonic switch
            {
                null => wallet.CreateWallet(128, Language.English, null),
                _ => wallet.GetWalletByMnemonic(_mnemonic)
            };
            _mnemonic = newWallet.Mnemonic;
            
            var keyPair = GetKeyPair(newWallet);

            return new AElfWallet(keyPair, _encryption);
        }

        private static KeyPair GetKeyPair(Wallet.BlockchainWallet newWallet)
        {
            // change to our own wrapper class
            var blockchainWallet = new KeyPair(newWallet.Address, newWallet.PrivateKey,
                newWallet.PublicKey);
            return blockchainWallet;
        }
    }
}