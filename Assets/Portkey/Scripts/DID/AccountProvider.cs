using Portkey.Core;
using Wallet;

namespace Portkey.DID
{
    public class AccountProvider : IAccountProvider<WalletAccount>
    {
        private string _mnemonic = null;
        
        public WalletAccount GetAccountFromPrivateKey(string privateKey)
        {
            var wallet = new global::Wallet.Wallet();
            var newWallet = wallet.GetWalletByPrivateKey(privateKey);
            
            // change to our own wrapper class
            var blockchainWallet = GetBlockchainWallet(newWallet);
            
            return new WalletAccount(blockchainWallet);
        }

        public WalletAccount CreateAccount()
        {
            var wallet = new global::Wallet.Wallet();

            var newWallet = _mnemonic switch
            {
                null => wallet.CreateWallet(128, Language.English, null),
                _ => wallet.GetWalletByMnemonic(_mnemonic)
            };
            _mnemonic = newWallet.Mnemonic;

            // change to our own wrapper class
            var blockchainWallet = GetBlockchainWallet(newWallet);
            
            return new WalletAccount(blockchainWallet);
        }

        private static BlockchainWallet GetBlockchainWallet(global::Wallet.Wallet.BlockchainWallet newWallet)
        {
            var blockchainWallet = new BlockchainWallet(newWallet.Address, newWallet.PrivateKey, newWallet.Mnemonic,
                newWallet.PublicKey);
            return blockchainWallet;
        }
    }
}