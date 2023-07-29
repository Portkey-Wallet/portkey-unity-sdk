using System.Text;
using AElf.Types;
using BIP39.HDWallet.Core;
using BIP39Wallet;
using Google.Protobuf;
using Portkey.Core;

namespace Portkey.DID
{
    /// <summary>
    /// EOA Wallet, used to act as management account of DID wallet.
    /// </summary>
    public class WalletAccount : AccountBase
    {
        public override Transaction SignTransaction(Transaction transaction)
        {
            var wallet = new Wallet();
            var byteArray = transaction.GetHash().ToByteArray();
            var signature = wallet.Sign(PrivateKey.FromHexToByteArray(), byteArray);

            transaction.Signature = ByteString.CopyFrom(signature);
            return transaction;
        }

        public override byte[] Sign(string data)
        {
            var wallet = new Wallet();
            var signature = wallet.Sign(PrivateKey.FromHexToByteArray(), Encoding.UTF8.GetBytes(data));

            return signature;
        }

        public WalletAccount(BlockchainWallet wallet) : base(wallet)
        {
        }
    }
}