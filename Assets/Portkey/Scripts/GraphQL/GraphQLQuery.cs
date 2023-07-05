using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine.Serialization;

namespace Portkey.GraphQL
{
    [Serializable]
    public class GraphQLQuery
    {
        public string name;
        public Type type;
        public string query;
        public string queryString;
        public string returnType;
        public List<string> queryOptions;
        public List<Field> fields;
        public bool isComplete;
        
        private string _args;

        public enum Type
        {
            Query,
            Mutation,
            Subscription
        }

        public void SetArgs(object inputObject)
        {
            var json = JsonConvert.SerializeObject(inputObject, new Utilities.EnumInputConverter());
            try
            {
                _args = Utilities.JsonToArgument(json);
            }
            catch (ArgumentException e)
            {
                Core.Debugger.LogException(e);
                throw;
            }
            BuildQueryString();
        }

        public void SetArgs(string inputString)
        {
            _args = inputString;
            BuildQueryString();
        }


        /// <summary>
        /// Construct a graphQL syntax query string from the field list, arguments and query name.
        /// Each field has parentIndexes which is a list of indexes of the fields that are its parents.
        /// The last element in parentIndexes is the immediate parent of the field, moving up as the index decreases.
        /// </summary>
        public void BuildQueryString()
        {
            isComplete = true;
            string data = null;
            Field previousField = null;
            foreach (var field in fields)
            {
                // if there was a parent on the previous field, check if we need to close brackets
                if (previousField != null && previousField.ancestors > 0)
                {
                    // find out how many closing bracket to output based on the amount of ancestors the previous field had
                    var count = previousField.ancestors - field.ancestors;
                    while (count > 0)
                    {
                        data += $"\n{GenerateStringTabs(count + 1)}}}";
                        count--;
                    }
                }
                
                //output the current field with indentation
                data += $"\n{GenerateStringTabs(field.ancestors + 2)}{field.name}";
                if(field.hasSubField)
                {
                    data += "{";
                }

                previousField = field;
            }

            // check what kind of query it is and construct the query string accordingly
            var arg = String.IsNullOrEmpty(_args) ? "" : $"({_args})";
            string word = type switch
            {
                Type.Query => "query",
                Type.Mutation => "mutation",
                Type.Subscription => "subscription",
                _ => "query"
            };

            query = data == null
                ? $"{word} {name}{{\n{GenerateStringTabs(1)}{queryString}{arg}\n}}"
                : $"{word} {name}{{\n{GenerateStringTabs(1)}{queryString}{arg}{{{data}\n{GenerateStringTabs(1)}}}\n}}";
        }

        private string GenerateStringTabs(int number)
        {
            string result = "";
            for (int i = 0; i < number; i++)
            {
                result += "    ";
            }

            return result;
        }
    }

    [Serializable]
    public class Field
    {
        public int index;

        public int Index
        {
            get => index;
            set
            {
                type = fieldOptions[value].type;
                name = fieldOptions[value].name;
                if (value != index)
                    hasChanged = true;
                index = value;

            }
        }

        public string name;
        public string type;
        public int ancestors;
        public bool hasSubField;
        public List<FieldOption> fieldOptions;

        public bool hasChanged;

        public Field()
        {
            fieldOptions = new List<FieldOption>();
            ancestors = 0;
            index = 0;
        }

        /// <summary>
        /// Check and set if the field has subfields. For Editor UI purposes to introduce a button to create sub field.
        /// </summary>
        /// <param name="schemaClass">Schema of the field to check against.</param>
        public void CheckSubFields(Introspection.SchemaClass schemaClass)
        {
            var t = schemaClass.data.__schema.types.Find((aType => aType.name == type));
            if (t.fields == null || t.fields.Count == 0)
            {
                hasSubField = false;
                return;
            }

            hasSubField = true;
        }

        [Serializable]
        public class FieldOption
        {
            public string name;
            public string type;

            public static implicit operator FieldOption(Field field)
            {
                return new FieldOption { name = field.name, type = field.type };
            }
        }

        public static explicit operator Field(Introspection.SchemaClass.Data.Schema.Type.Field schemaField)
        {
            var ofType = schemaField.type;
            string typeName;
            do
            {
                typeName = ofType.name;
                ofType = ofType.ofType;
            } while (ofType != null);

            return new Field { name = schemaField.name, type = typeName };
        }
    }
}