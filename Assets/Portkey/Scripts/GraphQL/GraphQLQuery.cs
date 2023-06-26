using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

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
            _args = Utilities.JsonToArgument(json);
            CompleteQuery();
        }

        public void SetArgs(string inputString)
        {
            _args = inputString;
            CompleteQuery();
        }



        public void CompleteQuery()
        {
            isComplete = true;
            string data = null;
            string parent = null;
            Field previousField = null;
            for (int i = 0; i < fields.Count; i++)
            {
                var field = fields[i];
                if (field.parentIndexes.Count == 0)
                {
                    if (parent == null)
                    {
                        data += $"\n{GenerateStringTabs(field.parentIndexes.Count + 2)}{field.name}";
                    }
                    else
                    {
                        int count = previousField.parentIndexes.Count - field.parentIndexes.Count;
                        while (count > 0)
                        {
                            data += $"\n{GenerateStringTabs(count + 1)}}}";
                            count--;
                        }

                        data += $"\n{GenerateStringTabs(field.parentIndexes.Count + 2)}{field.name}";
                        parent = null;

                    }

                    previousField = field;
                    continue;
                }

                if (fields[field.parentIndexes.Last()].name != parent)
                {

                    parent = fields[field.parentIndexes.Last()].name;

                    if (fields[field.parentIndexes.Last()] == previousField)
                    {

                        data += $"{{\n{GenerateStringTabs(field.parentIndexes.Count + 2)}{field.name}";
                    }
                    else
                    {
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
                    data += $"\n{GenerateStringTabs(field.parentIndexes.Count + 2)}{field.name}";
                    previousField = field;
                }

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
        public bool hasSubField;
        public List<PossibleField> possibleFields;

        public bool hasChanged;

        public Field()
        {
            possibleFields = new List<PossibleField>();
            parentIndexes = new List<int>();
            index = 0;
        }

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