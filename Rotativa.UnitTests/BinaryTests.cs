﻿using System.Drawing;
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
        [Test]
        public void Can_build_the_pdf_binary()
        {
            var localPath = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory);
            var solutionDir = localPath.Parent.Parent.Parent.FullName;
            var wkhtmltopdfPath = Path.Combine(solutionDir, "Rotativa", "Rotativa");
            var actionResult = new UrlAsPdf("https://github.com/webgio/Rotativa")
                {
                    WkhtmltopdfPath = wkhtmltopdfPath
                }; 
            var builder = new TestControllerBuilder();
            var controller = new HomeController();
            builder.InitializeController(controller);
            var pdfBinary = actionResult.BuildPdf();
            var pdfTester = new PdfTester();
            pdfTester.LoadPdf(pdfBinary);
            pdfTester.PdfIsValid.Should().Be.True();
            pdfTester.PdfContains("Rotativa").Should().Be.True();
        }

        [Test]
        public void Can_build_the_image_binary()
        {
            var localPath = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory);
			var solutionDir = localPath.Parent.Parent.Parent.FullName;
            var wkhtmltoimagePath = Path.Combine(solutionDir, "Rotativa", "Rotativa");
            var actionResult = new UrlAsImage("https://github.com/webgio/Rotativa")
            {
                WkhtmlPath = wkhtmltoimagePath
            };
            var builder = new TestControllerBuilder();
            var controller = new HomeController();
            builder.InitializeController(controller);
            var imageBinary = actionResult.BuildFile(controller.ControllerContext);

            var image = Image.FromStream(new MemoryStream(imageBinary));
            image.Should().Not.Be.Null();
            image.RawFormat.Should().Be.EqualTo(ImageFormat.Jpeg);
        }
    }
}
