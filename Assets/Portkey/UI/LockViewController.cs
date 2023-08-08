using System;
using Portkey.Core;
using Portkey.DID;
using Portkey.UI;
using TMPro;
using UnityEngine;

public class LockViewController : MonoBehaviour
{
    [SerializeField] private DID did;
    [SerializeField] private PINProgressComponent pinProgress;
    [SerializeField] private SetPINViewController setPinViewController;
    [SerializeField] private GameObject body;
    [SerializeField] private TextMeshProUGUI errorMessage;

    private string PIN = "";
    private bool openBiometric = false;
    private bool isBiometricPromptOpened = false;
    
    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus && did.IsLoggedIn() && setPinViewController.CurrentPIN != "" && !isBiometricPromptOpened)
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
        var biometric = did.GetBiometric();
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
            DisplayLock(false);
        }
        else
        {
            errorMessage.text = "PINs do not match!";
            pinProgress.SetPINProgress(0);
        }
        PIN = "";
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
}
