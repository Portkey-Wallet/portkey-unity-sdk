using System.Collections.Generic;
using System.Threading.Tasks;
using Portkey.Core;

namespace Portkey.Storage
{
    public class NonPersistentStorage<T> : IStorageSuite<T>
    {
        private Dictionary<string, T> _localStorage = new Dictionary<string, T>();

        public Task<T> GetItem(string key)
        {
            return _localStorage.TryGetValue(key, out T value) ? Task.FromResult(value) : Task.FromResult(default(T));
        }

        public Task SetItem(string key, T value)
        {
            _localStorage[key] = value;
            return Task.FromResult(0);
        }

        public Task<T> RemoveItem(string key)
        {
            T ret;
            if (_localStorage.TryGetValue(key, out ret))
            {
                _localStorage.Remove(key);
            }
            else
            {
                ret = default(T);
            }
            return Task.FromResult(ret);
        }
    }
}