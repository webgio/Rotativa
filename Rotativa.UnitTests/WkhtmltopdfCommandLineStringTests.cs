using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Rotativa.Options;
using SharpTestsEx;

namespace Rotativa.UnitTests
{
    public class TestAsPdfResult: AsPdfResultBase
    {
        public string GetConvertOptionsValue()
        {
            return base.GetConvertOptions();
        }
        protected override string GetUrl(System.Web.Mvc.ControllerContext context)
        {
            return string.Empty;
        }
    }
    [TestFixture]
    public class WkhtmltopdfCommandLineStringTests
    {
        [Test]
        public void WhenAPdfActioResultHasSetOptionsAsProperties_TheResultingCommandLineHasTheCorrectOptions()
        {
            var pdfResult = new TestAsPdfResult();
            var post = new Dictionary<string, string>();
            post.Add("param1", "value1");
            post.Add("param2", "value2");
            pdfResult.Post = post;
            pdfResult.PageOrientation = Orientation.Landscape;
            var commandlineOptions = pdfResult.GetConvertOptionsValue();
            commandlineOptions.Should().Contain("--post param1 value1 --post param2 value2");
            commandlineOptions.Should().Contain("-O Landscape");
        }
    }
}
