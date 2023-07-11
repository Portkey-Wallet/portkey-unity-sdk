using System;

namespace Portkey.Core
{
    public class BlockchainWallet
    {
        public string Address { get; private set; }
        public string PrivateKey { get; private set; }
        public string Mnemonic { get; private set; }
        
        public BlockchainWallet(string bip44Path, string address, string privateKey, Object keyPair, BlockchainWallet childWallet, string mnemonic)
        {
            Address = address;
            PrivateKey = privateKey;
            Mnemonic = mnemonic;
        }
    }
}