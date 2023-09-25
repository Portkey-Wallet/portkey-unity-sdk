using System.Collections;
using System.Collections.Generic;
using Portkey.Core;
using Portkey.GraphQL;
using Portkey.Utilities;
using UnityEngine;
using Types = GraphQLCodeGen.Types;

namespace Portkey.GraphQL
{
    /// <summary>
    /// GraphQL is the main class to interact with GraphQL.
    /// </summary>
    public class GraphQL: IDIDGraphQL, IGraphQL
    {
        private GraphQLConfig _graphQLConfig;

        private const string QUERY_CAHOLDERMANAGERINFO = "caHolderManagerInfo";
        private const string QUERY_LOGINGUARDIANINFO = "loginGuardianInfo";

        public GraphQL (GraphQLConfig graphQLConfig)
        {
            _graphQLConfig = graphQLConfig;
        }
        
        public IEnumerator GetHolderInfoByManager(string manager, string chainId,
            SuccessCallback<IList<CaHolderWithGuardian>> successCallback,
            ErrorCallback errorCallback)
        {
            var query = _graphQLConfig.GetQueryByName(QUERY_CAHOLDERMANAGERINFO);
            if (query == null)
            {
                errorCallback($"No Query by name <{QUERY_CAHOLDERMANAGERINFO}> found!");
                yield break;
            }

            var dto = new Types.GetCaHolderManagerInfoDto
            {
                manager = manager,
                chainId = chainId,
                maxResultCount = 1,
                skipCount = 0
            };
            
            query.SetArgs(new { dto = dto.GetInputObject() });
            
            Debugger.Log(query.query);
            
            yield return _graphQLConfig.Query<Types.Query>(query,
                response =>
                {
                    var ret = new List<CaHolderWithGuardian>();

                    if (response.caHolderManagerInfo == null || response.caHolderManagerInfo.Count == 0)
                    {
                        successCallback(ret);
                        return;
                    }

                    var holderWithGuardian = new CaHolderWithGuardian
                    {
                        holderManagerInfo = response.caHolderManagerInfo[0],
                        loginGuardianInfo = null
                    };
                    ret.Add(holderWithGuardian);

                    //prepare query for loginGuardianInfo
                    var query = _graphQLConfig.GetQueryByName(QUERY_LOGINGUARDIANINFO);
                    if (query == null)
                    {
                        errorCallback($"No Query by name <{QUERY_LOGINGUARDIANINFO}> found!");
                        return;
                    }

                    var dto = new Types.GetLoginGuardianInfoDto
                    {
                        caHash = response.caHolderManagerInfo[0].caHash,
                        chainId = chainId,
                        skipCount = 0,
                        maxResultCount = 100
                    };
                    query.SetArgs(new { dto = dto.GetInputObject() });
                    StaticCoroutine.StartCoroutine(_graphQLConfig.Query<Types.Query>(query,
                        responseToLogin =>
                        {
                            if (responseToLogin.loginGuardianInfo == null ||
                                responseToLogin.loginGuardianInfo.Count == 0)
                            {
                                ret[0].loginGuardianInfo = new List<Types.LoginGuardianDto>();
                            }
                            else
                            {
                                ret[0].loginGuardianInfo = responseToLogin.loginGuardianInfo;
                            }

                            successCallback(ret);
                        }, errorCallback));
                },
                errorCallback);
        }

        public IEnumerator Query<T>(string query, SuccessCallback<T> successCallback,
            ErrorCallback errorCallback)
        {
            return _graphQLConfig.Query<T>(query, successCallback, errorCallback);
        }

        public IEnumerator Query<T>(GraphQLQuery query, SuccessCallback<T> successCallback,
            ErrorCallback errorCallback)
        {
            return _graphQLConfig.Query<T>(query, successCallback, errorCallback);
        }

        public GraphQLQuery GetQueryByName(string queryName)
        {
            return _graphQLConfig.GetQueryByName(queryName);
        }
    }
}