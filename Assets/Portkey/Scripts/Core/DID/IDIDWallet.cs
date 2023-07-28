using System.Collections;

namespace Portkey.Core
{
    public interface IDIDWallet : IWallet, IDIDAccountApi
    {
        /// <summary>
        /// For adding a manager account to the DID.
        /// </summary>
        /// <param name="editManagerParams">Parameters for adding manager account.</param>
        public IEnumerator AddManager(EditManagerParams editManagerParams, IHttp.successCallback successCallback, ErrorCallback errorCallback);
        /// <summary>
        /// For removing a manager account to the DID.
        /// </summary>
        /// <param name="editManagerParams">Parameters for removing manager account.</param>
        public IEnumerator RemoveManager(EditManagerParams editManagerParams, IHttp.successCallback successCallback, ErrorCallback errorCallback);
    }
}