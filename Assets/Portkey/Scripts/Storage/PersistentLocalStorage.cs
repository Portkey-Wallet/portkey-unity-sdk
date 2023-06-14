using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Portkey.Core;

namespace Portkey.Storage
{
    public class PersistentLocalStorage : IStorageSuite<string>
    {
        private string _path;

        //constructor
        public PersistentLocalStorage(string path)
        {
            this._path = path;
        }
        
        public Task<string> GetItem(string filename)
        {
            string filePath = _path + $"{Path.DirectorySeparatorChar}{filename}";
            if (!File.Exists(filePath))
            {
                return Task.FromResult<string>(null);
            }
            return Task.FromResult(File.ReadAllText(filePath));
        }

        public Task SetItem(string filename, string value)
        {
            File.WriteAllText(_path + $"{Path.DirectorySeparatorChar}{filename}",value);
            return Task.FromResult(0);
        }

        public Task<string> RemoveItem(string filename)
        {
            Task<string> ret = GetItem(filename);
            File.Delete(_path + $"{Path.DirectorySeparatorChar}{filename}");
            
            return ret;
        }
    }
}