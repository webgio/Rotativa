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
        public Margins PageMargins { get; set; }

        /// <summary>
        /// Sets the page size.
        /// </summary>
        [OptionFlag("-s")]
        public Size? PageSize { get; set; }

        /// <summary>
        /// Sets the page width in mm.
        /// </summary>
        /// <remarks>Has priority over <see cref="PageSize"/> but <see cref="PageHeight"/> has to be also specified.</remarks>
        [OptionFlag("--page-width")]
        public double? PageWidth { get; set; }

        /// <summary>
        /// Sets the page height in mm.
        /// </summary>
        /// <remarks>Has priority over <see cref="PageSize"/> but <see cref="PageWidth"/> has to be also specified.</remarks>
        [OptionFlag("--page-height")]
        public double? PageHeight { get; set; }

        /// <summary>
        /// Sets the page orientation.
        /// </summary>
        [OptionFlag("-O")]
        public Orientation? PageOrientation { get; set; }

        /// <summary>
        /// Sets cookies.
        /// </summary>
        [OptionFlag("--cookie")]
        public Dictionary<string, string> Cookies { get; set; }

        /// <summary>
        /// Sets post values.
        /// </summary>
        [OptionFlag("--post")]
        public Dictionary<string, string> Post { get; set; }

        /// <summary>
        /// Indicates whether the page can run JavaScript.
        /// </summary>
        [OptionFlag("-n")]
        public bool IsJavaScriptDisabled { get; set; }

        /// <summary>
        /// Indicates whether the PDF should be generated in lower quality.
        /// </summary>
        [OptionFlag("-l")]
        public bool IsLowQuality { get; set; }

        /// <summary>
        /// Indicates whether the page background should be disabled.
        /// </summary>
        [OptionFlag("--no-background")]
        public bool IsBackgroundDisabled { get; set; }

        /// <summary>
        /// Minimum font size.
        /// </summary>
        [OptionFlag("--minimum-font-size")]
        public int? MinimumFontSize { get; set; }

        /// <summary>
        /// Number of copies to print into the PDF file.
        /// </summary>
        [OptionFlag("--copies")]
        public int? Copies { get; set; }

        /// <summary>
        /// Indicates whether the PDF should be generated in grayscale.
        /// </summary>
        [OptionFlag("-g")]
        public bool IsGrayScale { get; set; }

        /// <summary>
        /// Sets proxy server.
        /// </summary>
        [OptionFlag("-p")]
        public string Proxy { get; set; }

        /// <summary>
        /// HTTP Authentication username.
        /// </summary>
        [OptionFlag("--username")]
        public string UserName { get; set; }

        /// <summary>
        /// HTTP Authentication password.
        /// </summary>
        [OptionFlag("--password")]
        public string Password { get; set; }

        /// <summary>
        /// Use this if you need another switches that are not currently supported by Rotativa.
        /// </summary>
        [OptionFlag("")]
        public string CustomSwitches { get; set; }

        [Obsolete(@"Use BuildPdf(this.ControllerContext) method instead and use the resulting binary data to do what needed.")]
        public string SaveOnServerPath { get; set; }

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
            var result = new StringBuilder();

            if (PageMargins != null)
                result.Append(PageMargins.ToString());

            var fields = GetType().GetProperties();
            foreach (var fi in fields)
            {
                var of = fi.GetCustomAttributes(typeof(OptionFlag), true).FirstOrDefault() as OptionFlag;
                if (of == null)
                    continue;

                object value = fi.GetValue(this, null);
                if (value == null)
                    continue;

                if (fi.PropertyType == typeof(Dictionary<string, string>))
                {
                    var dictionary = (Dictionary<string, string>)value;
                    foreach (var d in dictionary)
                    {
                        result.AppendFormat(" {0} {1} {2}", of.Name, d.Key, d.Value);
                    }
                }
                else if (fi.PropertyType == typeof(bool))
                {
                    if ((bool)value)
                        result.AppendFormat(CultureInfo.InvariantCulture, " {0}", of.Name);
                }
                else
                {
                    result.AppendFormat(CultureInfo.InvariantCulture, " {0} {1}", of.Name, value);
                }
            }

            return result.ToString().Trim();
        }

        private string GetWkParams(ControllerContext context)
        {
            var switches = string.Empty;

            HttpCookie authenticationCookie = null;
            if (context.HttpContext.Request.Cookies != null && context.HttpContext.Request.Cookies.AllKeys.Contains(FormsAuthentication.FormsCookieName))
            {
                authenticationCookie = context.HttpContext.Request.Cookies[FormsAuthentication.FormsCookieName];
            }
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

        protected virtual byte[] CallTheDriver(ControllerContext context)
        {
            var switches = GetWkParams(context);
            var fileContent = WkhtmltopdfDriver.Convert(WkhtmltopdfPath, switches);
            return fileContent;
        }

        public byte[] BuildPdf(ControllerContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            if (WkhtmltopdfPath == string.Empty)
                WkhtmltopdfPath = HttpContext.Current.Server.MapPath("~/Rotativa");

            var fileContent = CallTheDriver(context);

            if (string.IsNullOrEmpty(SaveOnServerPath) == false)
            {
                File.WriteAllBytes(SaveOnServerPath, fileContent);
            }

            return fileContent;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            var fileContent = BuildPdf(context);

            var response = PrepareResponse(context.HttpContext.Response);

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
                response.AddHeader("Content-Disposition", string.Format("attachment; filename=\"{0}\"", SanitizeFileName(FileName)));

            response.AddHeader("Content-Type", ContentType);

            return response;
        }
    }
}