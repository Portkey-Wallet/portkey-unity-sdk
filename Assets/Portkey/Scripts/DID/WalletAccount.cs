using AElf;
using AElf.Client;
using AElf.Cryptography;
using AElf.Types;
using Portkey.Core;

namespace Portkey.DID
{
    /// <summary>
    /// EOA Wallet Account.
    /// TODO: implement this when AElf C# SDK is ready.
    /// </summary>
    public class WalletAccount : AccountBase
    {
        public override Transaction SignTransaction(Transaction transaction)
        {
            var client = new AElfClient("http://localhost:1235");
            return client.SignTransaction(PrivateKey, transaction);
        }

        public override byte[] Sign(string data)
        {
            var byteData = data.GetBytes();
            var client = new AElfClient("http://localhost:1235");
            return CryptoHelper.SignWithPrivateKey(ByteArrayHelper.HexStringToByteArray(PrivateKey), byteData);
        }

        public WalletAccount(BlockchainWallet wallet) : base(wallet)
        {
        }
    }
}