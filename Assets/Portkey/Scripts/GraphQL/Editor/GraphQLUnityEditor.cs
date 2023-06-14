using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using Portkey.Core;
using Portkey.Storage;
using UnityEditor;
using UnityEngine;

namespace Portkey.GraphQL.Editor
{
    [CustomEditor(typeof(GraphQL))]
    public class GraphQLUnityEditor : UnityEditor.Editor
    {
        private const string GENERATED_CODE_FOLDER = "/Portkey/Scripts/__Generated__";
        private IStorageSuite<string> _storage;
        private int index;
        private SerializedObject graphObject;

        public void OnEnable()
        {
            if (_storage == null)
            {
                _storage = new PersistentLocalStorage(Application.dataPath + GENERATED_CODE_FOLDER);
            }
        }

        public override void OnInspectorGUI(){
            GraphQL graph = (GraphQL) target;
            graphObject = new UnityEditor.SerializedObject(graph);
            GUIStyle style = new GUIStyle{fontSize = 15, alignment = TextAnchor.MiddleCenter};
            EditorGUILayout.LabelField(graph.name, style);
            EditorGUILayout.Space();
            
            UnityEditor.SerializedProperty graphRequest = graphObject.FindProperty("request");
            graphRequest.objectReferenceValue = EditorGUILayout.ObjectField(graphRequest.objectReferenceValue, typeof(IHttp), true);
            if (GUI.changed) graphRequest.serializedObject.ApplyModifiedProperties();
            
            if (!graph.InitSchema())
            {
                Debug.Log("Schema not initialized!");
                return;
            }
            if (GUILayout.Button("Reset")){
                graph.DeleteAllQueries();
            }

            EditorGUILayout.Space();
            EditorGUILayout.Space();
            UnityEditor.SerializedProperty graphUrl = graphObject.FindProperty("url");
            graphUrl.stringValue = EditorGUILayout.TextField("Url", graphUrl.stringValue);
            if (GUI.changed) graphUrl.serializedObject.ApplyModifiedProperties();
            
            if (GUILayout.Button("Introspect")){
                graph.Introspect();
            }
            
#if UNITY_EDITOR
            if (graph.IsLoading()){
                EditorGUILayout.LabelField("API is being introspected. Please wait...");
            }
#endif

            EditorGUILayout.Space();
            EditorGUILayout.Space();
            if (graph.GetSchemaClass() == null){
                return;
            }
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Create New Query")){
                graph.CreateNewQuery();
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            DisplayFields(graph, graph.queries, "Query");

            EditorUtility.SetDirty(graph);
        }

        private void DisplayFields(GraphQL graph, List<GraphQLQuery> queryList, string type){
            if (queryList != null){
                if (queryList.Count > 0)
                    EditorGUILayout.LabelField(type);
                for (int i = 0; i < queryList.Count; i++){
                    EditorGUILayout.Space();
                    EditorGUILayout.Space();
                    EditorGUILayout.Space();
                    GraphQLQuery query = queryList[i];
                    query.name = EditorGUILayout.TextField($"{type} Name", query.name);
                    string[] options = query.queryOptions.ToArray();
                    if (String.IsNullOrEmpty(query.returnType)){
                        index = EditorGUILayout.Popup(type, index, options);
                        query.queryString = options[index];
                        EditorGUILayout.LabelField(options[index]);
                        if (GUILayout.Button($"Confirm {type}")){
                            graph.GetQueryReturnType(query, options[index]);
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
                            graph.GetQueryReturnType(query, options[index]);
                            graph.AddField(query, query.returnType);
                        }
                    }
                    

                    foreach (Field field in query.fields){
                        GUI.color = new Color(0.8f,0.8f,0.8f);
                        string[] fieldOptions = field.possibleFields.Select((aField => aField.name)).ToArray();
                        EditorGUILayout.BeginHorizontal();
                        GUIStyle fieldStyle = EditorStyles.popup;
                        fieldStyle.contentOffset = new Vector2(field.parentIndexes.Count * 20, 0);
                        field.Index = EditorGUILayout.Popup(field.Index, fieldOptions, fieldStyle);
                        GUI.color = Color.white;
                        field.CheckSubFields(graph.GetSchemaClass());
                        if (field.hasSubField){
                            if (GUILayout.Button("Create Sub Field")){
                                graph.AddField(query, field.possibleFields[field.Index].type, field);
                                break;
                            }
                        }

                        if (GUILayout.Button("x", GUILayout.MaxWidth(20))){
                            int parentIndex = query.fields.FindIndex(aField => aField == field);
                            query.fields.RemoveAll(afield => afield.parentIndexes.Contains(parentIndex));
                            query.fields.Remove(field);
                            field.hasChanged = false;
                            break;
                        }

                        EditorGUILayout.EndHorizontal();

                        if (field.hasChanged){
                            int parentIndex = query.fields.FindIndex(aField => aField == field);
                            query.fields.RemoveAll(afield => afield.parentIndexes.Contains(parentIndex));
                            field.hasChanged = false;
                            break;
                        }

                        
                    }
                    EditorGUILayout.Space();
                    EditorGUILayout.Space();

                    if (query.fields.Count > 0){
                        if (GUILayout.Button($"Preview {type}")){
                            query.CompleteQuery();
                        }
                    }
                    
                    if (query.fields.Count > 0)
                    {
                        if (GUILayout.Button($"Generate {type}")) {
                            IGraphQLCodeGenerator codeGenerator = new GraphQLCSharpCodeGenerator(graph.GetSchemaClass(), _storage);
                            codeGenerator.GenerateDTOClass(query.returnType);
                        }
                    }

                    if (GUILayout.Button("Delete")){
                        graph.DeleteQuery(queryList, i);
                    }
                    
                }

                EditorGUILayout.Space();
            }
            
        }
    }
}

