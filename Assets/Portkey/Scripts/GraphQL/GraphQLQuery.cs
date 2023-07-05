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
            CompleteQuery();
        }

        public void SetArgs(string inputString)
        {
            _args = inputString;
            CompleteQuery();
        }


        /// <summary>
        /// Construct a graphQL syntax query string from the field list, arguments and query name.
        /// Each field has parentIndexes which is a list of indexes of the fields that are its parents.
        /// The last element in parentIndexes is the immediate parent of the field, moving up as the index decreases.
        /// </summary>
        public void CompleteQuery()
        {
            isComplete = true;
            string data = null;
            string parent = null;
            Field previousField = null;
            ///
            ///  {
            ///     a,
            /// }
            /// 
            ///     b
            /// 
            foreach (var (i, field) in fields.Select((item, i)=> (i, item)))
            {
                
                // If the current field has no parent
                if (field.parentIndexes.Count == 0)
                {
                    // if there was a parent on the previous field but not on this one, we need to close brackets for the previous field
                    if (parent != null)
                    {
                        // find out how many closing bracket to output based on the amount of parents the previous field had
                        int count = previousField.parentIndexes.Count - field.parentIndexes.Count;
                        while (count > 0)
                        {
                            data += $"\n{GenerateStringTabs(count + 1)}}}";
                            count--;
                        }
                        
                        parent = null;
                    }
                    
                    //output the current field with indentation
                    data += $"\n{GenerateStringTabs(field.parentIndexes.Count + 2)}{field.name}";

                    previousField = field;
                    continue;
                }

                // for handling fields with parents
                // if the current field has an immediate parent that is different from the previous field's parent
                if (fields[field.parentIndexes.Last()].name != parent)
                {
                    parent = fields[field.parentIndexes.Last()].name;

                    // if the field's immediate parent is the previous field
                    if (fields[field.parentIndexes.Last()] == previousField)
                    {
                        // output the current field with corresponding indentation
                        data += $"{{\n{GenerateStringTabs(field.parentIndexes.Count + 2)}{field.name}";
                    }
                    // else we have to close the brackets for the previous field and output the current field with corresponding indentation
                    else
                    {
                        // find out how many closing bracket to output based on the amount of parents the previous field had
                        int count = previousField.parentIndexes.Count - field.parentIndexes.Count;
                        while (count > 0)
                        {
                            data += $"\n{GenerateStringTabs(count + 1)}}}";
                            count--;
                        }

                        data += $"\n{GenerateStringTabs(field.parentIndexes.Count + 2)}{field.name}";
                    }

                    previousField = field;

                }
                else
                {
                    // since this field is under the same parent, we simply output with the corresponding indentation
                    data += $"\n{GenerateStringTabs(field.parentIndexes.Count + 2)}{field.name}";
                    previousField = field;
                }

                // if this is the last field, we need to close all the brackets
                if (i == fields.Count - 1)
                {
                    int count = previousField.parentIndexes.Count;
                    while (count > 0)
                    {
                        data += $"\n{GenerateStringTabs(count + 1)}}}";
                        count--;
                    }
                }
            }
            // check what kind of query it is and construct the query string accordingly
            var arg = String.IsNullOrEmpty(_args) ? "" : $"({_args})";
            string word;
            switch (type)
            {
                case Type.Query:
                    word = "query";
                    break;
                case Type.Mutation:
                    word = "mutation";
                    break;
                case Type.Subscription:
                    word = "subscription";
                    break;
                default:
                    word = "query";
                    break;
            }

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
                type = possibleFields[value].type;
                name = possibleFields[value].name;
                if (value != index)
                    hasChanged = true;
                index = value;

            }
        }

        public string name;
        public string type;
        public List<int> parentIndexes;
        public List<Field> ancestors;
        public bool IsTopLevel => ancestors == null || ancestors.Count == 0;
        public bool hasSubField;
        public List<PossibleField> possibleFields;

        public bool hasChanged;

        public Field()
        {
            possibleFields = new List<PossibleField>();
            parentIndexes = new List<int>();
            index = 0;
        }

        public Field CreateChild()
        {
            return new Field()
            {
                ancestors = new []{ancestors, new List<Field>{ this }}.SelectMany(item=>item).ToList()
            };
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
        public class PossibleField
        {
            public string name;
            public string type;

            public static implicit operator PossibleField(Field field)
            {
                return new PossibleField { name = field.name, type = field.type };
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