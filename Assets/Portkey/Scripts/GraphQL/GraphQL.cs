using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

namespace Portkey.GraphQL
{
    public class GraphQL : ScriptableObject, IGraphQLEditor, IGraphQL
    {
        [SerializeField]
        private string url;
        [SerializeField]
        public List<GraphQLQuery> queries;

        private string queryEndpoint;
        //TODO: change http request to IHTTPClient
        private UnityWebRequest request;
        // stores introspection raw result
        private string introspection;
        // auth token for posting if needed
        private string authToken;
        // stores schema of the introspection
        private Introspection.SchemaClass schemaClass;
        
#if UNITY_EDITOR
        // check if introspection is loading
        private bool loading = false;
#endif

        public void SetAuthToken(string auth)
        {
            authToken = auth;
        }
        
        public void GetHolderInfoByManager(string manager, string chainId)
        {
            throw new System.NotImplementedException();
        }

        public T Query<T>(string query)
        {
            throw new System.NotImplementedException();
        }

        public T Query<T>(GraphQLQuery query)
        {
            throw new System.NotImplementedException();
        }

        public GraphQLQuery GetQueryByName(string queryName)
        {
            return queries.Find(aQuery => aQuery.name == queryName);
        }

        private void HandleIntrospection()
        {
            if (!request.isDone)
                return;
            EditorApplication.update -= HandleIntrospection;
            introspection = request.downloadHandler.text;
            //TODO: change to save to file on interface
            //File.WriteAllText(Application.dataPath + $"{Path.DirectorySeparatorChar}{name}schema.txt",introspection);
            schemaClass = JsonConvert.DeserializeObject<Introspection.SchemaClass>(introspection);
            if (schemaClass.data.__schema.queryType != null)
                queryEndpoint = schemaClass.data.__schema.queryType.name;
            loading = false;
        }

        public void Introspect()
        {
            loading = true;
            //TODO: change to http interface
            //request = await HttpHandler.PostAsync(url, Introspection.schemaIntrospectionQuery, authToken);
            EditorApplication.update += HandleIntrospection;
        }

        public void InitSchema()
        {
            if (schemaClass == null){
                try{
                    //TODO: change to save to file on interface
                    //introspection = File.ReadAllText(Application.dataPath + $"{Path.DirectorySeparatorChar}{name}schema.txt");
                }
                catch{
                    return;
                }
                
                schemaClass = JsonConvert.DeserializeObject<Introspection.SchemaClass>(introspection);
                if (schemaClass.data.__schema.queryType != null)
                    queryEndpoint = schemaClass.data.__schema.queryType.name;
            }
        }

        public void CreateNewQuery()
        {
            InitSchema();
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

        public void AddField(GraphQLQuery query, string typeName, Field parent)
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
    }
}