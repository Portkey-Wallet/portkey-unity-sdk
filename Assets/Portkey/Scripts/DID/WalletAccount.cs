using Portkey.Core;

namespace Portkey.DID
{
    /// <summary>
    /// EOA Wallet Account.
    /// TODO: implement this when AElf C# SDK is ready.
    /// </summary>
    public class WalletAccount : AccountBase
    {
        public override Signature SignTransaction(string transaction)
        {
            throw new System.NotImplementedException();
        }

        public override byte[] Sign(string data)
        {
            throw new System.NotImplementedException();
        }

        public override KeyStore Encrypt(string password, string options = null)
        {
            throw new System.NotImplementedException();
        }

        public WalletAccount(BlockchainWallet wallet) : base(wallet)
        {
        }
    }
}