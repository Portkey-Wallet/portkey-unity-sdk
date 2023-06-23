using System.Collections;
using System.Collections.Generic;
using Portkey.GraphQL;
using UnityEngine;
using Types = GraphQLCodeGen.Types;

namespace Portkey.GraphQL
{
    public class GraphQL : MonoBehaviour, IDIDGraphQL, IGraphQL
    {
        [SerializeField] protected IGraphQL _graphQLConfig;

        private const string QUERY_CAHOLDERMANAGERINFO = "caHolderManagerInfo";
        private const string QUERY_LOGINGUARDIANINFO = "loginGuardianInfo";

        public IEnumerator GetHolderInfoByManager(string manager, string chainId,
            IGraphQL.successCallback<IList<CaHolderWithGuardian>> successCallback,
            IGraphQL.errorCallback errorCallback)
        {
            GraphQLQuery query = _graphQLConfig.GetQueryByName(QUERY_CAHOLDERMANAGERINFO);
            if (query == null)
            {
                errorCallback($"No Query by name <{QUERY_CAHOLDERMANAGERINFO}> found!");
                yield break;
            }

            Types.GetCaHolderManagerInfoDto dto = new Types.GetCaHolderManagerInfoDto();
            dto.manager = manager;
            dto.chainId = chainId;
            query.SetArgs(new { dto = dto.GetInputObject() });
            yield return _graphQLConfig.Query<Types.Query>(query,
                response =>
                {
                    List<CaHolderWithGuardian> ret = new List<CaHolderWithGuardian>();

                    if (response.caHolderManagerInfo == null || response.caHolderManagerInfo.Count == 0)
                    {
                        successCallback(ret);
                        return;
                    }

                    CaHolderWithGuardian holderWithGuardian = new CaHolderWithGuardian();
                    holderWithGuardian.holderManagerInfo = response.caHolderManagerInfo[0];
                    holderWithGuardian.loginGuardianInfo = null;
                    ret.Add(holderWithGuardian);

                    //prepare query for loginGuardianInfo
                    GraphQLQuery query = _graphQLConfig.GetQueryByName(QUERY_LOGINGUARDIANINFO);
                    if (query == null)
                    {
                        errorCallback($"No Query by name <{QUERY_LOGINGUARDIANINFO}> found!");
                        return;
                    }

                    Types.GetLoginGuardianInfoDto dto = new Types.GetLoginGuardianInfoDto();
                    dto.caHash = response.caHolderManagerInfo[0].caHash;
                    dto.chainId = chainId;
                    dto.skipCount = 0;
                    dto.maxResultCount = 100;
                    query.SetArgs(new { dto = dto.GetInputObject() });
                    StartCoroutine(_graphQLConfig.Query<Types.Query>(query,
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

        public IEnumerator Query<T>(string query, IGraphQL.successCallback<T> successCallback,
            IGraphQL.errorCallback errorCallback)
        {
            return _graphQLConfig.Query<T>(query, successCallback, errorCallback);
        }

        public IEnumerator Query<T>(GraphQLQuery query, IGraphQL.successCallback<T> successCallback,
            IGraphQL.errorCallback errorCallback)
        {
            return _graphQLConfig.Query<T>(query, successCallback, errorCallback);
        }

        public GraphQLQuery GetQueryByName(string queryName)
        {
            return _graphQLConfig.GetQueryByName(queryName);
        }
    }
}