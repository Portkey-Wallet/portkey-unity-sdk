using Portkey.Core;

namespace Portkey.UI
{
    public class GoogleGuardianText : IGuardianText
    {
        private readonly GuardianNew _guardian;
        
        public GoogleGuardianText(GuardianNew guardian)
        {
            _guardian = guardian;
        }

        public bool IsDisplayAccountTextOnly => false;
        public string AccountText => _guardian.details.firstName;
        public string DetailsText => _guardian.details.thirdPartyEmail;
    }
}