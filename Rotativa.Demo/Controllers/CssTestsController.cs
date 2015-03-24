using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Rotativa.Demo.Controllers
{
    public class CssTestsController : Controller
    {
        //
        // GET: /CssTests/

        public ActionResult Index()
        {
            return new ViewAsPdf();
        }

         public ActionResult IndexImage()
        {
            return new ViewAsImage("Index");
        }
    }
}
