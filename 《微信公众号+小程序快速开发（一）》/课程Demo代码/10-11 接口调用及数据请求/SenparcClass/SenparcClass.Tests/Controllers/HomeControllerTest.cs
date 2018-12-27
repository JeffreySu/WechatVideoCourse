using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SenparcClass;
using SenparcClass.Controllers;
using System.Threading;

namespace SenparcClass.Tests.Controllers
{
    [TestClass]
    public class HomeControllerTest
    {
        [TestMethod]
        public void Index()
        {
            // Arrange
            HomeController controller = new HomeController();

            // Act
            ViewResult result = controller.Index() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void About()
        {
            // Arrange
            HomeController controller = new HomeController();

            // Act
            ViewResult result = controller.About() as ViewResult;

            // Assert
            Assert.AreEqual("Your application description page.", result.ViewBag.Message);
        }

        [TestMethod]
        public void Contact()
        {
            // Arrange
            HomeController controller = new HomeController();

            // Act
            ViewResult result = controller.Contact() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }

        private int totalThreadCount = 100;
        private int finishedThreadCount = 0;

        [TestMethod]
        public void LockTest()
        {
            List<Thread> threads = new List<Thread>();
            for (int i = 0; i < totalThreadCount; i++)
            {
                // Arrange
                var thread = new Thread(RunSingleRequest);
                thread.Start();
            }

            while (totalThreadCount!= finishedThreadCount)
            {

            }

            Console.WriteLine("测试完成，线程总数："+ totalThreadCount);
        }

        private void RunSingleRequest() {
            HomeController controller = new HomeController();
            // Act
            ContentResult result = controller.LockTest() as ContentResult;

            Console.WriteLine("结果：" + result.Content);

            finishedThreadCount++;
        }
    }
}
