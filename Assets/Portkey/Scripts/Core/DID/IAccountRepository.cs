namespace Portkey.Core
{
    public interface IAccountRepository
    {
        /// <summary>
        /// For saving an account.
        /// </summary>
        /// <param name="password">Password to encrypt the DID Wallet Info.</param>
        /// <param name="keyName"></param>
        /// <returns>True if saving is successful, false otherwise.</returns>
        public bool Save(string password, string keyName);
        
        /// <summary>
        /// For loading an account.
        /// </summary>
        /// <param name="password">Password to decrypt the DID Wallet Info.</param>
        /// <param name="keyName"></param>
        public bool Load(string password, string keyName);
    }
}