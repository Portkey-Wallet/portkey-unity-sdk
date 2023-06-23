using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        private bool _loading = false;
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
            return request.Post(url, query, 
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
                query.CompleteQuery();
            }
            return Query<T>(query.query, successCallback, errorCallback);
        }

        public GraphQLQuery GetQueryByName(string queryName)
        {
            return queries.Find(aQuery => aQuery.name == queryName);
        }
        
        #region EditorOnly
#if UNITY_EDITOR
        //getter for loading
        public bool IsLoading()
        {
            return _loading;
        }
        
        public void OnEnable()
        {
            _schemaFileName = $"{name}schema.txt";
            if(_storage == null)
            {
                _storage = new PersistentLocalStorage(Application.dataPath + "/Portkey/Configs/GraphQL");
            }
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
            
            _loading = false;
        }
        
        public void Introspect()
        {
            if(_loading)
                return;
            
            _loading = true;

            string jsonData = JsonConvert.SerializeObject(new{query = Introspection.schemaIntrospectionQuery});
            
            byte[] postData = Encoding.ASCII.GetBytes(jsonData);
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
                //Debug.Log("Schema not initialized!");
                return;
            }
            if (queries == null)
                queries = new List<GraphQLQuery>();
            GraphQLQuery query = new GraphQLQuery{fields = new List<Field>(), queryOptions = new List<string>(), type = GraphQLQuery.Type.Query};
            
            Introspection.SchemaClass.Data.Schema.Type queryType = _schemaClass.data.__schema.types.Find((aType => aType.name == _queryEndpoint));
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
            Introspection.SchemaClass.Data.Schema.Type type = _schemaClass.data.__schema.types.Find((aType => aType.name == typeName));
            if (type?.fields == null || type.fields.Count == 0){
                return false;
            }

            return true;
        }
        
        public void AddAllFields(GraphQLQuery query, string typeName, Field parent = null)
        {
            Introspection.SchemaClass.Data.Schema.Type type = _schemaClass.data.__schema.types.Find((aType => aType.name == typeName));
            List<Introspection.SchemaClass.Data.Schema.Type.Field> subFields = type.fields;
            int parentIndex = query.fields.FindIndex(aField => aField == parent);
            List<int> parentIndexes = new List<int>();
            if (parent != null){
                parentIndexes = new List<int>(parent.parentIndexes){parentIndex};
            }
            Field fielder = new Field{parentIndexes = parentIndexes};
            
            foreach (Introspection.SchemaClass.Data.Schema.Type.Field field in subFields){
                fielder.possibleFields.Add((Field)field);
            }

            for(int i = 0; i < fielder.possibleFields.Count; i++){
                Field newField = new Field{parentIndexes = parentIndexes};
            
                foreach (Introspection.SchemaClass.Data.Schema.Type.Field field in subFields){
                    newField.possibleFields.Add((Field)field);
                }
                
                newField.Index = i;
                
                if (newField.parentIndexes.Count == 0)
                {
                    query.fields.Add(newField);
                }
                else{

                    int index;
                    index = query.fields.FindLastIndex(aField =>
                        aField.parentIndexes.Count > newField.parentIndexes.Count &&
                        aField.parentIndexes.Contains(newField.parentIndexes.Last()));

                    if (index == -1){
                        index = query.fields.FindLastIndex(aField =>
                            aField.parentIndexes.Count > newField.parentIndexes.Count &&
                            aField.parentIndexes.Last() == newField.parentIndexes.Last());
                    }

                    if (index == -1){
                        index = newField.parentIndexes.Last();
                    }

                    index++;
                    query.fields[parentIndex].hasChanged = false;
                    query.fields.Insert(index, newField);
                }
                
                newField.CheckSubFields(GetSchemaClass());
                
                if (newField.hasSubField){
                    AddAllFields(query, newField.possibleFields[newField.Index].type, newField);
                }
            }
        }

        public void AddField(GraphQLQuery query, string typeName, Field parent = null)
        {
            Introspection.SchemaClass.Data.Schema.Type type = _schemaClass.data.__schema.types.Find((aType => aType.name == typeName));
            List<Introspection.SchemaClass.Data.Schema.Type.Field> subFields = type.fields;
            int parentIndex = query.fields.FindIndex(aField => aField == parent);
            List<int> parentIndexes = new List<int>();
            if (parent != null){
                parentIndexes = new List<int>(parent.parentIndexes){parentIndex};
            }
            Field fielder = new Field{parentIndexes = parentIndexes};
            
            foreach (Introspection.SchemaClass.Data.Schema.Type.Field field in subFields){
                fielder.possibleFields.Add((Field)field);
            }

            if (fielder.parentIndexes.Count == 0){
                query.fields.Add(fielder);
            }
            else{

                int index;
                index = query.fields.FindLastIndex(aField =>
                    aField.parentIndexes.Count > fielder.parentIndexes.Count &&
                    aField.parentIndexes.Contains(fielder.parentIndexes.Last()));

                if (index == -1){
                    index = query.fields.FindLastIndex(aField =>
                        aField.parentIndexes.Count > fielder.parentIndexes.Count &&
                        aField.parentIndexes.Last() == fielder.parentIndexes.Last());
                }

                if (index == -1){
                    index = fielder.parentIndexes.Last();
                }

                index++;
                query.fields[parentIndex].hasChanged = false;
                query.fields.Insert(index, fielder);
            }
        }
        
        private string GetFieldType(Introspection.SchemaClass.Data.Schema.Type.Field field)
        {
            Field newField = (Field)field;
            return newField.type;
        }

        public void GetQueryReturnType(GraphQLQuery query, string queryName)
        {
            Introspection.SchemaClass.Data.Schema.Type queryType =
                _schemaClass.data.__schema.types.Find((aType => aType.name == _queryEndpoint));
            Introspection.SchemaClass.Data.Schema.Type.Field field =
                queryType.fields.Find((aField => aField.name == queryName));

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