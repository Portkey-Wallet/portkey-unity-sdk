namespace Portkey.Core
{
    using ScanLoginParam = EditManagerParams;
    
    public class LoginResult
    {
        //public RecoverStatusResult Status { get; private set; }
        public string SessionId { get; private set; }
        //public Error Error { get; private set; }
    }
    
    public interface IDIDAccountMethods
    {
        public bool Login(ScanLoginParam param);
        public LoginResult Login(AccountLoginParams param);
        public bool Logout(EditManagerParams param);
        //TODO: return RecoverStatusResult
        public string GetLoginStatus(string chainId, string sessionId);
        //TODO: return RegisterResult and take in RegisterParams
        public string Register(string param);
        //TODO: return RegisterStatusResult
        string GetRegisterStatus(string chainId, string sessionId);
        //TODO: return GetCAHolderByManagerResult and take in GetHolderInfoParams
        public string GetHolderInfo(string param);
        //TODO: return VerifierItem[]
        string GetVerifierServers(string chainId);
        //TODO: return CAHolderInfo
        string GetCAHolderInfo(string chainId);
    }
}