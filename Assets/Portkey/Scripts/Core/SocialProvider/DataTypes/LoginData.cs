using Portkey.Core;

namespace Portkey.SocialProvider
{
    public struct Data
    {
        public struct ExtraData
        {
            public DeviceInfoType deviceInfo;
            public string version;
        }

        public string type;
        public string address;
        public string id;
        public string netWorkType;
        public string chainType;
        public ExtraData extraData;
    }

    public struct AppData
    {
        public string websiteName;
        public string websiteIcon;
    }
}