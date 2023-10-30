using System;
using Portkey.Core;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace Portkey.UI
{
    public class BiometricViewController : MonoBehaviour
    {
        [FormerlySerializedAs("did")] [SerializeField] private DID.PortkeySDK portkeySDK;
        [SerializeField] private TextMeshProUGUI errorText;
        [SerializeField] private WalletViewController walletView;
        [SerializeField] private SetPINViewController setPinViewController;

        private DIDAccountInfo _accountInfo = null;
        
        public DIDAccountInfo AccountInfo
        {
            set => _accountInfo = value;
        }

        private void OnEnable()
        {
            ResetView();
            PromptBiometric(CompleteRegistration);
        }

        public void OnClickSkip()
        {
            CompleteRegistration();
        }
        
        public void OnClickBiometricIcon()
        {
            PromptBiometric(CompleteRegistration);
        }
        
        private void OpenWalletView(DIDAccountInfo accountInfo)
        {
            walletView.AccountInfo = accountInfo;
            walletView.gameObject.SetActive(true);
            CloseView();
        }
        
        private void CloseView()
        {
            gameObject.SetActive(false);
        }
        
        private void CompleteRegistration()
        {
            setPinViewController.IsLoginCompleted = true;
            OpenWalletView(_accountInfo);
        }
        
        private void PromptBiometric(Action onBiometricAuthenticated = null)
        {
            var biometric = portkeySDK.Biometric;
            if (biometric == null)
            {
                throw new Exception("Biometric is not supported on this device.");
            }

            var promptInfo = new IBiometric.BiometricPromptInfo
            {
                title = "Enable Biometric",
                subtitle = "Enable biometric to protect your account",
                description = "You may choose to enable authenticating with your biometric or skip this step.",
                negativeButtonText = "Skip"
            };
            biometric.Authenticate(promptInfo, pass =>
            {
                setPinViewController.UseBiometric = pass;
                if (!pass)
                {
                    errorText.text = "User cancelled.";
                }
                else
                {
                    onBiometricAuthenticated?.Invoke();   
                }
            }, OnError);
        }
        
        private void OnError(string error)
        {
            Debugger.LogError(error);
            errorText.text = error;
        }

        private void ResetView()
        {
            errorText.text = "";
        }
    }
}