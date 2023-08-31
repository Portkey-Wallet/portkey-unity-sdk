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
        
        public string AccountText => !string.IsNullOrEmpty(_guardian.firstName) ? _guardian.firstName : _guardian.thirdPartyEmail;

        public string DetailsText => (_guardian.isPrivate == "true") ? "******" : _guardian.thirdPartyEmail;
    }
}