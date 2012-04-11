using System;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Rotativa
{
    public class ViewAsPdf : ActionResult
    {
        private const string ContentType = "application/pdf";

        public string FileName { get; set; }
        public string WkhtmltopdfPath { get; set; }

        private readonly Controller _controller;
        private readonly string _viewName;
        private readonly string _masterName;
        private readonly object _model;

        public ViewAsPdf(Controller controller, string viewName, string masterName, object model)
        {
            WkhtmltopdfPath = string.Empty;

            _controller = controller;
            _viewName = viewName;
            _masterName = masterName;
            _model = model;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            var response = PrepareResponse(context.HttpContext.Response);

            if (WkhtmltopdfPath == string.Empty)
                WkhtmltopdfPath = HttpContext.Current.Server.MapPath("~/Rotativa");

            _controller.ViewData.Model = _model;

            using (var sw = new StringWriter())
            {
                ViewEngineResult viewResult = ViewEngines.Engines.FindView(_controller.ControllerContext, _viewName, _masterName);
                ViewContext viewContext = new ViewContext(_controller.ControllerContext, viewResult.View, _controller.ViewData, _controller.TempData, sw);
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

        private HttpResponseBase PrepareResponse(HttpResponseBase response)
        {
            response.ContentType = ContentType;

            if (!String.IsNullOrEmpty(FileName))
            {
                // TODO: strip non US_ANSI chars from the filename
                response.AddHeader("Content-Disposition", "attachment; filename=" + FileName);
            }

            response.AddHeader("Content-Type", ContentType);

            return response;
        }
    }
}
