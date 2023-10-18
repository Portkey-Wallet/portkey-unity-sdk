using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Portkey.DID;

    public class CancelLoadingViewController : MonoBehaviour
    {
        private DID DID { get; set; }
        private GameObject PreviousView { get; set; }
        private Action OnClickCloseCallback { get; set; }
        
        public void Initialize(DID did, GameObject previousView, Action onClickCloseCallback = null)
        {
            DID = did;
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
            DID.AuthService.Message.Loading(false);
            OnClickCloseCallback?.Invoke();
            CloseView();
        }

        public void CloseView()
        {
            gameObject.SetActive(false);
        }
    }

