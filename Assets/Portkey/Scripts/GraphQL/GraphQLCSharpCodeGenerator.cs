using System.Collections.Generic;
using System.Linq;
using System.Text;
using Portkey.Core;
using UnityEngine;

namespace Portkey.GraphQL
{
    public class GraphQLCSharpCodeGenerator : IGraphQLCodeGenerator
    {
        private Introspection.SchemaClass _schemaClass;
        private IStorageSuite<string> _storage;

        //constructor
        public GraphQLCSharpCodeGenerator(Introspection.SchemaClass schemaClass, IStorageSuite<string> storage)
        {
            this._schemaClass = schemaClass;
            this._storage = storage;
        }
        
        //setter for schemaClass
        public void SetSchemaClass(Introspection.SchemaClass schemaClass)
        {
            this._schemaClass = schemaClass;
        }
        
        public void GenerateDTOClass(string className)
        {
            //generate class header
            StringBuilder genHeaderCode = new StringBuilder();
            
            //generate class body
            var fields = Fields(className);
            
            if(fields == null)
            {
                Debug.LogError($"Cannot find class {className} in schema!");
                return;
            }
            
            bool listHeaderIncluded = false;

            StringBuilder genBodyCode = new StringBuilder();

            Dictionary<Introspection.SchemaClass.Data.Schema.Type.TypeKind, HashSet<string>> childClassList = new Dictionary<Introspection.SchemaClass.Data.Schema.Type.TypeKind, HashSet<string>>();
            
            for (int i = 0; i < fields.Count; i++){
                Introspection.SchemaClass.Data.Schema.Type.Field field = fields[i];

                string objectType = GetObjectTypeDeclaration(field.type, childClassList, ref listHeaderIncluded);
                genBodyCode.Append($"\t\tpublic {objectType} {field.name} {{get; set;}}\n");
            }
            
            if (listHeaderIncluded)
            {
                genHeaderCode.Append("using System.Collections.Generic;\n");
            }
            
            genBodyCode.Append("\t}\n");
            
            //insert namespace
            genHeaderCode.Append("namespace Portkey.GraphQL\n{\n");
            //insert class name
            genHeaderCode.Append($"\tpublic class {className}\n\t{{\n");
            //concatenate header and body
            genHeaderCode.Append(genBodyCode);
            //close namespace
            genHeaderCode.Append("}");
            
            //Debug.Log(genHeaderCode.ToString());
            _storage.SetItem(className + ".cs", genHeaderCode.ToString());
            
            //generate child classes
            GenerateChildClass(childClassList);
        }

        private static string GetObjectTypeDeclaration(Introspection.SchemaClass.Data.Schema.Type type,
            Dictionary<Introspection.SchemaClass.Data.Schema.Type.TypeKind, HashSet<string>> childClassList, ref bool listHeaderIncluded)
        {
            string ret = "";
            switch (type.kind)
            {
                case Introspection.SchemaClass.Data.Schema.Type.TypeKind.ENUM:
                case Introspection.SchemaClass.Data.Schema.Type.TypeKind.OBJECT:
                    ret = type.name;
                    ExtractClass(childClassList, type.kind, type.name);
                    break;
                case Introspection.SchemaClass.Data.Schema.Type.TypeKind.SCALAR:
                    string fieldType = GetFieldType(type.name);
                    ret = fieldType;
                    break;
                case Introspection.SchemaClass.Data.Schema.Type.TypeKind.NON_NULL:
                    ret = GetObjectTypeDeclaration(type.ofType, childClassList, ref listHeaderIncluded);
                    break;
                case Introspection.SchemaClass.Data.Schema.Type.TypeKind.LIST:
                    ret = $"IList<{GetObjectTypeDeclaration(type.ofType, childClassList, ref listHeaderIncluded)}>";
                    listHeaderIncluded = true;
                    break;
                default:
                    Debug.LogError("Unhandled Type!");
                    break;
            }

            return ret;
        }

