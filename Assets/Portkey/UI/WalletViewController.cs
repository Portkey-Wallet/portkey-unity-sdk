using System.Collections;
using System.Linq;
using Portkey.Contract;
using AElf.Contracts.MultiToken;
using Portkey.Core;
using Portkey.Utilities;
using TMPro;
using UnityEngine;

namespace Portkey.UI
{
    public class WalletViewController : MonoBehaviour
    {
        [SerializeField] private DID.DID did;
        
        [Header("UI")]
        [SerializeField] private TextMeshProUGUI addressText;
        [SerializeField] private GameObject confirmSignOutDialog;
        [SerializeField] private TextMeshProUGUI tokenLabelText;
        [SerializeField] private TextMeshProUGUI tokenBalanceText;
        
        [Header("View")]
        [SerializeField] private SignInViewController signInViewController;
        [SerializeField] private ErrorViewController errorView;
        [SerializeField] private LoadingViewController loadingView;
        [SerializeField] private SetPINViewController setPinViewController;
        [SerializeField] private GuardiansApprovalViewController guardiansApprovalViewController;
        [SerializeField] private LockViewController lockViewController;
        
        private DIDWalletInfo _walletInfo = null;
        private ContractBasic _tokenContract = null;
        private string _tokenSymbol = "ELF";
        private bool _isSignOut = false;
        
        public DIDWalletInfo WalletInfo
        {
            set => _walletInfo = value;
        }
        
        private void OnEnable()
        {
            confirmSignOutDialog.SetActive(false);
            _isSignOut = false;
            tokenBalanceText.text = "loading...";

            StartCoroutine(_tokenContract == null ? GetChainInfo() : PollTokenBalance());
        }

        private IEnumerator GetChainInfo()
        {
            yield return did.PortkeySocialService.GetChainsInfo(chains =>
            {
                var chainInfos = chains.items?.ToDictionary(info => info.chainId, info => info);
                if (chainInfos == null || !chainInfos.TryGetValue(_walletInfo.chainId, out var chainInfo))
                {
                    return;
                }
                
                var tokenAddress = chainInfo.defaultToken.address;
                _tokenSymbol = chainInfo.defaultToken.symbol;

                _tokenContract = new ContractBasic(did.GetChain(_walletInfo.chainId), tokenAddress);
                
                StartCoroutine(PollTokenBalance());
            }, OnError);
        }
        
        private IEnumerator PollTokenBalance()
        {
            while (!_isSignOut)
            {
                yield return UpdateTokenBalance();
                yield return new WaitForSeconds(5);
            }
        }
        
        private void Start()
        {
            addressText.text = _walletInfo.caInfo.caAddress;
        }

        private IEnumerator UpdateTokenBalance()
        {
            var balanceInput = new GetBalanceInput()
            {
                Owner = did.GetWallet().Address.ToAddress(),
                Symbol = _tokenSymbol
            };
            yield return _tokenContract.CallTransactionAsync<GetBalanceOutput>(did.GetWallet(), "GetBalance", balanceInput, output =>
            {
                tokenLabelText.text = output.Symbol;
                tokenBalanceText.text = output.Balance.ToString();
            }, OnError);
        }

        public void OnClickSignOut()
        {
            var param = new EditManagerParams
            {
                chainId = _walletInfo.chainId
            };

            ShowLoading(true, "Signing out of Portkey...");
            
            StartCoroutine(did.Logout(param, OnSuccessLogout, OnError));
        }
        
        private void OnSuccessLogout(bool success)
        {
            ShowLoading(false);
            
            if (!success)
            {
                OnError("Log out failed.");
                return;
            }

            _tokenContract = null;
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
            Debug.LogError(error);
            errorView.ShowErrorText(error);
            
            ShowLoading(false);
        }

        private void CloseView()
        {
            gameObject.SetActive(false);
        }
        
        private void ShowLoading(bool show, string text = "")
        {
            loadingView.DisplayLoading(show, text);
        }
    }
}