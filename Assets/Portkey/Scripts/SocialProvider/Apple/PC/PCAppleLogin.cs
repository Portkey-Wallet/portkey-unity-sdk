using Portkey.Core;

namespace Portkey.SocialProvider
{
    public class PCAppleLogin : AppleLoginBase
    {
        private static readonly string ACCESS_TOKEN =
            "eyJraWQiOiJZdXlYb1kiLCJhbGciOiJSUzI1NiJ9.eyJpc3MiOiJodHRwczovL2FwcGxlaWQuYXBwbGUuY29tIiwiYXVkIjoiY29tLnBvcnRrZXkuZGlkLmV4dGVuc2lvbi5zZXJ2aWNlIiwiZXhwIjoxNjkyODU5Njk1LCJpYXQiOjE2OTI3NzMyOTUsInN1YiI6IjAwMTc0OS5mZDcwOGUxMmUwNWQ0NjI1OGI0Y2RmODU1MmFmNjMzZS4wODExIiwiY19oYXNoIjoiUmNEeVBFTi1hWkJMTXBnN3dOeHNjdyIsImVtYWlsIjoicTVxbXg1bnZ6Z0Bwcml2YXRlcmVsYXkuYXBwbGVpZC5jb20iLCJlbWFpbF92ZXJpZmllZCI6InRydWUiLCJpc19wcml2YXRlX2VtYWlsIjoidHJ1ZSIsImF1dGhfdGltZSI6MTY5Mjc3MzI5NSwibm9uY2Vfc3VwcG9ydGVkIjp0cnVlfQ.HTzrn4OCOg6r14ynq2jonym0temTs8bg2SPw93H5hX_7nwuqRlsod_AmF6f0Z_9QStnmR4RjlMZdyekegIdPGHh-eIeBvfyJPL57cEtHOegOSxU4o-p5qiiy7afMiJOIPGoTW96VMEhqN89SnK8roaSLky4ECWsMXAOHBdrSXZla7jJWFX9b1ro-tsj4plLNcSAotG-wKYiIf-c4ccS_zB8RO3Y6rE_ZdngDthg3ZfZIUTKh7f01q-Pona2BS_j-ro0BpmyosQZoEngwPSo1yLTTWvm4Ajdnzw_Jt-LODn4c5cr-QAmaw2tWHkyP-hlYo0fSVcX0zxfi6h4K0_nrsA";
        
        public PCAppleLogin(PortkeyConfig config, IHttp request) : base(request)
        {
        }

        protected override void OnAuthenticate()
        {
            RequestSocialInfo(ACCESS_TOKEN, null, null);
        }
    }
}