using System.Collections;
using Portkey.Core;

namespace Portkey.GraphQL
{
    public interface IGraphQL
    {
        public void GetHolderInfoByManager(string manager, string chainId);
        public IEnumerator Query<T>(string query, IHttp.successCallback<T> successCallback, IHttp.errorCallback errorCallback);
        public IEnumerator Query<T>(GraphQLQuery query, IHttp.successCallback<T> successCallback, IHttp.errorCallback errorCallback);
    }
}