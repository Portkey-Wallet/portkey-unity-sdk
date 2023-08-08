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
    private bool openLock = false;
    
    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus && did.IsLoggedIn() && setPinViewController.CurrentPIN != "")
        {
            errorMessage.text = "";
            DisplayLock(true);
        }
    }

    private void LateUpdate()
    {
        if (!openLock)
        {
            return;
        }

        openLock = false;
        
        if(setPinViewController.UseBiometric)
        {
            PromptBiometric();
        }
    }

    private void PromptBiometric()
    {
        var biometric = did.GetBiometric();
        if (biometric == null)
        {
            return;
        }

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
        }, OnError);
    }
    
    private void OnError(string error)
    {
        Debugger.LogError(error);
    }

    public void ResetView()
    {
        errorMessage.text = "";
        DisplayLock(false);
    }

    private void DisplayLock(bool display)
    {
        body.SetActive(display);
        openLock = display;
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
