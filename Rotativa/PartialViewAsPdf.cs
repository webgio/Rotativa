using System;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Rotativa
{
    public class PartialViewAsPdf : ViewAsPdf
    {
        public PartialViewAsPdf()
        {
        }

        public PartialViewAsPdf(string partialViewName)
            : base(partialViewName)
        {
        }

        public PartialViewAsPdf(object model)
            : base(model)
        {
        }

        public PartialViewAsPdf(string partialViewName, object model)
            : base(partialViewName, model)
        {
        }
        
        protected override ViewEngineResult GetView(ControllerContext context, string viewName, string masterName)
        {
            return ViewEngines.Engines.FindPartialView(context, ViewName);
        }
    }
}