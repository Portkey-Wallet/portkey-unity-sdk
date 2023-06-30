using System;
using Portkey.DID;

namespace Portkey.Core
{
    public interface IAccountMethods
    {
        public Signature SignTransaction(string transaction);
        public byte[] Sign(string data);
        public KeyStore Encrypt(string password, string options = null);
    }
}