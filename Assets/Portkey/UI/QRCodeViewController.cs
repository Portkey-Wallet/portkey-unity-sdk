using Portkey.Core;
using Portkey.DID;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class QRCodeViewController : MonoBehaviour
{
    [FormerlySerializedAs("did")] [SerializeField] private PortkeySDK portkeySDK;
    [SerializeField] private Image qrCodeImage;
    
    public void Initialize(SuccessCallback<DIDAccountInfo> loginSuccess)
    {
        OpenView();
        
        StartCoroutine(portkeySDK.AuthService.LoginWithQRCode(DisplayQRCode, result =>
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
        portkeySDK.AuthService.Message.CancelLoginWithQRCode();
        gameObject.SetActive(false);
    }
}
