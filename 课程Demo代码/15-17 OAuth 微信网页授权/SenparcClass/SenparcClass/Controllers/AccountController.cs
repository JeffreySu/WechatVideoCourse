using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SenparcClass.Controllers
{
    public class AccountController : Controller
    {
        public ActionResult Index()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("OAuthLogin", "OAuth", new { returnUrl = Request.Url.PathAndQuery });
            }
            ViewData["PageName"] = "Index";
            ViewData["Msg"] = "您已登录，用户名：" + User.Identity.Name;
            ViewData["UserName"] = User.Identity.Name;
            return View("Index");
        }

        public ActionResult SecondIndex()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("OAuthLogin", "OAuth", new { returnUrl = Request.Url.PathAndQuery });
            }
            ViewData["PageName"] = "SecondIndex";
            ViewData["Msg"] = "您已登录，用户名：" + User.Identity.Name;
            ViewData["UserName"] = User.Identity.Name;
            return View("Index");
        }

    }
}