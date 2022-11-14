using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Reporting;
namespace DDNHRIS.Reports.SPMS
{
    
    public partial class Opcr : System.Web.UI.Page
    {
        InstanceReportSource rpt = new InstanceReportSource();
        protected void Page_Load(object sender, EventArgs e)
        {
            var officeCode = Request.Cookies["opcrCrookie"].Value.Split(',');

            if (Request["type"] == "opcr")
            {
                rpt.ReportDocument = new opcr();
                rpt.Parameters.Add("officeId", officeCode[0]);

            }

            OPCRViewer.ReportSource = rpt;

        }
    }
}