using System.IO;
using Portkey.Core;

namespace Portkey.Storage
{
    /// <summary>
    /// For persistent local storage on the device.
    /// </summary>
    public class PersistentLocalStorage : IStorageSuite<string>
    {
        private string _path;

        //constructor
        public PersistentLocalStorage(string path)
        {
            this._path = path;
        }
        
        public string GetItem(string filename)
        {
            var filePath = _path + $"{Path.DirectorySeparatorChar}{filename}";
            if (!File.Exists(filePath))
            {
                return null;
            }
            return File.ReadAllText(filePath);
        }

        public void SetItem(string filename, string value)
        {
            File.WriteAllText(_path + $"{Path.DirectorySeparatorChar}{filename}",value);
        }

        public string RemoveItem(string filename)
        {
            var ret = GetItem(filename);
            File.Delete(_path + $"{Path.DirectorySeparatorChar}{filename}");
            
            return ret;
        }
    }
}