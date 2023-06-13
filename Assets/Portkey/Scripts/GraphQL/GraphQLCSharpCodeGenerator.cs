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
        
        public void GenerateDTOClass(string className, List<Introspection.SchemaClass.Data.Schema.Type.Field> fields)
        {
            //generate class header
            StringBuilder genHeaderCode = new StringBuilder();
            bool listHeaderIncluded = false;

            StringBuilder genBodyCode = new StringBuilder();
            
            Dictionary< string, List<Introspection.SchemaClass.Data.Schema.Type.Field> > childClassList = new Dictionary<string, List<Introspection.SchemaClass.Data.Schema.Type.Field>>();
            
            for (int i = 0; i < fields.Count; i++){
                Introspection.SchemaClass.Data.Schema.Type.Field field = fields[i];
                string fieldType = field.type.name;
                    
                switch (field.type.kind)
                {
                    case Introspection.SchemaClass.Data.Schema.Type.TypeKind.OBJECT:
                        genBodyCode.Append($"{fieldType} {field.name} {{get; set;}}\n");
                        ExtractChildClass(childClassList, fieldType, field);
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
                        ExtractChildClass(childClassList, childClassName, field);
                        
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
            foreach (var childClass in childClassList)
            {
                for (int i = 0; i < schemaClass.data.__schema.types.Count(); ++i)
                {
                    Introspection.SchemaClass.Data.Schema.Type type = schemaClass.data.__schema.types[i];

                    if (type.name != childClass.Key)
                    {
                        continue;
                    }
                
                    GenerateDTOClass(type.name, type.fields);
                }
            }
        }

        private static void ExtractChildClass(Dictionary<string, List<Introspection.SchemaClass.Data.Schema.Type.Field>> childClassList, string childClassName, Introspection.SchemaClass.Data.Schema.Type.Field field)
        {
            //add child class to list to be generated later
            if (childClassList.TryGetValue(childClassName,
                    out List<Introspection.SchemaClass.Data.Schema.Type.Field> childList))
            {
                childList.Add(field);
            }
            else
            {
                childClassList.Add(childClassName, new List<Introspection.SchemaClass.Data.Schema.Type.Field>());
                childClassList[childClassName].Add(field);
            }

            return;
        }
    }
}