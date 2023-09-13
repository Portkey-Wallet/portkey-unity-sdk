using System;
using System.Collections;
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
        [SerializeField] private BiometricViewController biometricView;
        [SerializeField] private LoadingViewController loadingView;
        [SerializeField] private GameObject confirmBackView;

        private VerifierItem _verifierItem = null;
        private GuardianIdentifierInfo _guardianIdentifierInfo = null;
        private List<ApprovedGuardian> _guardiansApprovedList = new List<ApprovedGuardian>();
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
        public List<ApprovedGuardian> GuardiansApprovedList
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

        public VerifyCodeResult VerifyCodeResult { get; set; } = null;
        
        public bool IsLoginCompleted
        {
            get;
            set;
        } = false;

        public bool UseBiometric
        {
            get;
            set;
        } = false;

        private void OnEnable()
        {
            ResetToEnterPINState();
        }

        private void OnSetPINSuccess()
        {
            switch (_guardianIdentifierInfo.accountType)
            {
                case AccountType.Apple or AccountType.Google:
                    CheckAccessTokenExpired();
                    break;
                case AccountType.Email or AccountType.Phone:
                    SignIn(VerifyCodeResult);
                    break;
                default:
                    throw new ArgumentException($"Invalid account type: {_guardianIdentifierInfo.accountType}");
            }
        }

        private void CheckAccessTokenExpired()
        {
            var verifier = _verifierItem;
            var socialVerifier = did.GetSocialVerifier(_guardianIdentifierInfo.accountType);

            var param = new VerifyAccessTokenParam
            {
                verifierId = verifier.id,
                accessToken = _guardianIdentifierInfo.token,
                chainId = _guardianIdentifierInfo.chainId
            };
            socialVerifier.AuthenticateIfAccessTokenExpired(param, OnAuthenticated, OnStartLoading, OnError);
        }
        
        private void OnStartLoading(bool show)
        {
            if (Operation == OperationType.SIGN_UP)
            {
                ShowLoading(show, "Creating address on the chain...");
            }
            else
            {
                ShowLoading(show, "Initiating social recovery");
            }
        }

        private void OnAuthenticated(VerifyCodeResult result, string accessToken)
        {
            _guardianIdentifierInfo.token = accessToken;

            SignIn(result);
        }

        private void SignIn(VerifyCodeResult result)
        {
            var verifierDoc = result.verificationDoc;
            var signature = result.signature;
            
            if (_guardianIdentifierInfo.identifier == null)
            {
                OnError("Account missing!");
                return;
            }
            
            did.Reset();
            
            if(Operation == OperationType.SIGN_UP)
            {
                Register(verifierDoc.verifierId, verifierDoc.toString, signature);
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
                OpenNextView(walletInfo);
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

        private void OpenNextView(DIDWalletInfo walletInfo)
        {
            var biometric = did.GetBiometric();
            if (biometric == null)
            {
                OpenWalletView(walletInfo);
                return;
            }
            biometric.CanAuthenticate(canAuth =>
            {
                if (canAuth)
                {
                    //change to biometric view
                    OpenBiometricView(walletInfo);
                }
                else
                {
                    OpenWalletView(walletInfo);
                }
            });
        }

        private void OpenWalletView(DIDWalletInfo walletInfo)
        {
            walletView.WalletInfo = walletInfo;
            walletView.gameObject.SetActive(true);
            IsLoginCompleted = true;
            ShowLoading(false);
            CloseView();
        }
        
        private void OpenBiometricView(DIDWalletInfo walletInfo)
        {
            biometricView.WalletInfo = walletInfo;
            biometricView.gameObject.SetActive(true);
            ShowLoading(false);
            CloseView();
        }

        private void Register(string verifierId, string verificationDoc, string signature)
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
                verificationDoc = verificationDoc,
                signature = signature
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
                OpenNextView(walletInfo);
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
            errorMessage.text = "";
            
            if (CurrentPIN.Length != pinProgress.GetMaxPINLength())
            {
                return;
            }
            
            // user has entered a PIN, now we change state to confirm PIN
            if (_currentState == State.ENTER_PIN)
            {
                StartCoroutine(WaitAndChangeState(0.5f, State.CONFIRM_PIN));
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

        private IEnumerator WaitAndChangeState(float seconds, State state)
        {
            yield return new WaitForSeconds(seconds);
            ChangeState(state);
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
            errorMessage.text = "";
            UseBiometric = false;
            IsLoginCompleted = false;
            
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