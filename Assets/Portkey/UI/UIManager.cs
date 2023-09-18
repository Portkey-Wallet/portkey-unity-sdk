using System;
using UnityEngine;

namespace Portkey.UI
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private LoadingViewController _loadingView = null;
        [SerializeField] private ErrorViewController _errorView = null;
        [SerializeField] private DID.DID _did;

        private void Start()
        {
            _did.AuthService.Message.OnErrorEvent += _errorView.ShowErrorText;
        }
    }
}