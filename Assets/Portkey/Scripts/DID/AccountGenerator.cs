using System.Collections.Generic;
using Portkey.Core;

namespace Portkey.DID
{
    public class AccountGenerator : IAccountGenerator
    {
        public Account Create(SavedAccount savedAccount, ISigningKey signingKey)
        {
            return new Account
            {
                managementSigningKey = signingKey,
                accountDetails = new AccountDetails
                {
                    chainId = savedAccount.chainId,
                    caInfoMap = savedAccount.caInfoMap,
                    socialInfo = savedAccount.socialInfo
                }
            };
        }

        public Account Create(string chainId, string loginGuardianId, string caHash, string caAddress, ISigningKey signingKey)
        {
            var caInfo = new CAInfo
            {
                caHash = caHash,
                caAddress = caAddress
            };
            
            return new Account
            {
                managementSigningKey = signingKey,
                accountDetails = new AccountDetails
                {
                    chainId = chainId,
                    caInfoMap = new Dictionary<string, CAInfo>
                    {
                        {chainId, caInfo}
                    },
                    socialInfo = new AdditionalInfo
                    {
                        LoginAccount = loginGuardianId
                    }
                }
            };
        }
    }
}