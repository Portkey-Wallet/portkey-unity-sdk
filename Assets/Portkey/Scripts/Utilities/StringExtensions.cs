using System;
using System.Linq;
using AElf.Types;

namespace Portkey.Utilities
{
    public static class StringExtensions
    {
        public static string RemoveAllWhiteSpaces(this string str)
        {
            return string.Concat(str.Where(c => !char.IsWhiteSpace(c)));
        }
        
        public static string RemoveAllDash(this string str)
        {
            return string.Concat(str.Where(c => c != '-'));
        }
        
        public static byte[] HexToBytes(this string hex)
        {
            if(hex.Length % 2 != 0)
                throw new ArgumentException("Invalid hex string.");
            
            var bytes = new byte[hex.Length / 2];

            for (var i = 0; i < hex.Length; i += 2)
            {
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            }

            return bytes;
        }
        
        public static Address ToAddress(this string address) => address == null ? null : Address.FromBase58(address);
    }
}
