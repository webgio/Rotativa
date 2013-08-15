using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Rotativa.Demo
{
    public partial class TestWebForms : PageAsPdf
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            this.PDF.PageMargins = new Options.Margins
            {
                Bottom = 0,
                Right = 0,
                Left = 0,
                Top = 0
            };
        }
    }
}