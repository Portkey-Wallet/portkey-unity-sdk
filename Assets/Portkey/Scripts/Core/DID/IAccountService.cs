using System.Collections;

namespace Portkey.Core
{
    public class AccountDeletionValidationResult
    {
        public bool validatedAssets;
        public bool validatedGuardian;
        public bool validatedDevice;
    }
    
    internal class IsAccountDeletionPossibleResult
    {
        public bool entranceDisplay;
    }
    
    internal class DeleteAccountResult
    {
        public bool success;
    }
    
    internal class AccountDeletionParams
    {
        public string appleToken;
    }
    
    public interface IAccountService
    {
        /// <summary>
        /// An API check to see if account can be deleted. This is possible if account has only 1 login guardian that is using apple account.
        /// </summary>
        IEnumerator IsAccountDeletionPossible(ConnectToken connectToken, SuccessCallback<bool> successCallback, ErrorCallback errorCallback);
        /// <summary>
        /// Validate account deletion. User must have lesser than stipulated amount of assets, must have only 1 login guardian that is using apple account and that device is an iOS device.
        /// </summary>
        IEnumerator ValidateAccountDeletion(ConnectToken connectToken, SuccessCallback<AccountDeletionValidationResult> successCallback, ErrorCallback errorCallback);
        /// <summary>
        /// Delete account from database.
        /// </summary>
        IEnumerator DeleteAccount(ConnectToken connectToken, string appleToken, SuccessCallback<bool> successCallback, ErrorCallback errorCallback);
    }
}
