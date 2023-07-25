using UnityEngine;

namespace Portkey.Core
{
    public class DeviceInfoType
    {
        public string deviceName;
        public string deviceType;
        
        public static DeviceInfoType GetDeviceInfo(RuntimePlatform platform)
        {
            var info = new DeviceInfoType
            {
                deviceName = "Other",
                deviceType = "OTHER",
            };
            
            switch (platform)
            {
                case RuntimePlatform.Android:
                    info.deviceType = "ANDROID";
                    break;
                case RuntimePlatform.IPhonePlayer:
                    info.deviceType = "IOS";
                    break;
                case RuntimePlatform.WindowsPlayer:
                case RuntimePlatform.WindowsEditor:
                    info.deviceName = "Windows";
                    info.deviceType = "WINDOWS";
                    break;
                case RuntimePlatform.OSXPlayer:
                case RuntimePlatform.OSXEditor:
                    info.deviceName = "macOS";
                    info.deviceType = "MAC";
                    break;
            }

            return info;
        }
    }
}