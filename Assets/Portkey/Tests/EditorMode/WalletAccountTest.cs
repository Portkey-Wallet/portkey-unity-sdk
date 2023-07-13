using System;
using System.Linq;
using AElf.Client.Extensions;
using AElf.Types;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using NUnit.Framework;
using Portkey.Contracts.CA;
using Portkey.Core;
using Portkey.DID;

namespace Portkey.Test
{
    public class WalletAccountTest
    {
        private const string PRIVATE_KEY = "03bd0cea9730bcfc8045248fd7f4841ea19315995c44801a3dfede0ca872f808";
        private readonly IAccountProvider<WalletAccount> _accountProvider = new AccountProvider();
        
        [Test]
        public void SignTest()
        {
            var wallet = new WalletAccount(new BlockchainWallet("", "", ""))
            {
                PrivateKeyNow = PRIVATE_KEY
            };

            const string SIGNED =
                "59EF1D3B2B853FCA1E33D07765DEBAAF38A81442CFE90822D4334E8FCE9889D80C99A0BE1858C1F26B4D99987EFF6003F33B7C3F32BBDB9CEEC68A1E8A4DB4B000";
            //var wallet = _accountProvider.GetAccountFromPrivateKey(PRIVATE_KEY);
            var result = wallet.Sign("68656c6c6f20776f726c643939482801");
            
            Assert.AreEqual(SIGNED, result.ToHexString());
        }
        /*
        [Test]
        public void SignTransactionTest()
        {
            var wallet = new WalletAccount(new BlockchainWallet("", "", ""))
            {
                PrivateKeyNow = PRIVATE_KEY
            };

            var transaction = new Transaction()
            {
                From = "TrmPcaqbqmbrztv6iJuN4zeuDmQxvjF5ujbvDDQo9Q1B4ye2T".ToAddress(),
                To = Address.FromBase58("65dDNxzcd35jESiidFXN5JV8Z7pCwaFnepuYQToNefSgqk9"),
                MethodName = "GetVerifierServers",
                Params = new Empty().ToByteString(),
                RefBlockNumber = 1,
                RefBlockPrefix = ByteString.CopyFrom(new byte[] {0x01, 0x02, 0x03, 0x04})
            };

            const string SIGNED =
                "59EF1D3B2B853FCA1E33D07765DEBAAF38A81442CFE90822D4334E8FCE9889D80C99A0BE1858C1F26B4D99987EFF6003F33B7C3F32BBDB9CEEC68A1E8A4DB4B000";
            //var wallet = _accountProvider.GetAccountFromPrivateKey(PRIVATE_KEY);
            var result = wallet.SignTransaction(transaction);
            
            Assert.AreEqual(SIGNED, result.Signature.ToString());
        }*/
    }

    internal static class ByteArrayExtension
    {
        public static string ToHexString(this byte[] bytes)
        {
            return string.Join("", bytes.Select(byteValue => byteValue.ToString("X2")));
        }
    }
}