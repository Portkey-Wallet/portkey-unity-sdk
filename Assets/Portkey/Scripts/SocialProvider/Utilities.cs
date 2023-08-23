using System;
using System.Collections.Specialized;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Portkey.SocialProvider
{
    public static class Utilities
    {
        public static string GetCodeChallenge(string codeVerifier)
        {
            var hash = GetSHA256Hash(codeVerifier);
            return Base64URLRemovePadding(hash);
        }
        
        public static byte[] DecodeBase64(string base64Data)
        {
            base64Data = base64Data.Replace('-', '+').Replace('_', '/').PadRight(4*((base64Data.Length+3)/4), '=');
            return Convert.FromBase64String(base64Data);
        }

        private static byte[] GetSHA256Hash(string text)
        {
            var bytes = Encoding.ASCII.GetBytes(text);
            using var sha256 = new SHA256Managed();

            return sha256.ComputeHash(bytes);
        }

        private static string Base64URLRemovePadding(byte[] buffer)
        {
            var bufferStr = Convert.ToBase64String(buffer);
            var ret = new StringBuilder();
            foreach (var character in bufferStr)
            {
                switch (character)
                {
                    case '+':
                        ret.Append('-');
                        break;
                    case '/':
                        ret.Append('_');
                        break;
                    case '=':
                        break;
                    default:
                        ret.Append(character);
                        break;
                }
            }

            return ret.ToString();
        }
        
        public static NameValueCollection ParseQueryString(string url)
        {
            var result = new NameValueCollection();

            //this regex pattern is used to extract key-value pairs from a string where the key and value are separated by an equal sign (=), and multiple key-value pairs are separated by ampersands (&).
            foreach (Match match in Regex.Matches(url, @"(?<key>\w+)=(?<value>[^&]+)"))
            {
                result.Add(match.Groups["key"].Value, Uri.UnescapeDataString(match.Groups["value"].Value));
            }

            return result;
        }
        
        public static int GetRandomUnusedPort()
        {
            var listener = new TcpListener(IPAddress.Loopback, 0);
            listener.Start();

            var port = ((IPEndPoint) listener.LocalEndpoint).Port;
            listener.Stop();

            return port;
        }
    }
}