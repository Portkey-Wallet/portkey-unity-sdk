using System;
using System.Collections;
using System.Collections.Generic;
using Portkey.Contract;
using AElf.Contracts.MultiToken;
using Portkey.Core;
using Portkey.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace Portkey.UI
{
    [Serializable]
    public class TokenUIInfo
    {
        public TextMeshProUGUI chainLabelText;
        public TextMeshProUGUI addressText;
        public TextMeshProUGUI tokenLabelText;
        public TextMeshProUGUI tokenBalanceText;
    }
    
    public class WalletViewController : MonoBehaviour
    {
        [FormerlySerializedAs("did")] [SerializeField] private DID.PortkeySDK portkeySDK;
        
        [Header("UI")]
        [SerializeField] private GameObject confirmSignOutDialog;
        [SerializeField] private TextMeshProUGUI chainInfoText;
        [SerializeField] private AccountDeletionButton accountDeletionButton;

        [Header("Token UI")]
        [SerializeField] private TokenUIInfo[] tokenInfos;
        
        [Header("View")]
        [SerializeField] private SignInViewController signInViewController;
        [SerializeField] private SetPINViewController setPinViewController;
        [SerializeField] private GuardiansApprovalViewController guardiansApprovalViewController;
        [SerializeField] private LockViewController lockViewController;
        [SerializeField] private AccountCancellationView accountCancellationView;
        
        private DIDAccountInfo _accountInfo = null;
        private bool _isSignOut = false;

        private readonly string LOADING_MESSAGE = "Loading...";

        private readonly Dictionary<string, string> _chainIdToCaAddress = new Dictionary<string, string>();
        
        public DIDAccountInfo AccountInfo
        {
            set => _accountInfo = value;
        }
        
        private void OnEnable()
        {
            confirmSignOutDialog.SetActive(false);
            _isSignOut = false;
            chainInfoText.text = LOADING_MESSAGE;
            _chainIdToCaAddress.Clear();
            ResetTokenUIInfos();
            
            accountDeletionButton.Initialize(_accountInfo, OnError);
            
            chainInfoText.text = _accountInfo.chainId;
            
            portkeySDK.AuthService.Message.OnLogoutEvent += OnSuccessLogout;

            StartCoroutine(GetAvailableChains());
        }
        
        private void OnDisable()
        {
            portkeySDK.AuthService.Message.OnLogoutEvent -= OnSuccessLogout;
        }

        private void ResetTokenUIInfos()
        {
            foreach (var info in tokenInfos)
            {
                info.tokenBalanceText.text = "";
                info.chainLabelText.text = LOADING_MESSAGE;
                info.addressText.text = "";
                info.tokenLabelText.text = "";
            }
        }

        private IEnumerator GetAvailableChains()
        {
            yield return portkeySDK.ChainProvider.GetAvailableChainIds(chainIds =>
            {
                var index = 0;
                foreach (var chainId in chainIds)
                {
                    var uiIndex = index;
                    StartCoroutine(portkeySDK.ChainProvider.GetChain(chainId, chain =>
                    {
                        RequestHolderCaAddress(uiIndex, chain, _accountInfo.caInfo.caHash);
                    }, OnError));
                    ++index;
                }
            }, OnError);
        }

        private void RequestHolderCaAddress(int index, IChain chain, string caHash)
        {
            var getHolderInfoParam = new GetHolderInfoParams
            {
                caHash = caHash,
                chainId = chain.ChainInfo.chainId
            };
            StartCoroutine(PollHolderInfo(index, chain, getHolderInfoParam));
        }

        private IEnumerator PollHolderInfo(int index, IChain chain, GetHolderInfoParams holderInfoParams)
        {
            if (holderInfoParams.caHash == null)
            {
                var param = new GetHolderInfoByManagerParams
                {
                    manager = _accountInfo.signingKey.Address,
                    chainId = chain.ChainInfo.chainId
                };
                while (!_isSignOut && !_chainIdToCaAddress.ContainsKey(param.chainId))
                {
                    yield return portkeySDK.GetHolderInfo(param, (holderInfo) =>
                    {
                        _chainIdToCaAddress[param.chainId] = holderInfo.holderManagerInfo.caAddress;
                    
                        var tokenContract = new ContractBasic(chain, chain.ChainInfo.defaultToken.address);

                        StartCoroutine(PollTokenBalance(index, tokenContract, chain.ChainInfo.defaultToken));
                    }, error => { });

                    yield return new WaitForSeconds(5);
                }
                yield break;
            }
            while (!_isSignOut && !_chainIdToCaAddress.ContainsKey(holderInfoParams.chainId))
            {
                yield return portkeySDK.GetHolderInfoByContract(holderInfoParams, (holderInfo) =>
                {
                    if (holderInfo == null || holderInfo.guardianList == null ||
                        holderInfo.guardianList.guardians == null)
                    {
                        return;
                    }

                    _chainIdToCaAddress[holderInfoParams.chainId] = holderInfo.caAddress;
                    
                    var tokenContract = new ContractBasic(chain, chain.ChainInfo.defaultToken.address);

                    StartCoroutine(PollTokenBalance(index, tokenContract, chain.ChainInfo.defaultToken));
                }, error => { });

                yield return new WaitForSeconds(5);
            }
        }

        private IEnumerator PollTokenBalance(int index, IContract tokenContract, DefaultToken token)
        {
            while (!_isSignOut)
            {
                yield return UpdateTokenBalanceUI(index, tokenContract, token);
                yield return new WaitForSeconds(5);
            }
        }

        private IEnumerator UpdateTokenBalanceUI(int index, IContract tokenContract, DefaultToken token)
        {
            if (!_chainIdToCaAddress.TryGetValue(tokenContract.ChainId, out var caAddress))
            {
                yield break;
            }
            
            var balanceInput = new GetBalanceInput()
            {
                Owner = caAddress.ToAddress(),
                Symbol = token.symbol
            };
            yield return tokenContract.CallAsync<GetBalanceOutput>("GetBalance", balanceInput, output =>
            {
                tokenInfos[index].chainLabelText.text = tokenContract.ChainId;
                tokenInfos[index].tokenLabelText.text = output.Symbol;
                var decimals = 0;
                try
                {
                    decimals = int.Parse(token.decimals);
                }
                catch (Exception e)
                {
                    Debugger.LogException(e);
                }
                var denominator = Math.Pow(10, decimals);
                tokenInfos[index].tokenBalanceText.text = (output.Balance / denominator).ToString();
                tokenInfos[index].addressText.text = $"{output.Symbol}_{caAddress}_{tokenContract.ChainId}";
            }, OnError);
        }

        public void OnClickSignOut()
        {
            portkeySDK.AuthService.Message.Loading(true, "Signing out of Portkey...");

            StartCoroutine(portkeySDK.AuthService.Logout());
        }
        
        private void OnSuccessLogout(LogoutMessage logoutMessage)
        {
            portkeySDK.AuthService.Message.Loading(false);

            SignOut();

            if (logoutMessage == LogoutMessage.PortkeyExtensionLogout)
            {
                OnError("You are disconnected from the wallet.");
            }
        }

        private void SignOut()
        {
            _isSignOut = true;

            ResetViews();
            OpenSignInView();
        }

        public void OnClickDeleteAccount()
        {
            accountCancellationView.Initialize(_accountInfo, SignOut);
        }

        private void ResetViews()
        {
            setPinViewController.ResetToEnterPINState();
            guardiansApprovalViewController.ResetView();
            lockViewController.ResetView();
        }

        private void OpenSignInView()
        {
            signInViewController.gameObject.SetActive(true);
            CloseView();
        }
        
        private void OnError(string error)
        {
            portkeySDK.AuthService.Message.Error(error);
        }

        private void CloseView()
        {
            StopAllCoroutines();
            gameObject.SetActive(false);
        }
    }
}