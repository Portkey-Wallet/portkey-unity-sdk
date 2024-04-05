using UnityEngine;
using System.Collections.Generic;

namespace Portkey.Core
{
    /// <summary>
    /// Portkey configuration object. Contains only config data.
    /// </summary>
    [CreateAssetMenu(fileName = "PortkeyConfig", menuName = "Portkey/PortkeyConfig", order = 1)]
    public class PortkeyConfig : ScriptableObject
    {
        [Header("Portkey Endpoint")]
        [SerializeField]
        private string apiBaseUrl = "https://aa-portkey.portkey.finance";
        
        [Header("Portkey Token Endpoint")]
        [SerializeField]
        private string tokenApiUrl = "https://auth-aa-portkey.portkey.finance";
        
        [Header("OpenLogin Endpoint")]
        [SerializeField]
        private string openLoginUrl = "https://openlogin.portkey.finance/";
        
        [Header("Google PC Login")]
        [SerializeField]
        private string googlePCClientId = "931335042992-4cvdlgo4etblfe4t7dk9i1q7oouj2od0.apps.googleusercontent.com";
        [SerializeField]
        private string googlePCClientSecret = "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";
        
        [Header("Google Android Login")]
        [SerializeField]
        private string googleAndroidClientId = "931335042992-ousd4tdbui5n2msmqj94ppp632a27ofv.apps.googleusercontent.com";
        [SerializeField] 
        private string googleAndroidClientSecret = "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";
        
        [Header("Google iOS Login")]
        [SerializeField]
        private string googleIosClientId = "931335042992-n0aj79qor1t4qekpgbs5ahru9c891ker.apps.googleusercontent.com";
        [SerializeField]
        private string googleIosDotReverseClientId = "com.googleusercontent.apps.931335042992-n0aj79qor1t4qekpgbs5ahru9c891ker";

        [Header("Google WebGL Login")]
        [SerializeField]
        private string googleWebGLClientId = "931335042992-d8jgdbleopnpgjcmbqnf7dqhri93lj2m.apps.googleusercontent.com";
        
        [SerializeField]
        private string googleWebGLRedirectUri = "https://openlogin.portkey.finance/auth-callback";
        
        [Header("Telegram Login")]
        [SerializeField]
        private string telegramServiceUrl= "https://aa-portkey.portkey.finance";
        [SerializeField]
        private int telegramLoginPort= 53285;
        [SerializeField]
        private Dictionary<string, string> corsHeaders = new Dictionary<string, string>
        {
            { "Access-Control-Allow-Credentials", "true" },
            { "Access-Control-Allow-Headers", "Accept, X-Access-Token, X-Application-Name, X-Request-Sent-Time" },
            { "Access-Control-Allow-Methods", "GET, POST, OPTIONS" },
            { "Access-Control-Allow-Origin", "http://localhost:53285" },
        };
        
        [Header("Google Recaptcha Sitekeys")]
        [SerializeField]
        private string recaptchaWebSitekey = "6LfR_bElAAAAAJSOBuxle4dCFaciuu9zfxRQfQC0";
        [SerializeField]
        private string recaptchaAndroidSitekey = "6LcENaYnAAAAAGenYUvyat1R-TAzQaMZdOYBNSHC";


        [Header("Approval Settings")]
        [SerializeField] private int minApprovals = 3;
        [SerializeField] private int denominator = 5;
        
        [Header("Login App Settings")]
        [SerializeField] private TransportConfig portkeyTransportConfig;

        /// <summary>
        /// A getter for the chain infos.
        /// </summary>
        public string ApiBaseUrl => apiBaseUrl;
        public string TokenApiUrl => tokenApiUrl;
        public string OpenLoginUrl => openLoginUrl;
        public string GooglePCClientId => googlePCClientId;
        public string GooglePCClientSecret => googlePCClientSecret;
        public string GoogleAndroidClientId => googleAndroidClientId;
        public string GoogleAndroidClientSecret => googleAndroidClientSecret;
        public string GoogleIOSClientId => googleIosClientId;
        public string GoogleIOSDotReverseClientId => googleIosDotReverseClientId;
        public string GoogleWebGLClientId => googleWebGLClientId;
        public int TelegramLoginPort => telegramLoginPort;
        public string TelegramServiceUrl => telegramServiceUrl;
        public string GoogleWebGLRedirectUri => googleWebGLRedirectUri;
        public string RecaptchaWebSitekey => recaptchaWebSitekey;
        public string RecaptchaAndroidSitekey => recaptchaAndroidSitekey;
        public int MinApprovals => minApprovals;
        public int Denominator => denominator;
        public TransportConfig PortkeyTransportConfig => portkeyTransportConfig;
        public Dictionary<string, string> CorsHeaders => corsHeaders;
    }
}