using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using SharpTestsEx;

namespace Rotativa.Tests
{
    [TestFixture]
    public class RotativaTests
    {
        private IWebDriver selenium;
        private StringBuilder verificationErrors;

        [TestFixtureSetUp]
        public void SetupTest()
        {
            selenium = new InternetExplorerDriver();
            selenium.Manage().Timeouts().ImplicitlyWait(new TimeSpan(0, 0, 10));
            verificationErrors = new StringBuilder();
        }

        [SetUp]
        public void TestSetUp()
        {
            var rotativaDemoUrl = ConfigurationManager.AppSettings["RotativaDemoUrl"];
            selenium.Navigate().GoToUrl(rotativaDemoUrl);
        }

        [TestFixtureTearDown]
        public void FixtureTearDown()
        {
            if (selenium != null) selenium.Quit();
        }

        [Test]
        public void Is_the_site_reachable()
        {
            Assert.AreEqual("Home Page", selenium.Title);
        }
        
        [Test]
        public void Can_print_the_test_pdf()
        {

            var testLink = selenium.FindElement(By.LinkText("Test"));
            var pdfHref = testLink.GetAttribute("href");
            using (var wc = new WebClient())
            {
                var pdfResult = wc.DownloadData(new Uri(pdfHref));
                var pdfTester = new PdfTester();
                pdfTester.LoadPdf(pdfResult);
                pdfTester.PdfIsValid.Should().Be.True();
                pdfTester.PdfContains("My MVC Application").Should().Be.True();
            }
        }
        
        [Test]
        public void Can_print_the_authorized_pdf()
        {

            var testLink = selenium.FindElement(By.LinkText("Logged In Test"));
            var pdfHref = testLink.GetAttribute("href");
            testLink.Click();
            
            var username = selenium.FindElement(By.Id("UserName"));
            username.SendKeys("admin");
            var password = selenium.FindElement(By.Id("Password"));
            password.Clear();
            password.SendKeys("admin");
            password.Submit();
            var manage = selenium.Manage();
            var cookies = manage.Cookies.AllCookies;
            using (var wc = new WebClient())
            {
                foreach (var cookie in cookies)
                {
                    var cookieText = cookie.Name + "=" + cookie.Value;
                    wc.Headers.Add(HttpRequestHeader.Cookie, cookieText);
                }
                var pdfResult = wc.DownloadData(new Uri(pdfHref));
                var pdfTester = new PdfTester();
                pdfTester.LoadPdf(pdfResult);
                pdfTester.PdfIsValid.Should().Be.True();
                pdfTester.PdfContains("My MVC Application").Should().Be.True();
                pdfTester.PdfContains("admin").Should().Be.True();
            }
        }

        [Test]
        public void Can_print_the_pdf_from_a_view()
        {

            var testLink = selenium.FindElement(By.LinkText("Test View"));
            var pdfHref = testLink.GetAttribute("href");
            using (var wc = new WebClient())
            {
                var pdfResult = wc.DownloadData(new Uri(pdfHref));
                var pdfTester = new PdfTester();
                pdfTester.LoadPdf(pdfResult);
                pdfTester.PdfIsValid.Should().Be.True();
                pdfTester.PdfContains("My MVC Application").Should().Be.True();
            }
        }
        
        [Test]
        public void Can_print_the_pdf_from_a_vie_with_non_ascii_chars()
        {

            var testLink = selenium.FindElement(By.LinkText("Test View"));
            var pdfHref = testLink.GetAttribute("href");
            using (var wc = new WebClient())
            {
                var pdfResult = wc.DownloadData(new Uri(pdfHref));
                var pdfTester = new PdfTester();
                pdfTester.LoadPdf(pdfResult);
                pdfTester.PdfIsValid.Should().Be.True();
                pdfTester.PdfContains("àéù").Should().Be.True();
            }
        }

        [Test]
        public void Can_print_the_pdf_from_a_view_with_a_model()
        {

            var testLink = selenium.FindElement(By.LinkText("Test ViewAsPdf with a model"));
            var pdfHref = testLink.GetAttribute("href");
            var title = "This is a test";
            using (var wc = new WebClient())
            {
                var pdfResult = wc.DownloadData(new Uri(pdfHref));
                var pdfTester = new PdfTester();
                pdfTester.LoadPdf(pdfResult);
                pdfTester.PdfIsValid.Should().Be.True();
                pdfTester.PdfContains(title).Should().Be.True();
            }
        }

        [Test]
        public void Can_print_the_pdf_from_a_partial_view_with_a_model()
        {

            var testLink = selenium.FindElement(By.LinkText("Test PartialViewAsPdf with a model"));
            var pdfHref = testLink.GetAttribute("href");
            var content = "This is a test with a partial view";
            using (var wc = new WebClient())
            {
                var pdfResult = wc.DownloadData(new Uri(pdfHref));
                var pdfTester = new PdfTester();
                pdfTester.LoadPdf(pdfResult);
                pdfTester.PdfIsValid.Should().Be.True();
                pdfTester.PdfContains(content).Should().Be.True();
            }
        }
    }
}
