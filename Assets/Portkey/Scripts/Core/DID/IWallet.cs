namespace Portkey.Core
{
    /// <summary>
    /// Interface for a Contract Account Wallet.
    /// </summary>
    public interface IWallet
    {
        /// <summary>
        /// Initializes the Wallet with a new Account.
        /// </summary>
        public void Create();
        
        /// <summary>
        /// For saving the Wallet.
        /// </summary>
        /// <param name="password">Password to encrypt the wallet.</param>
        /// <param name="keyName"></param>
        /// <returns>True if saving is successful, false otherwise.</returns>
        public bool Save(string password, string keyName);
        
        /// <summary>
        /// For loading a Wallet.
        /// </summary>
        /// <param name="password">Password to decrypt the wallet.</param>
        /// <param name="keyName"></param>
        public void Load(string password, string keyName);
    }
}