namespace Portkey.Encryption
{
    public interface IEncryption
    {
        string Encrypt(string plainText, string password);
        string Decrypt(string cipherText, string password);
    }
}