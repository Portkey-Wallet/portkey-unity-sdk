using System;
using Newtonsoft.Json;
using Portkey.Core;

namespace Portkey.DID
{
    public class AccountRepository : IAccountRepository
    {
        private readonly IStorageSuite<string> _storageSuite;
        private readonly IEncryption _encryption;
        private readonly ISigningKeyGenerator _signingKeyGenerator;
        private readonly IAccountGenerator _accountGenerator;

        public AccountRepository(IStorageSuite<string> storageSuite, IEncryption encryption, ISigningKeyGenerator signingKeyGenerator, IAccountGenerator accountGenerator)
        {
            _storageSuite = storageSuite;
            _encryption = encryption;
            _signingKeyGenerator = signingKeyGenerator;
            _accountGenerator = accountGenerator;
        }
        
        public bool Save(string keyName, string password, Account account)
        {
            if (account == null)
            {
                throw new ArgumentException("Account is null.");
            }
            
            var encryptedPrivateKey = EncryptWallet(account.managementSigningKey, password);

            if (encryptedPrivateKey == null)
            {
                throw new ArgumentException("Unsupported method of login for saving.");
            }
            
            var aesPrivateKey = Convert.ToBase64String(encryptedPrivateKey);
            
            var savedAccount = new SavedAccount
            {
                aesPrivateKey = aesPrivateKey,
                caInfoMap = account.accountDetails.caInfoMap,
                socialInfo = account.accountDetails.socialInfo
            };
            
            var data = JsonConvert.SerializeObject(savedAccount);
            var encryptedData = _encryption.Encrypt(data, password);
            _storageSuite.SetItem(keyName, Convert.ToBase64String(encryptedData));
            return true;
        }

        public bool Load(string keyName, string password, out Account account)
        {
            account = null;
            
            var encryptedDataStr = _storageSuite.GetItem(keyName);
            if (encryptedDataStr == null)
            {
                throw new Exception("No data found.");
            }
            var encryptedData = Convert.FromBase64String(encryptedDataStr);
            var data = _encryption.Decrypt(encryptedData, password);
            if (data == null)
            {
                throw new Exception("Wrong password.");
            }

            SavedAccount savedAccount = null;
            try
            {
                savedAccount = JsonConvert.DeserializeObject<SavedAccount>(data);
            }
            catch (Exception e)
            {
                Debugger.LogException(e);
                return false;
            }
            
            var managementWallet = _signingKeyGenerator.CreateFromEncryptedPrivateKey(Convert.FromBase64String(savedAccount.aesPrivateKey), password);
            account = _accountGenerator.Create(savedAccount, managementWallet);
            
            return true;
        }
        
        private static byte[] EncryptWallet(IEncryptor encryptor, string password)
        {
            if (encryptor == null) throw new NullReferenceException("Encryptor does not exist!");
            return encryptor.Encrypt(password);
        }
    }
}