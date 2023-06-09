using System.Threading.Tasks;

namespace Portkey.Storage
{
    public interface IStorageSuite
    {
        Task<T> GetItem<T>(string key);
        Task SetItem<T>(string key, T value);
        Task RemoveItem(string key);
    }
}