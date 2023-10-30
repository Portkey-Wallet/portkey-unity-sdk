namespace Portkey.Core
{
    public class DIDAccountInfo
    {
        public CAInfo caInfo;
        public string chainId;
        public ISigningKey signingKey;
        public ManagerInfoType managerInfo;
        
        public DIDAccountInfo(string chainId, string guardianId, AccountType accountType, CAInfo caInfo, string sessionId, AddManagerType type, ISigningKey signingKey)
        {
            this.caInfo = caInfo;
            this.chainId = chainId;
            this.signingKey = signingKey;
            managerInfo = new ManagerInfoType
            {
                managerUniqueId = sessionId,
                guardianIdentifier = guardianId,
                accountType = accountType,
                type = type
            };
        }
        
        public DIDAccountInfo(CaHolderWithGuardian caHolder, ISigningKey signingKey)
        {
            caInfo = new CAInfo
            {
                caAddress = caHolder.holderManagerInfo.caAddress,
                caHash = caHolder.holderManagerInfo.caHash
            };
            chainId = caHolder.holderManagerInfo.originChainId;
            this.signingKey = signingKey;
            managerInfo = new ManagerInfoType
            {
                managerUniqueId = caHolder.holderManagerInfo.id,
                guardianIdentifier = caHolder.holderManagerInfo.id,
                accountType = (AccountType)caHolder.loginGuardianInfo[0].loginGuardian.type,
                type = AddManagerType.AddManager
            };
        }
    }

    public enum AddManagerType
    {
        Register,
        Recovery,
        AddManager,
    }
    
    public class ManagerInfoType
    {
        public string managerUniqueId;
        public string guardianIdentifier;
        public AccountType accountType;
        public AddManagerType type;
    }
}