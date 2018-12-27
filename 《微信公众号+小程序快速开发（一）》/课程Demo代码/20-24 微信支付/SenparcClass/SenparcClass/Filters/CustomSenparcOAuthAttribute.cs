using Senparc.Weixin.MP;
using Senparc.Weixin.MP.MvcExtension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SenparcClass.Filters
{
    public class CustomSenparcOAuthAttribute : SenparcOAuthAttribute
    {
        public CustomSenparcOAuthAttribute(string appId, string oauthCallbackUrl, OAuthScope oauthScope = OAuthScope.snsapi_userinfo)
            : base(appId, oauthCallbackUrl, oauthScope)
        {
            base._appId = base._appId ?? Service.Config.AppId;
        }

        public override bool IsLogined(HttpContextBase httpContext)
        {
            var isLogined = httpContext.User.Identity.IsAuthenticated;
            return isLogined;
        }
    }
}