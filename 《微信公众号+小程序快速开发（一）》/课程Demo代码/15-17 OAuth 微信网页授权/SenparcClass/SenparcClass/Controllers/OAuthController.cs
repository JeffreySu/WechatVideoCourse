using Senparc.Weixin.Exceptions;
using Senparc.Weixin.HttpUtility;
using Senparc.Weixin.MP.AdvancedAPIs;
using Senparc.Weixin.MP.AdvancedAPIs.OAuth;
using SenparcClass.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SenparcClass.Controllers
{
    public class OAuthController : Controller
    {
        public ActionResult Index()
        {
            var callbackUrl = "http://s101.class.senparc.com/OAuth/Callback?returnUrl=" +
                //"http://s101.class.senparc.com/OAuth/ReturnUrl".UrlEncode() +
                "&msg=Index";
            var state = "Senparc." + Guid.NewGuid().ToString("n");
            Session["OAuthState"] = state;
            var oauthOffcialUrl = Senparc.Weixin.MP.AdvancedAPIs.OAuthApi.GetAuthorizeUrl(Service.Config.AppId,
                 callbackUrl, state, Senparc.Weixin.MP.OAuthScope.snsapi_userinfo);

            ViewData["IsLogined"] = User.Identity.IsAuthenticated;
            ViewData["UserName"] = User.Identity.Name;

            return View("Index", model: oauthOffcialUrl);
        }

        public ActionResult Logout()
        {
            System.Web.Security.FormsAuthentication.SignOut();

            return RedirectToAction("Index");
        }

        /// <summary>
        /// 从官方授权页面接收回调
        /// </summary>
        /// <param name="code"></param>
        /// <param name="state"></param>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        public ActionResult Callback(string code, string state, string returnUrl, string msg)
        {
            //if (state == null || state != Session["OAuthState"] as string)
            //{
            //    return Content("非法进入！");
            //}

            if (string.IsNullOrEmpty(code))
            {
                return Content("用户拒绝了授权");
            }

            OAuthAccessTokenResult oauthResult = null;
            try
            {
                oauthResult = Senparc.Weixin.MP.AdvancedAPIs.OAuthApi.GetAccessToken(Config.AppId, Config.AppSecret, code);
            }
            catch (ErrorJsonResultException ex)
            {
                //处理
                throw;
            }

            var oauthAccessToken = oauthResult.access_token;
            var openId = oauthResult.openid;

            //var userInfo = UserApi.Info()
            var userInfo = OAuthApi.GetUserInfo(oauthAccessToken, openId);

            //更多的业务逻辑

            //进行登录
            System.Web.Security.FormsAuthentication.SetAuthCookie(openId, false);

            ////用户注册
            //if (!是否用户已注册)
            //{
            //    //进行注册

            //    var userInfo = 从数据库获取();
            //}
            //else
            //{
            //    var userInfo = OAuthApi.GetUserInfo(oauthAccessToken, openId);
            //}

            ViewData["Msg"] = msg;

            if (!string.IsNullOrEmpty(returnUrl))
            {
                return Redirect(returnUrl); 
            }
            else
            {
                return View(userInfo);
            }
        }

        /// <summary>
        /// 自动登录
        /// </summary>
        /// <returns></returns>
        public ActionResult OAuthLogin(string returnUrl)
        {
            var callbackUrl = "http://s101.class.senparc.com/OAuth/Callback?returnUrl=" +
                returnUrl.UrlEncode() +
                "&msg=OAuthLogin";
            var state = "Senparc." + Guid.NewGuid().ToString("n");
            Session["OAuthState"] = state;

            var oauthOffcialUrl = Senparc.Weixin.MP.AdvancedAPIs.OAuthApi.GetAuthorizeUrl(Service.Config.AppId,
              callbackUrl, state, Senparc.Weixin.MP.OAuthScope.snsapi_userinfo);
            return Redirect(oauthOffcialUrl);
        }
    }
}