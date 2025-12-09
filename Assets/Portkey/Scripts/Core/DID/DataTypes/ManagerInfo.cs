namespace Portkey.Core
{
    public class ManagerInfo
    {
        public string Address { get; private set; }
        public string ExtraData { get; private set; }
        
        public ManagerInfo(string address, string extraData)
        {
            Address = address;
            ExtraData = extraData;
        }
    }
}