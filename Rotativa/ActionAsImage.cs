using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Rotativa
{
    public class ActionAsImage : AsImageResultBase
    {
        private RouteValueDictionary routeValuesDict;
        private object routeValues;
        private string action;

        public ActionAsImage(string action)
        {
            this.action = action;
        }

        public ActionAsImage(string action, RouteValueDictionary routeValues)
            : this(action)
        {
            this.routeValuesDict = routeValues;
        }

        public ActionAsImage(string action, object routeValues)
            : this(action)
        {
            this.routeValues = routeValues;
        }

        protected override string GetUrl(HttpContext context)
        {
            var urlHelper = new UrlHelper(context.Request.RequestContext);

            string actionUrl = string.Empty;
            if (this.routeValues == null)
                actionUrl = urlHelper.Action(this.action, this.routeValuesDict);
            else if (this.routeValues != null)
                actionUrl = urlHelper.Action(this.action, this.routeValues);
            else
                actionUrl = urlHelper.Action(this.action);

            string url = String.Format("{0}://{1}{2}", context.Request.Url.Scheme, context.Request.Url.Authority, actionUrl);
            return url;
        }
    }
}
