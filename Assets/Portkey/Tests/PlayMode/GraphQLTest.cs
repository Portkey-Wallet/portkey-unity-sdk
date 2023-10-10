using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Portkey.Core;
using Portkey.GraphQL;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;

public class GraphQLTest
{
    public class GraphQLMonoTest : MonoBehaviour, IMonoBehaviourTest
    {
        protected bool runFinished = false;
        public bool IsTestFinished
        {
            get { return runFinished; }
        }
    }

    private static GraphQLConfig GetPortkeyGraphQlConfig(string searchFilter)
    {
        var guids = AssetDatabase.FindAssets(searchFilter);
        if (guids.Length == 0)
        {
            Assert.Fail($"No {nameof(GraphQLConfig)} found!");
        }
        else if (guids.Length > 0)
        {
            Debug.LogWarning($"More than one {nameof(GraphQLConfig)} found, taking first one");
        }

        var graphQL = (GraphQLConfig)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guids[0]), typeof(GraphQLConfig));
        return graphQL;
    }

    private class GraphQLMonoHTTPMockGetHolderInfoByManagerTest : GraphQLMonoTest
    {
        private GraphQL _graphQl;
        
        private void Awake()
        {
            var config = GetPortkeyGraphQlConfig($"t:{nameof(GraphQLConfig)} GraphQLConfigGetHolderInfoByManagerMock");
            _graphQl = new GraphQL(config);
        }

        public void Start()
        {
            StartCoroutine(_graphQl.GetHolderInfoByManager("manager_mock", "chainId_mock", SuccessCallback, ErrorCallback));
        }
        
        private void SuccessCallback(IList<CaHolderWithGuardian> response)
        {
            runFinished = true;
            Assert.AreEqual(1, response[0].loginGuardianInfo.Count);
        }
        
        private void ErrorCallback(string message)
        {
            runFinished = true;
            Assert.Fail($"Should not fail. Message: {message}");
        }
    }

    private class GraphQLMonoHTTPMockTest : GraphQLMonoTest
    {
        private const string QUERY_NAME_GETCAHOLDERINFO = "GetCAHolderInfo";
        private IGraphQL _graphQl;
        
        private void Awake()
        {
            var config = GetPortkeyGraphQlConfig($"t:{nameof(GraphQLConfig)} GraphQLConfigMock");
            _graphQl = new GraphQL(config);
        }

        public void Start()
        {
            var query = _graphQl.GetQueryByName(QUERY_NAME_GETCAHOLDERINFO);
            if (query == null)
            {
                Assert.Fail($"No Query by name <{QUERY_NAME_GETCAHOLDERINFO}> found!");
            };
            
            StartCoroutine(_graphQl.Query<GraphQLCodeGen.Types.Query>(query.name, query.query, SuccessCallback, ErrorCallback));
        }
        
        private void SuccessCallback(GraphQLCodeGen.Types.Query param)
        {
            runFinished = true;
            Assert.NotNull(param.caHolderInfo);
        }
        
        private void ErrorCallback(string message)
        {
            runFinished = true;
            Assert.Fail("Should not fail.");
        }
    }
    
    private const string QUERY_NAME_GETCAHOLDERINFO = "GetCAHolderInfo";
    private const string QUERY_NAME_CAHOLDERMANAGERCHANGERECORDINFO = "caHolderManagerChangeRecordInfo";

    [UnityTest]
    public IEnumerator GraphQLMockQueryTest()
    {
        yield return new MonoBehaviourTest<GraphQLMonoHTTPMockTest>();
    }
    
    [UnityTest]
    public IEnumerator GraphQLMockGetHolderInfoByManagerTest()
    {
        yield return new MonoBehaviourTest<GraphQLMonoHTTPMockGetHolderInfoByManagerTest>();
    }
    
    [UnityTest]
    public IEnumerator GraphQLPostWorks()
    {
        var config = GetPortkeyGraphQlConfig($"t:{nameof(GraphQLConfig)} PortkeyGraphQLConfig");
        IGraphQL graphQlTest = new GraphQL(config);
        
        var query = graphQlTest.GetQueryByName(QUERY_NAME_GETCAHOLDERINFO);
        if (query == null)
        {
            Assert.Fail($"No Query by name <{QUERY_NAME_GETCAHOLDERINFO}> found!");
        }
        var dto = new GraphQLCodeGen.Types.GetCaHolderInfoDto
        {
            skipCount = 0,
            maxResultCount = 1,
            caHash = "f78e0f6e5619863fe9bafc50be3641072be27cf449760d2f63aaa180a723bc9b"
        };
        query.SetArgs(new { dto = dto.GetInputObject()});
        
        return graphQlTest.Query<GraphQLCodeGen.Types.Query>(query.name, query.query, SuccessCallback, ErrorCallback);
    }

    [Test]
    public void JsonToArgumentTest()
    {
        const string JSON_INPUT = "{\"skipCount\":0,\"maxResultCount\":1,\"caHash\":\"f78e0f6e5619863fe9bafc50be3641072be27cf449760d2f63aaa180a723bc9b\"}";
        const string JSON_EXPECTED_OUTPUT = "skipCount: 0, maxResultCount: 1, caHash: \"f78e0f6e5619863fe9bafc50be3641072be27cf449760d2f63aaa180a723bc9b\"";
        try
        {
            var output = Utilities.JsonToArgument(JSON_INPUT);
            Assert.AreEqual(JSON_EXPECTED_OUTPUT, output);
        }
        catch (Exception e)
        {
            Assert.Fail("Should not fail. Message: " + e.Message);
        }
    }

    [Test]
    public void JsonToArgumentOnlyOneOpeningBracketTest()
    {
        const string JSON_INPUT = "{\"skipCount\":0,\"maxResultCount\":1,\"caHash\":\"f78e0f6e5619863fe9bafc50be3641072be27cf449760d2f63aaa180a723bc9b\"";
        try
        {
            var output = Utilities.JsonToArgument(JSON_INPUT);
            Assert.Fail("Argument should not be valid. Output: " + output);
        }
        catch (Exception e)
        {
            Assert.Pass("Should fail. Message: " + e.Message);
        }
    }
    
    [Test]
    public void JsonToArgumentNoOpeningBracketTest()
    {
        const string JSON_INPUT = "\"skipCount\":0,\"maxResultCount\":1,\"caHash\":\"f78e0f6e5619863fe9bafc50be3641072be27cf449760d2f63aaa180a723bc9b\"";
        try
        {
            var output = Utilities.JsonToArgument(JSON_INPUT);
            Assert.Fail("Argument should not be valid. Output: " + output);
        }
        catch (Exception e)
        {
            Assert.Pass("Should fail. Message: " + e.Message);
        }
    }
    
    [Test]
    public void JsonToArgumentArrayTest()
    {
        const string JSON_INPUT = "{\"userId\": 1,\"id\": 1,\"title\": \"delectus aut autem\",\"completed\": false,\"myList\": [{\"value\": 1,\"id\": \"WUGFPBGP\"},{\"value\": 3,\"id\": \"QFPNVASU\"}]}";
        const string JSON_EXPECTED_OUTPUT = "userId: 1, id: 1, title: \"delectus aut autem\", completed: false, myList: [{value: 1, id: \"WUGFPBGP\"}, {value: 3, id: \"QFPNVASU\"}]";
        try
        {
            var output = Utilities.JsonToArgument(JSON_INPUT);
            Assert.AreEqual(JSON_EXPECTED_OUTPUT, output);
        }
        catch (Exception e)
        {
            Assert.Fail("Should not fail. Message: " + e.Message);
        }
    }

    private void ErrorCallback(string message)
    {
        Assert.Fail($"Query by name <{QUERY_NAME_GETCAHOLDERINFO}> failed to be executed!\nError: {message}");
    }
    
    private void SuccessCallback(GraphQLCodeGen.Types.Query param)
    {
        Assert.NotNull(param.caHolderInfo);
    }

    [UnityTest]
    public IEnumerator GraphQLPostNullReturn()
    {
        var config = GetPortkeyGraphQlConfig($"t:{nameof(GraphQLConfig)} PortkeyGraphQLConfig");
        IGraphQL graphQlTest = new GraphQL(config);
        
        var query = graphQlTest.GetQueryByName(QUERY_NAME_CAHOLDERMANAGERCHANGERECORDINFO);
        if (query == null)
        {
            Assert.Fail($"No Query by name <{QUERY_NAME_CAHOLDERMANAGERCHANGERECORDINFO}> found!");
        };
        
        return graphQlTest.Query<GraphQLCodeGen.Types.Query>(query.name, query.query, GraphQLPostNullReturnSuccessCallback, GraphQLPostNullReturnErrorCallback);
    }
    
    private void GraphQLPostNullReturnErrorCallback(string message)
    {
        Assert.Pass($"Query by name <{QUERY_NAME_CAHOLDERMANAGERCHANGERECORDINFO}> failed!\nError: {message}");
    }

    private void GraphQLPostNullReturnSuccessCallback(GraphQLCodeGen.Types.Query param)
    {
        Assert.Fail($"Query by name <{QUERY_NAME_CAHOLDERMANAGERCHANGERECORDINFO}> responded with null without arguments being specified!");
    }
    
    [UnityTest]
    public IEnumerator GraphQLQueryNotFound()
    {
        var config = GetPortkeyGraphQlConfig($"t:{nameof(GraphQLConfig)} PortkeyGraphQLConfig");
        IGraphQL graphQlTest = new GraphQL(config);
        
        const string queryName = "QueryMock";
        
        var query = graphQlTest.GetQueryByName(queryName);
        if (query == null)
        {
            Assert.Pass($"No Query by name <{queryName}> found!");
        }

        Assert.Fail($"Should not have found Query <{queryName}>");
        yield break;
    }
    
    [UnityTest]
    public IEnumerator BuildQueryStringTest()
    {
        const string EXPECTED_RESULT = 
@"query GetCAHolderInfo {
    caHolderInfo {
        guardianList {
            guardians {
                isLoginGuardian
                salt
                identifierHash
                verifierId
                type
            }
        }
        originChainId
        managerInfos {
            extraData
            address
        }
        caAddress
        caHash
        chainId
        id
    }
}";
            
        var config = GetPortkeyGraphQlConfig($"t:{nameof(GraphQLConfig)} PortkeyGraphQLConfig");
        IGraphQL graphQlTest = new GraphQL(config);
        
        const string QUERY_NAME_GETCAHOLDERINFO = "GetCAHolderInfo";
        
        var query = graphQlTest.GetQueryByName(QUERY_NAME_GETCAHOLDERINFO);

        query.BuildQueryString();
        
        
        Assert.AreEqual(EXPECTED_RESULT, query.query);
        yield break;
    }
    
    [UnityTest]
    public IEnumerator BuildQueryStringWithArgTest()
    {
        const string EXPECTED_RESULT = 
@"query GetCAHolderInfo {
    caHolderInfo(dto: {caHash: ""caHash_mock"", maxResultCount: 1, skipCount: 0}) {
        guardianList {
            guardians {
                isLoginGuardian
                salt
                identifierHash
                verifierId
                type
            }
        }
        originChainId
        managerInfos {
            extraData
            address
        }
        caAddress
        caHash
        chainId
        id
    }
}";
            
        var config = GetPortkeyGraphQlConfig($"t:{nameof(GraphQLConfig)} PortkeyGraphQLConfig");
        IGraphQL graphQlTest = new GraphQL(config);
        
        var query = graphQlTest.GetQueryByName(QUERY_NAME_GETCAHOLDERINFO);
        if (query == null)
        {
            Assert.Fail($"No Query by name <{QUERY_NAME_GETCAHOLDERINFO}> found!");
        }
        var dto = new GraphQLCodeGen.Types.GetCaHolderInfoDto
        {
            skipCount = 0,
            maxResultCount = 1,
            caHash = "caHash_mock"
        };
        query.SetArgs(new { dto = dto.GetInputObject()});
        
        Assert.AreEqual(EXPECTED_RESULT, query.query);

        yield break;
    }
}
