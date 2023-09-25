namespace Portkey.Core
{
    public interface IWalletProvider
    {
        public IWallet CreateFromEncryptedPrivateKey(byte[] encryptedPrivateKey, string password);
        public IWallet CreateFromPrivateKey(string privateKey);
        public IWallet Create();
    }
}