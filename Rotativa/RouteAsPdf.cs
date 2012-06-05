﻿using System;
using System.Web.Mvc;
using System.Web.Routing;

namespace Rotativa
{
    public class RouteAsPdf: AsPdfResultBase
    {
        private RouteValueDictionary routeValuesDict;
        private object routeValues;
        private string routeName;

        public RouteAsPdf(string routeName)
            : base()
        {
            this.routeName = routeName;
        }

        public RouteAsPdf(string routeName, RouteValueDictionary routeValues)
            : this(routeName)
        {
            this.routeValuesDict = routeValues;
        }

        public RouteAsPdf(string routeName, object routeValues)
            : this(routeName)
        {
            this.routeValues = routeValues;
        }

        protected override string GetUrl(ControllerContext context)
        {
            var urlHelper = new UrlHelper(context.RequestContext);

            string actionUrl = string.Empty;
            if (this.routeValues == null)
                actionUrl = urlHelper.RouteUrl(this.routeName, this.routeValuesDict);
            else if (this.routeValues != null)
                actionUrl = urlHelper.RouteUrl(this.routeName, this.routeValues);
            else
                actionUrl = urlHelper.RouteUrl(this.routeName);

            string url = String.Format("{0}://{1}{2}", context.HttpContext.Request.Url.Scheme, context.HttpContext.Request.Url.Authority, actionUrl);
            return url;
        }
    }
}
