using System;
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
    public class GraphQLMonoTest : GraphQL, IMonoBehaviourTest
    {
        protected bool runFinished = false;
        public bool IsTestFinished
        {
            get { return runFinished; }
        }
        public void InitializeGraphQLConfig(string searchFilter = null)
        {
            if (searchFilter == null)
            {
                searchFilter = $"t:{nameof(GraphQLConfig)} PortkeyGraphQLConfig";
            }
            this._graphQLConfig = GetPortkeyGraphQlConfig(searchFilter);
        }

        protected static GraphQLConfig GetPortkeyGraphQlConfig(string searchFilter)
        {
            string[] guids = AssetDatabase.FindAssets(searchFilter);
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
    
    public class GraphQLMonoHTTPMockGetHolderInfoByManagerTest : GraphQLMonoTest
    {
        private void Awake()
        {
            InitializeGraphQLConfig($"t:{nameof(GraphQLConfig)} GraphQLConfigGetHolderInfoByManagerMock");
        }

        public void Start()
        {
            StartCoroutine(GetHolderInfoByManager("manager_mock", "chainId_mock", SuccessCallback, ErrorCallback));
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

    public class GraphQLMonoHTTPMockTest : GraphQLMonoTest
    {
        private const string QUERY_NAME_GETCAHOLDERINFO = "GetCAHolderInfo";

        private void Awake()
        {
            InitializeGraphQLConfig($"t:{nameof(GraphQLConfig)} GraphQLConfigMock");
        }

        public void Start()
        {
            GraphQLQuery query = GetQueryByName(QUERY_NAME_GETCAHOLDERINFO);
            if (query == null)
            {
                Assert.Fail($"No Query by name <{QUERY_NAME_GETCAHOLDERINFO}> found!");
            };
        
            StartCoroutine(Query<GraphQLCodeGen.Types.Query>(query.query, SuccessCallback, ErrorCallback));
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
        MonoBehaviourTest<GraphQLMonoTest> graphQlTest = new MonoBehaviourTest<GraphQLMonoTest>();
        graphQlTest.component.InitializeGraphQLConfig();
        
        GraphQLQuery query = ((IGraphQL)graphQlTest.component).GetQueryByName(QUERY_NAME_GETCAHOLDERINFO);
        if (query == null)
        {
            Assert.Fail($"No Query by name <{QUERY_NAME_GETCAHOLDERINFO}> found!");
        }
        GraphQLCodeGen.Types.GetCaHolderInfoDto dto = new GraphQLCodeGen.Types.GetCaHolderInfoDto();
        dto.skipCount = 0;
        dto.maxResultCount = 1;
        dto.caHash = "f78e0f6e5619863fe9bafc50be3641072be27cf449760d2f63aaa180a723bc9b";
        query.SetArgs(new { dto = dto.GetInputObject()});
        
        return ((IGraphQL)graphQlTest.component).Query<GraphQLCodeGen.Types.Query>(query.query, SuccessCallback, ErrorCallback);
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
        MonoBehaviourTest<GraphQLMonoTest> graphQlTest = new MonoBehaviourTest<GraphQLMonoTest>();
        graphQlTest.component.InitializeGraphQLConfig();
        
        GraphQLQuery query = ((IGraphQL)graphQlTest.component).GetQueryByName(QUERY_NAME_CAHOLDERMANAGERCHANGERECORDINFO);
        if (query == null)
        {
            Assert.Fail($"No Query by name <{QUERY_NAME_CAHOLDERMANAGERCHANGERECORDINFO}> found!");
        };
        
        return ((IGraphQL)graphQlTest.component).Query<GraphQLCodeGen.Types.Query>(query.query, GraphQLPostNullReturnSuccessCallback, GraphQLPostNullReturnErrorCallback);
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
        MonoBehaviourTest<GraphQLMonoTest> graphQlTest = new MonoBehaviourTest<GraphQLMonoTest>();
        graphQlTest.component.InitializeGraphQLConfig();
        
        const string QUERY_NAME = "QueryMock";
        
        GraphQLQuery query = ((IGraphQL)graphQlTest.component).GetQueryByName(QUERY_NAME);
        if (query == null)
        {
            Assert.Pass($"No Query by name <{QUERY_NAME}> found!");
        }

        Assert.Fail($"Should not have found Query <{QUERY_NAME}>");
        yield break;
    }
}