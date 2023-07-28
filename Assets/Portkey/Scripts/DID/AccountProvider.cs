using BIP39Wallet;
using Portkey.Core;

namespace Portkey.DID
{
    public class AccountProvider : IAccountProvider<WalletAccount>
    {
        private string _mnemonic = null;
        
        public WalletAccount GetAccountFromPrivateKey(string privateKey)
        {
            var wallet = new Wallet();
            var newWallet = wallet.GetWalletByPrivateKey(privateKey);
            
            // change to our own wrapper class
            var blockchainWallet = GetBlockchainWallet(newWallet);
            
            return new WalletAccount(blockchainWallet);
        }

        public WalletAccount CreateAccount()
        {
            var wallet = new Wallet();

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

        private static ExternallyOwnedAccount GetBlockchainWallet(Wallet.BlockchainWallet newWallet)
        {
            var blockchainWallet = new ExternallyOwnedAccount(newWallet.Address, newWallet.PrivateKey,
                newWallet.PublicKey);
            return blockchainWallet;
        }
    }
}