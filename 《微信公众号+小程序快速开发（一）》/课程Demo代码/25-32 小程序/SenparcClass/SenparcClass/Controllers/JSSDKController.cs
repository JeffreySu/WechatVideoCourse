using Senparc.Weixin.MP.Helpers;
using SenparcClass.Models.ViewData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SenparcClass.Controllers
{
    public class JSSDKController : OAuthBaseController
    {
        public JSSDKController()
        {

        }

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (ViewData.Model is Base_JSSDKVD)
            {
                var model = ViewData.Model as Base_JSSDKVD;

                var jssdkUiPackage = JSSDKHelper.GetJsSdkUiPackage(Service.Config.AppId, Service.Config.AppSecret,
              Request.Url.AbsoluteUri);

                model.JsSdkUiPackage = jssdkUiPackage;
            }

            base.OnActionExecuted(filterContext);
        }


        public ActionResult Index()
        {
            //var jssdkUiPackage = JSSDKHelper.GetJsSdkUiPackage(Service.Config.AppId, Service.Config.AppSecret,
            //    Request.Url.AbsoluteUri);

            var vd = new JSSDK_Index()
            {
                //JsSdkUiPackage = jssdkUiPackage,
                Msg = "当前用户信息：" + base.UserName
            };

            return View(vd);
        }

        //public ActionResult Action()
        //{
        //    return View();
        //}
    }
}