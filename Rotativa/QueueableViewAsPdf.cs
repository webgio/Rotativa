using System.Web;
using System.Web.Mvc;
using Rotativa.Extensions;

namespace Rotativa
{
    public class QueueableViewAsPdf : AsPdfResultBase
    {
        private readonly ControllerContext _context;
        private readonly object _model;
        private string _viewName;
        private readonly string _masterName;

        public string ViewHtmlString { get; private set; }

        public QueueableViewAsPdf(ControllerContext context)
        {
            _context = context;

            ViewHtmlString = GetHtmlFromView();
        }

        public QueueableViewAsPdf(ControllerContext context, string viewName)
        {
            _context = context;
            _viewName = viewName;

            ViewHtmlString = GetHtmlFromView();
        }

        public QueueableViewAsPdf(ControllerContext context, object model)
        {
            _context = context;
            _model = model;

            ViewHtmlString = GetHtmlFromView();
        }

        public QueueableViewAsPdf(ControllerContext context, string viewName, object model)
        {
            _context = context;
            _viewName = viewName;
            _model = model;

            ViewHtmlString = GetHtmlFromView();
        }

        public QueueableViewAsPdf(ControllerContext context, string viewName, string masterName, object model)
        {
            _context = context;
            _viewName = viewName;
            _masterName = masterName;
            _model = model;
            
            ViewHtmlString = GetHtmlFromView();
        }

        private string GetHtmlFromView()
        {
            this.WkhtmlPath = HttpContext.Current.Server.MapPath("~/Rotativa");

            if (string.IsNullOrEmpty(_viewName))
                _viewName = _context.RouteData.GetRequiredString("action");

            var viewResult = ViewEngines.Engines.FindView(_context, _viewName, _masterName);
            var html = _context.GetHtmlFromView(viewResult, _viewName, _model);

            return html;
        }

        public byte[] BuildFile()
        {
            var fileContent = WkhtmltopdfDriver.ConvertHtml(this.WkhtmlPath, this.GetConvertOptions(), ViewHtmlString);
            return fileContent;
        }

        protected override string GetUrl(ControllerContext context)
        {
            return string.Empty;
        }
    }
}