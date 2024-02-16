namespace Portkey.Core
{
    /// <summary>
    /// For the type of account when logging in or signing up.
    /// </summary>
    public enum AccountType
    {
        Email,
        Phone,
        Google,
        Apple,
        Telegram
    }
    
    public static class AccountTypeExtensions
    {
        public static bool IsSocialAccountType(this AccountType accountType)
        {
            switch(accountType){
                case AccountType.Apple:
                case AccountType.Google:
                case AccountType.Telegram:
                    return true;
                default: 
                    return false;
            }
        }
    }
}