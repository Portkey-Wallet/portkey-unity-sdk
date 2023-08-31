namespace Portkey.UI
{
    public interface IGuardianText
    {
        bool IsDisplayAccountTextOnly { get; }
        string AccountText { get; }
        string DetailsText { get; }
    }
}