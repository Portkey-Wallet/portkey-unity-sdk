using System;
using System.Collections.Generic;
using Portkey.Core;
using Portkey.SocialProvider;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using System.Collections;
using Portkey.Contracts.CA;
using Guardian = Portkey.Core.Guardian;
using Google.Protobuf;
using AElf.Types;

namespace Portkey.UI
{
    public class TransferSettingsViewController : MonoBehaviour
    {
        [Header("Views")]
        [SerializeField] protected GuardiansApprovalViewController guardianApprovalViewController;
        
		[Header("UI Elements")]
        [SerializeField] protected TMP_InputField inputFieldForTransactionLimit;
		[SerializeField] protected TMP_InputField inputFieldForDailyLimit;
        [SerializeField] protected Button sendRequestButton;
        [SerializeField] protected TextMeshProUGUI errorText1;
		[SerializeField] protected TextMeshProUGUI errorText2;
        
        public GameObject limitContainer;
        public GameObject PromptContainer;
        protected DIDAccountInfo _accountInfo = null;
        protected GameObject PreviousView { get; set; }
        protected DID.PortkeySDK PortkeySDK { get; set; }
        public static GetTransferLimitResult GetTransferLimitResult { get; set; } = null;
        public static List<Guardian> GuardiansList { get; set; } = null;
        public static IContract CaContract { get; set; } = null;
        public static List<ApprovedGuardian> _approvedGuardians = null;
        public static IContract.TransactionInfoDto TransactionInfoDto { get; set; } = null;
        
        protected void OnEnable()
        {
            ResetView();
            
        }
        
        public DIDAccountInfo AccountInfo
        {
            set => _accountInfo = value;
        }
        public List<ApprovedGuardian> ApprovedGuardians
        {
            set => _approvedGuardians = value;
        }
        
        public void Initialize(DID.PortkeySDK portkeySDK, GameObject previousView)
        {
            PortkeySDK = portkeySDK;
            PreviousView = previousView;
            OpenView();
            var inputParams = new GetTransferLimitParams
            {
                caHash = _accountInfo.caInfo.caHash,
                maxResultCount = 10
            };
            StartCoroutine(PortkeySDK.GetTransferLimit(_accountInfo, inputParams, result =>
            {
                GetTransferLimitResult = result;
                if (result.totalRecordCount > 0)
                {
                    inputFieldForTransactionLimit.text = GetTransferLimitResult.data[0].singleLimit.Substring(0, GetTransferLimitResult.data[0].singleLimit.Length - 8);
                    inputFieldForDailyLimit.text = GetTransferLimitResult.data[0].dailyLimit.Substring(0, GetTransferLimitResult.data[0].dailyLimit.Length - 8);
                    ShowContainer();
                }

                ;
            }, ErrorCallback));
            
        }
        
        public void OpenView()
        {
            gameObject.SetActive(true);
        }

        protected void ResetView()
        {
			
        }

        public void SetApprovedGuardians(List<ApprovedGuardian> approvedGuardians)
        {
            ApprovedGuardians = approvedGuardians;
        }

        public virtual void OnClickSendRequest()
		{
			var transactionLimitValue = LimitNumber.Parse(inputFieldForTransactionLimit.text);
            var dailyLimitValue = LimitNumber.Parse(inputFieldForDailyLimit.text);
            errorText1.text = transactionLimitValue.Int == 0 ? "Please enter a non-zero value" : null;
            errorText2.text = dailyLimitValue.Int == 0 ? "Please enter a non-zero value" : null;
            errorText1.text = transactionLimitValue.Int > dailyLimitValue.Int
                ? "Cannot exceed the daily limit"
                : errorText1.text;
            StartCoroutine(PortkeySDK.AuthService.GetGuardians(_accountInfo, result =>
            {
                GuardiansList = result;
            }));
            // guardianApprovalViewController.Initialize(GuardiansList, previousView: gameObject);
            // CloseView();
            StartCoroutine(WaitForApprovedGuardians(dailyLimitValue, transactionLimitValue));
            PromptContainer.SetActive(true);
        }
        private IEnumerator WaitForApprovedGuardians(LimitNumber dailyLimitValue, LimitNumber transactionLimitValue)
        {
            while (_approvedGuardians.Count == 0)  
            {
                yield return null; 
            }
            var setTransferLimitInput = new SetTransferLimitInput
            {
                CaHash = Hash.LoadFromHex(_accountInfo.caInfo.caHash),
                DailyLimit = dailyLimitValue.Int * 100000000L,
                SingleLimit = transactionLimitValue.Int * 100000000L,
                Symbol = "ELF",
            };
            foreach (var approvedGuardian in _approvedGuardians)
            {
                setTransferLimitInput.GuardiansApproved.Add(new GuardianInfo
                {
                    IdentifierHash = Hash.LoadFromHex(GuardiansList[0].idHash),
                    Type = GuardianType.OfApple,
                    VerificationInfo = new VerificationInfo
                    {
                        Id = Hash.LoadFromHex(approvedGuardian.verifierId),
                        Signature = ByteString.CopyFromUtf8(approvedGuardian.signature),
                        VerificationDoc = approvedGuardian.verificationDoc
                    }
                });
            };
            StartCoroutine(PortkeySDK.SetTransferLimit(_accountInfo, setTransferLimitInput, result =>
            {
                TransactionInfoDto = result;
            }, ErrorCallback));

        }

        public void OnClickBack()
        {
            ResetView();
            CloseView();
            PreviousView.SetActive(true);
        }
        
        
        public void OnClickClose()
        {
            CloseView();
        }

        public void ShowContainer()
        {
            limitContainer.SetActive(true);
        }

        public void HideContainer()
        {
            limitContainer.SetActive(false);
        }
        
        protected void CloseView()
        {
            gameObject.SetActive(false);
        }
        private void ErrorCallback(string param)
        {
            Debug.Log("errorCallback");
        }
        

    }
}