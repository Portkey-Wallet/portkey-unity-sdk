using Portkey.Core;

namespace Portkey.UI
{
    public class PhoneGuardianText : IGuardianText
    {
        private readonly GuardianNew _guardian;
        
        public PhoneGuardianText(GuardianNew guardian)
        {
            _guardian = guardian;
        }

        public bool IsDisplayAccountTextOnly => true;
        public string AccountText => _guardian.id;

        public string DetailsText => "";
    }
}