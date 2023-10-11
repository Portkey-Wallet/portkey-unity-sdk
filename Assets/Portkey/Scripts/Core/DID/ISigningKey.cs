namespace Portkey.Core
{
    public interface ISigningKey : ISigner, IEncryptor
    {
        public string Address { get; }
        public string PublicKey { get; }
    }
}