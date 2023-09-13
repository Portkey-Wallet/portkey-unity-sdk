namespace Portkey.Core
{
    public class GuardianNew
    {
        public AccountType accountType;
        public string id;
        public string idHash;
        public string chainId;
        public bool isLoginGuardian;
        public Verifier verifier;
    }

    public class Verifier
    {
        public string id;
        public string name;
    }
}