using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Portkey.Core;
using Portkey.Utilities;
using TMPro;
using UnityEngine;

namespace Portkey.UI
{
    public class SetPINViewController : MonoBehaviour
    {
        private enum State
        {
            ENTER_PIN = 0,
            CONFIRM_PIN = 1
        }

        public enum OperationType
        {
            SIGN_UP = 0,
            SIGN_IN = 1
        }

        [SerializeField] private ErrorViewController errorView;
        [SerializeField] private PINProgressComponent pinProgress;
        [SerializeField] private TextMeshProUGUI header;
        [SerializeField] private TextMeshProUGUI errorMessage;
        [SerializeField] private DID.DID did;
        [SerializeField] private WalletViewController walletView;
        [SerializeField] private LoadingViewController loadingView;
        [SerializeField] private GameObject confirmBackView;
        
        private VerifierItem _verifierItem = null;
        private GuardianIdentifierInfo _guardianIdentifierInfo = null;
        private List<GuardiansApproved> _guardiansApprovedList = new List<GuardiansApproved>();
        private IPortkeySocialService _portkeySocialService;
        private GameObject previousView = null;
        private State _currentState = State.ENTER_PIN;
        private List<string> _stateToPIN = new List<string>
        {
            "", 
            ""
        };
        private List<string> _stateToHeader = new List<string>
        {
            "Enter pin to protect your device", 
            "Confirm Pin"
        };
        
        public VerifierItem VerifierItem
        {
            set => _verifierItem = value;
        }
        public List<GuardiansApproved> GuardiansApprovedList
        {
            set => _guardiansApprovedList = value;
        }
        public GuardianIdentifierInfo GuardianIdentifierInfo
        {
            set => _guardianIdentifierInfo = value;
        }
        public OperationType Operation { get; set; } = OperationType.SIGN_UP;

        public string CurrentPIN
        {
            get => _stateToPIN[(int)_currentState];
            set => _stateToPIN[(int)_currentState] = value;
        }

        private void Start()
        {
            _portkeySocialService = did.PortkeySocialService;
        }

        private void OnEnable()
        {
            ResetToEnterPINState();
        }

        private void OnSetPINSuccess()
        {
            if (Operation == OperationType.SIGN_UP)
            {
                ShowLoading(true, "Creating address on the chain...");
            }
            else
            {
                ShowLoading(true, "Initiating social recovery");
            }
            
            CheckAccessTokenExpired();
        }

        private void CheckAccessTokenExpired()
        {
            var verifier = _verifierItem;
            var socialVerifier = did.GetSocialVerifier(AccountType.Google);

            var param = new VerifyAccessTokenParam
            {
                verifierId = verifier.id,
                accessToken = _guardianIdentifierInfo.token,
                chainId = _guardianIdentifierInfo.chainId
            };
            socialVerifier.AuthenticateIfAccessTokenExpired(param, OnAuthenticated, OnError);
        }

        private void OnAuthenticated(VerificationDoc verifierDoc, string accessToken, VerifyVerificationCodeResult verificationResult)
        {
            if (_guardianIdentifierInfo.identifier == null)
            {
                OnError("Account missing!");
                return;
            }
            _guardianIdentifierInfo.token = accessToken;
            
            did.Reset();
            
            if(Operation == OperationType.SIGN_UP)
            {
                Register(verifierDoc.verifierId, verificationResult);
            }
            else
            {
                Login();
            }
        }

        private void Login()
        {
            var extraData = "";
            try
            {
                extraData = EncodeExtraData(DeviceInfoType.GetDeviceInfo());
            }
            catch (Exception e)
            {
                OnError(e.Message);
                return;
            }

            var guardianIdentifier = _guardianIdentifierInfo.identifier.RemoveAllWhiteSpaces();
            var param = new AccountLoginParams
            {
                loginGuardianIdentifier = guardianIdentifier,
                guardiansApprovedList = _guardiansApprovedList.ToArray(),
                chainId = _guardianIdentifierInfo.chainId,
                extraData = extraData
            };
            StartCoroutine(did.Login(param, result =>
            {
                if(IsCAValid(result.Status))
                {
                    OnError("Login failed! Missing caAddress or caHash.");
                    return;
                }
                
                Debugger.Log("Login success!");
                
                var walletInfo = CreateDIDWalletInfo(result.Status, result.SessionId, AddManagerType.Recovery);
                // logged in, open wallet view and close PIN view
                OpenWalletView(walletInfo);
            }, OnError));
        }

        private DIDWalletInfo CreateDIDWalletInfo(CAInfo caInfo, string sessionId, AddManagerType type)
        {
            var walletInfo = new DIDWalletInfo
            {
                caInfo = caInfo,
                pin = CurrentPIN,
                chainId = _guardianIdentifierInfo.chainId,
                wallet = did.GetWallet(),
                managerInfo = new ManagerInfoType
                {
                    managerUniqueId = sessionId,
                    guardianIdentifier = _guardianIdentifierInfo.identifier,
                    accountType = _guardianIdentifierInfo.accountType,
                    type = type
                }
            };
            return walletInfo;
        }

