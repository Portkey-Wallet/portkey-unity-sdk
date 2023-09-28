using System.Threading.Tasks;
using AElf.Types;

namespace Portkey.Core
{
    public interface ISigner
    {
        public Task<Transaction> SignTransaction(Transaction transaction);
        public byte[] Sign(string data);
    }
}