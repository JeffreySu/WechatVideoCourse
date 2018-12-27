using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SenparcClass.Service
{
    public class TemplateMessageItem
    {
        public string Name { get; set; }
        public string AppId { get; set; }
        public string TemplateNumber { get; set; }
        public string TemplateId { get; set; }
        public string SubscribeMsgTemplateId { get; set; }

        public TemplateMessageItem(string name, string appId, string templateNumber, string templateId, string subscribeMsgTemplateId)
        {
            Name = name;
            AppId = appId;
            TemplateNumber = templateNumber;
            TemplateId = templateId;
            SubscribeMsgTemplateId = subscribeMsgTemplateId;
        }
    }

    public static class Config
    {
        public static string AppId = System.Configuration.ConfigurationManager.AppSettings["WeixinAppId"];
        public static string AppSecret = System.Configuration.ConfigurationManager.AppSettings["WeixinAppSecret"];
        public static string MchId = System.Configuration.ConfigurationManager.AppSettings["WeixinMchId"];
        public static string TenPayV3Notify = System.Configuration.ConfigurationManager.AppSettings["TenPayV3_TenpayNotify"];
        public static string TenPayV3_Key = System.Configuration.ConfigurationManager.AppSettings["TenPayV3_Key"];

        //小程序
        public static string WxOpenAppId = System.Configuration.ConfigurationManager.AppSettings["WxOpenWeixinAppId"];
        public static string WxOpenAppSecret = System.Configuration.ConfigurationManager.AppSettings["WxOpenWeixinAppSecret"];

        public static int LogRecordCount = 0;

        public static int LogExceptionRecordCount = 0;

        public static Dictionary<string, List<TemplateMessageItem>> TemplateMessageCollection;

        static Config()
        {
            TemplateMessageCollection = new Dictionary<string, List<TemplateMessageItem>>();

            TemplateMessageCollection[AppId] = new List<TemplateMessageItem>() {
                new TemplateMessageItem("视频培训测试", AppId, "Nil",
                    "nSS3Jx7q-eOhCM-bpv1jdSm3_slq2c1pxF_PKUNXj5g", "Nil"),
                //new TemplateMessageBag("课程提醒", AppId, "63l8YSI2uYqlZwb8dkMSy2Lp8caHcaWc2Id0b_XYvtM",
                //    "KU0hL0UVWzJA_8jmolH_o1UcNuNIcvQ36EiPcdd6F8Y", "OPENTM411013653"),
            };

            //var tmBag = TemplateMessageCollection[AppId].FirstOrDefault(z => z.Name == "课程提醒");
        }

        /// <summary>
        /// 获取TemplateMessageBag
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static TemplateMessageItem GetTemplateMessageBag(string appId,string name)
        {
            if (!TemplateMessageCollection.ContainsKey(appId))
            {
                return null;
            }

            var bagList = TemplateMessageCollection[appId];
            var bag = bagList.FirstOrDefault(z => z.Name == name);
            return bag;
        }
    }


    //public static string TemplateNumber = "OPENTM411013653";//模板消息编号

    //public static string TemplateId = "KU0hL0UVWzJA_8jmolH_o1UcNuNIcvQ36EiPcdd6F8Y";//模板ID

    //public static string SubscribeMsgTemplateId = "63l8YSI2uYqlZwb8dkMSy2Lp8caHcaWc2Id0b_XYvtM";//一次性订阅消息模板ID

}
