using NBitcoin;
using Portkey.Core;

namespace Portkey.DID
{
    public class AccountProvider : IAccountProvider<WalletAccount>
    {
        private string _mnemonic = null;
        
        public WalletAccount GetAccountFromPrivateKey(string privateKey)
        {
            var newWallet = BIP39Wallet.BIP39Wallet.Wallet.GetWalletByPrivateKey(privateKey);
            
            // change to our own wrapper class
            var blockchainWallet = GetBlockchainWallet(newWallet);
            
            return new WalletAccount(blockchainWallet);
        }

        public WalletAccount CreateAccount()
        {
            var newWallet = _mnemonic switch
            {
                null => BIP39Wallet.BIP39Wallet.Wallet.CreateWallet(128, Language.English, null),
                _ => BIP39Wallet.BIP39Wallet.Wallet.GetWalletByMnemonic(_mnemonic)
            };
            _mnemonic = newWallet.Mnemonic;

            // change to our own wrapper class
            var blockchainWallet = GetBlockchainWallet(newWallet);
            
            return new WalletAccount(blockchainWallet);
        }

        private static BlockchainWallet GetBlockchainWallet(BIP39Wallet.BIP39Wallet.Wallet.BlockchainWallet newWallet)
        {
            var blockchainWallet = new BlockchainWallet(newWallet.Address, newWallet.PrivateKey, newWallet.Mnemonic,
                newWallet.PublicKey);
            return blockchainWallet;
        }
    }
}