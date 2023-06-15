using System.Collections.Generic;
using Portkey.Core;

namespace Portkey.Storage
{
    public class NonPersistentStorage<T> : IStorageSuite<T>
    {
        private Dictionary<string, T> _localStorage = new Dictionary<string, T>();

        public T GetItem(string key)
        {
            return _localStorage.TryGetValue(key, out T value) ? value : default(T);
        }

        public void SetItem(string key, T value)
        {
            _localStorage[key] = value;
        }

        public T RemoveItem(string key)
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
            return ret;
        }
    }
}