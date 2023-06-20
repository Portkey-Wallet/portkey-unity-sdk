namespace Portkey.Core
{
    public interface IStorageSuite<T>
    {
        T GetItem(string key);
        void SetItem(string key, T value);
        T  RemoveItem(string key);
    }
}