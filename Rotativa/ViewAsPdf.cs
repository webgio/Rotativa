using System;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Rotativa
{
    public class ViewAsPdf : AsPdfResultBase
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

        public ViewAsPdf()
        {
            WkhtmltopdfPath = string.Empty;
            MasterName = string.Empty;
            ViewName = string.Empty;
            Model = null;
        }

        public ViewAsPdf(string viewName)
            : this()
        {
            ViewName = viewName;
        }

        public ViewAsPdf(object model)
            : this()
        {
            Model = model;
        }

        public ViewAsPdf(string viewName, object model)
            : this()
        {
            ViewName = viewName;
            Model = model;
        }

        public ViewAsPdf(string viewName, string masterName, object model)
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
            return ViewEngines.Engines.FindView(context, ViewName, MasterName);
        }

        protected override byte[] CallTheDriver(ControllerContext context)
        {
            context.Controller.ViewData.Model = Model;

            // use action name if the view name was not provided
            if (string.IsNullOrEmpty(ViewName))
                ViewName = context.RouteData.GetRequiredString("action");

            using (var sw = new StringWriter())
            {
                ViewEngineResult viewResult = GetView(context, ViewName, MasterName);

                // view not found, throw an exception with searched locations
                if (viewResult.View == null)
                {
                    var locations = new StringBuilder();
                    locations.AppendLine();

                    foreach (string location in viewResult.SearchedLocations)
                    {
                        locations.AppendLine(location);
                    }

                    throw new InvalidOperationException(string.Format("The view '{0}' or its master was not found, searched locations: {1}", ViewName, locations));
                }

                var viewContext = new ViewContext(context, viewResult.View, context.Controller.ViewData, context.Controller.TempData, sw);
                viewResult.View.Render(viewContext, sw);

                StringBuilder html = sw.GetStringBuilder();

                // replace href and src attributes with full URLs
                string baseUrl = string.Format("{0}://{1}", HttpContext.Current.Request.Url.Scheme, HttpContext.Current.Request.Url.Authority);
                html.Replace(" href=\"/", string.Format(" href=\"{0}/", baseUrl));
                html.Replace(" src=\"/", string.Format(" src=\"{0}/", baseUrl));

                var fileContent = WkhtmltopdfDriver.ConvertHtml(WkhtmltopdfPath, GetConvertOptions(), html.ToString());
                return fileContent;
            }
        }
    }
}