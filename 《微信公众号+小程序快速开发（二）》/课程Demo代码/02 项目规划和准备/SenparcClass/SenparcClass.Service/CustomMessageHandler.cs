using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Senparc.Weixin.MP;
using Senparc.Weixin.MP.AppStore;
using Senparc.Weixin.MP.Entities;
using Senparc.Weixin.MP.Entities.Request;
using Senparc.Weixin.MP.Helpers;
using Senparc.Weixin.MP.MessageHandlers;
using Senparc.Weixin.MP.Containers;
using System.Threading;
using Senparc.Weixin.MP.AdvancedAPIs;
using System.Text.RegularExpressions;
using Senparc.Weixin.HttpUtility;
using SenparcClass.Service.Class10;
using Senparc.Weixin.Exceptions;
using Senparc.NeuChar.Entities;
using Senparc.NeuChar.Entities.Request;
using Senparc.NeuChar.Helpers;
using Senparc.CO2NET.Extensions;
using Senparc.NeuChar;

namespace SenparcClass.Service
{
    public class CustomMessageHandler : MessageHandler<CustomMessageContext>
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public override XDocument Init(XDocument postDataDocument, IEncryptPostModel postData = null)
        {
            StartTime = DateTime.Now;
            return base.Init(postDataDocument, postData);
        }

        public CustomMessageHandler(Stream inputStream, PostModel postModel = null, int maxRecordCount = 0, DeveloperInfo developerInfo = null) : base(inputStream, postModel, maxRecordCount, developerInfo)
        {
            base.CurrentMessageContext.ExpireMinutes = 10;
        }

        public CustomMessageHandler(XDocument requestDocument, PostModel postModel = null, int maxRecordCount = 0, DeveloperInfo developerInfo = null) : base(requestDocument, postModel, maxRecordCount, developerInfo)
        {
        }

        public CustomMessageHandler(RequestMessageBase requestMessageBase, PostModel postModel = null, int maxRecordCount = 0, DeveloperInfo developerInfo = null) : base(requestMessageBase, postModel, maxRecordCount, developerInfo)
        {
        }

        public override IResponseMessageBase OnTextRequest(RequestMessageText requestMessage)
        {
            var handler = requestMessage.StartHandler(false)
                .Keyword("cmd", () =>
                {
                    var responseMessageText = requestMessage.CreateResponseMessage<ResponseMessageText>();

                    CurrentMessageContext.StorageData = new StorageModel()
                    {
                        IsInCmd = true
                    };

                    responseMessageText.Content += "\r\n已经进入CMD状态";

                    return responseMessageText;
                })
                .Keywords(new[] { "exit", "quit", "close" }, () =>
                  {
                      var responseMessageText = requestMessage.CreateResponseMessage<ResponseMessageText>();

                      var storageModel = CurrentMessageContext.StorageData as StorageModel;
                      if (storageModel != null)
                      {
                          storageModel.IsInCmd = false;
                      }
                      return responseMessageText;
                  }).Regex(@"^http", () =>
                  {
                      var responseMessageNews = requestMessage.CreateResponseMessage<ResponseMessageNews>();


                      var news = new Article()
                      {
                          Title = "您输入了网址：" + requestMessage.Content,
                          Description = "这里是描述，第一行\r\n这里是描述，第二行",
                          PicUrl = "http://sdk.weixin.senparc.com/images/book-cover-front-small-3d-transparent.png",
                          Url = requestMessage.Content
                      };

                      responseMessageNews.Articles.Add(news);

                      return responseMessageNews;
                  })
                  .Regex(@"天气 \S+", () =>
                  {
                      var city = Regex.Match(requestMessage.Content, @"(?<=天气 )(\S+)").Value;
                      var url = "http://www.sojson.com/open/api/weather/json.shtml?city={0}".FormatWith(city.UrlDecode());
                      var result = Senparc.CO2NET.HttpUtility.Get.GetJson<WeatherResult>(url, null, null);
                      var responseMessageText = requestMessage.CreateResponseMessage<ResponseMessageText>();
                      responseMessageText.Content = @"天气查询
=============
城市：{0}
添加时间：{1}
日期：{2}
Count：{3}".FormatWith(result.city, result.AddTime, result.date, result.count);
                      return responseMessageText;
                  })
                  .Default(() =>
                  {
                      var responseMessageText = requestMessage.CreateResponseMessage<ResponseMessageText>();
                      responseMessageText.Content = "这是一条默认的文本请求回复信息";
                      return responseMessageText;
                  });

            var responseMessage = handler.ResponseMessage;
            if (responseMessage is ResponseMessageText)
            {
                (responseMessage as ResponseMessageText).Content += "\r\n您输入了文字：" + requestMessage.Content;

                var storageModel = CurrentMessageContext.StorageData as StorageModel;
                if (storageModel != null)
                {
                    (responseMessage as ResponseMessageText).Content += "当前CMD Count：" +
                    storageModel.CmdCount;
                }
            }

            return handler.ResponseMessage as IResponseMessageBase;
        }

        public override IResponseMessageBase OnLocationRequest(RequestMessageLocation requestMessage)
        {
            var responseMessage = requestMessage.CreateResponseMessage<ResponseMessageText>();
            responseMessage.Content = "您发送了位置信息：Lat-{0}，Lon-{1}".FormatWith(requestMessage.Location_X,
                requestMessage.Location_Y);
            return responseMessage;
        }

