namespace Portkey.Core
{
    public interface IAccountProvider<T> where T : IAccountMethods
    {
        public T GetAccountFromPrivateKey(string privateKey);
        public T CreateAccount();
        public T Decrypt(string keyStore, string password);
    }
}