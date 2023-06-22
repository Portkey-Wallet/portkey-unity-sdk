using System;
using UnityEngine;

namespace Portkey.Transport
{
    [CreateAssetMenu(fileName = "AndroidTransport", menuName = "Portkey/Transport/AndroidTransport")]
    public class AndroidTransport : Core.Transport
    {
        [SerializeField]
        private string packageName;
        [SerializeField]
        private string downloadLink;
        
        protected override bool TrySend(string url)
        {
#if UNITY_ANDROID
            var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            var currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            var packageManager = currentActivity.Call<AndroidJavaObject>("getPackageManager");

            AndroidJavaObject packageInfo = null;
            try
            {
                // check if package exists
                packageInfo = packageManager.Call<AndroidJavaObject>("getPackageInfo", packageName, 0);
                if (packageInfo == null)
                {
                    Debug.LogError($"Package {packageName} not found");
                    return false;
                }
            }
            catch (Exception e)
            {
                return false;
            }
            
            var uriClass = new AndroidJavaClass("android.net.Uri");
            var uriData = uriClass.CallStatic<AndroidJavaObject>("parse", url);

            var i = packageManager.Call<AndroidJavaObject>("getLaunchIntentForPackage", packageName);

            //i.Call<AndroidJavaObject>("setClassName", "com.portkey.did", "com.roblox.client.ActivityProtocolLaunch");
            i.Call<AndroidJavaObject>("setAction", "android.content.Intent.ACTION_VIEW"); //android.intent.action.VIEW
            i.Call<AndroidJavaObject>("setData", uriData);

            currentActivity.Call("startActivity", i);
            return true;
#else
            return false;
#endif
        }

        protected override void OpenDownloadLink()
        {
#if UNITY_ANDROID
            Application.OpenURL(downloadLink);
#endif
        }
    }
}
