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
        public void OpenView()
        {
            gameObject.SetActive(true);
        }
        public void OnClickClose()
        {
            CloseView();
        }

        public void DeactiveObj()
        {
            gameObject.SetActive(false);
        }

        private void CloseView()
        {
            DID.AuthService.Message.Loading(false);
            gameObject.SetActive(false);
        }
    }

