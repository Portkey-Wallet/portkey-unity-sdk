using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Portkey.Core;
using Portkey.Storage;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

namespace Portkey.GraphQL
{
    /// <summary>
    /// GraphQLConfig is a ScriptableObject that stores the GraphQL API reference.
    /// It is the single point for user accessing GraphQL API in the form of a config file.
    /// </summary>
    [CreateAssetMenu(fileName = "API Reference", menuName = "Portkey/GraphQL/API Reference")]
    public class GraphQLConfig : ScriptableObject, IGraphQLEditor, IGraphQL
    {
        [SerializeField]
        private string url;
        [SerializeField]
        public List<GraphQLQuery> queries;
        [SerializeField]
        private IHttp request = null;
        
        private string _queryEndpoint;
        
        // stores introspection raw result
        private string _introspection;
        // stores schema of the introspection
        private Introspection.SchemaClass _schemaClass = null;

#if UNITY_EDITOR
        private IStorageSuite<string> _storage;
        // check if introspection is loading
        public bool IsLoading { get; private set; } = false;
        // request for introspection. Only used on editor functions because
        // Unity editor cannot run coroutines and async/await cannot be used on webgl
        private UnityWebRequest _editorRequest;
        //schema file name
        private string _schemaFileName;
#endif
        
        public Introspection.SchemaClass GetSchemaClass()
        {
            return _schemaClass;
        }

        public IEnumerator Query<T>(string query, IGraphQL.successCallback<T> successCallback, IGraphQL.errorCallback errorCallback)
        {
            var jsonData = JsonConvert.SerializeObject(new{query});
            return request.Post(url, jsonData, 
                (response) =>
                                {
                                    var json = JObject.Parse(response);
                                    string data = null;
                                    if (json.TryGetValue("errors", out var errorMessage))
                                    {
                                        //process data and wrapper
                                        var error = errorMessage.ToString();
                                        errorCallback(error);
                                        return;
                                    }
                                    
                                    if (json.TryGetValue("data", out var value))
                                    {
                                        //process data and wrapper
                                        data = value.ToString();
                                    }
                                    else
                                    {
                                        errorCallback("No data in response. Incorrect response format.");
                                        return;
                                    }
                                    //deserialize response
                                    var deserializedObject = JsonConvert.DeserializeObject<T>(data);
                                    //call success callback
                                    successCallback(deserializedObject);
                                }, 
                    (error) =>
                                {
                                    //call error callback
                                    errorCallback(error);
                                });
        }

        public IEnumerator Query<T>(GraphQLQuery query, IGraphQL.successCallback<T> successCallback, IGraphQL.errorCallback errorCallback)
        {
            if (String.IsNullOrEmpty(query.query))
            {
                query.BuildQueryString();
            }
            return Query<T>(query.query, successCallback, errorCallback);
        }

        public GraphQLQuery GetQueryByName(string queryName)
        {
            return queries.Find(aQuery => aQuery.name == queryName);
        }
        
        #region EditorOnly
#if UNITY_EDITOR
        public void OnEnable()
        {
            _schemaFileName = $"{name}schema.txt";
            _storage ??= new PersistentLocalStorage(Application.dataPath + "/Portkey/Configs/GraphQL");
        }
        
        private void HandleIntrospection()
        {
            if (!_editorRequest.isDone)
                return;
            EditorApplication.update -= HandleIntrospection;
            _introspection = _editorRequest.downloadHandler.text;
            _storage.SetItem(_schemaFileName, _introspection);
            _schemaClass = JsonConvert.DeserializeObject<Introspection.SchemaClass>(_introspection);
            if (_schemaClass.data.__schema.queryType != null)
                _queryEndpoint = _schemaClass.data.__schema.queryType.name;
            
            _editorRequest.uploadHandler.Dispose();
            _editorRequest.downloadHandler.Dispose();
            _editorRequest.Dispose();
            
            IsLoading = false;
        }
        
        public void Introspect()
        {
            if(IsLoading)
                return;
            
            IsLoading = true;

            string jsonData = JsonConvert.SerializeObject(new{query = Introspection.schemaIntrospectionQuery});
            
            var postData = Encoding.ASCII.GetBytes(jsonData);
            _editorRequest = new UnityWebRequest(url, UnityWebRequest.kHttpVerbPOST);
            
            _editorRequest.uploadHandler = new UploadHandlerRaw(postData);
            _editorRequest.disposeUploadHandlerOnDispose = true;
            _editorRequest.downloadHandler = new DownloadHandlerBuffer();
            _editorRequest.disposeDownloadHandlerOnDispose = true;
            
            _editorRequest.SetRequestHeader("Content-Type", "application/json");

            try
            {
                _editorRequest.SendWebRequest();
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }

            EditorApplication.update += HandleIntrospection;
        }

