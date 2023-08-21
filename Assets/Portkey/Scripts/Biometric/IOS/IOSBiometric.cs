using System.Collections;
using System.Collections.Concurrent;
using Portkey.Core;
using Portkey.Utilities;
using UnityEngine;

namespace Portkey.Biometric
{
    public class IOSBiometric : IBiometric
    {
        private class BiometricOutput
        {
            public bool isAuthenticated = false;
            public string message = null;
        }
        
        private IBiometric.SuccessCallback _onSuccess = null;
        private ErrorCallback _onError = null;
        
        public void Authenticate(IBiometric.BiometricPromptInfo info, IBiometric.SuccessCallback onSuccess, ErrorCallback onError)
        {
            _onSuccess = onSuccess;
            _onError = onError;
            
            StaticCoroutine.StartCoroutine(HandleBiometricOutput());
        }
        
        private IEnumerator HandleBiometricOutput()
        {
            while (true)
            {
                
                yield return null;
            }
        }
    }
}