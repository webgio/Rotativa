using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Rotativa
{
    public abstract class AsPdfResultBase : ActionResult
    {
        private const string ContentType = "application/pdf";

        public string FileName { get; set; }
        public string WkhtmltopdfPath { get; set; }
        public string CookieName { get; set; }

        protected AsPdfResultBase()
        {
            this.WkhtmltopdfPath = string.Empty;
            this.CookieName = ".ASPXAUTH";
        }

        protected abstract string GetUrl(ControllerContext context);

        private string GetWkParams(ControllerContext context)
        {
            var switches = string.Empty;

            HttpCookie authenticationCookie = context.HttpContext.Request.Cookies[FormsAuthentication.FormsCookieName];
            if (authenticationCookie != null)
            {
                var authCookieValue = authenticationCookie.Value;
                switches += " --cookie " + CookieName + " " + authCookieValue;
            }

            var url = GetUrl(context);
            switches += " " + url;
            return switches;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            var response = PrepareResponse(context.HttpContext.Response);

            var switches = GetWkParams(context);

            if (this.WkhtmltopdfPath == "") this.WkhtmltopdfPath = HttpContext.Current.Server.MapPath("~/Rotativa");
            var fileContent = WkhtmltopdfDriver.Convert(this.WkhtmltopdfPath, switches);

            response.OutputStream.Write(fileContent, 0, fileContent.Length);
        }

        private HttpResponseBase PrepareResponse(HttpResponseBase response)
        {
            response.ContentType = ContentType;

            if (!String.IsNullOrEmpty(this.FileName))
            {
                // TODO: strip non US_ANSI chars from the filename
                response.AddHeader("Content-Disposition", "attachment; filename=" + this.FileName);
            }
            response.AddHeader("Content-Type", ContentType);
            return response;
        }
    }
}
