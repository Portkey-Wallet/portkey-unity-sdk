using System;

namespace Portkey.Core
{
    /// <summary>
    /// A class that holds the address, private key, and mnemonic of a standard EOA wallet.
    /// </summary>
    public class BlockchainWallet
    {
        public string Address { get; private set; }
        public string PrivateKey { get; private set; }
        public string Mnemonic { get; private set; }
        
        public BlockchainWallet(string address, string privateKey, string mnemonic)
        {
            Address = address;
            PrivateKey = privateKey;
            Mnemonic = mnemonic;
        }
    }
}