namespace Portkey.Core
{
    public interface IEncryptor
    {
        byte[] Encrypt(string password);
    }
}