using System.Threading.Tasks;

namespace Portkey.Core
{
    public interface IStorageSuite<T>
    {
        Task<T> GetItem(string key);
        Task SetItem(string key, T value);
        Task<T>  RemoveItem(string key);
    }
}