        public bool InitSchema()
        {
            if (_schemaClass == null || _schemaClass.data == null){
                try{
                    _introspection = _storage.GetItem(_schemaFileName);
                }
                catch{
                    return false;
                }

                if (_schemaClass == null || _introspection == null)
                {
                    return false;
                }
                
                _schemaClass = JsonConvert.DeserializeObject<Introspection.SchemaClass>(_introspection);
                if (_schemaClass.data.__schema.queryType != null)
                    _queryEndpoint = _schemaClass.data.__schema.queryType.name;
            }

            return true;
        }

        public void CreateNewQuery()
        {
            if (!InitSchema())
            {
                return;
            }
            queries ??= new List<GraphQLQuery>();
            var query = new GraphQLQuery{fields = new List<Field>(), queryOptions = new List<string>(), operationType = GraphQLQuery.OperationType.Query};
            
            var queryType = _schemaClass.data.__schema.types.Find((aType => aType.name == _queryEndpoint));
            for (int i = 0; i < queryType.fields.Count; i++){
                query.queryOptions.Add(queryType.fields[i].name);
            }

            queries.Add(query);
        }

        public void EditQuery(GraphQLQuery query)
        {
            query.isComplete = false;
        }

        public bool CheckSubFields(string typeName)
        {
            var type = _schemaClass.data.__schema.types.Find((aType => aType.name == typeName));
            return type?.fields != null && type.fields.Count != 0;
        }
        
        /// <summary>
        /// A recursive way to add all fields of a query as an option field in Unity Editor.
        /// </summary>
        public void AddAllFields(GraphQLQuery query, string typeName, Field parent = null)
        {
            var type = _schemaClass.data.__schema.types.Find((aType => aType.name == typeName));
            var subFields = type.fields;
            var parentIndex = query.fields.FindIndex(aField => aField == parent);
            var ancestors = (parent == null) ? 0 : parent.ancestors + 1;

            // Loop through and initialize all sub fields for this field and add them as options
            for(var i = 0; i < subFields.Count; i++)
            {
                // start initializing child field
                var childField = new Field{ancestors = ancestors};
                foreach (var field in subFields)
                {
                    // add sibling field options to the field
                    childField.fieldOptions.Add((Field)field);
                }
                childField.Index = i;
                
                // add the child field to the query at the start of its parent
                query.fields.Insert(parentIndex+1, childField);
                
                childField.CheckSubFields(GetSchemaClass());
                
                // recursively initialize all sub fields of this field as options
                if (childField.hasSubField)
                {
                    AddAllFields(query, childField.fieldOptions[childField.Index].type, childField);
                }
            }
        }

        /// <summary>
        /// Function to add field option for a query in Unity Editor.
        /// </summary>
        /// <param name="query">Corresponding GraphQLQuery object to add field option for.</param>
        /// <param name="typeName">Name of the class in schema.</param>
        /// <param name="parent">The parent of the field.</param>
        public void AddField(GraphQLQuery query, string typeName, Field parent = null)
        {
            var type = _schemaClass.data.__schema.types.Find((aType => aType.name == typeName));
            var subFields = type.fields;
            var parentIndex = query.fields.FindIndex(aField => aField == parent);
            var ancestors = (parent == null) ? 0 : parent.ancestors + 1;
            var childField = new Field{ancestors = ancestors};
            
            // add all possible field options for the new field based on its siblings
            foreach (var field in subFields){
                childField.fieldOptions.Add((Field)field);
            }
            
            childField.Index = 0;
            childField.CheckSubFields(GetSchemaClass());

            // add the child field to the query at the start of its parent
            query.fields.Insert(parentIndex+1, childField);
        }

        private string GetFieldType(Introspection.SchemaClass.Data.Schema.Type.Field field)
        {
            var newField = (Field)field;
            return newField.type;
        }

        public void GetQueryReturnType(GraphQLQuery query, string queryName)
        {
            var queryType = _schemaClass.data.__schema.types.Find((aType => aType.name == _queryEndpoint));
            var field = queryType.fields.Find((aField => aField.name == queryName));

            query.returnType = GetFieldType(field);
        }

        public void DeleteQuery(List<GraphQLQuery> query, int index)
        {
            query.RemoveAt(index);
        }

        public void DeleteAllQueries()
        {
            queries = new List<GraphQLQuery>();
        }
#endif
        #endregion
    }
}