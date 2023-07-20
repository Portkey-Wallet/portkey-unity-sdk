using AElf.Client;
using Portkey.Core;
using Wallet;
using Wallet = Wallet.Wallet;

namespace Portkey.DID
{
    public class AccountProvider : IAccountProvider<WalletAccount>
    {
        public WalletAccount GetAccountFromPrivateKey(string privateKey)
        {
            throw new System.NotImplementedException();
        }

        public WalletAccount CreateAccount()
        {
            var wallet = new global::Wallet.Wallet();
            var accountInfo = wallet.CreateWallet(128, Language.English, null);
            throw new System.NotImplementedException();
        }
    }
}