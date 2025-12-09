using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

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
            string ToArgument(JToken jToken, bool embrace)
            {
                switch (jToken)
                {
                    case JArray jArray:
                        return $"[{string.Join(", ", jArray.Select(tok=>ToArgument(tok, true)))}]";
                    case JObject jObject:
                        var items = new List<string>();
                        foreach (var (key, value) in jObject)
                        {
                            if(value == null)
                                continue;
                            if(value.Type is JTokenType.None or JTokenType.Undefined)
                                continue;
                            items.Add($"{key}: {ToArgument(value, true)}");
                        }

                        return embrace ? $"{{{string.Join(", ", items)}}}" : string.Join(", ", items);
                    case JValue value:
                        return value.Type switch
                        {
                            JTokenType.None => "",
                            JTokenType.Object => $"{{{ToArgument(value.Value<JObject>(), true)}}}",
                            JTokenType.Array => ToArgument(value.Value<JArray>(), true),
                            JTokenType.String => JsonConvert.ToString(value.Value<string>()),
                            JTokenType.Boolean => value.Value<bool>() ? "true" : "false",
                            JTokenType.Null => "",
                            JTokenType.Undefined => "",
                            _ => value.ToString(CultureInfo.InvariantCulture)
                        };
                    default:
                        throw new ArgumentOutOfRangeException(nameof(jToken));
                }
            }

            return ToArgument(JObject.Parse(jsonInput), false);
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