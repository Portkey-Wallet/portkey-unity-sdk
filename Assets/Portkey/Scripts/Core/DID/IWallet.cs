namespace Portkey.Core
{
    /// <summary>
    /// Interface for a Contract Account Wallet.
    /// </summary>
    public interface IWallet
    {
        /// <summary>
        /// For saving the Portkey DID Account.
        /// </summary>
        /// <param name="password">Password to encrypt the account.</param>
        /// <param name="keyName">The key to save or retrieve the saved data.</param>
        /// <returns>True if saving is successful, false otherwise.</returns>
        public bool Save(string password, string keyName);
        
        /// <summary>
        /// For loading the Portkey DID Account.
        /// </summary>
        /// <param name="password">Password to decrypt the account.</param>
        /// <param name="keyName">The key to load and retrieve the saved data.</param>
        /// <returns>An IWallet that contains the account details.</returns>
        public IWallet Load(string password, string keyName);
    }
}