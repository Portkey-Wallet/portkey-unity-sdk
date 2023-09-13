using System.Text.RegularExpressions;

namespace Portkey.Core
{
    public class PhoneNumber
    {
        public new string ToString { get; private set; }
        
        public static PhoneNumber Parse(string phoneNumber)
        {
            if (!IsValidPhoneNumber(phoneNumber))
            {
                throw new System.Exception($"Invalid phone number: {phoneNumber}");
            }
            
            return new PhoneNumber
            {
                ToString = phoneNumber
            };
        }
        
        private static bool IsValidPhoneNumber(string number)
        {
            return Regex.Match(number, @"^(\+[0-9]*)$").Success;
        }
    }
}