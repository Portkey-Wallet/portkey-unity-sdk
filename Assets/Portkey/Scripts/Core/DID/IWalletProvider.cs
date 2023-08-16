namespace Portkey.Core
{
    public interface IWalletProvider
    {
        public WalletBase GetAccountFromPrivateKey(string privateKey);
        public WalletBase CreateAccount();
    }
}