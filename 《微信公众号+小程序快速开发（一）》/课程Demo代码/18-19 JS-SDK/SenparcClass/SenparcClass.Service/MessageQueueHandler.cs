using Senparc.Weixin.MessageQueue;
using Senparc.Weixin.MP.AdvancedAPIs;
using Senparc.Weixin.MP.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SenparcClass.Service
{
    public class MessageQueueHandler
    {
        public IResponseMessageBase SendMessage(string openId, IResponseMessageBase responseMessage)
        {
            var messageQueue = new SenparcMessageQueue();

            if (responseMessage is ResponseMessageText)
            {
                {
                    var mqKey = SenparcMessageQueue.GenerateKey("MessageHandlerSendMessageAsync", responseMessage.GetType(), Guid.NewGuid().ToString(), "SendMessage");
                    messageQueue.Add(mqKey, () =>
                    {
                        var asyncResponseMessage = responseMessage as ResponseMessageText;
                        asyncResponseMessage.Content += "\r\n\r\n - 这条消息来自客服接口-1";

                        //发送客服消息
                        CustomApi.SendText(Config.AppId, openId, asyncResponseMessage.Content);
                    });
                }
                {
                    var mqKey = SenparcMessageQueue.GenerateKey("MessageHandlerSendMessageAsync", responseMessage.GetType(), Guid.NewGuid().ToString(), "SendMessage");
                    messageQueue.Add(mqKey, () =>
                    {
                        var asyncResponseMessage = responseMessage as ResponseMessageText;
                        asyncResponseMessage.Content += "\r\n\r\n - 这条消息来自客服接口-2";

                        //发送客服消息
                        CustomApi.SendText(Config.AppId, openId, asyncResponseMessage.Content);
                    });
                }
               
               
                return new ResponseMessageNoResponse();
            }
            //else if ()
            //{
            //    //...
            //}

            return responseMessage;
        }
    }
}
