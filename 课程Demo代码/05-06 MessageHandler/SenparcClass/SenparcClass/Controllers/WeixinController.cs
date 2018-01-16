using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using Senparc.Weixin.MP;
using Senparc.Weixin.MP.Entities.Request;
using Senparc.Weixin.MP.MvcExtension;
using SenparcClass.Service;

namespace SenparcClass.Controllers
{
    public class WeixinController : Controller
    {
        public static readonly string Token = WebConfigurationManager.AppSettings["WeixinToken"];//与微信公众账号后台的Token设置保持一致，区分大小写。

        public static readonly string EncodingAESKey = WebConfigurationManager.AppSettings["WeixinEncodingAESKey"];//与微信公众账号后台的EncodingAESKey设置保持一致，区分大小写。

        readonly Func<string> _getRandomFileName = () => DateTime.Now.ToString("yyyyMMdd-HHmmss") + Guid.NewGuid().ToString("n").Substring(0, 6);

        [HttpGet]
        [ActionName("Index")]
        public ActionResult Get(PostModel postModel, string echostr)
        {
            if (CheckSignature.Check(postModel.Signature, postModel.Timestamp, postModel.Nonce, Token))
            {
                return Content(echostr); //返回随机字符串则表示验证通过
            }
            else
            {
                return Content("failed:" + postModel.Signature + "," + CheckSignature.GetSignature(postModel.Timestamp, postModel.Nonce, Token) + "。" +
                    "如果你在浏览器中看到这句话，说明此地址可以被作为微信公众账号后台的Url，请注意保持Token一致。");
            }
        }

        [HttpPost]
        [ActionName("Index")]
        public ActionResult Post(PostModel postModel)
        {
            postModel.EncodingAESKey = EncodingAESKey;

            //创建MessgeHandler实例
            var messageHandler = new CustomMessageHandler(Request.InputStream, postModel, 10);


            //去重
            messageHandler.OmitRepeatedMessage = true;

            #region 记录 Request 日志

            var logPath = Server.MapPath(string.Format("~/App_Data/MP/{0}/", DateTime.Now.ToString("yyyy-MM-dd")));
            if (!Directory.Exists(logPath))
            {
                Directory.CreateDirectory(logPath);
            }

            //测试时可开启此记录，帮助跟踪数据，使用前请确保App_Data文件夹存在，且有读写权限。
            messageHandler.RequestDocument.Save(Path.Combine(logPath, string.Format("{0}_Request_{1}_{2}.txt", _getRandomFileName(),
                messageHandler.RequestMessage.FromUserName,
                messageHandler.RequestMessage.MsgType)));
            if (messageHandler.UsingEcryptMessage)
            {
                messageHandler.EcryptRequestDocument.Save(Path.Combine(logPath, string.Format("{0}_Request_Ecrypt_{1}_{2}.txt", _getRandomFileName(),
                    messageHandler.RequestMessage.FromUserName,
                    messageHandler.RequestMessage.MsgType)));
            }

            #endregion

            //执行
            messageHandler.Execute();

            #region 记录 Response 日志

            //测试时可开启，帮助跟踪数据

            //if (messageHandler.ResponseDocument == null)
            //{
            //    throw new Exception(messageHandler.RequestDocument.ToString());
            //}
            if (messageHandler.ResponseDocument != null)
            {
                messageHandler.ResponseDocument.Save(Path.Combine(logPath, string.Format("{0}_Response_{1}_{2}.txt", _getRandomFileName(),
                    messageHandler.ResponseMessage.ToUserName,
                    messageHandler.ResponseMessage.MsgType)));
            }

            if (messageHandler.UsingEcryptMessage && messageHandler.FinalResponseDocument != null)
            {
                //记录加密后的响应信息
                messageHandler.FinalResponseDocument.Save(Path.Combine(logPath, string.Format("{0}_Response_Final_{1}_{2}.txt", _getRandomFileName(),
                    messageHandler.ResponseMessage.ToUserName,
                    messageHandler.ResponseMessage.MsgType)));
            }

            #endregion


            //返回
            //return Content(messageHandler.FinalResponseDocument.ToString());

            return new FixWeixinBugWeixinResult(messageHandler);
        }
    }
}