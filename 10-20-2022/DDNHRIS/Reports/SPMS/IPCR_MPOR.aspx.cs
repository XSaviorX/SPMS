using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Reporting;

namespace DDNHRIS.Reports.SPMS
{
    public partial class IPCR_MPOR1 : System.Web.UI.Page
    {
        InstanceReportSource rpt = new InstanceReportSource();
        protected void Page_Load(object sender, EventArgs e)
        {
            var officeCode = Request.Cookies["mporCrookie"].Value.Split(',');

            if (Request["type"] == "mpor")
            {
                rpt.ReportDocument = new IPCR_MPOR();
                rpt.Parameters.Add("EIC", officeCode[0]);
                rpt.Parameters.Add("officeId", officeCode[1]);
                rpt.Parameters.Add("month", officeCode[2]);
                rpt.Parameters.Add("monthId", officeCode[3]);
                rpt.Parameters.Add("year", officeCode[4]);
                rpt.Parameters.Add("semester", officeCode[5]);


            }

            MPORViewer.ReportSource = rpt;
        }
    }
}