using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Portkey.GraphQL
{
    public class GraphQLCSharpCodeGenerator : IGraphQLCodeGenerator
    {
        private Introspection.SchemaClass schemaClass;

        //constructor
        public GraphQLCSharpCodeGenerator(Introspection.SchemaClass schemaClass)
        {
            this.schemaClass = schemaClass;
        }
        
        //setter for schemaClass
        public void SetSchemaClass(Introspection.SchemaClass schemaClass)
        {
            this.schemaClass = schemaClass;
        }
        
        public void GenerateDTOClass(string className)
        {
            var fields = Fields(className);
            
            if(fields == null)
            {
                Debug.LogError($"Cannot find class {className} in schema!");
                return;
            }

            //generate class header
            StringBuilder genHeaderCode = new StringBuilder();
            bool listHeaderIncluded = false;

            StringBuilder genBodyCode = new StringBuilder();

            HashSet<string> childClassList = new HashSet<string>();
            
            for (int i = 0; i < fields.Count; i++){
                Introspection.SchemaClass.Data.Schema.Type.Field field = fields[i];
                string fieldType = field.type.name;
                    
                switch (field.type.kind)
                {
                    case Introspection.SchemaClass.Data.Schema.Type.TypeKind.OBJECT:
                        genBodyCode.Append($"{fieldType} {field.name} {{get; set;}}\n");
                        ExtractChildClass(childClassList, fieldType);
                        break;
                    case Introspection.SchemaClass.Data.Schema.Type.TypeKind.SCALAR: ;
                        switch (field.type.name)
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
                        }
                        genBodyCode.Append($"{fieldType} {field.name} {{get; set;}}\n");
                        break;
                    case Introspection.SchemaClass.Data.Schema.Type.TypeKind.LIST:
                        string childClassName = field.type.ofType.name;
                        genBodyCode.Append($"IList<{childClassName}> {field.name} {{get; set;}}\n");
                        ExtractChildClass(childClassList, childClassName);
                        
                        if(!listHeaderIncluded)
                        {
                            genHeaderCode.Append("using System.Collections.Generic;\n");
                            listHeaderIncluded = true;
                        }
                        
                        break;
                }
            }
            
            genBodyCode.Append("}");
            
            //insert class name
            genHeaderCode.Append($"public class {className}\n{{\n");
            //concatenate header and body
            genHeaderCode.Append(genBodyCode);
            
            Debug.Log(genHeaderCode.ToString());
            
            //generate child classes
            GenerateChildClass(childClassList);
        }

        private void GenerateChildClass(HashSet<string> childClassList)
        {
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
            }
        }

        private List<Introspection.SchemaClass.Data.Schema.Type.Field> Fields(string className)
        {
            List<Introspection.SchemaClass.Data.Schema.Type.Field> fields = null;
            for (int i = 0; i < schemaClass.data.__schema.types.Count(); ++i)
            {
                Introspection.SchemaClass.Data.Schema.Type type = schemaClass.data.__schema.types[i];

                if (type.name != className)
                {
                    continue;
                }

                fields = type.fields;
            }

            return fields;
        }

        private static void ExtractChildClass(HashSet<string> childClassList, string childClassName)
        {
            childClassList.Add(childClassName);
        }
    }
}