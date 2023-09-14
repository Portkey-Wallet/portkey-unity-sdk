using Portkey.Core;
using Portkey.SocialProvider;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Portkey.UI
{
    public class EmailLoginViewController : MonoBehaviour
    {
        [Header("Views")]
        [SerializeField] protected GuardiansApprovalViewController guardianApprovalViewController;
        [SerializeField] protected UnregisteredViewController unregisteredView;
        [SerializeField] protected ErrorViewController errorView;
        [SerializeField] protected LoadingViewController loadingView;
        
        [Header("UI Elements")]
        [SerializeField] protected TMP_InputField inputField;
        [SerializeField] protected Button loginButton;
        [SerializeField] protected TextMeshProUGUI errorText;
        
        public GameObject PreviousView { get; set; }
        public DID.DID DID { get; set; }

        protected void OnEnable()
        {
            ResetView();
        }

        protected void ResetView()
        {
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
            StartLoading();
            //DID.AuthService.GetGuardians(EmailAddress.Parse(inputField.text), CheckSignUpOrLogin, OnError);
            DID.AuthService.HasGuardian(inputField.text, AccountType.Email, "", CheckSignUpOrLogin, OnError);
        }
        
        private void ShowLoading(bool show, string text = "")
        {
            loadingView.DisplayLoading(show, text);
        }
        
        protected void StartLoading()
        {
            ShowLoading(true, "Checking account on the chain...");
        }
        
        protected void OnError(string error)
        {
            Debugger.LogError(error);
            ShowLoading(false);
            errorView.ShowErrorText(error);
        }
        
        protected void CheckSignUpOrLogin(GuardianIdentifierInfo info)
        {
            ShowLoading(false);
            
            switch (info.isLoginGuardian)
            {
                case false:
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