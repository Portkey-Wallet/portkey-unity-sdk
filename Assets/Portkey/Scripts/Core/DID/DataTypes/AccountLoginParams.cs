namespace Portkey.Core
{
    public class AccountLoginParams
    {
        public string loginGuardianIdentifier;
        public ApprovedGuardian[] guardiansApprovedList;
        public string extraData;
        public string chainId;
    }
}