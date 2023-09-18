using Portkey.Core;

namespace Portkey.UI
{
    public class AppleGuardianText : IGuardianText
    {
        private readonly GuardianNew _guardian;
        
        public AppleGuardianText(GuardianNew guardian)
        {
            _guardian = guardian;
        }

        public bool IsDisplayAccountTextOnly => string.IsNullOrEmpty(_guardian.details.firstName);
        public string AccountText => !string.IsNullOrEmpty(_guardian.details.firstName) ? _guardian.details.firstName : _guardian.details.thirdPartyEmail;

        public string DetailsText => (_guardian.details.isPrivate == "true") ? "******" : _guardian.details.thirdPartyEmail;
    }
}