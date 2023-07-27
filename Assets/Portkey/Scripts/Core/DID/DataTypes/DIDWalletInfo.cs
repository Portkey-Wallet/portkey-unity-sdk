namespace Portkey.Core
{
    public class DIDWalletInfo
    {
        public CAInfo caInfo;
        public string pin;
        public string chainId;
        public BlockchainWallet wallet;
        public ManagerInfoType managerInfo;
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