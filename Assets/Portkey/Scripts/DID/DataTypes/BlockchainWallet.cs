using System;

namespace Portkey.DID
{
    public class BlockchainWallet
    {
        public string BIP44Path { get; private set; }
        public string Address { get; private set; }
        public string PrivateKey { get; private set; }
        public Object KeyPair { get; private set; }
        public BlockchainWallet ChildWallet { get; private set; }
        public string Mnemonic { get; private set; }
        
        public BlockchainWallet(string bip44Path, string address, string privateKey, Object keyPair, BlockchainWallet childWallet, string mnemonic)
        {
            BIP44Path = bip44Path;
            Address = address;
            PrivateKey = privateKey;
            KeyPair = keyPair;
            ChildWallet = childWallet;
            Mnemonic = mnemonic;
        }
    }
}