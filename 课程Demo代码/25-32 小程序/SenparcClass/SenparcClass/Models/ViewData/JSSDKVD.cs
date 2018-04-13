using Senparc.Weixin.MP.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SenparcClass.Models.ViewData
{
    public class Base_JSSDKVD : OAuthBaseVD
    {
        public JsSdkUiPackage JsSdkUiPackage { get; set; }
    }

    public class JSSDK_Index : Base_JSSDKVD
    {
        public string Msg { get; set; }
    }
}