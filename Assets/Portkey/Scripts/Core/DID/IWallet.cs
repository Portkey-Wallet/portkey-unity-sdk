using AElf.Types;

namespace Portkey.Core
{
    public interface IWallet : ISigner
    {
        public string Address { get; }
        public string PublicKey { get; }
    }
}