using AElf.Types;

namespace Portkey.Core
{
    public interface IAccountMethods
    {
        public Transaction SignTransaction(Transaction transaction);
        public byte[] Sign(string data);
    }
}