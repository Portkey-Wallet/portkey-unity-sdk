namespace Portkey.Core
{
    public class Guardian
    {
        public AccountType accountType;
        public string id;
        public string idHash;
        public string chainId;
        public bool isLoginGuardian;
        public Verifier verifier;
        public SocialDetails details;
    }
    
    public class SocialDetails
    {
        public string thirdPartyEmail;
        public string isPrivate;
        public string firstName;
        public string lastName;
    }

    public class Verifier
    {
        public string id;
        public string name;
    }
}