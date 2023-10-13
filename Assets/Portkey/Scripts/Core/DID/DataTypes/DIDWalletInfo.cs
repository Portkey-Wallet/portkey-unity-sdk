namespace Portkey.Core
{
    public class DIDWalletInfo
    {
        public CAInfo caInfo;
        public string chainId;
        public ISigningKey wallet;
        public ManagerInfoType managerInfo;
        
        public DIDWalletInfo(string chainId, string guardianId, AccountType accountType, CAInfo caInfo, string sessionId, AddManagerType type, ISigningKey signingKey)
        {
            this.caInfo = caInfo;
            this.chainId = chainId;
            wallet = signingKey;
            managerInfo = new ManagerInfoType
            {
                managerUniqueId = sessionId,
                guardianIdentifier = guardianId,
                accountType = accountType,
                type = type
            };
        }
        
        public DIDWalletInfo(CaHolderWithGuardian caHolder, ISigningKey signingKey)
        {
            caInfo = new CAInfo
            {
                caAddress = caHolder.holderManagerInfo.caAddress,
                caHash = caHolder.holderManagerInfo.caHash
            };
            chainId = caHolder.holderManagerInfo.originChainId;
            wallet = signingKey;
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