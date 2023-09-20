using Portkey.Core;

namespace Portkey.UI
{
    public class GoogleGuardianText : IGuardianText
    {
        private readonly Guardian _guardian;
        
        public GoogleGuardianText(Guardian guardian)
        {
            _guardian = guardian;
        }

        public bool IsDisplayAccountTextOnly => false;
        public string AccountText => _guardian.details.firstName;
        public string DetailsText => _guardian.details.thirdPartyEmail;
    }
}