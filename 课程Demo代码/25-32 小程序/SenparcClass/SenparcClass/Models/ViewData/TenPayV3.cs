using Senparc.Weixin.MP.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SenparcClass.Models.ViewData
{
    public class Base_TenPayV3VD : OAuthBaseVD
    {
        public JsSdkUiPackage JsSdkUiPackage { get; set; }
    }

    public class TenPayV3_Index : Base_TenPayV3VD
    {
        public string[] ProductList { get; set; }
    }


    public class TenPayV3_Odrer : Base_TenPayV3VD
    {
        public string Product { get; set; }
        public string Package { get; internal set; }
        public string PaySign { get; internal set; }
    }
}