using System.Collections;
using System.Collections.Generic;
using Portkey.Core;

namespace Portkey.GraphQL
{
    /// <summary>
    /// An interface to GraphQL calls.
    /// </summary>
    public interface IGraphQL
    {
        /// <summary>Delegate for success callback.</summary>
        /// <param name="response">The response from the request.</param>
        public delegate void successCallback<T>(T response);
        /// <summary>Delegate for error callback.</summary>
        /// <param name="message">The response from the request.</param>
        public delegate void errorCallback(string message);
        public void GetHolderInfoByManager(string manager, string chainId);
        /// <summary>For making a GraphQL query.</summary>
        /// <param name="query">The query to make in string.</param>
        /// <param name="successCallback">Callback function when post of query is successful.</param>
        /// <param name="errorCallback">Callback function when error occurs.</param>
        public IEnumerator Query<T>(string query, successCallback<T> successCallback, errorCallback errorCallback);
        /// <summary>For making a GraphQL query.</summary>
        /// <param name="query">The query to make in GraphQLQuery.</param>
        /// <param name="successCallback">Callback function when post of query is successful.</param>
        /// <param name="errorCallback">Callback function when error occurs.</param>
        public IEnumerator Query<T>(GraphQLQuery query, successCallback<T> successCallback, errorCallback errorCallback);
    }
}