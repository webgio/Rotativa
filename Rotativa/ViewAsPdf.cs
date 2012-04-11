using System;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Rotativa
{
    public class ViewAsPdf : AsPdfResultBase
    {
        private readonly string _viewName;
        private readonly string _masterName;
        private readonly object _model;

        public ViewAsPdf(string viewName, object model)
        {
            WkhtmltopdfPath = string.Empty;
            _masterName = null;

            _viewName = viewName;
            _model = model;
        }

        public ViewAsPdf(string viewName, string masterName, object model)
            : this(viewName, model)
        {
            _masterName = masterName;
        }

        protected override string GetUrl(ControllerContext context)
        {
            return string.Empty;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            var response = PrepareResponse(context.HttpContext.Response);

            if (WkhtmltopdfPath == string.Empty)
                WkhtmltopdfPath = HttpContext.Current.Server.MapPath("~/Rotativa");

            context.Controller.ViewData.Model = _model;

            using (var sw = new StringWriter())
            {
                ViewEngineResult viewResult = ViewEngines.Engines.FindView(context, _viewName, _masterName);
                ViewContext viewContext = new ViewContext(context, viewResult.View, context.Controller.ViewData, context.Controller.TempData, sw);
                viewResult.View.Render(viewContext, sw);

                StringBuilder html = sw.GetStringBuilder();

                // replace href and src attributes with full URLs
                string baseUrl = string.Format("{0}://{1}", HttpContext.Current.Request.Url.Scheme, HttpContext.Current.Request.Url.Authority);
                html.Replace(" href=\"/", string.Format(" href=\"{0}/", baseUrl));
                html.Replace(" src=\"/", string.Format(" src=\"{0}/", baseUrl));

                var fileContent = WkhtmltopdfDriver.ConvertHtml(WkhtmltopdfPath, html.ToString());
                response.OutputStream.Write(fileContent, 0, fileContent.Length);
            }
        }
    }
}
