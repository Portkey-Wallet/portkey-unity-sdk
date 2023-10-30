using System.Collections;
using Portkey.Core;
using Portkey.DID;
using Portkey.UI;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class LockViewController : MonoBehaviour
{
    [FormerlySerializedAs("did")] [SerializeField] private PortkeySDK portkeySDK;
    [SerializeField] private PINProgressComponent pinProgress;
    [SerializeField] private SetPINViewController setPinViewController;
    [SerializeField] private GameObject body;
    [SerializeField] private TextMeshProUGUI errorMessage;
    [SerializeField] private Button biometricButton;

    private string PIN = "";
    private bool openBiometric = false;
    private bool isBiometricPromptOpened = false;
    
    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus && portkeySDK.IsLoggedIn() && setPinViewController.IsLoginCompleted && !isBiometricPromptOpened)
        {
            ResetPIN();
            DisplayLock(true);
            openBiometric = setPinViewController.UseBiometric;
        }
    }

    private void ResetPIN()
    {
        PIN = "";
        errorMessage.text = "";
        pinProgress.SetPINProgress(0);
    }

    private void LateUpdate()
    {
        if (!openBiometric)
        {
            return;
        }

        openBiometric = false;
        
        PromptBiometric();
    }

    private void PromptBiometric()
    {
        var biometric = portkeySDK.Biometric;
        if (biometric == null)
        {
            return;
        }
        
        isBiometricPromptOpened = true;

        var promptInfo = new IBiometric.BiometricPromptInfo
        {
            title = "Biometric Authentication",
            subtitle = "Biometric Authentication",
            description = "You may choose to autheticate with your biometric or cancel.",
            negativeButtonText = "Cancel"
        };
        biometric.Authenticate(promptInfo, pass =>
        {
            DisplayLock(!pass);
            isBiometricPromptOpened = false;
        }, OnError);
    }
    
    private void OnError(string error)
    {
        isBiometricPromptOpened = false;
        Debugger.LogError(error);
    }

    public void ResetView()
    {
        ResetPIN();
        DisplayLock(false);
        openBiometric = false;
        isBiometricPromptOpened = false;
    }

    private void DisplayLock(bool display)
    {
        body.SetActive(display);
        biometricButton.gameObject.SetActive(display && portkeySDK.Biometric != null && setPinViewController.UseBiometric);
    }
    
    public void OnClickNumber(int number)
    {
        if (PIN.Length == pinProgress.GetMaxPINLength())
        {
            return;
        }
        
        PIN += number.ToString();
        pinProgress.SetPINProgress(PIN.Length);
        errorMessage.text = "";
        
        if (PIN.Length != pinProgress.GetMaxPINLength())
        {
            return;
        }
        
        if(PIN == setPinViewController.CurrentPIN)
        {
            StartCoroutine(WaitAndDisplayLock(0.5f, false));
        }
        else
        {
            errorMessage.text = "PINs do not match!";
            pinProgress.SetPINProgress(0);
        }
        PIN = "";
    }
    
    private IEnumerator WaitAndDisplayLock(float seconds, bool display)
    {
        yield return new WaitForSeconds(seconds);
        DisplayLock(display);
    }
    
    public void OnClickBackspace()
    {
        if (PIN.Length == 0)
        {
            return;
        }
        PIN = PIN[..^1];
        pinProgress.SetPINProgress(PIN.Length);
    }
    
    public void OnClickBiometric()
    {
        PromptBiometric();
    }
}