        private void OpenWalletView(DIDWalletInfo walletInfo)
        {
            walletView.gameObject.SetActive(true);
            walletView.WalletInfo = walletInfo;
            ShowLoading(false);
            CloseView();
        }

        private void Register(string verifierId, VerifyVerificationCodeResult verificationResult)
        {
            var extraData = "";
            try
            {
                extraData = EncodeExtraData(DeviceInfoType.GetDeviceInfo());
            }
            catch (Exception e)
            {
                OnError(e.Message);
                return;
            }

            var guardianIdentifier = _guardianIdentifierInfo.identifier.RemoveAllWhiteSpaces();
            var param = new RegisterParams
            {
                type = _guardianIdentifierInfo.accountType, 
                loginGuardianIdentifier = guardianIdentifier,
                chainId = _guardianIdentifierInfo.chainId,
                verifierId = verifierId,
                extraData = extraData,
                verificationDoc = verificationResult.verificationDoc,
                signature = verificationResult.signature
            };
            StartCoroutine(did.Register(param, result =>
            {
                if(IsCAValid(result.Status))
                {
                    OnError("Register failed! Missing caAddress or caHash.");
                    return;
                }
                
                Debugger.Log("Register success!");

                var walletInfo = CreateDIDWalletInfo(result.Status, result.SessionId, AddManagerType.Register);
                // logged in, open wallet view and close PIN view
                OpenWalletView(walletInfo);
            }, OnError));
        }

        private static bool IsCAValid(CAInfo caInfo)
        {
            return caInfo.caAddress == null || caInfo.caHash == null;
        }

        private static string EncodeExtraData(DeviceInfoType deviceInfo)
        {
            var extraData = new ExtraData
            {
                transactionTime = new DateTimeOffset(DateTime.Now).ToUnixTimeMilliseconds().ToString(),
                deviceInfo = JsonConvert.SerializeObject(deviceInfo)
            };
            return JsonConvert.SerializeObject(extraData);
        }

        private void OnError(string error)
        {
            ShowLoading(false);
            Debugger.LogError(error);
            errorView.ShowErrorText(error);
        }
        
        private void ShowLoading(bool show, string text = "")
        {
            loadingView.DisplayLoading(show, text);
        }

        private void CloseView()
        {
            gameObject.SetActive(false);
        }

        public void OnClickNumber(int number)
        {
            if (CurrentPIN.Length == pinProgress.GetMaxPINLength())
            {
                return;
            }
            CurrentPIN += number.ToString();
            pinProgress.SetPINProgress(CurrentPIN.Length);
            
            if (CurrentPIN.Length != pinProgress.GetMaxPINLength())
            {
                return;
            }
            
            // user has entered a PIN, now we change state to confirm PIN
            if (_currentState == State.ENTER_PIN)
            {
                ChangeState(State.CONFIRM_PIN);
                errorMessage.text = "";
            }
            else
            {
                // user has confirmed PIN, now we set the PIN
                if (_stateToPIN[(int)State.ENTER_PIN] == _stateToPIN[(int)State.CONFIRM_PIN])
                {
                    OnSetPINSuccess();
                }
                else
                {
                    errorMessage.text = "Pins do not match";
                    CurrentPIN = "";
                    pinProgress.SetPINProgress(0);
                }
            }
        }

        private void ChangeState(State state)
        {
            if(_currentState == state)
            {
                return;
            }
            
            _currentState = state;
            header.text = _stateToHeader[(int)_currentState];
            
            pinProgress.SetPINProgress(0);
        }

        public void OnClickBackspace()
        {
            if (CurrentPIN.Length == 0)
            {
                return;
            }
            CurrentPIN = CurrentPIN[..^1];
            pinProgress.SetPINProgress(CurrentPIN.Length);
        }
        
        public void OnClickClose()
        {
            CloseView();
        }

        public void OnClickBack()
        {
            if (_currentState == State.CONFIRM_PIN)
            {
                ResetToEnterPINState();
                return;
            }
            
            confirmBackView.SetActive(true);
        }

        public void OnClickConfirmBack()
        {
            if (previousView == null)
            {
                throw new Exception("No previous view set");
            }
            previousView.SetActive(true);
            CloseView();
        }

        public void ResetToEnterPINState()
        {
            ClearPIN();
            ChangeState(State.ENTER_PIN);
            
            confirmBackView.SetActive(false);
        }

        private void ClearPIN()
        {
            _stateToPIN[(int)State.ENTER_PIN] = "";
            _stateToPIN[(int)State.CONFIRM_PIN] = "";
            
            pinProgress.SetPINProgress(0);
        }

        public void SetPreviousView(GameObject view)
        {
            previousView = view;
        }
    }
}