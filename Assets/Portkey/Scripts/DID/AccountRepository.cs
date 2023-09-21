using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Portkey.Core;

namespace Portkey.DID
{
    public class AccountRepository : IAccountRepository
    {
        private readonly IStorageSuite<string> _storageSuite;
        private readonly IEncryption _encryption;
        private readonly IWalletProvider _walletProvider;

        private class SavedAccount
        {
            public string aesPrivateKey = null;
            public Dictionary<string, CAInfo> caInfoMap = new Dictionary<string, CAInfo>();
            public SocialInfo socialInfo = new SocialInfo();
        }
        
        public AccountRepository(IStorageSuite<string> storageSuite, IEncryption encryption, IWalletProvider walletProvider)
        {
            _storageSuite = storageSuite;
            _encryption = encryption;
            _walletProvider = walletProvider;
        }
        
        public bool Save(string keyName, string password, Account account)
        {
            var encryptedPrivateKey = EncryptWallet(account.managementWallet, password);
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
            
            var managementWallet = _walletProvider.CreateFromEncryptedPrivateKey(Convert.FromBase64String(savedAccount.aesPrivateKey), password);
            account = CreateAccount(savedAccount, managementWallet);
            
            return true;
        }
        
        private static byte[] EncryptWallet(IEncryptor encryptor, string password)
        {
            if (encryptor == null) throw new NullReferenceException("Encryptor does not exist!");
            return encryptor.Encrypt(password);
        }

        private static Account CreateAccount(SavedAccount savedAccount, IWallet wallet)
        {
            return new Account
            {
                managementWallet = wallet,
                accountDetails = new AccountDetails
                {
                    caInfoMap = savedAccount.caInfoMap,
                    socialInfo = savedAccount.socialInfo
                }
            };
        }
    }
}