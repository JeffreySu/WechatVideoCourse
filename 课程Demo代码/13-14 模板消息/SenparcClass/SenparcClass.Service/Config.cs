using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SenparcClass.Service
{
    public class TemplateMessageBag
    {
        public string Name { get; set; }
        public string AppId { get; set; }
        public string TemplateNumber { get; set; }
        public string TemplateId { get; set; }
        public string SubscribeMsgTemplateId { get; set; }

        public TemplateMessageBag(string name, string appId, string templateNumber, string templateId, string subscribeMsgTemplateId)
        {
            Name = name;
            AppId = AppId;
            TemplateNumber = templateNumber;
            TemplateId = templateId;
            SubscribeMsgTemplateId = subscribeMsgTemplateId;

        }
    }

    public static class Config
    {
        public static string AppId = System.Configuration.ConfigurationManager.AppSettings["WeixinAppId"];

        public static int LogRecordCount = 0;

        public static int LogExceptionRecordCount = 0;

        public static Dictionary<string, List<TemplateMessageBag>> TemplateMessageCollection;

        static Config()
        {
            TemplateMessageCollection = new Dictionary<string, List<TemplateMessageBag>>();


            TemplateMessageCollection[AppId] = new List<TemplateMessageBag>() {
                new TemplateMessageBag("课程提醒", AppId, "63l8YSI2uYqlZwb8dkMSy2Lp8caHcaWc2Id0b_XYvtM",
                    "KU0hL0UVWzJA_8jmolH_o1UcNuNIcvQ36EiPcdd6F8Y", "OPENTM411013653"),
                //new TemplateMessageBag("课程提醒", AppId, "63l8YSI2uYqlZwb8dkMSy2Lp8caHcaWc2Id0b_XYvtM",
                //    "KU0hL0UVWzJA_8jmolH_o1UcNuNIcvQ36EiPcdd6F8Y", "OPENTM411013653"),
            };

            //var tmBag = TemplateMessageCollection[AppId].FirstOrDefault(z => z.Name == "课程提醒");
        }
    }


    //public static string TemplateNumber = "OPENTM411013653";//模板消息编号

    //public static string TemplateId = "KU0hL0UVWzJA_8jmolH_o1UcNuNIcvQ36EiPcdd6F8Y";//模板ID

    //public static string SubscribeMsgTemplateId = "63l8YSI2uYqlZwb8dkMSy2Lp8caHcaWc2Id0b_XYvtM";//一次性订阅消息模板ID

}
