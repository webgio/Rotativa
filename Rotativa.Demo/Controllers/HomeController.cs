using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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
            return new ActionAsPdf("Index") { FileName = "Test.pdf" };
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
