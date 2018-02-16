using Senparc.Weixin.Entities.TemplateMessage;
using Senparc.Weixin.MP.AdvancedAPIs.TemplateMessage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SenparcClass.Service.Class14
{
    public class TemplateMessageCourseNotice : TemplateMessageBase
    {
        public TemplateDataItem first { get; set; }
        public TemplateDataItem keyword1 { get; set; }
        public TemplateDataItem keyword2 { get; set; }
        public TemplateDataItem keyword3 { get; set; }
        public TemplateDataItem keyword4 { get; set; }
        public TemplateDataItem keyword5 { get; set; }
        public TemplateDataItem remark { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_first"></param>
        /// <param name="time">时间</param>
        /// <param name="host"></param>
        /// <param name="service"></param>
        /// <param name="status"></param>
        /// <param name="message"></param>
        /// <param name="_remark"></param>
        /// <param name="url"></param>
        /// <param name="templateId"></param>
        public TemplateMessageCourseNotice(
            string _first,
            string time,
            string host,
            string service,
            string status,
            string message,
            string _remark,
            string url, string templateId = "nSS3Jx7q-eOhCM-bpv1jdSm3_slq2c1pxF_PKUNXj5g")
            : base(templateId, url, "视频培训测试")
        {
            first = new TemplateDataItem(_first);
            keyword1 = new TemplateDataItem(time);
            keyword2 = new TemplateDataItem(host);
            keyword3 = new TemplateDataItem(service);
            keyword4 = new TemplateDataItem(status);
            keyword5 = new TemplateDataItem(message);
            remark = new TemplateDataItem(_remark);
        }
    }
}
