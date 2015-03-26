using System;
using System.Web;
using System.Web.Mvc;

namespace Rotativa
{
    public class UrlAsImage : AsImageResultBase
    {
        private readonly string _url;

        public UrlAsImage(string url)
        {
            _url = url ?? string.Empty;
        }

        protected override string GetUrl(HttpContext context)
        {
            string urlLower = _url.ToLower();
            if (urlLower.StartsWith("http://") || urlLower.StartsWith("https://"))
                return _url;

            string url = String.Format("{0}://{1}{2}", context.Request.Url.Scheme, context.Request.Url.Authority, _url);
            return url;
        }
    }
}
