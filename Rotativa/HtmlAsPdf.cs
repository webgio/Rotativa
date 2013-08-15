using System;
using System.IO;
using System.Web;
using System.Web.Mvc;

namespace Rotativa
{
    /// <summary>
    /// Allow a developer to render HTML as a PDF
    /// </summary>
    public class HtmlAsPdf : AsPdfResultBase
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public HtmlAsPdf()
        {
            WkhtmltopdfPath = String.Empty;
        }

        /// <summary>
        /// Returns an empty string, as Wkhtmltopdf will receive our HTML directly.
        /// </summary>
        /// <returns>An empty string.</returns>
        protected override string GetUrl(ControllerContext context)
        {
            return String.Empty;
        }

        /// <summary>
        /// Render a PDF representation of the passed markup.
        /// </summary>
        /// <param name="html">The markup to render as PDF.</param>
        /// <returns>A binary byte-array containing the rendered PDF.</returns>
        protected byte[] CallTheDriver(String html)
        {
            // replace href and src attributes with full URLs
            string baseUrl = string.Format("{0}://{1}/", HttpContext.Current.Request.Url.Scheme, HttpContext.Current.Request.Url.Authority);
            string relativeUrl = string.Format("{0}://{1}{2}", HttpContext.Current.Request.Url.Scheme, HttpContext.Current.Request.Url.Authority, HttpContext.Current.Request.Url.AbsolutePath);

            if (!relativeUrl.EndsWith("/")) relativeUrl = relativeUrl + "/";

            string buffer = _urlRootReplacement.Replace(html, String.Format("$1{0}", baseUrl));
            buffer = _urlRelativeReplacement.Replace(buffer, String.Format("$1{0}", relativeUrl));

            var fileContent = WkhtmltopdfDriver.ConvertHtml(WkhtmltopdfPath, GetConvertOptions(), buffer);
            return fileContent;
        }

        [Obsolete("Not Used By HtmlAsPdf", true)]
        protected override byte[] CallTheDriver(ControllerContext context) { throw new NotImplementedException("This class only used to render WebForms"); }

        /// <summary>
        /// Render a PDF representation of the passed markup.
        /// Will handle default filepaths, and saving copies to server.
        /// </summary>
        /// <param name="html">The markup to render as PDF.</param>
        /// <returns>A binary byte-array containing the rendered PDF.</returns>
        public byte[] BuildPdf(String html)
        {
            if (html == null)
                throw new ArgumentNullException("html");

            if (WkhtmltopdfPath == string.Empty)
                WkhtmltopdfPath = HttpContext.Current.Server.MapPath("~/Rotativa");

            var fileContent = CallTheDriver(html);

            if (string.IsNullOrEmpty(SaveOnServerPath) == false)
            {
                File.WriteAllBytes(SaveOnServerPath, fileContent);
            }

            return fileContent;
        }

        /// <summary>
        /// Render a PDF representation of the passed markup.
        /// </summary>
        /// <param name="html">The markup to render as PDF.</param>
        public void ExecuteResult(String html)
        {
            var context = new HttpContextWrapper(HttpContext.Current);

            var fileContent = this.BuildPdf(html);

            var response = this.PrepareResponse(context.Response);

            response.OutputStream.Write(fileContent, 0, fileContent.Length);
        }
    }
}
