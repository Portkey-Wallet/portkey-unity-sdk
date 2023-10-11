using System.Collections;
using Portkey.GraphQL;

namespace Portkey.Core
{
    /// <summary>
    /// An interface to GraphQL calls.
    /// </summary>
    public interface IGraphQL
    {
        /// <summary>For making a GraphQL query.</summary>
        /// <param name="operationName">name of the operation</param>
        /// <param name="query">The query to make in string.</param>
        /// <param name="successCallback">Callback function when post of query is successful.</param>
        /// <param name="errorCallback">Callback function when error occurs.</param>
        /// <typeparam name="T">The GraphQL return query type.</typeparam>
        public IEnumerator Query<T>(string operationName, string query, SuccessCallback<T> successCallback,
            ErrorCallback errorCallback);
        /// <summary>For making a GraphQL query.</summary>
        /// <param name="query">The query to make in GraphQLQuery.</param>
        /// <param name="successCallback">Callback function when post of query is successful.</param>
        /// <param name="errorCallback">Callback function when error occurs.</param>
        public IEnumerator Query<T>(GraphQLQuery query, SuccessCallback<T> successCallback, ErrorCallback errorCallback);
        /// <summary>For getting a query by name.</summary>
        /// <param name="queryName">The name of the query.</param>
        public GraphQLQuery GetQueryByName(string queryName);
    }
}