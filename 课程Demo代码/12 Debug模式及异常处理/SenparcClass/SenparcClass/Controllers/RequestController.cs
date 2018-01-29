using Senparc.Weixin.Helpers.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
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

    }
}