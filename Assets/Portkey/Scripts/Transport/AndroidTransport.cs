using System;
using UnityEngine;

namespace Portkey.Transport
{
    /// <summary>
    /// Class that implements Android transport to support android deeplinks.
    /// </summary>
    [CreateAssetMenu(fileName = "AndroidTransport", menuName = "Portkey/Transport/AndroidTransport")]
    public class AndroidTransport : Core.Transport
    {
        /// <summary>
        /// Android package name.
        /// </summary>
        [SerializeField]
        private string packageName;

        protected override bool TrySend(string url)
        {
#if UNITY_ANDROID
            var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            var currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            var packageManager = currentActivity.Call<AndroidJavaObject>("getPackageManager");

            if (!IsPackageExist(packageManager, packageName))
            {
                return false;
            }
            
            var uriClass = new AndroidJavaClass("android.net.Uri");
            var uriData = uriClass.CallStatic<AndroidJavaObject>("parse", url);

            var i = packageManager.Call<AndroidJavaObject>("getLaunchIntentForPackage", packageName);
            if (i == null)
            {
                Debug.LogError($"Intent is null. Package name specified: {packageName}.");
                return false;
            }
            
            i.Call<AndroidJavaObject>("setAction", "android.intent.action.VIEW");
            i.Call<AndroidJavaObject>("setData", uriData);

            currentActivity.Call("startActivity", i);
            return true;
#else
            return false;
#endif
        }

        /// <summary>
        /// Checks if the package exists.
        /// </summary>
        private static bool IsPackageExist(AndroidJavaObject packageManager, string packageName)
        {
            try
            {
                // check if package exists
                var packageInfo = packageManager.Call<AndroidJavaObject>("getPackageInfo", packageName, 0);
                if (packageInfo == null)
                {
                    Debug.LogError($"Package {packageName} not found");
                    return false;
                }
            }
            catch (Exception e)
            {
                Debug.Log($"Package {packageName} not found. Exception: {e.Message}");
                return false;
            }

            return true;
        }

        protected override void OpenDownloadLink(string downloadLink)
        {
#if UNITY_ANDROID
            Application.OpenURL(downloadLink);
#endif
        }
    }
}
