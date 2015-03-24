namespace Rotativa.Extensions
{
    using System;
    using System.IO;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;

    public static class ControllerContextExtensions
    {
        public static string GetHtmlFromView(this ControllerContext context, ViewEngineResult viewResult, string viewName, object model)
        {
            context.Controller.ViewData.Model = model;
            using (StringWriter sw = new StringWriter())
            {
                // view not found, throw an exception with searched locations
                if (viewResult.View == null)
                {
                    var locations = new StringBuilder();
                    locations.AppendLine();

                    foreach (string location in viewResult.SearchedLocations)
                    {
                        locations.AppendLine(location);
                    }

                    throw new InvalidOperationException(
                        string.Format(
                            "The view '{0}' or its master was not found, searched locations: {1}", viewName, locations));
                }

                ViewContext viewContext = new ViewContext(context, viewResult.View, context.Controller.ViewData, context.Controller.TempData, sw);
                viewResult.View.Render(viewContext, sw);

                string html = sw.GetStringBuilder().ToString();
                string baseUrl = string.Format("{0}://{1}", HttpContext.Current.Request.Url.Scheme, HttpContext.Current.Request.Url.Authority);
                html = Regex.Replace(html, "<head>", string.Format("<head><base href=\"{0}\" />", baseUrl), RegexOptions.IgnoreCase);
                return html;
            }
        }
    }
}
