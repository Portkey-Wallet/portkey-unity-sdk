using Portkey.Core;
using UnityEngine;

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
    
    public class DeviceInfoType
    {
        public string deviceName;
        public int deviceType;
        
        public static DeviceInfoType GetDeviceInfo()
        {
            var platform = Application.platform;
            
            var info = new DeviceInfoType
            {
                deviceName = "Other",
                deviceType = 0,
            };
            
            switch (platform)
            {
                case RuntimePlatform.Android:
                    info.deviceName = SystemInfo.deviceModel;
                    info.deviceType = 4;
                    break;
                case RuntimePlatform.IPhonePlayer:
                    info.deviceName = SystemInfo.deviceModel;
                    info.deviceType = 2;
                    break;
                case RuntimePlatform.WindowsPlayer:
                case RuntimePlatform.WindowsEditor:
                    info.deviceName = "Windows";
                    info.deviceType = 3;
                    break;
                case RuntimePlatform.OSXPlayer:
                case RuntimePlatform.OSXEditor:
                    info.deviceName = "macOS";
                    info.deviceType = 1;
                    break;
            }

            return info;
        }
    }
}