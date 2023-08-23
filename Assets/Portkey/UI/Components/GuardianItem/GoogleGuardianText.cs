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
        
        public string AccountText => _guardian.firstName;
        public string DetailsText => _guardian.thirdPartyEmail;
    }
}