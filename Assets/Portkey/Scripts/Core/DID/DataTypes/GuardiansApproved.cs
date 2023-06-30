namespace Portkey.Core
{
    public class GuardiansApproved
    {
        public AccountType AccountType { get; private set; }
        public string Identifier { get; private set; }
        public string verifierId { get; private set; }
        public string verificationDoc { get; private set; }
        public string Signature { get; private set; }
    }
}