using Senparc.Weixin.Cache;
using Senparc.Weixin.Helpers.Extensions;
using Senparc.Weixin.MP.Containers;
using SenparcClass.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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

        public ActionResult CustomMessage(string openId = "oxRg0uLsnpHjb8o93uVnwMK_WAVw")
        {
            for (int i = 0; i < 3; i++)
            {
                Thread.Sleep(500);
                try
                {
                    Senparc.Weixin.MP.AdvancedAPIs.CustomApi.SendText(Config.AppId, openId, "从服务器发来的客服消息：" + (3 - i));
                }
                catch (Exception)
                {
                    //额外的一些处理
                }

            }

            var result = Senparc.Weixin.MP.AdvancedAPIs.CustomApi.SendText(Config.AppId, openId, "从服务器发来的客服消息。" + DateTime.Now);

            return Content(result.ToJson());
        }

        public async Task<ActionResult> CustomMessageAsync(string openId = "oxRg0uLsnpHjb8o93uVnwMK_WAVw")
        {
            return await Task.Factory.StartNew(async () =>
             {
                 for (int i = 0; i < 4; i++)
                 {
                     var result = await Senparc.Weixin.MP.AdvancedAPIs.CustomApi.SendTextAsync(Config.AppId, openId, "【异步方法】从服务器发来的客服消息。时间：{0}，编号：{1}".FormatWith(DateTime.Now.ToString("HH:mm:ss.ffff"), i + 1));
                 }
                 return Content("消息发送完毕！");
             }).Result;
        }

        public async Task<ActionResult> CustomMessageSync(string openId = "oxRg0uLsnpHjb8o93uVnwMK_WAVw")
        {
            return await Task.Factory.StartNew(async () =>
            {
                for (int i = 0; i < 4; i++)
                {
                    //var result = await Senparc.Weixin.MP.AdvancedAPIs.CustomApi.SendTextAsync(Config.AppId, openId, "从服务器发来的客服消息。时间：{0}，编号：{1}".FormatWith(DateTime.Now, i + 1));
                    var accessToken = AccessTokenContainer.GetAccessToken(Config.AppId);
                    var result = Senparc.Weixin.MP.AdvancedAPIs.CustomApi.SendText(Config.AppId, openId, "【同步方法】从服务器发来的客服消息。时间：{0}，编号：{1}新AccessToken：{2}".FormatWith(DateTime.Now.ToString("HH:mm:ss.ffff"), i + 1, accessToken));
                }
                return Content("消息发送完毕！");
            }).Result;
        }

        public async Task<ActionResult> ChangeAccessToken(string openId = "oxRg0uLsnpHjb8o93uVnwMK_WAVw")
        {
            return await Task.Factory.StartNew(async () =>
            {
                for (int i = 0; i < 4; i++)
                {
                    var accessToken = await AccessTokenContainer.GetAccessTokenAsync(Config.AppId, true);

                    var result = await Senparc.Weixin.MP.AdvancedAPIs.CustomApi.SendTextAsync(accessToken, openId, "【异步方法】从服务器发来的客服消息。时间：{0}，编号：{1}\r\n\r\n新AccessToken：{2}".FormatWith(DateTime.Now.ToString("HH:mm:ss.ffff"), i + 1, accessToken));
                }
                return Content("消息发送完毕！");
            }).Result;
        }
    }
}