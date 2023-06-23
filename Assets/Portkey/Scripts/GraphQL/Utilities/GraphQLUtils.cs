using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Portkey.GraphQL
{
    /// <summary>
    /// Utilities class for GraphQL to help with data conversion.
    /// </summary>
    public static class Utilities
    {
        /// <summary>
        /// Function to convert a JSON string to a GraphQL argument string.
        /// It transforms json format into GraphQL argument format when querying.
        /// 
        /// For example:
        /// JSON:
        /// {
        ///    "dto": {
        ///        "caHash": "f78e0f6e5619863fe9bafc50be3641072be27cf449760d2f63aaa180a723bc9b",
        ///        "skipCount": 0,
        ///        "maxResultCount": 1
        ///    }
        /// }
        /// After transforming:
        /// dto :{ caHash :"f78e0f6e5619863fe9bafc50be3641072be27cf449760d2f63aaa180a723bc9b", skipCount :0, maxResultCount :1}
        /// </summary>
        /// <param name="jsonInput">The JSON string to convert.</param>
        public static string JsonToArgument(string jsonInput)
        {
            var jsonChar = jsonInput.ToCharArray();
            var indexes = new List<int>();
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

            var result = new string(jsonChar);
            return result;
        }
        
        /// <summary>Enum converter for JSON serialization.</summary>
        public class EnumInputConverter : StringEnumConverter
        {
            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer){
                if (value == null){
                    writer.WriteNull();
                }
                else{
                    Enum @enum = (Enum) value;
                    var enumText = @enum.ToString("G");
                    writer.WriteRawValue(enumText);
                }
            }
        }
    }
}