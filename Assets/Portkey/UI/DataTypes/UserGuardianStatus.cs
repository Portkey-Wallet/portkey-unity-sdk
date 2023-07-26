using Portkey.Core;

namespace Portkey.UI
{
    public enum VerifierStatus
    {
        NotVerified,
        Verifying,
        Verified
    }
    public class GuardianItem
    {
        public Guardian guardian = null;
        public string identifier = null;
        public VerifierItem verifier = null;
        public string key = null; // `${identifier}&${verifier?.name}`,
        public string thirdPartyEmail = null;
        public bool isPrivate = false;
        public string firstName = null;
        public string lastName = null;
        public string accessToken = null;
    }
    public class UserGuardianStatus
    {
        public VerifierStatus status = VerifierStatus.NotVerified;

        public string signature = null;
        public string verificationDoc = null;
        public string verifierId = null;
        public string sessionId = null;
        public bool isInitStatus = false;

        public GuardianItem guardianItem = null;
    }
}