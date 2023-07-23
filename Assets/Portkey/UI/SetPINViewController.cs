using System;
using System.Collections.Generic;
using System.Linq;
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
        [SerializeField] private PINProgress pinProgress;
        [FormerlySerializedAs("pinLength")] [SerializeField] private int maxPINLength = 6;
        [SerializeField] private TextMeshProUGUI header;
        [SerializeField] private TextMeshProUGUI errorMessage;
        [SerializeField] private DID.DID did;
        [SerializeField] private GameObject walletView;
        
        private GuardianIdentifierInfo _guardianIdentifierInfo = null;
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
        
        public void SetGuardianIdentifierInfo(GuardianIdentifierInfo info)
        {
            _guardianIdentifierInfo = info;
        }

        public void OnClickSetPIN()
        {
        }

        private void OnSetPINSuccess()
        {
            //TODO: save PIN into storage
            //TODO: Get verifier server
            StartCoroutine(_portkeySocialService.GetChainsInfo((chainInfos) =>
            {
                var chainInfo = chainInfos.items.FirstOrDefault(info => info.chainId == _guardianIdentifierInfo.chainId);
                if(chainInfo == null)
                {
                    OnError($"Chain info [{_guardianIdentifierInfo.chainId}] not found");
                    return;
                }

                StartCoroutine(did.GetVerifierServers(chainInfo.chainId, result =>
                {
                    if (result == null)
                    {
                        OnError($"Verifier server [{chainInfo.chainId}] not found");
                        return;
                    }

                    var verifierServer = result[0];
                    CheckAccessTokenExpired(verifierServer);
                }, OnError));
            }, OnError));
            //TODO: VerifyGoogleToken
            //TODO: service.register
            
            //_portkeySocialService.VerifyGoogleToken()
            
            //CloseView();
        }

        private void CheckAccessTokenExpired(VerifierItem verifier)
        {
            // check if login access token is expired
            var login = did.GetSocialLogin(_guardianIdentifierInfo.accountType);
            login.RequestSocialInfo(_guardianIdentifierInfo.token, (socialInfo) =>
            {
                if(socialInfo == null)
                {
                    //login expired, need to re-login
                    login.Authenticate((info) =>
                    {
                        _guardianIdentifierInfo.token = info.access_token;
                        VerifyGoogleToken(verifier);
                    }, OnError);
                }
                else
                {
                    VerifyGoogleToken(verifier);
                }
            }, OnError);

            VerifyGoogleToken(verifier);
        }

        private void VerifyGoogleToken(VerifierItem verifier)
        {
            var param = new VerifyGoogleTokenParams
            {
                accessToken = _guardianIdentifierInfo.token,
                chainId = _guardianIdentifierInfo.chainId,
                verifierId = verifier.id
            };
            StartCoroutine(_portkeySocialService.VerifyGoogleToken(param, (verificationResult) =>
            {
                var verificationDoc = ProcessVerificationDoc(verificationResult.verificationDoc);
                //TODO: set guardian list
                SignUp(verifier, verificationResult);
            }, OnError));
        }

        private void SignUp(VerifierItem verifier, VerifyVerificationCodeResult verificationResult)
        {
            if (_guardianIdentifierInfo.identifier == null)
            {
                OnError("Account missing!");
                return;
            }
            did.Reset();
            
            Register(verifier, verificationResult);
        }

        public void Register(VerifierItem verifier, VerifyVerificationCodeResult verificationResult)
        {
            var guardianIdentifier = string.Concat(_guardianIdentifierInfo.identifier.Where(c => !char.IsWhiteSpace(c)));
            var param = new RegisterParams
            {
                type = _guardianIdentifierInfo.accountType, 
                loginGuardianIdentifier = guardianIdentifier,
                chainId = _guardianIdentifierInfo.chainId,
                verifierId = verifier.id,
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
                CloseView();
            }, OnError));
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
            //CloseView();
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