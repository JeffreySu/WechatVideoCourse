using Senparc.CO2NET;
using Senparc.CO2NET.RegisterServices;
using Senparc.Weixin;
using Senparc.Weixin.Cache;
using Senparc.Weixin.Cache.Redis;
using Senparc.Weixin.Entities;
using Senparc.Weixin.MP;
using Senparc.Weixin.WxOpen;
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

            /* 
           * CO2NET 全局注册开始
           * 建议按照以下顺序进行注册
           */

            /*
             * CO2NET 是从 Senparc.Weixin 分离的底层公共基础模块，经过了长达 6 年的迭代优化。
             * 关于 CO2NET 在所有项目中的通用设置可参考 CO2NET 的 Sample：
             * https://github.com/Senparc/Senparc.CO2NET/blob/master/Sample/Senparc.CO2NET.Sample.netcore/Startup.cs
             */


            //设置全局 Debug 状态
            var isGLobalDebug = true;
            //全局设置参数，将被储存到 Senparc.CO2NET.Config.SenparcSetting
            var senparcSetting = SenparcSetting.BuildFromWebConfig(isGLobalDebug);
            //也可以通过这种方法在程序任意位置设置全局 Debug 状态：
            //Senparc.CO2NET.Config.IsDebug = isGLobalDebug;

            //CO2NET 全局注册，必须！！
            IRegisterService register = RegisterService.Start(senparcSetting).UseSenparcGlobal();

            /* 微信配置开始
            * 建议按照以下顺序进行注册
            */

            //设置微信 Debug 状态
            var isWeixinDebug = true;
            //全局设置参数，将被储存到 Senparc.Weixin.Config.SenparcWeixinSetting
            var senparcWeixinSetting = SenparcWeixinSetting.BuildFromWebConfig(isWeixinDebug);
            //也可以通过这种方法在程序任意位置设置微信的 Debug 状态：
            //Senparc.Weixin.Config.IsDebug = isWeixinDebug;

            register.UseSenparcWeixin(senparcWeixinSetting, Senparc.CO2NET.Config.SenparcSetting);

            WeixinTraceConfig();//配置微信日志记录
            RegisterWeixinCache();
            RegisterSenparcWeixin(register, senparcWeixinSetting);

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
            var redisConfigurationStr = Senparc.CO2NET.Config.SenparcSetting.Cache_Redis_Configuration;

            Senparc.CO2NET.Cache.Redis.Register.SetConfigurationOption(redisConfigurationStr);

            //以下会立即将全局缓存设置为 Redis
            Senparc.CO2NET.Cache.Redis.Register.UseKeyValueRedisNow();//键值对缓存策略（推荐）
            #endregion
        }


        private void RegisterSenparcWeixin(IRegisterService register, SenparcWeixinSetting senparcWeixinSetting)
        {
            var weixinSetting = Senparc.Weixin.Config.SenparcWeixinSetting;

            register.RegisterMpAccount(senparcWeixinSetting, "微信公众号测试号-Jeffrey")
                    .RegisterWxOpenAccount(senparcWeixinSetting, "小程序");
        }

        private void WeixinTraceConfig()
        {
            Senparc.Weixin.Config.IsDebug = true;//开启日志记录状态

            Senparc.Weixin.WeixinTrace.OnLogFunc = () =>
            {
                Service.Config.LogRecordCount++;
            };

            Senparc.Weixin.WeixinTrace.OnWeixinExceptionFunc = ex =>
            {
                Service.Config.LogExceptionRecordCount++;
            };

        }
    }
}
