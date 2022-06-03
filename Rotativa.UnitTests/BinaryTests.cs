using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Web.Mvc;
using MvcContrib.TestHelper;
using NUnit.Framework;
using Rotativa;
using Rotativa.Demo.Controllers;
using Rotativa.Tests;
using SharpTestsEx;

namespace Rotativa.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using MvcContrib.TestHelper.Fakes;

    /// <summary>
    /// Testing for binary generation
    /// </summary>
    [TestFixture]
    public class BinaryTests
    {
        private const string TestUrl = "https://github.com/webgio/Rotativa";

        [Test]
        public void Can_build_the_pdf_binary()
        {
            //Arrange
            var actionResult = CreatePdfActionResult();
            var controller = CreateTestController();

            //Act
            var pdfBinary = actionResult.BuildFile(controller.ControllerContext);

            //Assert
            var pdfTester = new PdfTester();
            pdfTester.LoadPdf(pdfBinary);
            pdfTester.PdfIsValid.Should().Be.True();
            pdfTester.PdfContains("Rotativa").Should().Be.True();
        }

        [Test]
        public void Failed_to_build_the_pdf_binary_when_timeout_exceed()
        {
            //Arrange
            var actionResult = CreatePdfActionResult(convertTimeout: 1);
            var controller = CreateTestController();
            void TestMethod()
            {
                actionResult.BuildFile(controller.ControllerContext);
            }
            //Act

            //Assert
            Assert.Throws<TimeoutException>(TestMethod);

        }

        [Test]
        public void Can_build_the_image_binary()
        {
            //Arrange
            var actionResult = CreateImageActionResult();
            var controller = CreateTestController();

            //Act
            var imageBinary = actionResult.BuildFile(controller.ControllerContext);

            //Assert
            var image = Image.FromStream(new MemoryStream(imageBinary));
            image.Should().Not.Be.Null();
            image.RawFormat.Should().Be.EqualTo(ImageFormat.Jpeg);
        }

        [Test]
        public void Failed_to_build_the_image_binary_when_timeout_exceed()
        {
            //Arrange
            var actionResult = CreateImageActionResult(convertTimeout: 1);
            var controller = CreateTestController();
            void TestMethod()
            {
                actionResult.BuildFile(controller.ControllerContext);
            }
            //Act

            //Assert
            Assert.Throws<TimeoutException>(TestMethod);
        }

        private static AsImageResultBase CreateImageActionResult(string url = TestUrl, int? convertTimeout = null)
            => new UrlAsImage(url)
            {
                WkhtmlPath = GetWkhtmlPath(),
                ConvertTimeout = convertTimeout,
            };

        private static AsPdfResultBase CreatePdfActionResult(string url = TestUrl, int? convertTimeout = null)
            => new UrlAsPdf(url)
            {
                WkhtmlPath = GetWkhtmlPath(),
                ConvertTimeout = convertTimeout,
            };

        private static string GetWkhtmlPath()
        {
            var localPath = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory);
            var solutionDir = localPath.Parent.Parent.Parent.FullName;
            var wkhtmlPath = Path.Combine(solutionDir, "Rotativa", "Rotativa");
            return wkhtmlPath;
        }

        private static Controller CreateTestController()
        {
            var builder = new TestControllerBuilder();
            var controller = new HomeController();
            builder.InitializeController(controller);
            return controller;
        }
    }
}
