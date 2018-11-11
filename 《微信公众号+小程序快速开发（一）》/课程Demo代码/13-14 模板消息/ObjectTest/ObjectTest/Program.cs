using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectTest
{
    //测试项目，课程中暂未用到


    public class ObjectTestClass
    {
        public int Id { get; set; }
        public DateTime DateTime { get; set; }
        public string Data { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            List<string> xmlList = new List<string>();

            //XML数据格式
            var xmlFormat = @"<xml><Id>{0}</Id><DateTime>{1}</DateTime><Data>{2}</Data></xml>";
            int xmlCount = 100000;
            for (int i = 0; i < xmlCount; i++)
            {
                var xml = string.Format(xmlFormat, i, DateTime.Now.ToString(), "This is a String No." + i);
                xmlList.Add(xml);
            }

            //遍历生成对象
            List<dynamic> dynamicList = new List<dynamic>();
            List<object> objectList = new List<object>();
            List<ObjectTestClass> classList = new List<ObjectTestClass>();

            foreach (var item in xmlList)
            {

            }

        }
    }
}
