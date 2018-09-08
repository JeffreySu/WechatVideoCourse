using Senparc.Weixin;
using Senparc.Weixin.Cache;
using Senparc.Weixin.Cache.Redis;
using SenparcClass.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace SenparcClass
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            Senparc.WebSocket.WebSocketConfig.RegisterRoutes(RouteTable.Routes);
            Senparc.WebSocket.WebSocketConfig.RegisterMessageHandler<CustomWebSocketMessageHandler>();

            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            var dt1 = DateTime.Now;

            WeixinTraceConfig();//配置微信日志记录
            RegisterWeixinCache();
            RegisterThreads();//必须执行在RegisterSenparcWeixin()方法之前
            RegisterSenparcWeixin();

            var dt2 = DateTime.Now;
            Senparc.Weixin.WeixinTrace.SendCustomLog("系统日志", "系统已经启动，启动时间：" + (dt2 - dt1).TotalMilliseconds + "ms");
        }


        /// <summary>
        /// 自定义缓存策略
        /// </summary>
        private void RegisterWeixinCache()
        {
            // 当同一个分布式缓存同时服务于多个网站（应用程序池）时，可以使用命名空间将其隔离（非必须）
            Senparc.Weixin.Config.DefaultCacheNamespace = "SenparcClassWeixinCache";

            #region  Redis配置
            //如果留空，默认为localhost（默认端口）
            var redisConfiguration = System.Configuration.ConfigurationManager.AppSettings["Cache_Redis_Configuration"];
            RedisManager.ConfigurationOption = redisConfiguration;

            //如果不执行下面的注册过程，则默认使用本地缓存

            if (!string.IsNullOrEmpty(redisConfiguration) && redisConfiguration != "Redis配置")
            {
                CacheStrategyFactory.RegisterObjectCacheStrategy(() => RedisObjectCacheStrategy.Instance);//Redis
            }

            #endregion
        }

        private void RegisterThreads()
        {
            Senparc.Weixin.Threads.ThreadUtility.Register();
        }

        private void RegisterSenparcWeixin()
        {
            var appId = System.Configuration.ConfigurationManager.AppSettings["WeixinAppId"];
            var appSecret = System.Configuration.ConfigurationManager.AppSettings["WeixinAppSecret"];

            Senparc.Weixin.MP.Containers.AccessTokenContainer.Register(appId, appSecret, "微信公众号测试号-Jeffrey");
            Senparc.Weixin.MP.Containers.AccessTokenContainer.Register("APPID2", "APPSECRET2", "微信公众号测试号-Bob");//可以注册多个公众号

            //小程序
            Senparc.Weixin.MP.Containers.AccessTokenContainer.Register(SenparcClass.Service.Config.WxOpenAppId, SenparcClass.Service.Config.WxOpenAppSecret, "WX快速开发小程序");

            //Senparc.Weixin.MP.Containers.JsApiTicketContainer.Register(appId, appSecret, "微信公众号测试号-Jeffrey-JsApiTicket");
        }

        private void WeixinTraceConfig()
        {
            Senparc.Weixin.Config.IsDebug = true;//开启日志记录状态

            Senparc.Weixin.WeixinTrace.OnLogFunc = () =>
            {
                Service.Config.LogRecordCount++;
            };

            Senparc.Weixin.WeixinTrace.OnWeixinExceptionFunc = ex => {
                Service.Config.LogExceptionRecordCount++;
            };

        }
    }
}
