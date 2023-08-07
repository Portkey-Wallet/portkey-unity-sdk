using Portkey.Core;

namespace Portkey.Biometric
{
    public class BiometricProvider : IBiometricProvider
    {
        public IBiometric GetBiometric()
        {
#if UNITY_STANDALONE || UNITY_EDITOR
            return null;
#elif UNITY_ANDROID
            return new AndroidBiometric();
#elif UNITY_IOS
#else
            return null;
#endif
        }
    }
}