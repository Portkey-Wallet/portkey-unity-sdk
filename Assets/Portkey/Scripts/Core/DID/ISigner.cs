using AElf.Types;

namespace Portkey.Core
{
    public interface ISigner
    {
        public Transaction SignTransaction(Transaction transaction);
        public byte[] Sign(string data);
    }
}