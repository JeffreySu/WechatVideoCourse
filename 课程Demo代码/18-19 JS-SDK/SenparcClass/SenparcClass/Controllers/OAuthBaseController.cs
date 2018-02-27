using SenparcClass.Filters;
using SenparcClass.Models.ViewData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SenparcClass.Controllers
{
    [CustomSenparcOAuth(null, "/OAuth/Callback", Senparc.Weixin.MP.OAuthScope.snsapi_base)]
    public class OAuthBaseController : Controller
    {
        public string UserName { get; set; }
        public DateTime PageStartTime { get; set; }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            var isLogined = User.Identity.IsAuthenticated;
            if (true)
            {
                UserName = User.Identity.Name;
            }
        }

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            base.OnActionExecuted(filterContext);

            if (ViewData.Model is OAuthBaseVD)
            {
                var model = ViewData.Model as OAuthBaseVD;
                model.UserName = UserName;
                model.PageRenderTime = DateTime.Now;
            }
        }
    }
}