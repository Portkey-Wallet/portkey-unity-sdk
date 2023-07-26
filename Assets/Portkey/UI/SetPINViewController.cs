using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Portkey.Core;
using Portkey.Utilities;
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
        
        private class VerificationDoc
        {
            public string type;
            public string guardianIdentifier;
            public string verificationTime;
            public string verifierAddress;
            public string salt;
        }

        //[SerializeField] private GameObject errorView;
        //[SerializeField] private GameObject pinView;
        //[SerializeField] private GameObject walletView;
        //[SerializeField] private DID.DID did;
        //[SerializeField] private GameObject guardianApprovalView;
        [SerializeField] private PINProgressComponent pinProgress;
        [SerializeField] private int maxPINLength = 6;
        [SerializeField] private TextMeshProUGUI header;
        [SerializeField] private TextMeshProUGUI errorMessage;
        [SerializeField] private DID.DID did;
        [SerializeField] private GameObject walletView;
        
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
            "Set PIN", 
            "Confirm PIN"
        };
        
        public List<GuardiansApproved> GuardiansApprovedList
        {
            set => _guardiansApprovedList = value;
        }
        
        public GuardianIdentifierInfo GuardianIdentifierInfo
        {
            set => _guardianIdentifierInfo = value;
        }

        private string CurrentPIN
        {
            get => _stateToPIN[(int)_currentState];
            set => _stateToPIN[(int)_currentState] = value;
        }

        private void Start()
        {
            _portkeySocialService = did.PortkeySocialService;
        }

        private void OnSetPINSuccess()
        {
            //Sign up flow
            //save PIN into storage
            //Get verifier server
            //VerifyGoogleToken
            //service.register

            StartCoroutine(did.GetVerifierServers(_guardianIdentifierInfo.chainId, CheckAccessTokenExpired, OnError));
        }

        private void CheckAccessTokenExpired(VerifierItem[] verifierServerList)
        {
            var verifier = verifierServerList[0];
            var socialVerifier = did.GetSocialVerifier(AccountType.Google);

            var param = new VerifyAccessTokenParam
            {
                verifierId = verifier.id,
                accessToken = _guardianIdentifierInfo.token,
                chainId = _guardianIdentifierInfo.chainId
            };
            socialVerifier.AuthenticateIfAccessTokenExpired(param, SignUp, OnError);
        }

        private void SignUp(string verifierId, string accessToken, VerifyVerificationCodeResult verificationResult)
        {
            if (_guardianIdentifierInfo.identifier == null)
            {
                OnError("Account missing!");
                return;
            }
            _guardianIdentifierInfo.token = accessToken;
            
            did.Reset();
            
            Register(verifierId, verificationResult);
        }

        public void Register(string verifierId, VerifyVerificationCodeResult verificationResult)
        {
            var extraData = "";
            try
            {
                extraData = EncodeExtraData(DeviceInfoType.GetDeviceInfo(Application.platform));
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
                if(result.Status.caAddress == null || result.Status.caHash == null)
                {
                    OnError("Register failed! Missing caAddress or caHash.");
                    return;
                }
                
                // logged in, open wallet view and close PIN view
                walletView.SetActive(true);
                Debugger.Log("Register success!");
                CloseView();
            }, OnError));
        }
        
        private static string EncodeExtraData(DeviceInfoType deviceInfo)
        {
            return JsonConvert.SerializeObject(deviceInfo);
        }

        private static VerificationDoc ProcessVerificationDoc(string verificationDoc)
        {
            var documentValue = verificationDoc.Split(',');
            var verificationDocObj = new VerificationDoc
            {
                type = documentValue[0],
                guardianIdentifier = documentValue[1],
                verificationTime = documentValue[2],
                verifierAddress = documentValue[3],
                salt = documentValue[4]
            };
            return verificationDocObj;
        }

        private void OnError(string error)
        {
            //errorView.SetActive(true);
            Debugger.LogError(error);
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
            if (CurrentPIN.Length == maxPINLength)
            {
                return;
            }
            CurrentPIN += number.ToString();
            pinProgress.SetPINProgress(CurrentPIN.Length);
            
            if (CurrentPIN.Length != maxPINLength)
            {
                return;
            }
            
            // user has entered a PIN, now we change state to confirm PIN
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
                    ResetToEnterPINState(true);
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

        private void ResetToEnterPINState(bool error = false)
        {
            if (error)
            {
                errorMessage.text = "PINs do not match!";
            }
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