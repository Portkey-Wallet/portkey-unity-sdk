using Portkey.Core;

namespace Portkey.DID
{

    public class DIDWallet<T> : IDIDWallet where T : AccountBase
    {
        private IPortkeySocialService _socialService;
        private T _managementAccount;

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

        public RecoverStatusResult GetLoginStatus(string chainId, string sessionId)
        {
            throw new System.NotImplementedException();
        }

        public RegisterResult Register(RegisterParams param)
        {
            throw new System.NotImplementedException();
        }

        public RegisterStatusResult GetRegisterStatus(string chainId, string sessionId)
        {
            throw new System.NotImplementedException();
        }

        public GetCAHolderByManagerResult GetHolderInfo(GetHolderInfoParams param)
        {
            throw new System.NotImplementedException();
        }

        public VerifierItem[] GetVerifierServers(string chainId)
        {
            throw new System.NotImplementedException();
        }

        public CAHolderInfo GetCAHolderInfo(string chainId)
        {
            throw new System.NotImplementedException();
        }
    }
}