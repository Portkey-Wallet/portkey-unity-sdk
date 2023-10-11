using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Portkey.Contract;
using AElf.Contracts.MultiToken;
using Portkey.Core;
using Portkey.Utilities;
using TMPro;
using UnityEngine;

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
        [SerializeField] private DID.DID did;
        
        [Header("UI")]
        [SerializeField] private GameObject confirmSignOutDialog;
        [SerializeField] private TextMeshProUGUI chainInfoText;

        [Header("Token UI")]
        [SerializeField] private TokenUIInfo[] tokenInfos;
        
        [Header("View")]
        [SerializeField] private SignInViewController signInViewController;
        [SerializeField] private SetPINViewController setPinViewController;
        [SerializeField] private GuardiansApprovalViewController guardiansApprovalViewController;
        [SerializeField] private LockViewController lockViewController;
        
        private DIDWalletInfo _walletInfo = null;
        private bool _isSignOut = false;

        private readonly string LOADING_MESSAGE = "Loading...";

        private readonly Dictionary<string, string> _chainIdToCaAddress = new Dictionary<string, string>();
        
        public DIDWalletInfo WalletInfo
        {
            set => _walletInfo = value;
        }
        
        private void OnEnable()
        {
            confirmSignOutDialog.SetActive(false);
            _isSignOut = false;
            chainInfoText.text = LOADING_MESSAGE;
            _chainIdToCaAddress.Clear();
            ResetTokenUIInfos();

            StartCoroutine(GetChainInfo());
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

        private IEnumerator GetChainInfo()
        {
            yield return did.PortkeySocialService.GetChainsInfo(chains =>
            {
                var chainInfos = chains.items?.ToDictionary(info => info.chainId, info => info);
                if (chainInfos == null || !chainInfos.TryGetValue(_walletInfo.chainId, out var currChainInfo))
                {
                    return;
                }
                
                chainInfoText.text = currChainInfo.chainId;
                
                SetupTokenUpdate(chainInfos);
            }, OnError);
        }

        private void SetupTokenUpdate(Dictionary<string, ChainInfo> chainInfos)
        {
            var index = 0;
            foreach (var chainInfo in chainInfos)
            {
                if (index >= tokenInfos.Length)
                    break;

                var token = chainInfo.Value.defaultToken;
                var tokenAddress = token.address;

                var tokenSymbol = token.symbol;
                RequestInfo(chainInfo, tokenAddress, index, token);
                ++index;
            }
        }

        private void RequestInfo(KeyValuePair<string, ChainInfo> chainInfo, string tokenAddress, int index, DefaultToken token)
        {
            StartCoroutine(did.GetChain(chainInfo.Key,
            chain =>
            {
                RequestInfoFromContract(chain, tokenAddress, chainInfo.Key, index, token);
            },
            error =>
            {
                Debugger.LogError(error);
            }));
        }

        private void RequestInfoFromContract(IChain chain, string tokenAddress, string chainId, int index, DefaultToken token)
        {
            var tokenContract = new ContractBasic(chain, tokenAddress);

            RequestHolderInfoByChainId(chainId, index, tokenContract, token);
        }

        private void RequestHolderInfoByChainId(string chainId, int index, ContractBasic tokenContract, DefaultToken token)
        {
            var getHolderInfoParam = new GetHolderInfoParams
            {
                caHash = _walletInfo.caInfo.caHash,
                chainId = chainId
            };
            StartCoroutine(PollHolderInfo(index, tokenContract, token, getHolderInfoParam));
        }

        private IEnumerator PollHolderInfo(int index, IContract tokenContract, DefaultToken token, GetHolderInfoParams holderInfoParams)
        {
            while (!_isSignOut || !_chainIdToCaAddress.ContainsKey(holderInfoParams.chainId))
            {
                yield return did.GetHolderInfoByContract(holderInfoParams, (holderInfo) =>
                {
                    if (holderInfo == null || holderInfo.guardianList == null ||
                        holderInfo.guardianList.guardians == null)
                    {
                        return;
                    }

                    _chainIdToCaAddress[holderInfoParams.chainId] = holderInfo.caAddress;

                    StartCoroutine(PollTokenBalance(index, tokenContract, token));
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
            yield return tokenContract.CallAsync<GetBalanceOutput>(_walletInfo.wallet, "GetBalance", balanceInput, output =>
            {
                var ownerAddress = output.Owner;
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
            did.AuthService.Message.Loading(true, "Signing out of Portkey...");

            StartCoroutine(did.AuthService.Logout(OnSuccessLogout));
        }
        
        private void OnSuccessLogout(bool success)
        {
            did.AuthService.Message.Loading(false);
            
            if (!success)
            {
                OnError("Log out failed.");
                return;
            }

            _isSignOut = true;
            
            ResetViews();
            OpenSignInView();
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
            did.AuthService.Message.Error(error);
        }

        private void CloseView()
        {
            gameObject.SetActive(false);
        }
    }
}