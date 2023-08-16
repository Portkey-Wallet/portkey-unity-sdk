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
    public class AElfWallet : IWallet
    {
        private readonly KeyPair _keyPair;

        public Transaction SignTransaction(Transaction transaction)
        {
            var byteArray = transaction.GetHash().ToByteArray();
            var numArray = CryptoHelper.SignWithPrivateKey(ByteArrayHelper.HexStringToByteArray(_keyPair.PrivateKey), byteArray);
            transaction.Signature = ByteString.CopyFrom(numArray);
            return transaction;
        }

        public byte[] Sign(string data)
        {
            var byteData = data.GetBytes();
            var privy = ByteArrayHelper.HexStringToByteArray(_keyPair.PrivateKey);
            return CryptoHelper.SignWithPrivateKey(privy, byteData);
        }

        public AElfWallet(KeyPair keyPair)
        {
            _keyPair = keyPair;
        }

        public string Address => _keyPair.Address;
        public string PublicKey => _keyPair.PublicKey;
    }
}