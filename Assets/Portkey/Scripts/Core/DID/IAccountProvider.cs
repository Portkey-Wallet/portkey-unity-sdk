namespace Portkey.Core
{
    public interface IAccountProvider<T> where T : ISigner
    {
        public T GetAccountFromPrivateKey(string privateKey);
        public T CreateAccount();
    }
}