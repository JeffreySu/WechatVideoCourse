using Senparc.Weixin.Cache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace SenparcClass.Controllers
{
    public class HomeController : Controller
    {


        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        private static int _count = 0;
        public static int Count
        {
            get
            {
                Thread.Sleep(100);
                return _count;
            }
            set
            {
                Thread.Sleep(100);
                _count = value;
            }
        }


        public ActionResult LockTest()
        {
            var strategy = CacheStrategyFactory.GetObjectCacheStrategyInstance();
            using (strategy.BeginCacheLock("SenparcClass", "LockTest"))
            {
                var count = Count;
                Thread.Sleep(2);
                Count = count + 1;
                return Content(count.ToString());
            }
        }
    }
}