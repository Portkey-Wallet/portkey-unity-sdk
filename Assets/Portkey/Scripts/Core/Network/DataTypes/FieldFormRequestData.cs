using System.Collections.Generic;

namespace Portkey.Core
{
    public class Empty
    {
        public static readonly Empty Default = new Empty();
    }
    
    public class FieldFormRequestData<T> : IRequestData
    {
        public string Url;
        public Dictionary<string, string> Headers = new Dictionary<string, string>();
        public T FieldFormsObject;
        public string ContentType => "application/x-www-form-urlencoded";
    }
}