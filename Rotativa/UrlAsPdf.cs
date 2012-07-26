using System;
using System.Web.Mvc;

namespace Rotativa
{
    public class UrlAsPdf : AsPdfResultBase
    {
        private readonly string _url;

        public UrlAsPdf(string url)
        {
            _url = url ?? string.Empty;
        }

        protected override string GetUrl(ControllerContext context)
        {
            string urlLower = _url.ToLower();
            if (urlLower.StartsWith("http://") || urlLower.StartsWith("https://"))
                return _url;

            string url = String.Format("{0}://{1}{2}", context.HttpContext.Request.Url.Scheme, context.HttpContext.Request.Url.Authority, _url);
            return url;
        }
    }
}