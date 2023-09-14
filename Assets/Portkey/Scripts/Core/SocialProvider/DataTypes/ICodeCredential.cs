namespace Portkey.Core
{
    public interface ICodeCredential : ICredential
    {
        string VerifierId { get; }
        string ChainId { get; }
    }
}