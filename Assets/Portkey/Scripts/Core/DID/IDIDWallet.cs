using System.Collections;

namespace Portkey.Core
{
    public interface IDIDWallet : IDIDAccountApi
    {
        /// <summary>
        /// Initializes the DID Wallet with a new EOA if it does not have one.
        /// </summary>
        public void InitializeAccount();
        
        /// <summary>
        /// For saving the DID Wallet.
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
        public void Load(string password, string keyName);
    }
}