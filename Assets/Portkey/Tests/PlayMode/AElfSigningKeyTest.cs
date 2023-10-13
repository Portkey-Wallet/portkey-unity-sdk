using System.Collections;
using System.Linq;
using AElf;
using AElf.Types;
using Google.Protobuf;
using NUnit.Framework;
using Portkey.Core;
using Portkey.DID;
using Portkey.Encryption;
using Portkey.Utilities;
using UnityEngine.TestTools;
using Empty = Google.Protobuf.WellKnownTypes.Empty;

namespace Portkey.Test
{
    public class AElfSigningKeyTest
    {
        private const string PRIVATE_KEY = "03bd0cea9730bcfc8045248fd7f4841ea19315995c44801a3dfede0ca872f808";
        private readonly ISigningKeyGenerator _signingKeyGenerator = new SigningKeyGenerator(new AESEncryption());
        
        [UnityTest]
        public IEnumerator SignTest()
        {
            var done = false;
            
            const string SIGNED =
                "59EF1D3B2B853FCA1E33D07765DEBAAF38A81442CFE90822D4334E8FCE9889D80C99A0BE1858C1F26B4D99987EFF6003F33B7C3F32BBDB9CEEC68A1E8A4DB4B000";
            var wallet = _signingKeyGenerator.CreateFromPrivateKey(PRIVATE_KEY);
            //var result = wallet.Sign("68656c6c6f20776f726c643939482801");
            yield return wallet.Sign("68656c6c6f20776f726c643939482801", signature =>
            {
                Assert.AreEqual(SIGNED, signature.ToHexString());
                done = true;
            }, error =>
            {
                Assert.Fail("Sign failed!");
                done = true;
            });
            
            while (!done)
                yield return null;
        }
        
        [Test]
        public void HexToBytesLengthTest()
        {
            const string HEX = "9f8cccbb20c48b5df56e8595296e0f33998381f9b2b1b8ba6777ba73a1e959c44f6cbdc653f920202ab5437c5e4bef9a1f9b8e6dc57f77a2a963976ceb38f56a01";
            var bytes = HEX.HexToBytes();
            
            Assert.AreEqual(65, bytes.Length);
        }
        
        [UnityTest]
        public IEnumerator SignTransactionTest()
        {
            var done = false;
            
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
            var wallet = _signingKeyGenerator.CreateFromPrivateKey(PRIVATE_KEY);
            yield return wallet.SignTransaction(transaction, signedTransaction =>
            {
                Assert.AreEqual(SIGNED, signedTransaction.Signature.ToBase64());
                done = true;
            }, error =>
            {
                Assert.Fail("Sign failed!");
                done = true;
            });
            
            while (!done)
                yield return null;
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