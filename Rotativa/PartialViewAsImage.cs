using System.Web.Mvc;

namespace Rotativa
{
    public class PartialViewAsImage : ViewAsImage
    {
        public PartialViewAsImage()
        {
        }

        public PartialViewAsImage(string partialViewName)
            : base(partialViewName)
        {
        }

        public PartialViewAsImage(object model)
            : base(model)
        {
        }

        public PartialViewAsImage(string partialViewName, object model)
            : base(partialViewName, model)
        {
        }

        protected override ViewEngineResult GetView(ControllerContext context, string viewName, string masterName)
        {
            return ViewEngines.Engines.FindPartialView(context, viewName);
        }
    }
}
