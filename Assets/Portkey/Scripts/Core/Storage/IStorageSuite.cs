namespace Portkey.Core
{
    /// <summary>
    /// An interface for storage suites. User must define their storage suite for account management stoorage.
    /// </summary>
    public interface IStorageSuite<T>
    {
        T GetItem(string key);
        void SetItem(string key, T value);
        T  RemoveItem(string key);
    }
}