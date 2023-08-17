using System.Linq;
using AElf.Client.Extensions;
using AElf.Types;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using NUnit.Framework;
using Portkey.Core;
using Portkey.DID;
using Portkey.Encryption;

namespace Portkey.Test
{
    public class AElfWalletTest
    {
        private const string PRIVATE_KEY = "03bd0cea9730bcfc8045248fd7f4841ea19315995c44801a3dfede0ca872f808";
        private readonly IWalletProvider _walletProvider = new WalletProvider(new AESEncryption());
        
        [Test]
        public void SignTest()
        {
            const string SIGNED =
                "59EF1D3B2B853FCA1E33D07765DEBAAF38A81442CFE90822D4334E8FCE9889D80C99A0BE1858C1F26B4D99987EFF6003F33B7C3F32BBDB9CEEC68A1E8A4DB4B000";
            var wallet = _walletProvider.GetAccountFromPrivateKey(PRIVATE_KEY);
            var result = wallet.Sign("68656c6c6f20776f726c643939482801");
            
            Assert.AreEqual(SIGNED, result.ToHexString());
        }
        
        [Test]
        public void SignTransactionTest()
        {
            var transaction = new Transaction()
            {
                From = "TrmPcaqbqmbrztv6iJuN4zeuDmQxvjF5ujbvDDQo9Q1B4ye2T".ToAddress(),
                To = "2imqjpkCwnvYzfnr61Lp2XQVN2JU17LPkA9AZzmRZzV5LRRWmR".ToAddress(),
                MethodName = "GetVerifierServers",
                Params = new Empty().ToByteString(),
                RefBlockNumber = 1,
                RefBlockPrefix = ByteString.CopyFrom(new byte[] {0x01, 0x02, 0x03, 0x04})
            };

            const string SIGNED =
                "WMfWt3EGPQ06YpVmXup4tXjZQBunSrgycHKeu3ZT4/F9oSgi2oSKN7ZhNu9dUjsDkTgRkb/2F9LDPcpTm8BgTwE\u003d";
            var wallet = _walletProvider.GetAccountFromPrivateKey(PRIVATE_KEY);
            var result = wallet.SignTransaction(transaction);
            
            Assert.AreEqual(SIGNED, result.Signature.ToBase64());
        }
    }

    internal static class ByteArrayExtension
    {
        public static string ToHexString(this byte[] bytes)
        {
            return string.Join("", bytes.Select(byteValue => byteValue.ToString("X2")));
        }
    }
}