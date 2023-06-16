using System.Collections;
using System.Collections.Generic;
using Portkey.Core;

namespace Portkey.GraphQL
{
    public interface IGraphQL
    {
        public delegate void successCallback<T>(T response);
        public delegate void errorCallback(string msg);
        public void GetHolderInfoByManager(string manager, string chainId);
        public IEnumerator Query<T>(string query, successCallback<T> successCallback, errorCallback errorCallback);
        public IEnumerator Query<T>(GraphQLQuery query, successCallback<T> successCallback, errorCallback errorCallback);
    }
}