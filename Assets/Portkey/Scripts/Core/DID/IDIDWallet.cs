namespace Portkey.Core
{
    public interface IDIDWallet : IDIDAccountApi
    {
        /// <summary>
        /// For adding a manager account to the DID.
        /// </summary>
        /// <param name="password">Password to encrypt the DID Wallet Info.</param>
        /// <param name="keyName"></param>
        /// <returns>True if saving is successful, false otherwise.</returns>
        public bool Save(string password, string keyName);
        
        /// <summary>
        /// For loading the DID Wallet.
        /// </summary>
        /// <param name="password">Password to decrypt the DID Wallet Info.</param>
        /// <param name="keyName"></param>
        public IDIDWallet Load(string password, string keyName);
    }
}