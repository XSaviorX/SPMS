using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Reporting;

namespace DDNHRIS.Reports.SPMS
{
    public partial class Target1 : System.Web.UI.Page
    {
        InstanceReportSource rpt = new InstanceReportSource();
        protected void Page_Load(object sender, EventArgs e)
        {
            var officeCode = Request.Cookies["targetCrookie"].Value.Split(',');

            if (Request["type"] == "Target")
            {
                rpt.ReportDocument = new Target();
                rpt.Parameters.Add("officeId", officeCode[0]);
            }
            
            TargetViewer.ReportSource = rpt;
        }
    }
}