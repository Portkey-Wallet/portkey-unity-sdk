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
        //TODO: remove this and replace with PrivateKey
        public string PrivateKeyNow { get; set; }

        public override Transaction SignTransaction(Transaction transaction)
        {
            var client = new AElfClient("http://localhost:1235");
            return client.SignTransaction(PrivateKeyNow, transaction);
        }

        public override byte[] Sign(string data)
        {
            var byteData = data.GetBytes();
            var privy = ByteArrayHelper.HexStringToByteArray(PrivateKeyNow);
            return CryptoHelper.SignWithPrivateKey(privy, byteData);
        }

        public WalletAccount(BlockchainWallet wallet) : base(wallet)
        {
        }
    }
}