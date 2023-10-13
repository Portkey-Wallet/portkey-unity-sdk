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
        
        public static Address ToAddress(this string address) => address == null ? null : Address.FromBase58(address);
    }
}
