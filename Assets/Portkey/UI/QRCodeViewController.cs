using Portkey.Core;
using Portkey.DID;
using UnityEngine;
using UnityEngine.UI;

public class QRCodeViewController : MonoBehaviour
{
    [SerializeField] private DID did;
    [SerializeField] private Image qrCodeImage;
    
    public void Initialize(SuccessCallback<DIDWalletInfo> loginSuccess)
    {
        OpenView();
        
        StartCoroutine(did.AuthService.LoginWithQRCode(DisplayQRCode, result =>
        {
            CloseView();
            loginSuccess?.Invoke(result);
        }));
    }
    
    private void DisplayQRCode(Texture2D qrCode)
    {
        qrCodeImage.material.mainTexture = qrCode;
    }
    
    private void OpenView()
    {
        gameObject.SetActive(true);
    }
    
    public void CloseView()
    {
        did.AuthService.Message.CancelLoginWithQRCode();
        gameObject.SetActive(false);
    }
}
