using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Reporting;

namespace DDNHRIS.Reports.SPMS
{
    public partial class IPCR_TARGET1 : System.Web.UI.Page
    {
        InstanceReportSource rpt = new InstanceReportSource();
        protected void Page_Load(object sender, EventArgs e)
        {
            var officeCode = Request.Cookies["ipcrCrookie"].Value.Split(',');

            if (Request["type"] == "Target")
            {
                rpt.ReportDocument = new IPCR_TARGET();
                rpt.Parameters.Add("officeId", officeCode[1]);
                rpt.Parameters.Add("EIC", officeCode[0]);

                
            }

            IPCRViewer.ReportSource = rpt;
        }
    }
}