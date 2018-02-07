using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SenparcClass.Service
{
    public class Config
    {
        public static string AppId = System.Configuration.ConfigurationManager.AppSettings["WeixinAppId"];
    }
}
