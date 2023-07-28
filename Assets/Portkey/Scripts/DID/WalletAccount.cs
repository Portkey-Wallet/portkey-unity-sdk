using AElf;
using AElf.Cryptography;
using AElf.Types;
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
            var byteArray = transaction.GetHash().ToByteArray();
            var numArray = CryptoHelper.SignWithPrivateKey(ByteArrayHelper.HexStringToByteArray(PrivateKey), byteArray);
            transaction.Signature = ByteString.CopyFrom(numArray);
            return transaction;
        }

        public override byte[] Sign(string data)
        {
            var byteData = data.GetBytes();
            var privy = ByteArrayHelper.HexStringToByteArray(PrivateKey);
            return CryptoHelper.SignWithPrivateKey(privy, byteData);
        }

        public WalletAccount(ExternallyOwnedAccount wallet) : base(wallet)
        {
        }
    }
}