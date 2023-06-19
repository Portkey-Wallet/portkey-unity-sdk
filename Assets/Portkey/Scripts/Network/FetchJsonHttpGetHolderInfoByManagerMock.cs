using System.Collections;
using Portkey.Core;
using UnityEngine;
using UnityEngine.Serialization;

namespace Portkey.Network
{
    [CreateAssetMenu(fileName = "FetchJsonHttpGetHolderInfoByManagerMock", menuName = "Portkey/Network/FetchJsonHttpGetHolderInfoByManagerMock")]
    public class FetchJsonHttpGetHolderInfoByManagerMock : IHttp
    {
        [SerializeField] private string caHolderManagerInfoResponse;
        [SerializeField] private string loginGuardianInfoResponse;
        public override IEnumerator Get(string url, IHttp.successCallback successCallback, IHttp.errorCallback errorCallback)
        {
            errorCallback("Should not be called!");
            yield break;
        }

        public override IEnumerator Post(string url, string body, IHttp.successCallback successCallback, IHttp.errorCallback errorCallback)
        {
            if (body.Contains("caHolderManagerInfo"))
            {
                successCallback(caHolderManagerInfoResponse);
            }
            else
            {
                successCallback(loginGuardianInfoResponse);
            }

            yield break;
        }
    }
}