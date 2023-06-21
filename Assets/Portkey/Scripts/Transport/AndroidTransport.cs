using Portkey.Core;
using UnityEngine;

namespace Portkey.Transport
{
    [CreateAssetMenu(fileName = "AndroidTransport", menuName = "Portkey/Transport/AndroidTransport")]
    public class AndroidTransport : ScriptableObject, ITransport
    {
        [SerializeField]
        private string _packageName;
        public void Send(string url)
        {
#if UNITY_ANDROID
            var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            var currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            var packageManager = currentActivity.Call<AndroidJavaObject>("getPackageManager");

            var uriClass = new AndroidJavaClass("android.net.Uri");
            var uriData = uriClass.CallStatic<AndroidJavaObject>("parse", url);

            var i = packageManager.Call<AndroidJavaObject>("getLaunchIntentForPackage", _packageName);

            //i.Call<AndroidJavaObject>("setClassName", "com.portkey.did", "com.roblox.client.ActivityProtocolLaunch");
            i.Call<AndroidJavaObject>("setAction", "android.content.Intent.ACTION_VIEW"); //android.intent.action.VIEW
            i.Call<AndroidJavaObject>("setData", uriData);

            currentActivity.Call("startActivity", i);
#endif
        }
    }
}
