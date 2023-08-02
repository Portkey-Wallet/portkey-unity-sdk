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
    
    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus && did.IsLoggedIn() && setPinViewController.CurrentPIN != "")
        {
            errorMessage.text = "";
            body.SetActive(true);
        }
    }

    public void ResetView()
    {
        errorMessage.text = "";
        body.SetActive(false);
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
            body.SetActive(false);
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
