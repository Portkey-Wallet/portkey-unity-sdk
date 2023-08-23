namespace Portkey.Utilities
{
    public static class JwtDecoder
    {
        public static string DecodePayload(string jwtToken)
        {
            var splitToken = jwtToken.Split('.');

            // must have at least header and payload
            if (splitToken.Length < 2)
            {
                throw new System.Exception("Invalid token");
            }
            
            var base64EncodedPayload = splitToken[1];
            var payload = Base64Decode(base64EncodedPayload);
            return payload;
        }
        
        private static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = SocialProvider.Utilities.DecodeBase64(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }
    }
}