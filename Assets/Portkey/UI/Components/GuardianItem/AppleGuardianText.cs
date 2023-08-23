using Portkey.Core;

namespace Portkey.UI
{
    public class AppleGuardianText : IGuardianText
    {
        private readonly Guardian _guardian;
        
        public AppleGuardianText(Guardian guardian)
        {
            _guardian = guardian;
        }
        
        public string AccountText => _guardian.thirdPartyEmail;
        public string DetailsText => "";
    }
}