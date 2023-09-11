using Portkey.Core;

namespace Portkey.UI
{
    public class PhoneGuardianText : IGuardianText
    {
        private readonly Guardian _guardian;
        
        public PhoneGuardianText(Guardian guardian)
        {
            _guardian = guardian;
        }

        public bool IsDisplayAccountTextOnly => true;
        public string AccountText => _guardian.guardianIdentifier;

        public string DetailsText => "";
    }
}