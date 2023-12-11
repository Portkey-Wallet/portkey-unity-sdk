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

namespace Portkey.UI
{
    public class TransferSettingsViewController : MonoBehaviour
    {
		[Header("UI Elements")]
        [SerializeField] protected TMP_InputField inputFieldForTransactionLimit;
		[SerializeField] protected TMP_InputField inputFieldForDailyLimit;
        [SerializeField] protected Button sendRequestButton;
        [SerializeField] protected TextMeshProUGUI errorText1;
		[SerializeField] protected TextMeshProUGUI errorText2;

		public GameObject limitContainer;
        protected GameObject PreviousView { get; set; }
        protected DID.PortkeySDK PortkeySDK { get; set; }
        public static GetTransferLimitResult GetTransferLimitResult { get; set; } = null;
        protected string CaHash { get; set; }
        
        protected void OnEnable()
        {
            ResetView();
            
        }
        
        public void Initialize(DID.PortkeySDK portkeySDK, GameObject previousView, string caHash)
        {
            PortkeySDK = portkeySDK;
            PreviousView = previousView;
            CaHash = caHash;
            OpenView();
            var inputParams = new GetTransferLimitParams
            {
                caHash = caHash
            };
            StartCoroutine(PortkeySDK.GetTransferLimit(inputParams, result =>
            {
                GetTransferLimitResult = result;
            }, PortkeySDK.AuthService.Message.Error));
        }
        
        public void OpenView()
        {
            gameObject.SetActive(true);
        }

        protected void ResetView()
        {
			
        }

		public virtual void OnClickSendRequest()
		{
			var transactionLimitValue = LimitNumber.Parse(inputFieldForTransactionLimit.text);
            var dailyLimitValue = LimitNumber.Parse(inputFieldForDailyLimit.text);
            if (transactionLimitValue.Int == 0)
            {
                errorText1.text = "Please enter an nonzero value";
                return;
            }
            if (dailyLimitValue.Int == 0)
            {
                errorText2.text = "Please enter an nonzero value";
                return;
            }
            if (transactionLimitValue.Int > dailyLimitValue.Int)
            {
                errorText1.text = "Cannot exceed the daily limit";
            }

            var defaultLimitForElf = GetTransferLimitResult;

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
        

    }
}