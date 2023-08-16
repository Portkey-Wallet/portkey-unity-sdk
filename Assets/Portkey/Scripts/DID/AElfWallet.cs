using System.Text;
using AElf;
using AElf.Types;
using Google.Protobuf;
using Portkey.Core;

namespace Portkey.DID
{
    /// <summary>
    /// EOA Wallet, used to act as management account of DID wallet.
    /// </summary>
    public class AElfWallet : WalletBase
    {
        public override Transaction SignTransaction(Transaction transaction)
        {
            var byteArray = transaction.GetHash().ToByteArray();
            var signature = Sign(byteArray);

            transaction.Signature = ByteString.CopyFrom(signature);
            return transaction;
        }

        public override byte[] Sign(string data)
        {
            var privateKeyBytes = ByteArrayHelper.HexStringToByteArray(PrivateKey);
            var signature = Sign(Encoding.UTF8.GetBytes(data));

            return signature;
        }
        
        private byte[] Sign(byte[] byteArray)
        {
            var privateKeyBytes = ByteArrayHelper.HexStringToByteArray(PrivateKey);
            var signature = BIP39Wallet.BIP39Wallet.Wallet.Sign(privateKeyBytes, byteArray);
            return signature;
        }

        public AElfWallet(KeyPair keyPair) : base(keyPair)
        {
        }
    }
}