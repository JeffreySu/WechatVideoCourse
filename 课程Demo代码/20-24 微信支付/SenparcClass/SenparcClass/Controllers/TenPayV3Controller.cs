using Senparc.Weixin.Exceptions;
using Senparc.Weixin.MP;
using Senparc.Weixin.MP.Helpers;
using Senparc.Weixin.MP.TenPayLibV3;
using SenparcClass.Models.ViewData;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace SenparcClass.Controllers
{
    public class TenPayV3Controller : OAuthBaseController
    {
        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (ViewData.Model is Base_TenPayV3VD)
            {
                var model = ViewData.Model as Base_TenPayV3VD;

                var jssdkUiPackage = JSSDKHelper.GetJsSdkUiPackage(Service.Config.AppId, Service.Config.AppSecret,
              Request.Url.AbsoluteUri);

                model.JsSdkUiPackage = jssdkUiPackage;
            }

            base.OnActionExecuted(filterContext);
        }

        public ActionResult Index()
        {
            var vd = new TenPayV3_Index()
            {
                ProductList = new[] { "产品1", "产品2", "产品3" }
            };
            return View(vd);
        }

        public ActionResult Order(string name)
        {
            var body = name ?? "SenparcProduct";
            var price = 100;//单位：分
            var sp_billno = string.Format("{0}{1}{2}", "1234567890", DateTime.Now.ToString("yyyyMMddHHmmss"),
                        TenPayV3Util.BuildRandomStr(6));
            var openId = User.Identity.Name;
            var nonceStr = TenPayV3Util.GetNoncestr();
            var timeStamp = TenPayV3Util.GetTimestamp();


            var xmlDataInfo = new TenPayV3UnifiedorderRequestData(
                Service.Config.AppId, Service.Config.MchId, body, sp_billno, price, Request.UserHostAddress,
                Service.Config.TenPayV3Notify, TenPayV3Type.JSAPI, openId, Service.Config.TenPayV3_Key, nonceStr);

            var result = TenPayV3.Unifiedorder(xmlDataInfo);//调用统一订单接口
                                                            //JsSdkUiPackage jsPackage = new JsSdkUiPackage(TenPayV3Info.AppId, timeStamp, nonceStr,);
            var package = string.Format("prepay_id={0}", result.prepay_id);

            var paySign = TenPayV3.GetJsPaySign(Service.Config.AppId, timeStamp, nonceStr, package, Service.Config.TenPayV3_Key);

            var vd = new TenPayV3_Odrer()
            {
                Product = name,
                Package = package,
                PaySign = paySign
            };

            //临时记录订单信息，留给退款申请接口测试使用
            Session["BillNo"] = sp_billno;//储存在数据库
            Session["BillFee"] = price;

            return View(vd);
        }

        /// <summary>
        /// 支付回调URL，对应于Service.Config.TenPayV3Notify
        /// </summary>
        /// <returns></returns>
        public ActionResult PayNotifyUrl()
        {
            try
            {
                ResponseHandler resHandler = new ResponseHandler(null);

                string return_code = resHandler.GetParameter("return_code");
                string return_msg = resHandler.GetParameter("return_msg");

                string res = null;

                resHandler.SetKey(Service.Config.TenPayV3_Key);

                //验证请求是否从微信发过来（安全）!!!!!
                if (resHandler.IsTenpaySign() && return_code.ToUpper() == "SUCCESS")
                {
                    res = "success";//正确的订单处理
                    //直到这里，才能认为交易真正成功了，可以进行数据库操作，但是别忘了返回规定格式的消息！
                }
                else
                {
                    res = "wrong";//错误的订单处理
                }

                /* 这里可以进行订单处理的逻辑 */

                //TODO：判断订单状态（Lock）

                //发送支付成功的模板消息
                if (res == "success")
                {
                    try
                    {
                        //TODO：订单状态更新


                        string appId = Service.Config.AppId;//与微信公众账号后台的AppId设置保持一致，区分大小写。
                        string openId = resHandler.GetParameter("openid");
                        //var templateData = new WeixinTemplate_PaySuccess("https://weixin.senparc.com", "购买商品", "状态：" + return_code);

                        Senparc.Weixin.WeixinTrace.SendCustomLog("支付成功模板消息参数", appId + " , " + openId);

                        //var result = AdvancedAPIs.TemplateApi.SendTemplateMessage(appId, openId, templateData);
                    }
                    catch (Exception ex)
                    {
                        Senparc.Weixin.WeixinTrace.SendCustomLog("支付成功模板消息", ex.ToString());
                    }

                }

                #region 记录日志

                var logDir = Server.MapPath(string.Format("~/App_Data/TenPayNotify/{0}", DateTime.Now.ToString("yyyyMMdd")));
                if (!Directory.Exists(logDir))
                {
                    Directory.CreateDirectory(logDir);
                }

                var logPath = Path.Combine(logDir, string.Format("{0}-{1}-{2}.txt", DateTime.Now.ToString("yyyyMMdd"), DateTime.Now.ToString("HHmmss"), Guid.NewGuid().ToString("n").Substring(0, 8)));

                using (var fileStream = System.IO.File.OpenWrite(logPath))
                {
                    var notifyXml = resHandler.ParseXML();
                    //fileStream.Write(Encoding.Default.GetBytes(res), 0, Encoding.Default.GetByteCount(res));

                    fileStream.Write(Encoding.Default.GetBytes(notifyXml), 0, Encoding.Default.GetByteCount(notifyXml));
                    fileStream.Close();
                }

                #endregion


                string xml = string.Format(@"<xml>
<return_code><![CDATA[{0}]]></return_code>
<return_msg><![CDATA[{1}]]></return_msg>
</xml>", return_code, return_msg);
                return Content(xml, "text/xml");
            }
            catch (Exception ex)
            {
                new WeixinException(ex.Message, ex);
                throw;
            }
        }
    }
}