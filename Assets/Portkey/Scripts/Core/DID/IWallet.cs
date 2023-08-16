namespace Portkey.Core
{
    public interface IWallet : ISigner
    {
        protected KeyPair KeyPair { get; set; }
        protected string PrivateKey => KeyPair.PrivateKey;
        public string Address => KeyPair.Address;
        public string PublicKey => KeyPair.PublicKey;
    }
}