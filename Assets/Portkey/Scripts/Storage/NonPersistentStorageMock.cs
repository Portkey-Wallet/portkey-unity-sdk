using System.Collections.Generic;
using Portkey.Core;

namespace Portkey.Storage
{
    public class NonPersistentStorageMock<T> : IStorageSuite<T>
    {
        private string _name;
        private Dictionary<string, T> _localStorage = new Dictionary<string, T>();

        //constructor
        public NonPersistentStorageMock(string name)
        {
            this._name = name;
        }
        
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