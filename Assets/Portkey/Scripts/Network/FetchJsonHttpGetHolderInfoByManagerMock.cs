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
        public override IEnumerator Get(JsonRequestData data, IHttp.successCallback successCallback, ErrorCallback errorCallback)
        {
            errorCallback("Should not be called!");
            yield break;
        }

        public override IEnumerator Post(JsonRequestData data, IHttp.successCallback successCallback, ErrorCallback errorCallback)
        {
            if (data.JsonData.Contains("caHolderManagerInfo"))
            {
                successCallback(caHolderManagerInfoResponse);
            }
            else
            {
                successCallback(loginGuardianInfoResponse);
            }

            yield break;
        }

        public override IEnumerator PostFieldForm<T>(FieldFormRequestData<T> data, successCallback successCallback, ErrorCallback errorCallback)
        {
            throw new System.NotImplementedException();
        }
    }
}