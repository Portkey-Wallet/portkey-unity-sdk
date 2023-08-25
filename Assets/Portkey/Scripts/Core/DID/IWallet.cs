using AElf.Types;

namespace Portkey.Core
{
    public interface IWallet : ISigner, IEncryptor
    {
        public string Address { get; }
        public string PublicKey { get; }
    }
}