using System;
using System.IO;
using System.Web;
using System.Web.UI;

namespace Rotativa
{
    public class PageAsPdf : System.Web.UI.Page
    {
        protected HtmlAsPdf PDF = new HtmlAsPdf();

        protected override void Render(System.Web.UI.HtmlTextWriter writer)
        {
            // Check that we're not already rendering a PDF
            if (HttpContext.Current.Items["Rotativa.RenderingPDF"] as Boolean? != true)
            {
                // Flag the context so that we don't go into stack overflow
                HttpContext.Current.Items["Rotativa.RenderingPDF"] = true;

                StringWriter sw = new StringWriter();
                HtmlTextWriter hw = new HtmlTextWriter(sw);
                // Render the page to our StringWriter
                base.Render(hw);

                // Render the PDF
                this.PDF.ExecuteResult(sw.ToString());
            }
            else
                // Render the page normally
                base.Render(writer);
        }
    }
}
