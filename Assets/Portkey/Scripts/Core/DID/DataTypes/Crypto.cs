namespace Portkey.Core
{
    public class Crypto
    {
        public class CipherParameters
        {
            public int Iv { get; set; }
        }
        
        public class KdfParameters
        {
            public long Dklen { get; set; }
            public long N { get; set; }
            public long P { get; set; }
            public long R { get; set; }
            public string Salt { get; set; }
        }
        
        public string Cipher { get; private set; }
        public string Ciphertext { get; private set; }
        public CipherParameters CipherParams { get; private set; }
        public string Kdf { get; private set; }
        public KdfParameters KdfParams { get; private set; }
        public string Mac { get; private set; }
        public string MnemonicEncrypted { get; private set; }
        
        public Crypto(string cipher, string ciphertext, CipherParameters cipherParams, string kdf, KdfParameters kdfParams, string mac, string mnemonicEncrypted)
        {
            Cipher = cipher;
            Ciphertext = ciphertext;
            CipherParams = cipherParams;
            Kdf = kdf;
            KdfParams = kdfParams;
            Mac = mac;
            MnemonicEncrypted = mnemonicEncrypted;
        }
    }
}