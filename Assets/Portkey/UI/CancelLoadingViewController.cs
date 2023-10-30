using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Portkey.DID;

    public class CancelLoadingViewController : MonoBehaviour
    {
        private PortkeySDK PortkeySDK { get; set; }
        private GameObject PreviousView { get; set; }
        private Action OnClickCloseCallback { get; set; }
        
        public void Initialize(PortkeySDK portkeySDK, GameObject previousView, Action onClickCloseCallback = null)
        {
            PortkeySDK = portkeySDK;
            PreviousView = previousView;
            OnClickCloseCallback = onClickCloseCallback;

            OpenView();
        }

        private void OpenView()
        {
            gameObject.SetActive(true);
        }
        public void OnClickClose()
        {
            PortkeySDK.AuthService.Message.Loading(false);
            OnClickCloseCallback?.Invoke();
            CloseView();
        }

        public void CloseView()
        {
            gameObject.SetActive(false);
        }
    }

