using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DDNHRIS.Reports.Profiling;
using DDNHRIS.Reports.RSPPublication;
using Telerik.Reporting;

namespace DDNHRIS.Reports
{
    public partial class Application : System.Web.UI.Page
    {
        InstanceReportSource rpt = new InstanceReportSource();
        protected void Page_Load(object sender, EventArgs e)
        {
            var application = Request.Cookies["request_cookie"].Value.Split(',');
            if (Request["type"] == "ApplicantProfile")
            {
                rpt.ReportDocument = new ApplicantProfile();
                rpt.Parameters.Add("applicationCode", application[0]);
                rpt.Parameters.Add("applicantCode", application[1]);
            }
            if (Request["type"] == "ApplicantProfileShortDetails")
            {
                rpt.ReportDocument = new applicantProfileShortDetails();
                rpt.Parameters.Add("applicationCode", application[0]);
                rpt.Parameters.Add("applicantCode", application[1]);
            }
            if (Request["type"] == "applicantList")
            {
                rpt.ReportDocument = new applicationList();
                rpt.Parameters.Add("publicationItemCode", application[0]);
            }
            if (Request["type"] == "positionJD")
            {
                rpt.ReportDocument = new positionJD();
                rpt.Parameters.Add("publicationItemCode", application[0]);
                rpt.Parameters.Add("plantillaCode", application[1]);
            }
            if (Request["type"] == "consolidatedScreening")
            {
                rpt.ReportDocument = new consolidatedScreening();
                rpt.Parameters.Add("publicationItemCode", application[0]);
                //rpt.Parameters.Add("plantillaCode", application[1]);
            }
            if (Request["type"] == "consolidatedBEI")
            {
                rpt.ReportDocument = new consolidateBEI();
                rpt.Parameters.Add("publicationItemCode", application[0]);
                //rpt.Parameters.Add("plantillaCode", application[1]);
            }
            if (Request["type"] == "applicantAssessment")
            {
                rpt.ReportDocument = new applicantAssessment();
                //rpt.Parameters.Add("publicationItemCode", application[0]);
                rpt.Parameters.Add("applicationCode", application[0]);
                rpt.Parameters.Add("applicantCode", application[1]);

            }
            ApplicationViewer.ReportSource = rpt;
        }
    }
}