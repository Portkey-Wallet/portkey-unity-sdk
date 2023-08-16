namespace Portkey.Core
{
    public interface IWalletProvider
    {
        public IWallet GetAccountFromPrivateKey(string privateKey);
        public IWallet CreateAccount();
    }
}