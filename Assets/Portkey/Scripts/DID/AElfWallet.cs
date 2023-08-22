using System.Text;
using AElf;
using AElf.Types;
using Google.Protobuf;
using Portkey.Core;
using KeyPair = Portkey.Core.KeyPair;

namespace Portkey.DID
{
    /// <summary>
    /// EOA Wallet, used to act as management account of DID wallet.
    /// </summary>
    public class AElfWallet : IWallet
    {
        private readonly KeyPair _keyPair;
        private readonly IEncryption _encryption;
        
        public string Address => _keyPair.Address;
        public string PublicKey => _keyPair.PublicKey;

        public AElfWallet(KeyPair keyPair, IEncryption encryption)
        {
            _keyPair = keyPair;
            _encryption = encryption;
        }
        
        public Transaction SignTransaction(Transaction transaction)
        {
            var byteArray = transaction.GetHash().ToByteArray();
            var signature = Sign(byteArray);

            transaction.Signature = ByteString.CopyFrom(signature);
            return transaction;
        }

        public byte[] Sign(string data)
        {
            var privateKeyBytes = ByteArrayHelper.HexStringToByteArray(_keyPair.PrivateKey);
            var signature = Sign(Encoding.UTF8.GetBytes(data));

            return signature;
        }
        
        private byte[] Sign(byte[] byteArray)
        {
            var privateKeyBytes = ByteArrayHelper.HexStringToByteArray(_keyPair.PrivateKey);
            var signature = BIP39Wallet.BIP39Wallet.Wallet.Sign(privateKeyBytes, byteArray);
            return signature;
        }

        public byte[] Encrypt(string password)
        {
            return _encryption.Encrypt(_keyPair.PrivateKey, password);
        }
    }
}