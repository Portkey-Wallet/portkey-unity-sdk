namespace Portkey.Core
{
    using ScanLoginParam = EditManagerParams;
    
    public class LoginResult
    {
        public RecoverStatusResult Status { get; private set; }
        public string SessionId { get; private set; }
        public Error Error { get; private set; }
        
        LoginResult(RecoverStatusResult status, string sessionId)
        {
            Status = status;
            SessionId = sessionId;
        }
    }
    
    public class Error
    {
        public string Name { get; private set; }
        public string Message { get; private set; }
        public string Stack { get; private set; }
        
        public Error(string code, string message, string stack)
        {
            Name = code;
            Message = message;
            Stack = stack;
        }
    }
    
    public class VerifierItem
    {
        public string Id { get; private set; }
        public string Name { get; private set; }
        public string ImageUrl { get; private set; }
        public string[] EndPoints { get; private set; }
        public string[] VerifierAddresses { get; private set; }
        
        public VerifierItem(string id, string name, string imageUrl, string[] endPoints, string[] verifierAddresses)
        {
            Id = id;
            Name = name;
            ImageUrl = imageUrl;
            EndPoints = endPoints;
            VerifierAddresses = verifierAddresses;
        }
    }
    
    public interface IDIDAccountMethods
    {
        public bool Login(ScanLoginParam param);
        public LoginResult Login(AccountLoginParams param);
        public bool Logout(EditManagerParams param);
        public RecoverStatusResult GetLoginStatus(string chainId, string sessionId);
        public RegisterResult Register(RegisterParams param);
        RegisterStatusResult GetRegisterStatus(string chainId, string sessionId);
        public GetCAHolderByManagerResult GetHolderInfo(GetHolderInfoParams param);
        VerifierItem[] GetVerifierServers(string chainId);
        CAHolderInfo GetCAHolderInfo(string chainId);
    }
}