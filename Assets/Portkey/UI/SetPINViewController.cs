using System;
using System.Collections.Generic;
using Portkey.Core;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace Portkey.UI
{
    public class SetPINViewController : MonoBehaviour
    {
        private enum State
        {
            ENTER_PIN = 0,
            CONFIRM_PIN = 1
        }

        //[SerializeField] private GameObject errorView;
        //[SerializeField] private GameObject pinView;
        //[SerializeField] private GameObject walletView;
        //[SerializeField] private DID.DID did;
        //[SerializeField] private GameObject guardianApprovalView;
        [SerializeField] private PINProgress pinProgress;
        [SerializeField] private int pinLength = 6;
        [SerializeField] private TextMeshProUGUI header;
        [SerializeField] private DID.DID did;
        
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
            "Set PIN", 
            "Confirm PIN"
        };

        private string CurrentPIN
        {
            get => _stateToPIN[(int)_currentState];
            set => _stateToPIN[(int)_currentState] = value;
        }

        private void Start()
        {
            _portkeySocialService = did.PortkeySocialService;
        }

        public void OnClickSetPIN()
        {
        }

        private void OnSetPINSuccess()
        {
            //TODO: save PIN into storage
            //TODO: AddManager
            
            //_portkeySocialService.VerifyGoogleToken()
            
            CloseView();
        }

        private void OnSetPINError(string error)
        {
            //errorView.SetActive(true);
            CloseView();
        }

        private void CloseView()
        {
            gameObject.SetActive(false);
        }

        public void OnClickNumber(int number)
        {
            if (CurrentPIN.Length == pinLength)
            {
                return;
            }
            CurrentPIN += number.ToString();
            pinProgress.SetPINProgress(CurrentPIN.Length);
            
            // user has entered a PIN, now we change state to confirm PIN
            if (CurrentPIN.Length == pinLength)
            {
                if (_currentState == State.ENTER_PIN)
                {
                    ChangeState(State.CONFIRM_PIN);
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
                        ResetToEnterPINState();
                    }
                }
            }
        }

        private void ChangeState(State state)
        {
            _currentState = state;
            header.text = _stateToHeader[(int)_currentState];
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

        public void OnClickBack()
        {
            if (_currentState == State.CONFIRM_PIN)
            {
                ResetToEnterPINState();
                return;
            }
            if (previousView == null)
            {
                throw new Exception("No previous view set");
            }
            previousView.SetActive(true);
            CloseView();
        }

        private void ResetToEnterPINState()
        {
            ClearPIN();
            ChangeState(State.ENTER_PIN);
        }

        private void ClearPIN()
        {
            _stateToPIN[(int)State.ENTER_PIN] = "";
            _stateToPIN[(int)State.CONFIRM_PIN] = "";
        }

        public void SetPreviousView(GameObject view)
        {
            previousView = view;
        }
    }
}