        public override IResponseMessageBase OnEvent_ClickRequest(RequestMessageEvent_Click requestMessage)
        {
            if (requestMessage.EventKey == "123")
            {
                var responseMessage = requestMessage.CreateResponseMessage<ResponseMessageNews>();


                var news = new Article()
                {
                    Title = "您点击了按钮：" + requestMessage.EventKey,
                    Description = "这里是描述，第一行\r\n这里是描述，第二行",
                    PicUrl = "http://sdk.weixin.senparc.com/images/book-cover-front-small-3d-transparent.png",
                    Url = "sdk.weixin.senparc.com"
                };

                responseMessage.Articles.Add(news);


                return responseMessage;
            }
            if (requestMessage.EventKey == "A")
            {
                var responseMessage = requestMessage.CreateResponseMessage<ResponseMessageText>();

                var storageModel = CurrentMessageContext.StorageData as StorageModel;
                if (storageModel != null)
                {
                    if (storageModel.IsInCmd)
                    {
                        responseMessage.Content = "当前已经进入CMD状态";
                        responseMessage.Content += "\r\n您的上一条消息类型为：" +
                                                   CurrentMessageContext.RequestMessages.Last().MsgType;
                    }
                    else
                    {
                        responseMessage.Content = "当前已经退出CMD状态";
                    }
                }
                else
                {
                    responseMessage.Content = "找不到Session数据";
                }

                return responseMessage;
            }
            if (requestMessage.EventKey == "B")
            {
                return new ResponseMessageNoResponse();
            }
            else
            {
                var responseMessage = requestMessage.CreateResponseMessage<ResponseMessageText>();
                responseMessage.Content = "您点击了按钮：" + requestMessage.EventKey;
                return responseMessage;
            }
        }

        public override IResponseMessageBase OnTextOrEventRequest(RequestMessageText requestMessage)
        {
            if (requestMessage.Content == "123")
            {
                var responseMessageText = requestMessage.CreateResponseMessage<ResponseMessageText>();
                responseMessageText.Content = "您在OnTextOrEventRequest消息中执行到了特定条件。当前关键字：" + requestMessage.Content;

                var result = Senparc.Weixin.MP.AdvancedAPIs.CustomApi.SendText(Config.AppId, base.WeixinOpenId, "你马上要收到一条文字消息。<a href=\"https://weixin.senparc.com\">点击这里打开SDK官网</a>\r\n这里已经换了一行\r\n这里又换了一行");

                return responseMessageText;
            }

            return base.OnTextOrEventRequest(requestMessage);
        }


        public override void OnExecuting()
        {
            var storageModel = CurrentMessageContext.StorageData as StorageModel;
            if (storageModel != null && storageModel.IsInCmd)
            {
                storageModel.CmdCount++;

                if (storageModel.CmdCount >= 5)
                {
                    //ResponseMessage = new ResponseMessageNoResponse();//不返回任何消息

                    var responseMessageText = RequestMessage.CreateResponseMessage<ResponseMessageText>();
                    responseMessageText.Content = "CmdCount已经大于等于5！";
                    ResponseMessage = responseMessageText;
                    base.CancelExcute = true;
                }
            }

            base.OnExecuting();
        }

        public override void OnExecuted()
        {
            if (ResponseMessage is ResponseMessageText)
            {
                (ResponseMessage as ResponseMessageText).Content += "\r\n【盛派网络】";

                //数据库处理（t >5 s）
                //队列、线程/Thread
            }

            base.OnExecuted();

            //Thread.Sleep(5000);

            //EndTime = DateTime.Now;
            //var runTime = (EndTime - StartTime).TotalSeconds;
            //if (runTime > 4)
            //{
            //    var queueHandler = new MessageQueueHandler();
            //    ResponseMessage = queueHandler.SendMessage(WeixinOpenId, ResponseMessage);
            //}
        }

        public override IResponseMessageBase OnEvent_SubscribeRequest(RequestMessageEvent_Subscribe requestMessage)
        {
            //var appId = System.Configuration.ConfigurationManager.AppSettings["WeixinAppId"];

            //var accessToken = AccessTokenContainer.GetAccessToken(Config.AppId);
            var userInfo = Senparc.Weixin.MP.AdvancedAPIs.UserApi.Info(Config.AppId, base.WeixinOpenId);

            string nickName = userInfo.nickname;
            string title = userInfo.sex == 1 ? "先生" : (userInfo.sex == 2 ? "女士" : "");

            var responseMessage = requestMessage.CreateResponseMessage<ResponseMessageText>();
            responseMessage.Content = "欢迎 【{0}{1}】 关注《微信公众号+小程序快速开发》课程！".FormatWith(nickName, title);
            return responseMessage;
        }

        public override IResponseMessageBase OnEvent_TemplateSendJobFinishRequest(RequestMessageEvent_TemplateSendJobFinish requestMessage)
        {
            if (requestMessage.Status != "success")
            {
                //进行逻辑处理
            }
            else
            {
                try
                {
                    //注意：此方法内不能再发送模板消息，否则会引发系循环
                    Senparc.Weixin.MP.AdvancedAPIs.CustomApi.SendText(Service.Config.AppId, WeixinOpenId, "模板消息发送成功，MsgId：" + requestMessage.MsgID);
                }
                catch (WeixinException ex)
                {
                    //处理
                }
            }


            return new SuccessResponseMessage();
        }

        public override IResponseMessageBase DefaultResponseMessage(IRequestMessageBase requestMessage)
        {
            var responseMessage = requestMessage.CreateResponseMessage<ResponseMessageText>();
            responseMessage.Content = "当前服务器时间：" + DateTime.Now;
            return responseMessage;
        }
    }
}
