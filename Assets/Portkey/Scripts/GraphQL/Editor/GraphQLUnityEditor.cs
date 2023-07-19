using System;
using System.Collections.Generic;
using System.Linq;
using Portkey.Core;
using Portkey.Storage;
using UnityEditor;
using UnityEngine;

namespace Portkey.GraphQL.Editor
{
#if UNITY_EDITOR
    /// <summary>
    /// GraphQLUnityEditor is a custom editor for GraphQLConfig.
    /// It helps user add custom queries to GraphQLConfig.
    /// </summary>
    [CustomEditor(typeof(GraphQLConfig))]
    public class GraphQLUnityEditor : UnityEditor.Editor
    {
        private const string GENERATED_CODE_FOLDER = "/Portkey/Scripts/__Generated__";
        private IStorageSuite<string> _storage;
        private int _index;
        private SerializedObject _graphObject;

        public void OnEnable()
        {
            if (_storage == null)
            {
                _storage = new PersistentLocalStorage(Application.dataPath + GENERATED_CODE_FOLDER);
            }
        }

        public override void OnInspectorGUI(){
            GraphQLConfig graph = (GraphQLConfig) target;
            _graphObject = new UnityEditor.SerializedObject(graph);
            var style = new GUIStyle{fontSize = 15, alignment = TextAnchor.MiddleCenter};
            EditorGUILayout.LabelField(graph.name, style);
            EditorGUILayout.Space();
            
            UnityEditor.SerializedProperty graphRequest = _graphObject.FindProperty("request");
            graphRequest.objectReferenceValue = EditorGUILayout.ObjectField(graphRequest.objectReferenceValue, typeof(IHttp), true);
            if (GUI.changed) graphRequest.serializedObject.ApplyModifiedProperties();
            
            if (GUILayout.Button("Reset")){
                graph.DeleteAllQueries();
            }

            EditorGUILayout.Space();
            EditorGUILayout.Space();
            var graphUrl = _graphObject.FindProperty("url");
            graphUrl.stringValue = EditorGUILayout.TextField("Url", graphUrl.stringValue);
            if (GUI.changed) graphUrl.serializedObject.ApplyModifiedProperties();
            
            if (GUILayout.Button("Introspect"))
            {
                graph.Introspect();
            }
            
            if (!graph.InitSchema())
            {
                Debug.Log("Schema not initialized!");
                return;
            }
            
            if (graph.IsLoading)
            {
                EditorGUILayout.LabelField("API is being introspected. Please wait...");
            }

            EditorGUILayout.Space();
            EditorGUILayout.Space();
            if (graph.GetSchemaClass() == null){
                return;
            }
            
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Create New Query"))
            {
                graph.CreateNewQuery();
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            DisplayFields(graph, graph.queries, "Query");

            EditorUtility.SetDirty(graph);
        }

        private void DisplayFields(GraphQLConfig graph, List<GraphQLQuery> queryList, string type)
        {
            if (queryList != null){
                if (queryList.Count > 0)
                    EditorGUILayout.LabelField(type);
                for (int i = 0; i < queryList.Count; i++){
                    EditorGUILayout.Space();
                    EditorGUILayout.Space();
                    EditorGUILayout.Space();
                    var query = queryList[i];
                    query.name = EditorGUILayout.TextField($"{type} Name", query.name);
                    var options = query.queryOptions.ToArray();
                    if (String.IsNullOrEmpty(query.returnType)){
                        _index = EditorGUILayout.Popup(type, _index, options);
                        query.queryString = options[_index];
                        EditorGUILayout.LabelField(options[_index]);
                        if (GUILayout.Button($"Confirm {type}")){
                            graph.GetQueryReturnType(query, options[_index]);
                        }
                        if (GUILayout.Button("Delete")){
                            graph.DeleteQuery(queryList, i);
                        }

                        continue;
                    }

                    if (query.isComplete){
                        GUILayout.Label(query.query);
                        if (query.fields.Count > 0){
                            if (GUILayout.Button($"Edit {type}")){
                                graph.EditQuery(query);
                            }
                        }

                        if (GUILayout.Button("Delete")){
                            graph.DeleteQuery(queryList, i);
                        }

                        continue;
                    }


                    EditorGUILayout.LabelField(query.queryString,
                        $"Return Type: {query.returnType}");
                    if (graph.CheckSubFields(query.returnType)){
                        if (GUILayout.Button("Create Field")){
                            graph.GetQueryReturnType(query, options[_index]);
                            graph.AddField(query, query.returnType);
                            return;
                        }
                        
                        if (GUILayout.Button("Add All Fields")){
                            graph.GetQueryReturnType(query, options[_index]);
                            graph.AddAllFields(query, query.returnType);
                            return;
                        }
                    }

                    foreach (Field field in query.fields){
                        GUI.color = new Color(0.8f,0.8f,0.8f);
                        var fieldOptions = field.fieldOptions.Select((aField => aField.name)).ToArray();
                        EditorGUILayout.BeginHorizontal();
                        var fieldStyle = EditorStyles.popup;
                        fieldStyle.contentOffset = new Vector2(field.ancestors * 20, 0);
                        var newIndex = EditorGUILayout.Popup(field.Index, fieldOptions, fieldStyle);
                        if (newIndex != field.Index){
                            RemoveChildFields(field, query);
                            field.Index = newIndex;
                        }
                        GUI.color = Color.white;
                        field.CheckSubFields(graph.GetSchemaClass());
                        if (field.hasSubField){
                            if (GUILayout.Button("Create Sub Field")){
                                graph.AddField(query, field.fieldOptions[field.Index].type, field);
                                break;
                            }
                        }

                        if (GUILayout.Button("x", GUILayout.MaxWidth(20))){
                            
                            RemoveChildFields(field, query);
                            
                            query.fields.Remove(field);
                            field.hasChanged = false;

                            break;
                        }

                        if (field.hasChanged)
                        {
                            field.hasChanged = false;
                            break;
                        }

                        EditorGUILayout.EndHorizontal();
                    }
                    EditorGUILayout.Space();
                    EditorGUILayout.Space();

                    if (query.fields.Count > 0){
                        if (GUILayout.Button($"Preview {type}")){
                            query.BuildQueryString();
                        }
                    }

                    if (GUILayout.Button("Delete")){
                        graph.DeleteQuery(queryList, i);
                    }
                    
                }

                EditorGUILayout.Space();
            }
            
        }

        private static void RemoveChildFields(Field field, GraphQLQuery query)
        {
            if (field.hasSubField)
            {
                int startIndex = query.fields.FindIndex(aField => aField == field) + 1;
                var ancestors = field.ancestors + 1;

                int indexToRemove = -1;
                // find the last field under the same ancestor
                for (var m = startIndex; m < query.fields.Count; ++m)
                {
                    if (query.fields[m].ancestors < ancestors)
                    {
                        indexToRemove = m;
                        break;
                    }
                }

                //there is no found child in this object
                if (indexToRemove == -1)
                {
                    return;
                }

                query.fields.RemoveRange(startIndex, indexToRemove - startIndex);
            }
        }
    }
#endif
}

