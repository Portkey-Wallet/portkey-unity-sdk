using Portkey.Core;

namespace Portkey.UI
{
    public class TelegramGuardianText : IGuardianText
    {
        private readonly Guardian _guardian;
        
        public TelegramGuardianText(Guardian guardian)
        {
            _guardian = guardian;
        }

        public bool IsDisplayAccountTextOnly => string.IsNullOrEmpty(_guardian.details.firstName);
        public string AccountText => !string.IsNullOrEmpty(_guardian.details.firstName) ? _guardian.details.firstName : _guardian.details.thirdPartyEmail;
        
        public string DetailsText => (_guardian.details.isPrivate == "true") ? "******" : _guardian.details.thirdPartyEmail;
    }
}