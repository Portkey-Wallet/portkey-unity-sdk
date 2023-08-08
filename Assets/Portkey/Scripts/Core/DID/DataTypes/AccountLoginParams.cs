namespace Portkey.Core
{
    public class AccountLoginParams
    {
        public string loginGuardianIdentifier;
        public GuardiansApproved[] guardiansApprovedList;
        public string extraData;
        public string chainId;
    }
}