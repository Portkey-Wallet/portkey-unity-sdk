using System.Collections.Generic;

namespace Portkey.Core
{
    public class JsonRequestData : IRequestData
    {
        public string Url = null;
        public Dictionary<string, string> Headers = new Dictionary<string, string>();
        public string JsonData = null;
        public string ContentType => "application/json";
    }
}