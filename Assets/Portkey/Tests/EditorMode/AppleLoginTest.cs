using NUnit.Framework;
using Portkey.Core;
using Portkey.Network;
using Portkey.SocialProvider;
using Portkey.Utilities;

namespace Portkey.Test
{
    public class AppleLoginTest
    {
        private static readonly string PORTKEYCONFIG_NAME = "PortkeyConfig";
        private static readonly string ACCESS_TOKEN = "eyJraWQiOiJZdXlYb1kiLCJhbGciOiJSUzI1NiJ9.eyJpc3MiOiJodHRwczovL2FwcGxlaWQuYXBwbGUuY29tIiwiYXVkIjoiY29tLnBvcnRrZXkuZGlkLmV4dGVuc2lvbi5zZXJ2aWNlIiwiZXhwIjoxNjkyODU5Njk1LCJpYXQiOjE2OTI3NzMyOTUsInN1YiI6IjAwMTc0OS5mZDcwOGUxMmUwNWQ0NjI1OGI0Y2RmODU1MmFmNjMzZS4wODExIiwiY19oYXNoIjoiUmNEeVBFTi1hWkJMTXBnN3dOeHNjdyIsImVtYWlsIjoicTVxbXg1bnZ6Z0Bwcml2YXRlcmVsYXkuYXBwbGVpZC5jb20iLCJlbWFpbF92ZXJpZmllZCI6InRydWUiLCJpc19wcml2YXRlX2VtYWlsIjoidHJ1ZSIsImF1dGhfdGltZSI6MTY5Mjc3MzI5NSwibm9uY2Vfc3VwcG9ydGVkIjp0cnVlfQ.HTzrn4OCOg6r14ynq2jonym0temTs8bg2SPw93H5hX_7nwuqRlsod_AmF6f0Z_9QStnmR4RjlMZdyekegIdPGHh-eIeBvfyJPL57cEtHOegOSxU4o-p5qiiy7afMiJOIPGoTW96VMEhqN89SnK8roaSLky4ECWsMXAOHBdrSXZla7jJWFX9b1ro-tsj4plLNcSAotG-wKYiIf-c4ccS_zB8RO3Y6rE_ZdngDthg3ZfZIUTKh7f01q-Pona2BS_j-ro0BpmyosQZoEngwPSo1yLTTWvm4Ajdnzw_Jt-LODn4c5cr-QAmaw2tWHkyP-hlYo0fSVcX0zxfi6h4K0_nrsA";
        private static readonly string EMAIL = "q5qmx5nvzg@privaterelay.appleid.com";
        
        [Test]
        public void RequestSocialInfoTest()
        {
            var portkeyConfig = UnityTestUtilities.GetPortkeyConfig(PORTKEYCONFIG_NAME);
            ISocialLogin androidAppleLogin = new AndroidAppleLogin(portkeyConfig, new RequestHttp());
            
            androidAppleLogin.RequestSocialInfo(ACCESS_TOKEN, info =>
            {
                Assert.AreEqual(EMAIL, info.socialInfo.email); 
            }, error =>
            {
                Assert.Fail($"Should not have failed: {error}"); 
            });
        }
    }
}