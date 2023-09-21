namespace Portkey.Core
{
    public interface IAccountRepository
    {
        /// <summary>
        /// For saving an account.
        /// </summary>
        /// <param name="keyName">Key name to store the account.</param>
        /// <param name="password">Password to encrypt the DID Wallet Info.</param>
        /// <param name="account">The account to be encrypted.</param>
        /// <returns>True if saving is successful, false otherwise.</returns>
        public bool Save(string keyName, string password, Account account);
        
        /// <summary>
        /// For loading an account.
        /// </summary>
        /// <param name="keyName">Key name to store the account.</param>
        /// <param name="password">Password to decrypt the DID Wallet Info.</param>
        /// <param name="account">Returns the account after decrypting.</param>
        public bool Load(string keyName, string password, out Account account);
    }
}