using Portkey.Core;

namespace Portkey.DID
{

    public class DIDWallet<T> : IDIDWallet where T : AccountBase
    {
        public void Create()
        {
            throw new System.NotImplementedException();
        }

        public bool Save(string password, string keyName)
        {
            throw new System.NotImplementedException();
        }

        public void Load(string password, string keyName)
        {
            throw new System.NotImplementedException();
        }

        public bool Login(EditManagerParams param)
        {
            throw new System.NotImplementedException();
        }

        public LoginResult Login(AccountLoginParams param)
        {
            throw new System.NotImplementedException();
        }

        public bool Logout(EditManagerParams param)
        {
            throw new System.NotImplementedException();
        }

        public string GetLoginStatus(string chainId, string sessionId)
        {
            throw new System.NotImplementedException();
        }

        public string Register(string param)
        {
            throw new System.NotImplementedException();
        }

        public string GetRegisterStatus(string chainId, string sessionId)
        {
            throw new System.NotImplementedException();
        }

        public string GetHolderInfo(string param)
        {
            throw new System.NotImplementedException();
        }

        public string GetVerifierServers(string chainId)
        {
            throw new System.NotImplementedException();
        }

        public string GetCAHolderInfo(string chainId)
        {
            throw new System.NotImplementedException();
        }
    }
}