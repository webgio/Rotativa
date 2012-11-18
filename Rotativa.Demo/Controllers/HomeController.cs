using System;
using System.IO;
using System.Web.Mvc;
using Rotativa.Demo.Models;
using Rotativa.Options;

namespace Rotativa.Demo.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index(string name)
        {
            ViewBag.Message = string.Format("Hello {0} to ASP.NET MVC!", name);

            return View();
        }

        [Authorize]
        public ActionResult AuthorizedIndex()
        {
            ViewBag.Message = "Welcome to logged in ASP.NET MVC!";

            return View("Index");
        }

        public ActionResult Test()
        {
            return new ActionAsPdf("Index", new { name = "Giorgio" }) { FileName = "Test.pdf" };
        }

        public ActionResult TestUrl()
        {
            // Now I realize that this isn't very expressive example of why this can be useful.
            // However imagine that you have your own UrlHelper extensions like UrlHelper.User(...)
            // where you create correct URL according to passed conditions, prepare some complex model, etc.

            var urlHelper = new UrlHelper(Request.RequestContext);
            string url = urlHelper.Action("Index", new { name = "Giorgio II." });

            return new UrlAsPdf(url) { FileName = "TestUrl.pdf" };
        }

        public ActionResult TestExternalUrl()
        {
            // In some cases you might want to pull completely different URL that is not related to your application.
            // You can do that by specifying full URL.

            return new UrlAsPdf("http://www.github.com")
                       {
                           FileName = "TestExternalUrl.pdf",
                           PageMargins = new Margins(0, 0, 0, 0)
                       };
        }

        public ActionResult TestView()
        {
            // The more usual way of using this would be to have a Model object that you would pass into ViewAsPdf
            // and work with that Model inside your View.
            // Good example could be an Order Summary page on some fictional E-shop.

            // Probably the biggest advantage of this approach is that you have Session object available.

            ViewBag.Message = string.Format("Hello {0} to ASP.NET MVC!", "Giorgio III.");
            return new ViewAsPdf("Index")
            {
                FileName = "TestView.pdf",
                PageSize = Size.A3,
                PageOrientation = Orientation.Landscape,
                PageMargins = { Left = 0, Right = 0 }
            };
        }

        public ActionResult TestSaveOnServer(string fileName)
        {
            // The more usual way of using this would be to have a Model object that you would pass into ViewAsPdf
            // and work with that Model inside your View.
            // Good example could be an Order Summary page on some fictional E-shop.

            // Probably the biggest advantage of this approach is that you have Session object available.
            var filePath = Path.Combine(Server.MapPath("/App_Data"), fileName);
            ViewBag.Message = string.Format("Hello {0} to ASP.NET MVC!", "Giorgio III.");
            return new ViewAsPdf("Index")
            {
                FileName = fileName,
                PageSize = Size.A3,
                PageOrientation = Orientation.Landscape,
                PageMargins = { Left = 0, Right = 0 },
                SaveOnServerPath = filePath
            };
        }

        public ActionResult TestViewWithModel(string id)
        {
            var model = new TestViewModel {DocTitle = id, DocContent = "This is a test"};
            return new ViewAsPdf(model);
        }

        public ActionResult TestPartialViewWithModel(string id)
        {
            var model = new TestViewModel { DocTitle = id, DocContent = "This is a test" };
            return new PartialViewAsPdf(model);
        }

        public ActionResult ErrorTest()
        {
            return new ActionAsPdf("SomethingBad") { FileName = "Test.pdf" };
        }

        public ActionResult SomethingBad()
        {
            return Redirect("http://thisdoesntexists");
        }


        [Authorize]
        public ActionResult AuthorizedTest()
        {
            return new ActionAsPdf("AuthorizedIndex") { FileName = "AuthorizedTest.pdf" };
        }

        public ActionResult RouteTest()
        {
            return new RouteAsPdf("TestRoute", new {name = "Giorgio"}) { FileName = "Test.pdf" };
        }
    }
}
