using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Unity.VisualScripting.YamlDotNet.Core.Tokens;

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
            var jsonCharList = jsonInput.ToList();
            
            // remove starting parenthesis if any
            if(jsonCharList[0] == '{' && jsonCharList[^1] == '}')
            {
                jsonCharList[0] = ' ';
                jsonCharList[^1] = ' ';
            }
            else
            {
                throw new ArgumentException("Invalid JSON input. JSON input must be an object.");
            }

            var startQuote = jsonCharList.Count;
            // loop until there are no more quotes found encapsulating keys
            while (startQuote > 0)
            {
                // find the separator between key and value from the back of the char array
                var separator = jsonCharList.LastIndexOf(':', startQuote - 1);
                if(separator == -1)
                    break;
                // find the quotes around the key
                var endQuote = jsonCharList.LastIndexOf('"', separator - 1);
                if(endQuote == -1)
                    break;
                startQuote = jsonCharList.LastIndexOf('"', endQuote - 1);
                if(startQuote == -1)
                    break;
                // remove quotes from key
                jsonCharList[startQuote] = ' ';
                jsonCharList[endQuote] = ' ';
            }

            return new string(jsonCharList.ToArray());
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