        private static string GetFieldType(string fieldTypeName)
        {
            string fieldType = "";
            switch (fieldTypeName)
            {
                case "String":
                    fieldType = "string";
                    break;
                case "Boolean":
                    fieldType = "bool";
                    break;
                case "Int":
                    fieldType = "int";
                    break;
                case "Long":
                    fieldType = "long";
                    break;
                default:
                    Debug.LogError("Unrecognized Type!");
                    break;
            }

            return fieldType;
        }

        private void GenerateEnum(string enumName)
        {
            List<Introspection.SchemaClass.Data.Schema.Type.EnumValue> enumValues = null;
            for (int i = 0; i < _schemaClass.data.__schema.types.Count(); ++i)
            {
                Introspection.SchemaClass.Data.Schema.Type type = _schemaClass.data.__schema.types[i];

                if (type.name != enumName)
                {
                    continue;
                }

                enumValues = type.enumValues;
            }

            if (enumValues == null)
            {
                Debug.LogError("Cannot find enum " + enumName + " in schema!");
                return;
            }
            
            //generate class header
            StringBuilder genHeaderCode = new StringBuilder();
            
            //generate class body
            StringBuilder genBodyCode = new StringBuilder();

            for (int i = 0; i < enumValues.Count; i++)
            {
                genBodyCode.Append($"\t\t{enumValues[i].name},\n");
            }

            genBodyCode.Append("\t}\n");
            
            //insert namespace
            genHeaderCode.Append("namespace Portkey.GraphQL\n{\n");
            //insert class name
            genHeaderCode.Append($"\tpublic enum {enumName}\n\t{{\n");
            //concatenate header and body
            genHeaderCode.Append(genBodyCode);
            //close namespace
            genHeaderCode.Append("}");
            
            //Debug.Log(genHeaderCode.ToString());
            _storage.SetItem(enumName + ".cs", genHeaderCode.ToString());
        }

        private void GenerateChildClass(Dictionary<Introspection.SchemaClass.Data.Schema.Type.TypeKind, HashSet<string>> childClassList)
        {
            //loop through childClassList
            foreach (var pair in childClassList)
            {
                if(pair.Key == Introspection.SchemaClass.Data.Schema.Type.TypeKind.ENUM)
                {
                    foreach (var childClass in pair.Value)
                    {
                        GenerateEnum(childClass);
                    }
                }
                else
                {
                    foreach (var childClass in pair.Value)
                    {
                        GenerateDTOClass(childClass);
                    }
                }
            }
            
            /*
            foreach (var childClass in childClassList)
            {
                for (int i = 0; i < schemaClass.data.__schema.types.Count(); ++i)
                {
                    Introspection.SchemaClass.Data.Schema.Type type = schemaClass.data.__schema.types[i];

                    if (type.name != childClass)
                    {
                        continue;
                    }

                    GenerateDTOClass(type.name);
                }
            }*/
        }

        private List<Introspection.SchemaClass.Data.Schema.Type.Field> Fields(string className)
        {
            List<Introspection.SchemaClass.Data.Schema.Type.Field> fields = null;
            for (int i = 0; i < _schemaClass.data.__schema.types.Count(); ++i)
            {
                Introspection.SchemaClass.Data.Schema.Type type = _schemaClass.data.__schema.types[i];

                if (type.name != className)
                {
                    continue;
                }

                fields = type.fields;
            }

            return fields;
        }

        private static void ExtractClass(Dictionary<Introspection.SchemaClass.Data.Schema.Type.TypeKind, HashSet<string>> childClassList, Introspection.SchemaClass.Data.Schema.Type.TypeKind type, string childClassName)
        {
            if (childClassList.TryGetValue(type, out var value))
            {
                value.Add(childClassName);
            }
            else
            {
                childClassList.Add(type, new HashSet<string> {childClassName});
            }
        }
    }
}