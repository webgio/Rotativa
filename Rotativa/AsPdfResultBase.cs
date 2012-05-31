using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Rotativa.Options;

namespace Rotativa
{
    public abstract class AsPdfResultBase : ActionResult
    {
        private const string ContentType = "application/pdf";

        /// <summary>
        /// This will be send to the browser as a name of the generated PDF file.
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Path to wkhtmltopdf binary.
        /// </summary>
        public string WkhtmltopdfPath { get; set; }

        /// <summary>
        /// Custom name of authentication cookie used by forms authentication.
        /// </summary>
        [Obsolete("Use FormsAuthenticationCookieName instead of CookieName.")]
        public string CookieName
        {
            get { return FormsAuthenticationCookieName; }
            set { FormsAuthenticationCookieName = value; }
        }

        /// <summary>
        /// Custom name of authentication cookie used by forms authentication.
        /// </summary>
        public string FormsAuthenticationCookieName { get; set; }

        /// <summary>
        /// Sets the page margins.
        /// </summary>
        public Margins PageMargins;

        /// <summary>
        /// Sets the page size.
        /// </summary>
        [OptionFlag("-s")]
        public Size? PageSize;

        /// <summary>
        /// Sets the page width in mm. Has priority over <see cref="PageSize"/> but <see cref="PageHeight"/> has to be also specified.
        /// </summary>
        [OptionFlag("--page-width")]
        public double? PageWidth;

        /// <summary>
        /// Sets the page height in mm. Has priority over <see cref="PageSize"/> but <see cref="PageWidth"/> has to be also specified.
        /// </summary>
        [OptionFlag("--page-height")]
        public double? PageHeight;

        /// <summary>
        /// Sets the page orientation.
        /// </summary>
        [OptionFlag("-O")]
        public Orientation? PageOrientation;

        /// <summary>
        /// Sets cookies.
        /// </summary>
        [OptionFlag("--cookie")]
        public Dictionary<string, string> Cookies;

        /// <summary>
        /// Sets post values.
        /// </summary>
        [OptionFlag("--post")]
        public Dictionary<string, string> Post;

        /// <summary>
        /// Indicates whether the page can run JavaScript.
        /// </summary>
        [OptionFlag("-n")]
        public bool IsJavaScriptDisabled;

        /// <summary>
        /// Indicates whether the PDF should be generated in lower quality.
        /// </summary>
        [OptionFlag("-l")]
        public bool IsLowQuality;

        /// <summary>
        /// Indicates whether the page background should be disabled.
        /// </summary>
        [OptionFlag("--no-background")]
        public bool IsBackgroundDisabled;

        /// <summary>
        /// Minimum font size.
        /// </summary>
        [OptionFlag("--minimum-font-size")]
        public int? MinimumFontSize;

        /// <summary>
        /// Number of copies to print into the PDF file.
        /// </summary>
        [OptionFlag("--copies")]
        public int? Copies;

        /// <summary>
        /// Indicates whether the PDF should be generated in grayscale.
        /// </summary>
        [OptionFlag("-g")]
        public bool IsGrayScale;

        /// <summary>
        /// Sets proxy server.
        /// </summary>
        [OptionFlag("-p")]
        public string Proxy;

        /// <summary>
        /// HTTP Authentication username.
        /// </summary>
        [OptionFlag("--username")]
        public string UserName;

        /// <summary>
        /// HTTP Authentication password.
        /// </summary>
        [OptionFlag("--password")]
        public string Password;

        /// <summary>
        /// Use this if you need another switches that are not currently supported by Rotativa.
        /// </summary>
        [OptionFlag("")]
        public string CustomSwitches;

        protected AsPdfResultBase()
        {
            WkhtmltopdfPath = string.Empty;
            FormsAuthenticationCookieName = ".ASPXAUTH";
            PageMargins = new Margins();
        }

        protected abstract string GetUrl(ControllerContext context);

        /// <summary>
        /// Returns properties with OptionFlag attribute as one line that can be passed to wkhtmltopdf binary.
        /// </summary>
        /// <returns>Command line parameter that can be directly passed to wkhtmltopdf binary.</returns>
        protected string GetConvertOptions()
        {
            // use en-US locale when converting floating-point numbers to string
            var ci = new CultureInfo("en-US", false);

            var result = new StringBuilder();

            if (PageMargins != null)
                result.Append(PageMargins.ToString());

            FieldInfo[] fields = GetType().GetFields();
            foreach (var fi in fields)
            {
                var of = fi.GetCustomAttributes(typeof(OptionFlag), true).FirstOrDefault() as OptionFlag;
                if (of == null)
                    continue;

                object value = fi.GetValue(this);
                if (value == null)
                    continue;

                if (fi.FieldType == typeof(Dictionary<string, string>))
                {
                    var dictionary = (Dictionary<string, string>)value;
                    foreach (var d in dictionary)
                    {
                        result.AppendFormat(" {0} {1} {2}", of.Name, d.Key, d.Value);
                    }
                }
                else if (fi.FieldType == typeof(bool))
                {
                    if ((bool)value)
                        result.AppendFormat(ci, " {0}", of.Name);
                }
                else
                {
                    result.AppendFormat(ci, " {0} {1}", of.Name, value);
                }
            }

            return result.ToString().Trim();
        }

        private string GetWkParams(ControllerContext context)
        {
            var switches = string.Empty;

            HttpCookie authenticationCookie = context.HttpContext.Request.Cookies[FormsAuthentication.FormsCookieName];
            if (authenticationCookie != null)
            {
                var authCookieValue = authenticationCookie.Value;
                switches += " --cookie " + FormsAuthenticationCookieName + " " + authCookieValue;
            }

            switches += " " + GetConvertOptions();

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

            if (WkhtmltopdfPath == string.Empty)
                WkhtmltopdfPath = HttpContext.Current.Server.MapPath("~/Rotativa");

            var fileContent = WkhtmltopdfDriver.Convert(WkhtmltopdfPath, switches);

            response.OutputStream.Write(fileContent, 0, fileContent.Length);
        }

        private static string SanitizeFileName(string name)
        {
            string invalidChars = Regex.Escape(new string(Path.GetInvalidPathChars()) + new string(Path.GetInvalidFileNameChars()));
            string invalidCharsPattern = string.Format(@"[{0}]+", invalidChars);

            string result = Regex.Replace(name, invalidCharsPattern, "_");
            return result;
        }

        protected HttpResponseBase PrepareResponse(HttpResponseBase response)
        {
            response.ContentType = ContentType;

            if (!String.IsNullOrEmpty(FileName))
                response.AddHeader("Content-Disposition", "attachment; filename=" + SanitizeFileName(FileName));

            response.AddHeader("Content-Type", ContentType);

            return response;
        }
    }
}