using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SenparcClass.Service.Class10
{
    public class WeatherResult
    {
        public string date { get; set; }
        public string message { get; set; }
        public int status { get; set; }
        public string city { get; set; }
        public int count { get; set; }
        public DateTime AddTime { get; set; }

        public WeatherResult()
        {
            AddTime = DateTime.Now;
        }
    }
}
