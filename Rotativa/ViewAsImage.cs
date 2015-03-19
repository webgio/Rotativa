using System.Web.Mvc;
using Rotativa.Extensions;

namespace Rotativa
{
    public class ViewAsImage : AsImageResultBase
    {
        private string _viewName;

        public string ViewName
        {
            get { return _viewName ?? string.Empty; }
            set { _viewName = value; }
        }

        private string _masterName;

        public string MasterName
        {
            get { return _masterName ?? string.Empty; }
            set { _masterName = value; }
        }

        public object Model { get; set; }

        public ViewAsImage()
        {
            this.WkhtmlPath = string.Empty;
            MasterName = string.Empty;
            ViewName = string.Empty;
            Model = null;
        }

        public ViewAsImage(string viewName)
            : this()
        {
            ViewName = viewName;
        }

        public ViewAsImage(object model)
            : this()
        {
            Model = model;
        }

        public ViewAsImage(string viewName, object model)
            : this()
        {
            ViewName = viewName;
            Model = model;
        }

        public ViewAsImage(string viewName, string masterName, object model)
            : this(viewName, model)
        {
            MasterName = masterName;
        }

        protected override string GetUrl(ControllerContext context)
        {
            return string.Empty;
        }

        protected virtual ViewEngineResult GetView(ControllerContext context, string viewName, string masterName)
        {
            return ViewEngines.Engines.FindView(context, viewName, masterName);
        }

        protected override byte[] CallTheDriver(ControllerContext context)
        {
            // use action name if the view name was not provided
            string viewName = ViewName;
            if (string.IsNullOrEmpty(viewName))
                viewName = context.RouteData.GetRequiredString("action");

            ViewEngineResult viewResult = GetView(context, viewName, MasterName);
            string html = context.GetHtmlFromView(viewResult, viewName, Model);
            byte[] fileContent = WkhtmltoimageDriver.ConvertHtml(this.WkhtmlPath, this.GetConvertOptions(), html);
            return fileContent;
        }
    }
}