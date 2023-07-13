using System.Collections.Generic;

namespace Portkey.Core
{
    public class JsonRequestData : IRequestData
    {
        public string Url;
        public Dictionary<string, string> Headers;
        public string JsonData;

        public string GetContentType()
        {
            return "application/json";
        }
    }
}