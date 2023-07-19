﻿namespace Portkey.Core
{
    public class SocialLoginInfo
    {
        public string access_token;
        public SocialInfo socialInfo;
    }
    
    public class SocialInfo
    {
        // Social user ID;
        public string sub;
        public string name;
        public string given_name;
        public string family_name;
        public string picture;
        public string email;
        public bool email_verified;
        public string locale;
    }
}