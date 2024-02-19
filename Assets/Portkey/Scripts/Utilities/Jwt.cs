using Newtonsoft.Json.Linq;
using Portkey.Utilities;

namespace Portkey.SocialProvider
{
    public class Jwt
    {
        private readonly JObject _jObject;

        public Jwt(string accessToken)
        {
            _jObject = DecodePayload(accessToken);
        }

        private static JObject DecodePayload(string accessToken)
        {
            var payload = JwtDecoder.DecodePayload(accessToken);
            var jObject = JObject.Parse(payload);
            return jObject;
        }

        public T GetValue<T>(string propertyName)
        {
            return _jObject.GetValue<T>(propertyName);
        }
    }
}