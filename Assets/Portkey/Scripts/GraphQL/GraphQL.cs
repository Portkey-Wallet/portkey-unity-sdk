using System.Collections;
using System.Collections.Generic;
using Portkey.Core;
using Portkey.GraphQL;
using UnityEngine;

namespace Portkey.GraphQL
{
    /// <summary>
    /// Main class for accessing GraphQL queries.
    /// </summary>
    public class GraphQL : MonoBehaviour, IGraphQL
    {
        [SerializeField]
        private GraphQLConfig config;
        
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void GetHolderInfoByManager(string manager, string chainId)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerator Query<T>(string query, IHttp.successCallback<T> successCallback, IHttp.errorCallback errorCallback)
        {
            return config.Query<T>(query, successCallback, errorCallback);
        }

        public IEnumerator Query<T>(GraphQLQuery query, IHttp.successCallback<T> successCallback, IHttp.errorCallback errorCallback)
        {
            return config.Query<T>(query, successCallback, errorCallback);
        }
    }
}