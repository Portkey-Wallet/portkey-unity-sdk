using System.Linq;

namespace Portkey.Utilities
{
    public static class StringExtensions
    {
        public static string RemoveAllWhiteSpaces(this string str)
        {
            return string.Concat(str.Where(c => !char.IsWhiteSpace(c)));
        }
    }
}
