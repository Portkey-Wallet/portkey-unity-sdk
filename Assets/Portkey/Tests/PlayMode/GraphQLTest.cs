using System.Collections;
using NUnit.Framework;
using Portkey.GraphQL;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;

public class GraphQLTest
{
    private const string QUERY_NAME = "CaHolderInfo";
    
    [UnityTest]
    public IEnumerator GraphQLPostWorks()
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
        GraphQLConfig graphQL = (GraphQLConfig)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guids[0]), typeof(GraphQLConfig));

        GraphQLQuery query = graphQL.GetQueryByName(QUERY_NAME);
        if (query == null)
        {
            Assert.Fail($"No Query by name <{QUERY_NAME}> found!");
        }
        query.SetArgs(new { dto = new {skipCount = 0, maxResultCount = 1, caHash = "f78e0f6e5619863fe9bafc50be3641072be27cf449760d2f63aaa180a723bc9b"} });

        return graphQL.Query<CAHolderInfoDto>(query.query, SuccessCallback, ErrorCallback);
    }

    private void ErrorCallback(string msg)
    {
        Assert.Fail($"Query by name <{QUERY_NAME}> failed to be executed!\nError: {msg}");
    }

    private void SuccessCallback(CAHolderInfoDto param)
    {
        Assert.Pass($"Query by name <{QUERY_NAME}> executed successfully!");
    }
}
