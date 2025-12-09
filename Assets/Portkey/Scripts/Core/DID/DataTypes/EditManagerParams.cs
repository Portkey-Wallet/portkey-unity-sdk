namespace Portkey.Core
{
    public class EditManagerParams
    {
        public string ChainId { get; private set; }
        public string CaHash { get; private set; }
        public ManagerInfo ManagerInfo { get; private set; }
        
        public EditManagerParams(string chainId, string caHash, ManagerInfo managerInfo)
        {
            ChainId = chainId;
            CaHash = caHash;
            ManagerInfo = managerInfo;
        }
    }
}