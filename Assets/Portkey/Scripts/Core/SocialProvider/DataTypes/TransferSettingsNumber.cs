using System.Text.RegularExpressions;

namespace Portkey.Core
{
    public class LimitNumber
    {
        public int Int { get; private set; }
        
        public static LimitNumber Parse(string limitNumber)
        {
            int number;
            if (!int.TryParse(limitNumber, out number))
            {
                throw new System.Exception($"Invalid limit number: {limitNumber}");
            }
            
            return new LimitNumber
            {
                Int = number
            };
        }
    }
}