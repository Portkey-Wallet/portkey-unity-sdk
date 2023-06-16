using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Portkey.GraphQL;
using UnityEditor;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.TestTools;

public class GraphQLTest
{
    private const string QUERY_NAME_GETCAHOLDERINFO = "GetCAHolderInfo";
    private const string QUERY_NAME_CAHOLDERMANAGERCHANGERECORDINFO = "caHolderManagerChangeRecordInfo";

    private GraphQLConfig graphQL = GetPortkeyGraphQlConfig();
    
    [UnityTest]
    public IEnumerator GraphQLPostWorks()
    {
        GraphQLQuery query = graphQL.GetQueryByName(QUERY_NAME_GETCAHOLDERINFO);
        if (query == null)
        {
            Assert.Fail($"No Query by name <{QUERY_NAME_GETCAHOLDERINFO}> found!");
        }
        GraphQLCodeGen.Types.GetCaHolderInfoDto dto = new GraphQLCodeGen.Types.GetCaHolderInfoDto();
        dto.skipCount = 0;
        dto.maxResultCount = 1;
        dto.caHash = "f78e0f6e5619863fe9bafc50be3641072be27cf449760d2f63aaa180a723bc9b";
        query.SetArgs(new { dto = dto.GetInputObject()});
        
        return graphQL.Query<GraphQLCodeGen.Types.Query>(query.query, SuccessCallback, ErrorCallback);
    }

    private void ErrorCallback(string msg)
    {
        Assert.Fail($"Query by name <{QUERY_NAME_GETCAHOLDERINFO}> failed to be executed!\nError: {msg}");
    }

    private void SuccessCallback(GraphQLCodeGen.Types.Query param)
    {
        Assert.NotNull(param.caHolderInfo);
    }
    
    [UnityTest]
    public IEnumerator GraphQLPostNullReturn()
    {
        GraphQLQuery query = graphQL.GetQueryByName(QUERY_NAME_CAHOLDERMANAGERCHANGERECORDINFO);
        if (query == null)
        {
            Assert.Fail($"No Query by name <{QUERY_NAME_CAHOLDERMANAGERCHANGERECORDINFO}> found!");
        };
        
        return graphQL.Query<GraphQLCodeGen.Types.Query>(query.query, GraphQLPostNullReturnSuccessCallback, GraphQLPostNullReturnErrorCallback);
    }
    
    private void GraphQLPostNullReturnErrorCallback(string msg)
    {
        Assert.Fail($"Query by name <{QUERY_NAME_CAHOLDERMANAGERCHANGERECORDINFO}> should succeed with null!\nError: {msg}");
    }

    private void GraphQLPostNullReturnSuccessCallback(GraphQLCodeGen.Types.Query param)
    {
        Assert.Null(param.caHolderManagerChangeRecordInfo, $"Query by name <{QUERY_NAME_CAHOLDERMANAGERCHANGERECORDINFO}> responded with null without arguments being specified!");
    }
    
    [UnityTest]
    public IEnumerator GraphQLQueryNotFound()
    {
        const string QUERY_NAME = "QueryMock";
        
        GraphQLQuery query = graphQL.GetQueryByName(QUERY_NAME);
        if (query == null)
        {
            Assert.Pass($"No Query by name <{QUERY_NAME}> found!");
        }

        Assert.Fail($"Should not have found Query <{QUERY_NAME}>");
        yield break;
    }

    private static GraphQLConfig GetPortkeyGraphQlConfig()
    {
        string[] guids = AssetDatabase.FindAssets($"t:{nameof(GraphQLConfig)}");
        if (guids.Length == 0)
        {
            Assert.Fail($"No {nameof(GraphQLConfig)} found!");
        }
        else if (guids.Length > 0)
        {
            Debug.LogWarning($"More than one {nameof(GraphQLConfig)} found, taking first one");
        }

        GraphQLConfig graphQL =
            (GraphQLConfig)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guids[0]), typeof(GraphQLConfig));
        return graphQL;
    }
}
