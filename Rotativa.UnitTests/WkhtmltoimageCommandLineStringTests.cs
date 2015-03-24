using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Rotativa.Options;
using SharpTestsEx;

namespace Rotativa.UnitTests
{
    public class WkhtmltoimageCommandLineStringTests
    {
        public class TestAsImageResult : AsImageResultBase
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
                var imageResult = new TestAsImageResult();
                var post = new Dictionary<string, string>();
                post.Add("param1", "value1");
                post.Add("param2", "value2");
                imageResult.Post = post;
                imageResult.Format = ImageFormat.png;
                var commandlineOptions = imageResult.GetConvertOptionsValue();
                commandlineOptions.Should().Contain("--post param1 value1 --post param2 value2");
                commandlineOptions.Should().Contain("-f png");
            }
        }
    }
}
