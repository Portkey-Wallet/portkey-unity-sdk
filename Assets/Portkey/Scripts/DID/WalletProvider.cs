using NBitcoin;
using Portkey.Core;
using KeyPair = Portkey.Core.KeyPair;

namespace Portkey.DID
{
    public class WalletProvider : IWalletProvider
    {
        private string _mnemonic = null;
        
        public IWallet GetAccountFromPrivateKey(string privateKey)
        {
            var newWallet = BIP39Wallet.BIP39Wallet.Wallet.GetWalletByPrivateKey(privateKey);
            
            // change to our own wrapper class
            var blockchainWallet = GetBlockchainWallet(newWallet);
            
            return new AElfWallet(blockchainWallet);
        }

        public IWallet CreateAccount()
        {
            var newWallet = _mnemonic switch
            {
                null => BIP39Wallet.BIP39Wallet.Wallet.CreateWallet(128, Language.English, null),
                _ => BIP39Wallet.BIP39Wallet.Wallet.GetWalletByMnemonic(_mnemonic)
            };
            _mnemonic = newWallet.Mnemonic;

            // change to our own wrapper class
            var blockchainWallet = GetBlockchainWallet(newWallet);
            
            return new AElfWallet(blockchainWallet);
        }

        private static KeyPair GetBlockchainWallet(BIP39Wallet.BIP39Wallet.Wallet.BlockchainWallet newWallet)
        {
            var keyPair = new KeyPair(newWallet.Address, newWallet.PrivateKey, newWallet.PublicKey);
            return keyPair;
        }
    }
}