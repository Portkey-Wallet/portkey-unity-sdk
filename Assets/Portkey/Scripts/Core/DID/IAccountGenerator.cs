namespace Portkey.Core
{
    public interface IAccountGenerator
    {
        Account Create(SavedAccount savedAccount, ISigningKey signingKey);
        Account Create(string chainId, string loginGuardianId, string caHash, string caAddress, ISigningKey signingKey);
    }
}