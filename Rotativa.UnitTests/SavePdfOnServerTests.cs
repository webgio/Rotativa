using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using MvcIntegrationTestFramework.Browsing;
using MvcIntegrationTestFramework.Hosting;
using NUnit.Framework;
using SharpTestsEx;

namespace Rotativa.UnitTests
{
    [TestFixture]
    public class SavePdfOnServerTests
    {
        private AppHost appHost;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            //If you MVC project is not in the root of your solution directory then include the path
            //e.g. AppHost.Simulate("Website\MyMvcApplication")
            appHost = AppHost.Simulate("Rotativa.Demo");
        }

        [Test]
        public void GivenAViewResultWithSaveOption_WhenIRequestTheAction_IShouldSeeTheFileOnTheServer()
        {
            appHost.Start(browsingSession =>
            {
                // Request the root URL
                var fileName = Guid.NewGuid().ToString() + ".pdf";
                var siteDir = AppDomain.CurrentDomain.BaseDirectory;
                var filePath = Path.Combine(siteDir, "App_Data", fileName);
                RequestResult result = browsingSession.Get("/Home/TestSaveOnServer?fileName=" + fileName);
                var text = result.ResponseText;
                //MemoryStream memoryStream = new MemoryStream(0x10000);
                //using (Stream responseStream = result.Response.OutputStream)
                //{
                //    byte[] buffer = new byte[0x1000];
                //    int bytes;
                //    while ((bytes = responseStream.Read(buffer, 0, buffer.Length)) > 0)
                //    {
                //        memoryStream.Write(buffer, 0, bytes);
                //    }
                //}
                //var pdfBytes = memoryStream.ToArray();
                Assert.IsTrue(File.Exists(filePath));
                File.Delete(filePath);
            });
        }
    }
}
