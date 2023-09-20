using Portkey.Core;

namespace Portkey.UI
{
    public class EmailGuardianText : IGuardianText
    {
        private readonly Guardian _guardian;
        
        public EmailGuardianText(Guardian guardian)
        {
            _guardian = guardian;
        }

        public bool IsDisplayAccountTextOnly => true;
        public string AccountText => _guardian.id;

        public string DetailsText => "";
    }
}