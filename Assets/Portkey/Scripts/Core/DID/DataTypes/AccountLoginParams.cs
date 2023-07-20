namespace Portkey.Core
{
    public class AccountLoginParams
    {
        public string LoginGuardianIdentifier { get; private set; }
        public GuardiansApproved[] GuardiansApprovedList { get; private set; }
        public string ExtraData { get; private set; }
        public string ChainId { get; private set; }
        public Context Context { get; private set; }
        
        public AccountLoginParams(string loginGuardianIdentifier, GuardiansApproved[] guardiansApprovedList, string extraData, string chainId, Context context)
        {
            LoginGuardianIdentifier = loginGuardianIdentifier;
            GuardiansApprovedList = guardiansApprovedList;
            ExtraData = extraData;
            ChainId = chainId;
            Context = context;
        }
    }
}