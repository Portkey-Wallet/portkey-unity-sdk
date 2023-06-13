using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Portkey.GraphQL
{
    public static class Utils
    {
        public static string JsonToArgument(string jsonInput)
        {
            char[] jsonChar = jsonInput.ToCharArray();
            List<int> indexes = new List<int>();
            jsonChar[0] = ' ';
            jsonChar[jsonChar.Length - 1] = ' ';
            for (int i = 0; i < jsonChar.Length; i++)
            {
                if (jsonChar[i] == '\"')
                {
                    if (indexes.Count == 2)
                        indexes = new List<int>();
                    indexes.Add(i);
                }

                if (jsonChar[i] == ':')
                {
                    jsonChar[indexes[0]] = ' ';
                    jsonChar[indexes[1]] = ' ';
                }
            }

            string result = new string(jsonChar);
            return result;
        }
        
        public class EnumInputConverter : StringEnumConverter
        {
            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer){
                if (value == null){
                    writer.WriteNull();
                }
                else{
                    Enum @enum = (Enum) value;
                    string enumText = @enum.ToString("G");
                    writer.WriteRawValue(enumText);
                }
            }
        }
    }
}