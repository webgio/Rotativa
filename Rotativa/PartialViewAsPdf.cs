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

        protected override byte[] CallTheDriver(ControllerContext context)
        {
            context.Controller.ViewData.Model = Model;

            // use action name if the view name was not provided
            if (string.IsNullOrEmpty(ViewName))
                ViewName = context.RouteData.GetRequiredString("action");

            using (var sw = new StringWriter())
            {
                ViewEngineResult partialViewResult = ViewEngines.Engines.FindPartialView(context, ViewName);

                // view not found, throw an exception with searched locations
                if (partialViewResult.View == null)
                {
                    var locations = new StringBuilder();
                    locations.AppendLine();

                    foreach (string location in partialViewResult.SearchedLocations)
                    {
                        locations.AppendLine(location);
                    }

                    throw new InvalidOperationException(
                        string.Format("The partial view '{0}' was not found, searched locations: {1}", ViewName,
                                      locations));
                }

                var viewContext = new ViewContext(context, partialViewResult.View, context.Controller.ViewData,
                                                  context.Controller.TempData, sw);
                partialViewResult.View.Render(viewContext, sw);

                StringBuilder html = sw.GetStringBuilder();

                // replace href and src attributes with full URLs
                string baseUrl = string.Format("{0}://{1}", HttpContext.Current.Request.Url.Scheme,
                                               HttpContext.Current.Request.Url.Authority);
                html.Replace(" href=\"/", string.Format(" href=\"{0}/", baseUrl));
                html.Replace(" src=\"/", string.Format(" src=\"{0}/", baseUrl));

                var fileContent = WkhtmltopdfDriver.ConvertHtml(WkhtmltopdfPath, GetConvertOptions(), html.ToString());
                return fileContent;
            }
        }
    }
}