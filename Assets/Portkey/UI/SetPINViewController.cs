using System;
using System.Collections;
using System.Collections.Generic;
using Portkey.Core;
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

        private Action _onPinComplete = null;
        private GameObject _previousView = null;
        private State _currentState = State.ENTER_PIN;
        private readonly List<string> _stateToPIN = new List<string>
        {
            "", 
            ""
        };
        private readonly List<string> _stateToHeader = new List<string>
        {
            "Enter pin to protect your device", 
            "Confirm Pin"
        };

        public string CurrentPIN
        {
            get => _stateToPIN[(int)_currentState];
            set => _stateToPIN[(int)_currentState] = value;
        }
        
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
        
        public void Initialize(DIDWalletInfo walletInfo)
        {
            //sign up using socials
            _onPinComplete = () =>
            {
                OpenNextView(walletInfo);
            };
            OpenView();
        }
        
        public void Initialize(VerifiedCredential verifiedCredential)
        {
            _onPinComplete = () =>
            {
                did.AuthService.Message.Loading(true, "Creating address on the chain...");
                StartCoroutine(did.AuthService.SignUp(verifiedCredential, OpenNextView));
            };
            OpenView();
        }
        
        public void Initialize(Guardian loginGuardian, List<ApprovedGuardian> approvedGuardians)
        {
            //login
            _onPinComplete = () =>
            {
                did.AuthService.Message.Loading(true, "Initiating social recovery");
                StartCoroutine(did.AuthService.Login(loginGuardian, approvedGuardians, OpenNextView));
            };
            OpenView();
        }
        
        public void OpenView()
        {
            gameObject.SetActive(true);
        }
        
        private void OnEnable()
        {
            ResetToEnterPINState();
        }

        private void OnSetPINSuccess()
        {
            _onPinComplete?.Invoke();
        }

        private void OpenNextView(DIDWalletInfo walletInfo)
        {
            var biometric = did.Biometric;
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
            did.AuthService.Message.Loading(false);
            CloseView();
        }
        
        private void OpenBiometricView(DIDWalletInfo walletInfo)
        {
            biometricView.WalletInfo = walletInfo;
            biometricView.gameObject.SetActive(true);
            did.AuthService.Message.Loading(false);
            CloseView();
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
            if (_previousView == null)
            {
                throw new Exception("No previous view set");
            }
            _previousView.SetActive(true);
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
            _previousView = view;
        }
    }
}