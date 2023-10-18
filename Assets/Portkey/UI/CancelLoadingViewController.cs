using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Portkey.DID;

    public class CancelLoadingViewController : MonoBehaviour
    {
        protected DID DID { get; set; }
        protected GameObject PreviousView { get; set; }
        
        public void Initialize(DID did, GameObject previousView)
        {
            DID = did;
            PreviousView = previousView;

            OpenView();
        }

        private void OpenView()
        {
            gameObject.SetActive(true);
        }
        public void OnClickClose()
        {
            DID.AuthService.Message.Loading(false);
            CloseView();
        }

        public void CloseView()
        {
            gameObject.SetActive(false);
        }
    }

