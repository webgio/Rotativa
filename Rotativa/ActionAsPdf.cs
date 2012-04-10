using System;
using System.Web.Mvc;
using System.Web.Routing;

namespace Rotativa
{
    public class ActionAsPdf : AsPdfResultBase
    {
        private RouteValueDictionary routeValuesDict;
        private object routeValues;
        private string action;

        public ActionAsPdf(string action)
            : base()
        {
            this.action = action;
        }

        public ActionAsPdf(string action, RouteValueDictionary routeValues)
            : this(action)
        {
            this.routeValuesDict = routeValues;
        }

        public ActionAsPdf(string action, object routeValues)
            : this(action)
        {
            this.routeValues = routeValues;
        }

        protected override string GetUrl(ControllerContext context)
        {
            var urlHelper = new UrlHelper(context.RequestContext);

            string actionUrl = string.Empty;
            if (this.routeValues == null)
                actionUrl = urlHelper.Action(this.action, this.routeValuesDict);
            else if (this.routeValues != null)
                actionUrl = urlHelper.Action(this.action, this.routeValues);
            else
                actionUrl = urlHelper.Action(this.action);

            string url = String.Format("{0}://{1}{2}", context.HttpContext.Request.Url.Scheme, context.HttpContext.Request.Url.Authority, actionUrl);
            return url;
        }
    }
}
