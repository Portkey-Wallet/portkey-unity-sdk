using Portkey.Core;
using Portkey.SocialProvider;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Portkey.UI
{
    public class EmailLoginViewController : MonoBehaviour
    {
        private readonly string TITLE = "Login with Email";
        
        [Header("Views")]
        [SerializeField] private GuardiansApprovalViewController guardianApprovalViewController;
        [SerializeField] private UnregisteredViewController unregisteredView;
        [SerializeField] private ErrorViewController errorView;
        [SerializeField] private LoadingViewController loadingView;
        
        [Header("UI Elements")]
        [SerializeField] private TextMeshProUGUI title;
        [SerializeField] private TMP_InputField inputField;
        [SerializeField] private Button loginButton;
        [SerializeField] private TextMeshProUGUI errorText;
        
        public GameObject PreviousView { get; set; }
        public DID.DID DID { get; set; }

        private void OnEnable()
        {
            ResetView();
        }

        private void ResetView()
        {
            title.text = TITLE;
            inputField.text = "";
            errorText.text = "";
            loginButton.interactable = false;
        }

        public void OnClickBack()
        {
            ResetView();
            CloseView();
            PreviousView.SetActive(true);
        }

        public void OnClickLogin()
        {
            OnStartLoading(true);
            DID.AuthService.HasGuardian(inputField.text, AccountType.Email, "", CheckSignUpOrLogin, OnError);
        }
        
        private void ShowLoading(bool show, string text = "")
        {
            loadingView.DisplayLoading(show, text);
        }
        
        private void OnStartLoading(bool show)
        {
            ShowLoading(show, "Checking account on the chain...");
        }
        
        private void OnError(string error)
        {
            Debugger.LogError(error);
            ShowLoading(false);
            errorView.ShowErrorText(error);
        }
        
        private void CheckSignUpOrLogin(GuardianIdentifierInfo info)
        {
            ShowLoading(false);
            
            switch (info.isLoginGuardian)
            {
                case true:
                    unregisteredView.gameObject.SetActive(true);
                    unregisteredView.SetGuardianIdentifierInfo(info);
                    break;
                default:
                    //Change to Login View
                    PrepareGuardiansApprovalView(info);
                    break;
            }
        }
        
        private void PrepareGuardiansApprovalView(GuardianIdentifierInfo info)
        {
            guardianApprovalViewController.SetGuardianIdentifierInfo(info);
            guardianApprovalViewController.InitializeData(OpenGuardiansApprovalView, OnError);
        }
        
        private void OpenGuardiansApprovalView()
        {
            guardianApprovalViewController.gameObject.SetActive(true);
            CloseView();
        }

        public void OnValueChanged()
        {
            var value = inputField.text;
            if (LoginHelper.IsValidEmail(value))
            {
                loginButton.interactable = true;
                errorText.text = "";
                return;
            }
            
            errorText.text = string.IsNullOrEmpty(value) ? "" : "Invalid email.";
            loginButton.interactable = false;
        }
        
        public void OnClickClose()
        {
            CloseView();
        }
        
        private void CloseView()
        {
            gameObject.SetActive(false);
        }
    }
}