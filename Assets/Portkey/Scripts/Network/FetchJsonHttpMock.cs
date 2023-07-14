using System.Collections;
using Portkey.Core;
using UnityEngine;

namespace Portkey.Network
{
    [CreateAssetMenu(fileName = "FetchJsonHttpMock", menuName = "Portkey/Network/FetchJsonHttpMock")]
    public class FetchJsonHttpMock : IHttp
    {
        [SerializeField] private string response;
        public override IEnumerator Get(string url, IHttp.successCallback successCallback, IHttp.errorCallback errorCallback)
        {
            successCallback(response);
            yield break;
        }

        public override IEnumerator Post(string url, string body, IHttp.successCallback successCallback, IHttp.errorCallback errorCallback)
        {
            successCallback(response);
            yield break;
        }
    }
}