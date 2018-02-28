using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SenparcClass.Models.ViewData
{
    public class OAuthBaseVD
    {
        public string UserName { get; set; }
        public DateTime PageRenderTime { get; internal set; }
    }
}