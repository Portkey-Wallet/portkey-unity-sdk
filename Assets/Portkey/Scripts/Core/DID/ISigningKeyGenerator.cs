namespace Portkey.Core
{
    public interface ISigningKeyGenerator
    {
        public ISigningKey CreateFromEncryptedPrivateKey(byte[] encryptedPrivateKey, string password);
        public ISigningKey CreateFromPrivateKey(string privateKey);
        public ISigningKey Create();
    }
}