using Senparc.CO2NET.Extensions;
using Senparc.Weixin.HttpUtility;
using Senparc.Weixin.TenPay;
using Senparc.Weixin.TenPay.V3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SenparcClass.Controllers
{
    public class ProductController : Controller
    {
        #region H5支付

        /// <summary>
        /// H5支付
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="hc"></param>
        /// <returns></returns>
        public ActionResult H5Pay(int orderId = 0)
        {
            {
                try
                {
                    string openId = null;//此时在外部浏览器，无法或得到OpenId

                    string sp_billno = Request["order_no"];
                    if (string.IsNullOrEmpty(sp_billno))
                    {
                        //生成订单10位序列号，此处用时间和随机数生成，商户根据自己调整，保证唯一
                        sp_billno = string.Format("{0}{1}{2}", "1234567890", DateTime.Now.ToString("yyyyMMddHHmmss"),
                                   TenPayV3Util.BuildRandomStr(6));
                    }
                    else
                    {
                        sp_billno = Request["order_no"];
                    }

                    var timeStamp = TenPayV3Util.GetTimestamp();
                    var nonceStr = TenPayV3Util.GetNoncestr();

                    var body = "SenparcProduct";//产品名称
                    var price = 100;//单位：分

                    //var ip = Request.Params["REMOTE_ADDR"];
                    var xmlDataInfo = new TenPayV3UnifiedorderRequestData(Service.Config.AppId, Service.Config.MchId,
                        body, sp_billno, price, Request.UserHostAddress, Service.Config.TenPayV3Notify,
                        TenPayV3Type.MWEB/*此处无论传什么，方法内部都会强制变为MWEB*/, openId,
                        Service.Config.TenPayV3_Key, nonceStr);

                    var result = TenPayV3.Html5Order(xmlDataInfo);//调用统一订单接口
                                                                  //JsSdkUiPackage jsPackage = new JsSdkUiPackage(TenPayV3Info.AppId, timeStamp, nonceStr,);

                    /*
                     * result:{"device_info":"","trade_type":"MWEB","prepay_id":"wx20170810143223420ae5b0dd0537136306","code_url":"","mweb_url":"https://wx.tenpay.com/cgi-bin/mmpayweb-bin/checkmweb?prepay_id=wx20170810143223420ae5b0dd0537136306\u0026package=1505175207","appid":"wx669ef95216eef885","mch_id":"1241385402","sub_appid":"","sub_mch_id":"","nonce_str":"juTchIZyhXvZ2Rfy","sign":"5A37D55A897C854F64CCCC4C94CDAFE3","result_code":"SUCCESS","err_code":"","err_code_des":"","return_code":"SUCCESS","return_msg":null}
                     */
                    //return Json(result, JsonRequestBehavior.AllowGet);

                    var package = string.Format("prepay_id={0}", result.prepay_id);

                    //ViewData["product"] = product;

                    ViewData["appId"] = Service.Config.AppId;
                    ViewData["timeStamp"] = timeStamp;
                    ViewData["nonceStr"] = nonceStr;
                    ViewData["package"] = package;
                    ViewData["paySign"] = TenPayV3.GetJsPaySign(Service.Config.AppId, timeStamp, nonceStr, package, Service.Config.TenPayV3_Key);

                    //设置成功页面（也可以不设置，支付成功后默认返回来源地址）
                    var returnUrl =
                        string.Format("https://sdk.weixin.senparc.com/TenpayV3/H5PaySuccess?orderId={0}",
                            orderId);

                    var mwebUrl = result.mweb_url;
                    if (!string.IsNullOrEmpty(returnUrl))
                    {
                        mwebUrl += string.Format("&redirect_url={0}", returnUrl.AsUrlData());
                    }

                    ViewData["MWebUrl"] = mwebUrl;

                    //临时记录订单信息，留给退款申请接口测试使用
                    Session["BillNo"] = sp_billno;
                    Session["BillFee"] = price;

                    return View();
                }
                catch (Exception ex)
                {
                    var msg = ex.Message;
                    msg += "<br>" + ex.StackTrace;
                    msg += "<br>==Source==<br>" + ex.Source;

                    if (ex.InnerException != null)
                    {
                        msg += "<br>===InnerException===<br>" + ex.InnerException.Message;
                    }
                    return Content(msg);
                }
            }
        }

        #endregion

    }
}