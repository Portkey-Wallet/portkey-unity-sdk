using System.Collections;
using Portkey.Core;
using UnityEngine;

namespace Portkey.Network
{
    [CreateAssetMenu(fileName = "FetchJsonHttpMock", menuName = "Portkey/Network/FetchJsonHttpMock")]
    public class FetchJsonHttpMock : IHttp
    {
        [SerializeField] private string response;
        public override IEnumerator Get(JsonRequestData data, IHttp.successCallback successCallback, ErrorCallback errorCallback)
        {
            successCallback(response);
            yield break;
        }

        public override IEnumerator Post(JsonRequestData data, IHttp.successCallback successCallback, ErrorCallback errorCallback)
        {
            successCallback(response);
            yield break;
        }

        public override IEnumerator PostFieldForm<T>(FieldFormRequestData<T> data, successCallback successCallback, ErrorCallback errorCallback)
        {
            throw new System.NotImplementedException();
        }
    }
}