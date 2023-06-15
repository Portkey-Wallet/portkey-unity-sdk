using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Portkey.Core;
using Portkey.Network;
using Portkey.Storage;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
#if UNITY_EDITOR
using System.Threading.Tasks;
#endif

namespace Portkey.GraphQL
{
    [CreateAssetMenu(fileName = "API Reference", menuName = "Portkey/GraphQL/API Reference")]
    public class GraphQLConfig : ScriptableObject, IGraphQLEditor, IGraphQL
    {
        [SerializeField]
        private string url;
        [SerializeField]
        public List<GraphQLQuery> queries;
        [SerializeField]
        private IHttp request = null;
        
        private string queryEndpoint;
        
        // stores introspection raw result
        private string introspection;
        // stores schema of the introspection
        private Introspection.SchemaClass schemaClass = null;

#if UNITY_EDITOR
        private IStorageSuite<string> storage;
        // check if introspection is loading
        private bool loading = false;
        // request for introspection. Only used on editor functions because
        // Unity editor cannot run coroutines and async/await cannot be used on webgl
        private UnityWebRequest editorRequest;
        //schema file name
        private string schemaFileName;
#endif
        
        //getter for schemaClass
        public Introspection.SchemaClass GetSchemaClass()
        {
            return schemaClass;
        }
        
        public void GetHolderInfoByManager(string manager, string chainId)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerator Query<T>(string query, IHttp.successCallback<T> successCallback, IHttp.errorCallback errorCallback)
        {
            return request.Post<T>(url, query, successCallback, errorCallback);
        }

        public IEnumerator Query<T>(GraphQLQuery query, IHttp.successCallback<T> successCallback, IHttp.errorCallback errorCallback)
        {
            if (String.IsNullOrEmpty(query.query))
            {
                query.CompleteQuery();
            }
            return request.Post<T>(url, query.query, successCallback, errorCallback);
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
            return loading;
        }
        
        public void OnEnable()
        {
            schemaFileName = $"{name}schema.txt";
            if(storage == null)
            {
                storage = new PersistentLocalStorage(Application.dataPath + "/Portkey/Configs/GraphQL");
            }
        }
        
        private void HandleIntrospection()
        {
            if (!editorRequest.isDone)
                return;
            EditorApplication.update -= HandleIntrospection;
            introspection = editorRequest.downloadHandler.text;
            storage.SetItem(schemaFileName, introspection);
            schemaClass = JsonConvert.DeserializeObject<Introspection.SchemaClass>(introspection);
            if (schemaClass.data.__schema.queryType != null)
                queryEndpoint = schemaClass.data.__schema.queryType.name;
            
            editorRequest.uploadHandler.Dispose();
            editorRequest.downloadHandler.Dispose();
            editorRequest.Dispose();
            
            loading = false;
        }
        
        public void Introspect()
        {
            loading = true;

            string jsonData = JsonConvert.SerializeObject(new{query = Introspection.schemaIntrospectionQuery});
            
            byte[] postData = Encoding.ASCII.GetBytes(jsonData);
            editorRequest = new UnityWebRequest(url, UnityWebRequest.kHttpVerbPOST);
            
            editorRequest.uploadHandler = new UploadHandlerRaw(postData);
            editorRequest.disposeUploadHandlerOnDispose = true;
            editorRequest.downloadHandler = new DownloadHandlerBuffer();
            editorRequest.disposeDownloadHandlerOnDispose = true;
            
            editorRequest.SetRequestHeader("Content-Type", "application/json");

            try
            {
                editorRequest.SendWebRequest();
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }

            EditorApplication.update += HandleIntrospection;
        }

        public bool InitSchema()
        {
            if (schemaClass == null || schemaClass.data == null){
                try{
                    introspection = storage.GetItem(schemaFileName);
                }
                catch{
                    return false;
                }

                if (schemaClass == null || introspection == null)
                {
                    return false;
                }
                
                schemaClass = JsonConvert.DeserializeObject<Introspection.SchemaClass>(introspection);
                if (schemaClass.data.__schema.queryType != null)
                    queryEndpoint = schemaClass.data.__schema.queryType.name;
            }

            return true;
        }

        public void CreateNewQuery()
        {
            if (!InitSchema())
            {
                Debug.Log("Schema not initialized!");
                return;
            }
            if (queries == null)
                queries = new List<GraphQLQuery>();
            GraphQLQuery query = new GraphQLQuery{fields = new List<Field>(), queryOptions = new List<string>(), type = GraphQLQuery.Type.Query};
            
            Introspection.SchemaClass.Data.Schema.Type queryType = schemaClass.data.__schema.types.Find((aType => aType.name == queryEndpoint));
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
            Introspection.SchemaClass.Data.Schema.Type type = schemaClass.data.__schema.types.Find((aType => aType.name == typeName));
            if (type?.fields == null || type.fields.Count == 0){
                return false;
            }

            return true;
        }

        public void AddField(GraphQLQuery query, string typeName, Field parent = null)
        {
            Introspection.SchemaClass.Data.Schema.Type type = schemaClass.data.__schema.types.Find((aType => aType.name == typeName));
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
                schemaClass.data.__schema.types.Find((aType => aType.name == queryEndpoint));
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