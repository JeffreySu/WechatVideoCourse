using Senparc.Weixin.Exceptions;
using Senparc.Weixin.Helpers.Extensions;
using Senparc.Weixin.MP.AdvancedAPIs.TemplateMessage;
using Senparc.Weixin.MP.Containers;
using SenparcClass.Service.Class14;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SenparcClass.Controllers
{
    public class RequestController : Controller
    {
        public ActionResult Get(string url = "https://www.baidu.com")
        {
            var html = Senparc.Weixin.HttpUtility.RequestUtility.HttpGet(url, encoding: Encoding.GetEncoding("GB2312"));

            html += "<script>alert('This is a remote page')</script>";

            return Content(html);
        }

        public ActionResult SimulateLogin()
        {
            var url = "http://www.baidu.com";
            var cookieContainer = new CookieContainer();
            var html = Senparc.Weixin.HttpUtility.RequestUtility.HttpGet(url, cookieContainer, Encoding.UTF8, null, null, false);

            return Content(html);
        }

        public ActionResult Post(string url = "https://sdk.weixin.senparc.com/AsyncMethods/TemplateMessageTest", string code = "")
        {

            var formData = new Dictionary<string, string>();
            formData["checkcode"] = code;
            var html = Senparc.Weixin.HttpUtility.RequestUtility.HttpPost(url, null, formData);

            html += "<span sytle='color:red'>alert('it's a remote page by POST')</script>";

            return Content(html);
        }

        public ActionResult GetImage(string url = "https://sdk.weixin.senparc.com/images/book-cover-front-small-3d-transparent.png")
        {
            var filePath = Server.MapPath("~/App_Data/");
            //Server.MapPath("~/App_Data/DownloadImage_{0}.jpg".FormatWith(DateTime.Now.Ticks));
            var fileName = Senparc.Weixin.HttpUtility.Get.Download(url, filePath);
            var newFileName = fileName + ".png";
            System.IO.File.Move(fileName, newFileName);

            //Form 表单上传本地文件
            var dic = new Dictionary<string, string>();
            dic["file"] = newFileName;
            var uploadUrl = "http://localhost:60716/Request/UploadFile";
            //ServicePointManager.Expect100Continue = false;

            var uploadResult = Senparc.Weixin.HttpUtility.RequestUtility.HttpPost(uploadUrl, fileDictionary: dic);

            return Content("图片已保存到：" + fileName + "<br/>" + uploadResult);
        }

        public ActionResult GetAndUploadImage(string url = "https://sdk.weixin.senparc.com/images/book-cover-front-small-3d-transparent.png")
        {
            var fileName = Server.MapPath("~/App_Data/DownloadImage_{0}.jpg".FormatWith(DateTime.Now.Ticks));

            using (var ms = new MemoryStream())
            {
                Senparc.Weixin.HttpUtility.Get.Download(url, ms);
                ms.Seek(0, SeekOrigin.Begin);

                //上传流
                var uploadUrl = "http://localhost:60716/Request/UploadImage";
                Senparc.Weixin.HttpUtility.RequestUtility.HttpPost(uploadUrl, null, ms);
            }

            return Content("图片已保存到：" + fileName);
        }

        [HttpPost]
        public ActionResult UploadImage()
        {
            var stream = Request.InputStream;
            var fileName = Server.MapPath("~/App_Data/UploadImage.jpg");
            using (var fileStream = new FileStream(fileName, FileMode.OpenOrCreate))
            {
                stream.CopyTo(fileStream);
                stream.Flush();
            }

            return Content("文件已经下载、上传，保存到：" + fileName);
        }

        [HttpPost]
        public ActionResult UploadFile(HttpPostedFileBase file)
        {
            var stream = file.InputStream;
            var fileName = Server.MapPath("~/App_Data/UploadFile.jpg");
            using (var fileStream = new FileStream(fileName, FileMode.OpenOrCreate))
            {
                stream.CopyTo(fileStream);
                stream.Flush();
            }

            return Content("文件已经下载、上传，保存到：" + fileName);
        }

        public class MyClass
        {
            public string Data { get; set; }
        }


        public ActionResult GetAccessToken()
        {

            //var myClass = new MyClass();

            //try
            //{
            //    if (myClass.Data == null)
            //    {
            //        throw new WeixinNullReferenceException("myClass.Data为null", myClass);
            //    }
            //}
            //catch (WeixinNullReferenceException ex)
            //{
            //    if (true)
            //    {
            //        var obj = ex.ParentObject as MyClass;
            //        obj.Data = "Not Null";

            //        Senparc.Weixin.WeixinTrace.SendCustomLog("系统日志", ex.Message);
            //    }
            //}

            //var length = myClass.Data.Length;
            //return Content("Length：" + length);

            var accessToken = AccessTokenContainer.GetAccessToken(Service.Config.AppId + "invalid appid", true);

            Senparc.Weixin.WeixinTrace.SendCustomLog("接口日志", "获取了新的AccessToken");

            return Content("最新的AccessToken：" + accessToken + "<br />当前LogRecordCount：" + Service.Config.LogRecordCount);
        }


    //    public void Send()
    //    {
    //        Task.Factory.StartNew(async () =>
    //        {
    //            var url = "https://weixin.senparc.com";
    //            var data = new TemplateMessageCourseNotice("欢迎学习微信开发", DateTime.Now.ToString(),
    //Request.UserHostAddress, "微信公众号+小程序快速开发第14节", "录制中", "祝大家新年快乐！",
    //"感谢大家对盛派网络的支持！", url);
    //            var result = await Senparc.Weixin.MP.AdvancedAPIs.TemplateApi.SendTemplateMessageAsync(Service.Config.AppId, openId, data);
    //        });
    //    }

        public async Task<ActionResult> SendTemplateMessage(string openId = "oxRg0uLsnpHjb8o93uVnwMK_WAVw")
        {
            //var bag = Service.Config.GetTemplateMessageBag(Service.Config.AppId, "视频培训测试");

            //if (bag == null)
            //{
            //    throw new WeixinException("模板名称不存在！模板：培训测试");
            //}

            //{{first.DATA}} Time：{{keyword1.DATA}} Host：{{keyword2.DATA}} Service：{{keyword3.DATA}} Status：{{keyword4.DATA}} Message：{{keyword5.DATA}} {{remark.DATA}}
            //var data = new
            //{
            //    first    = new TemplateDataItem("欢迎学习微信开发", "#ff0000"),
            //    keyword1 = new TemplateDataItem(DateTime.Now.ToString()),
            //    keyword2 = new TemplateDataItem(Request.UserHostAddress),
            //    keyword3 = new TemplateDataItem("微信公众号+小程序快速开发第14节"),
            //    keyword4 = new TemplateDataItem("录制中"),
            //    keyword5 = new TemplateDataItem("祝大家新年快乐！"),
            //    remark      = new TemplateDataItem("感谢大家对盛派网络的支持！")
            //};
            //var result = Senparc.Weixin.MP.AdvancedAPIs
            //.TemplateApi.SendTemplateMessage(Service.Config.AppId, openId, bag.TemplateId, url, data);

            var url = "https://weixin.senparc.com";


            var data = new TemplateMessageCourseNotice("欢迎学习微信开发", DateTime.Now.ToString(),
                Request.UserHostAddress, "微信公众号+小程序快速开发第14节", "录制中", "祝大家新年快乐！",
                "感谢大家对盛派网络的支持！", url);
            var result = await Senparc.Weixin.MP.AdvancedAPIs.TemplateApi
                .SendTemplateMessageAsync(Service.Config.AppId, openId, data);

            return Content(result.ToJson());
        }
    }
}