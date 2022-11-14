using DDNHRIS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Reporting;

namespace DDNHRIS.Reports
{
 
    [SessionTimeout]
      
    public partial class HRIS : System.Web.UI.Page
    {
             
        InstanceReportSource rpt = new InstanceReportSource();
        
        protected void Page_Load(object sender, EventArgs e)
        {
            
            var reportType = Session["ReportType"].ToString();
            var paramCode = Session["PrintReport"].ToString();
          

            switch (reportType)
            {

                case "Appointment":
                    rpt.ReportDocument = new DDNHRIS.Reports.Appointment.AppointmentReport();
                    rpt.Parameters.Add("paramCode", paramCode);
                    break;

                case "PDF":
                    rpt.ReportDocument = new DDNHRIS.Reports.Appointment.PDF();
                    rpt.Parameters.Add("paramCode", paramCode);
                    break;

                case "PDFBACK":
                    string[] partA = paramCode.Split(':');
                    rpt.ReportDocument = new DDNHRIS.Reports.Appointment.PDFBackB();
                      rpt.Parameters.Add("paramCode", partA[0]);
                      rpt.Parameters.Add("plantillaCode", partA[1]);
                    break;
                    
                case "PDFBACKB":
                    string[] partB = paramCode.Split(':'); 
                    rpt.ReportDocument = new DDNHRIS.Reports.Appointment.PDFBackB();
                    rpt.Parameters.Add("paramCode", partB[0]);
                    rpt.Parameters.Add("plantillaCode", partB[1]);
                    break;

                case "CSCLIMITATION":
                    rpt.ReportDocument = new DDNHRIS.Reports.CSClimitation.CSClimitation();
                    rpt.Parameters.Add("paramCode", paramCode);
                    break;
                                   
                case "ASSUMPTDUTY":
                    rpt.ReportDocument = new DDNHRIS.Reports.AssumptionDuty.AssumptionDuty();
                    rpt.Parameters.Add("paramCode", paramCode);
                    break;

                case "OATHOFFICE":
                    rpt.ReportDocument = new DDNHRIS.Reports.Oath.Oath();
                    rpt.Parameters.Add("paramCode", paramCode);
                    break;

                case "FUNDSAVAILABILITY":
                    rpt.ReportDocument = new DDNHRIS.Reports.CSClimitation.CSClimitation();
                    rpt.Parameters.Add("paramCode", paramCode);
                    break;

                case "APPTCasual_34D":
                    rpt.ReportDocument = new DDNHRIS.Report.AppointmentCasual34D();
                    rpt.Parameters.Add("paramCode", paramCode);
                    break;

                case "APPTCasual_34D_R": //RIGHT MARGIN
                    rpt.ReportDocument = new DDNHRIS.Report.AppointmentCasual34D_R();
                    rpt.Parameters.Add("paramCode", paramCode);
                    break;


                case "APPTCasual_34F":
                    rpt.ReportDocument = new DDNHRIS.Report.AppointmentCasualF();
                    rpt.Parameters.Add("paramCode", paramCode);
                    break;

                case "APPTCasual_34F_R": //RIGHT MARGIN
                    rpt.ReportDocument = new DDNHRIS.Report.AppointmentCasualF_R();
                    rpt.Parameters.Add("paramCode", paramCode);
                    break;

                case "APPTJOBORDER":
                    rpt.ReportDocument = new DDNHRIS.Reports.AppointmentCasual.AppointmentJobOrder();
                    rpt.Parameters.Add("paramCode", paramCode);
                    break;

                case "APPTJOBORDER_R": //RIGHT MARGIN
                    rpt.ReportDocument = new DDNHRIS.Reports.AppointmentCasual.AppointmentJobOrder_R();
                    rpt.Parameters.Add("paramCode", paramCode);
                    break;
                                    
                case "APPTHonorarium": 
                    rpt.ReportDocument = new DDNHRIS.Reports.AppointmentCasual.AppointmentHonorarium();
                    rpt.Parameters.Add("paramCode", paramCode);
                    break;

                case "APPTHonorarium_R": //RIGHT MARGIN
                    rpt.ReportDocument = new DDNHRIS.Reports.AppointmentCasual.AppointmentHonorarium_R();
                    rpt.Parameters.Add("paramCode", paramCode);
                    break;


                case "CASUALOATH":
                    rpt.ReportDocument = new DDNHRIS.Reports.AppointmentCasual.CasualOath();
                    rpt.Parameters.Add("paramCode", paramCode);
                    break;


                case "PDFCASUAL":
                    rpt.ReportDocument = new DDNHRIS.Reports.AppointmentCasual.PDFCasual();
                    rpt.Parameters.Add("paramCode", paramCode);
                    break;

                case "PDFCASUALBACK":
                    rpt.ReportDocument = new DDNHRIS.Reports.AppointmentCasual.PDFCasualBack();
                    rpt.Parameters.Add("paramCode", paramCode);
                    break;

                case "PlantillaProposed":
                    rpt.ReportDocument = new DDNHRIS.Reports.Plantilla.PlantillaProposed();
                    rpt.Parameters.Add("paramCode", paramCode);
                    break;

                case "LBPFORM3":
                    rpt.ReportDocument = new DDNHRIS.Reports.Plantilla.PlantillaLBPForm3();
                    rpt.Parameters.Add("paramCode", paramCode);
                    break;

                case "LBPFORM3A":
                    rpt.ReportDocument = new DDNHRIS.Reports.Plantilla.PlantillaLBPForm3A();
                    rpt.Parameters.Add("paramCode", paramCode);
                    break;

                case "PlantillaUnfunded":
                    rpt.ReportDocument = new DDNHRIS.Reports.Plantilla.PlantillaUnfunded();
                    rpt.Parameters.Add("paramCode", paramCode);
                    break;

                //case "VACANTPOSITIONS":
                //    rpt.ReportDocument = new DDNHRIS.Reports.Plantilla.PlantillaUnfunded();
                //    rpt.Parameters.Add("paramCode", paramCode);
                //    break;

                case "ServiceRecord":
                    string[] srRemarks = paramCode.Split('|');

                    if (srRemarks[0].ToString() == "LG")
                    {
                        rpt.ReportDocument = new DDNHRIS.Reports.ServiceRecord.ServiceRecord_B();
                    }
                    else
                    {
                        rpt.ReportDocument = new DDNHRIS.Reports.ServiceRecord.ServiceRecord_A();
                    }
                   
                    rpt.Parameters.Add("paramCode", srRemarks[1]);
                    rpt.Parameters.Add("paramText", srRemarks[2]);
                    rpt.Parameters.Add("paramRemarks", srRemarks[3]);
                    break;

                case "NOSI":
                    rpt.ReportDocument = new DDNHRIS.Reports.Plantilla.NOSI();
                    rpt.Parameters.Add("paramCode", paramCode);
                    break;

                case "NOSA":
                    rpt.ReportDocument = new DDNHRIS.Reports.Plantilla.NOSA();
                    rpt.Parameters.Add("paramCode", paramCode);
                    break;

                case "NOSACa":
                    rpt.ReportDocument = new DDNHRIS.Reports.Plantilla.NOSACa();
                    rpt.Parameters.Add("paramCode", paramCode);
                    break;

                case "PlantillaCurrent":
                    rpt.ReportDocument = new DDNHRIS.Reports.Plantilla.PlantillaCurrent();
                    rpt.Parameters.Add("paramCode", paramCode);
                    break;

                case "VANCANTPOSITIONS":
                    rpt.ReportDocument = new DDNHRIS.Reports.Plantilla.PlantillaVacant();
                    rpt.Parameters.Add("paramCode", paramCode);
                    break;

                case "PlantillaCSCForm":
                    rpt.ReportDocument = new DDNHRIS.Reports.Plantilla.PlantillaCSC();
                    rpt.Parameters.Add("paramCode", paramCode);
                    break;
                    
                case "PlantillaCSCUForm":
                    rpt.ReportDocument = new DDNHRIS.Reports.Plantilla.PlantillaCSCU();
                    rpt.Parameters.Add("paramCode", paramCode);
                    break;

                case "PlantillaDBMForm":
                    rpt.ReportDocument = new DDNHRIS.Reports.Plantilla.PlantillaLBPForm3A();
                    rpt.Parameters.Add("paramCode", paramCode);
                    break;

                case "RAI":
                    rpt.ReportDocument = new DDNHRIS.Reports.CSCReports.RAI();
                    rpt.Parameters.Add("paramCode", paramCode);
                    break;

                case "RAIALL":
                    rpt.ReportDocument = new DDNHRIS.Reports.CSCReports.RAIALL();
                    rpt.Parameters.Add("paramCode", paramCode);
                    break;
                    
                case "RAIPLANTILLA":
                    rpt.ReportDocument = new DDNHRIS.Reports.CSCReports.RAIPlantilla();
                    rpt.Parameters.Add("paramCode", paramCode);
                    break;
                                   
                case "RAIPLANTILLAXLS":
                    rpt.ReportDocument = new DDNHRIS.Reports.CSCReports.RAIPlantillaXls();
                    rpt.Parameters.Add("paramCode", paramCode);
                    break;

                case "RAIPLANTILLABACK":
                    rpt.ReportDocument = new DDNHRIS.Reports.CSCReports.RAIBack();
                    rpt.Parameters.Add("paramCode", paramCode);
                    break;
 
                case "PUBLICATION":
                    rpt.ReportDocument = new DDNHRIS.Reports.RSPPublication.PublicationU();
                    rpt.Parameters.Add("paramCode", paramCode);
                    break;


                case "PUBLICATION_JD":
                    rpt.ReportDocument = new DDNHRIS.Reports.RSPPublication.JobDescription();
                    rpt.Parameters.Add("paramCode", paramCode);
                    break;

                case "LISTBYFUNDSOURCE":
                    rpt.ReportDocument = new DDNHRIS.Reports.UtilityReport.ListByFundSource();
                    rpt.Parameters.Add("paramCode", paramCode);
                    break;

                case "VACANTPOSITIONSUMM":
                    rpt.ReportDocument = new DDNHRIS.Reports.UtilityReport.VacantPositionSum();
                    rpt.Parameters.Add("paramCode", paramCode);
                    break;

                case "PDSPAGE1":
                    rpt.ReportDocument = new DDNHRIS.Reports.PDS.PDSPage1();
                    rpt.Parameters.Add("paramCode", paramCode);
                    break;

                case "PDSPAGE2":
                    rpt.ReportDocument = new DDNHRIS.Reports.PDS.PDSPage2();
                    rpt.Parameters.Add("paramCode", paramCode);
                    break;

                case "PDSPAGE3":
                    rpt.ReportDocument = new DDNHRIS.Reports.PDS.PDSPage3();
                    rpt.Parameters.Add("paramCode", paramCode);
                    break;

                case "PDSPAGE4":
                    rpt.ReportDocument = new DDNHRIS.Reports.PDS.PDSPage4();
                    rpt.Parameters.Add("paramCode", paramCode);
                    break;
                    
                case "APPLCNTPROFILE":
                    rpt.ReportDocument = new DDNHRIS.Reports.Profiling.ApplicantProfile();
                    rpt.Parameters.Add("paramCode", paramCode);
                    break;
                                   
                case "PSMATRIX":
                    rpt.ReportDocument = new DDNHRIS.Reports.UtilityReport.PSCompMatrixA();
                    rpt.Parameters.Add("paramCode", paramCode);
                    break;

                case "PRINTPS":
                    rpt.ReportDocument = new DDNHRIS.Reports.UtilityReport.PersonnelServicesPBO();
                    rpt.Parameters.Add("paramCode", paramCode);
                    break;
                     
                case "PLANTILLAVACANT":
                    rpt.ReportDocument = new DDNHRIS.Reports.UtilityReport.VacantPositionSum();
                    rpt.Parameters.Add("paramCode", paramCode);
                    break;
                case "PSCOMPUTATION":
                    rpt.ReportDocument = new DDNHRIS.Reports.UtilityReport.PSComputation();
                    rpt.Parameters.Add("paramCode", paramCode);
                    break;
                case "PSCOMPUTATIONNGS":
                    rpt.ReportDocument = new DDNHRIS.Reports.UtilityReport.PSComputationNGS();
                    rpt.Parameters.Add("paramCode", paramCode);
                    break;

                case "PSBYFUNDSOURCE":
                    rpt.ReportDocument = new DDNHRIS.Reports.UtilityReport.PSComputationByCharges();
                    rpt.Parameters.Add("paramCode", paramCode);
                    break;


                case "PSSAVINGS":
                    rpt.ReportDocument = new DDNHRIS.Reports.UtilityReport.PSSavings();
                    rpt.Parameters.Add("paramCode", paramCode);
                    break;


                case "COMPTASSMNTRESULT":
                    rpt.ReportDocument = new DDNHRIS.Reports.ComptAssmntResult.ComptAssmntResult();

                    var paramCodeA = Session["PrintReport"].ToString();
                    string[] partCarrot = paramCodeA.Split(':');
                    
                    rpt.Parameters.Add("paramcomptCode", partCarrot[1].ToString());
                    rpt.Parameters.Add("paramcomptGroupCode", partCarrot[2].ToString());
                    rpt.Parameters.Add("paramassmntGroupCode", partCarrot[0].ToString());
                    break;


                case "EMPBYCHARGES":
                    rpt.ReportDocument = new DDNHRIS.Reports.UtilityReport.EmpByWorkGroupCharges();

                    var paramCodeD = Session["PrintReport"].ToString();
                    string[] partDurian = paramCodeD.Split(':');

                    rpt.Parameters.Add("fundSourceCode", partDurian[0].ToString());
                    rpt.Parameters.Add("workGroupCode", partDurian[1].ToString());
                    rpt.Parameters.Add("employmentStatusCode", partDurian[2].ToString());
                    break;
                    
                case "SALN":
                    string id = "SALN202012310001";
                    rpt.ReportDocument = new DDNHRIS.Reports.UtilityReport.SALN();
                    rpt.Parameters.Add("batchCode", id);
                    rpt.Parameters.Add("deptCode", paramCode);
                    break;
                    
                case "EMPOFFICEWORKGROUP":

                    var paramWorkGrp = Session["PrintReport"].ToString();
                    string[] empWorkGroup = paramWorkGrp.Split('|');

                    rpt.ReportDocument = new DDNHRIS.Reports.UtilityReport.EmployeeByWorkGroup();
                    rpt.Parameters.Add("paramWorkGroupCode", empWorkGroup[0].ToString());
                    rpt.Parameters.Add("paramEmploymentStatusCode", empWorkGroup[1].ToString());
                    break;

                case "SURVEYREP":
                    string[] surveyRemarks = paramCode.Split('|');
                    rpt.ReportDocument = new DDNHRIS.Reports.LND.SurveyReport();
                    rpt.Parameters.Add("paramId", surveyRemarks[0]);
                    rpt.Parameters.Add("officeName", surveyRemarks[1]);
                    break;


                //
                    

                //EMPBYCHARGES
                
                //
                //case "PlantillaReport2":
                //    paramName = Session["PrintReport2"].ToString();
                //    rpt.ReportDocument = new PlantillaReport2();
                //    rpt.Parameters.Add("paramDept", paramName);
                //    break;

                //case "ProposedReport":
                //    paramName = Session["PrintReport3"].ToString();
                //    rpt.ReportDocument = new ProposedReport();
                //    rpt.Parameters.Add("param", paramName);

                //    break;

                //case "ServiceRecord":
                //    paramName = Session["parameterSes"].ToString();
                //    rpt.ReportDocument = new ServiceRecord.ServiceRecord();
                //    rpt.Parameters.Add("paramEIC", paramName);
                //    break;

                //case "Publication":
                //    var publicationCode = Session["publicationCode"].ToString();
                //    if (Request["cont"] == "JD")
                //    {
                //        rpt.ReportDocument = new RSPPublication.JobDescription();
                //    }
                //    else
                //    {
                //        rpt.ReportDocument = new Publication.Publication();
                //    }
                //    rpt.Parameters.Add("publicationCode", publicationCode);
                //    break;

                //case "NosiReport":
                //    paramName = Session["PrintNosi"].ToString();
                //    rpt.ReportDocument = new NosiReport.NosiReport();
                //    rpt.Parameters.Add("paramEIC", paramName);
                //    break;

                //case "Appointment":
                //    paramName = Session["AppointmentCode"].ToString();
                //    rpt.ReportDocument = new Appointment.AppointmentReport();
                //    rpt.Parameters.Add("paramAppCode", paramName);
                //    break;

                //case "AppointmentCasual":
                //    paramName = Session["fundSourceCode"].ToString();
                //    rpt.ReportDocument = new Appointment.AppointmentCasualReport();
                //    rpt.Parameters.Add("paramFundCode", paramName);
                //    break;

                //case "AssumptionOfDuty":
                //    paramName = Session["AppointmentCode"].ToString();
                //    rpt.ReportDocument = new AssumptionDuty.AssumptionDuty();
                //    rpt.Parameters.Add("paramAppCode", paramName);
                //    break;

                //case "PDFfront":
                //    paramName = Session["AppointmentCode"].ToString();
                //    //paramName = Session["PlantillaCode"].ToString();
                //    rpt.ReportDocument = new PDF_Front.PDFfront();
                //    rpt.Parameters.Add("paramAppCode", paramName);
                //    //rpt.Parameters.Add("paramPlanCode", paramName);
                //    break;

                //    //FOR REMOVAL SINCE FRONT AND BACK ARE MERGE
                //case "PDFback":               
                //    paramName = Session["PlantillaCode"].ToString();
                //    string paramDeptHead = Session["DeptHead"].ToString();
                //    string paramAppCode = Session["appointmentCode"].ToString();
                //    string paramJobDesc = Session["jobDesc"].ToString();

                //    rpt.ReportDocument = new PDF_Back.PDFBack();
                //    rpt.Parameters.Add("paramPlanCode", paramName);
                //    rpt.Parameters.Add("paramDeptHead", paramDeptHead);
                //    rpt.Parameters.Add("paramAppCode", paramAppCode);
                //    rpt.Parameters.Add("paramJobDesc", paramJobDesc);
                //    break;

                //case "CSClimitation":
                //    paramName = Session["AppointmentCode"].ToString();
                //    rpt.ReportDocument = new CSClimitation.CSClimitation();
                //    rpt.Parameters.Add("paramAppCode", paramName);
                //    break;

                //case "OathOfOffice":
                //    paramName = Session["AppointmentCode"].ToString();
                //    rpt.ReportDocument = new Oath.Oath();
                //    rpt.Parameters.Add("paramAppCode", paramName);
                //    break;

                //case "AvailabilityFunds":
                //    paramName = Session["AppointmentCode"].ToString();
                //    rpt.ReportDocument = new AvailableFunds.AvailableFunds();
                //    rpt.Parameters.Add("paramAppCode", paramName);
                //    break;

                //case "WES":
                //    var reportParameter = Request.Cookies["reportParameter"] != null ? Request.Cookies["reportParameter"].Value.Split('|') : null;
                //    rpt.ReportDocument = new Application.WES.WESReport();
                //    rpt.Parameters.Add("applicationCode", reportParameter[0]);
                //    break;

                //case "ApplicantProfile":
                //    var ApplicantProfile = Request.Cookies["reportParameter"] != null ? Request.Cookies["reportParameter"].Value.Split('|') : null;
                //    rpt.ReportDocument = new PSB.Profile.ApplicantProfile();
                //    rpt.Parameters.Add("applicationCode", ApplicantProfile[0]);
                //    break;

                //case "PersonnelServices":
                //    var departmentCode = Session["reportParameter"] != null ? Session["reportParameter"].ToString() : null;
                //    rpt.ReportDocument = new BudgetaryReq.PersonenlServices();
                //    rpt.Parameters.Add("departmentCode", departmentCode);
                //    break;

                //case "Separation":
                //    var dateEncoded = Session["reportParameter"] != null ? Session["reportParameter"].ToString() : null;
                //    rpt.ReportDocument = new Separation.Separation();
                //    rpt.Parameters.Add("dateEncoded", dateEncoded);
                //    break;

                //case "PrintNosa":
                //    paramName = Session["NosaReport"].ToString();
                //    paramDept = Session["NosaReport2"].ToString();
                //    rpt.ReportDocument = new NosaReport.NosaReport();
                //    rpt.Parameters.Add("paramEIC", paramName);
                //    rpt.Parameters.Add("paramDept", paramDept);
                //    break;

                //case "PrintCoreCompetency":
                //    string paramComptCode = Session["comptCode"].ToString();
                //    string paramComptGroupCode = Session["comptGroupCode"].ToString();
                //    string paramAssmntGroupCode = Session["assmntGroupCode"].ToString();

                //    rpt.ReportDocument = new Competency.CoreComptReport();
                //    rpt.Parameters.Add("paramcomptCode", paramComptCode);
                //    rpt.Parameters.Add("paramcomptGroupCode", paramComptGroupCode);
                //    rpt.Parameters.Add("paramassmntGroupCode", paramAssmntGroupCode);
                //    break;

                //case "PrintSummaryCompetency":
                //    string paramComptCode2 = Session["comptCode"].ToString();
                //    string paramComptGroupCode2 = Session["comptGroupCode"].ToString();
                //    string paramAssmntGroupCode2 = Session["assmntGroupCode"].ToString();


                //    rpt.ReportDocument = new Competency.SummaryMinMaj();
                //    rpt.Parameters.Add("paramcomptCode", paramComptCode2);
                //    rpt.Parameters.Add("paramcomptGroupCode", paramComptGroupCode2);
                //    rpt.Parameters.Add("paramassmntGroupCode", paramAssmntGroupCode2);
                //    break;

                //case "IndividualCompetencyReport":
                //    paramName = Session["IndividualCompetency"].ToString();
                //    rpt.ReportDocument = new IndividualComptencyReport.IndividualCompetencyReport();
                //    rpt.Parameters.Add("paramEIC", paramName);
                //    break;

                //case "BudgetarySavingsReport":
                //    paramName = Session["BudgetarySavings"].ToString();
                //    rpt.ReportDocument = new BudgetarySavingsReport.BudgetarySavingsReport();
                //    rpt.Parameters.Add("paramDept", paramName);
                //   break;

                //case "VacantPosition":
                //   paramName = Session["PrintReport4"].ToString();
                //   string paramCount = Session["PrintCount"].ToString();
                //   rpt.ReportDocument = new VacantPosition();
                //   rpt.Parameters.Add("paramCode", paramName);
                //   rpt.Parameters.Add("paramCount", paramCount);

                //   break;

                //case "PlantillaProposed":
                //   paramName = Session["PrintReport"].ToString();
                //   rpt.ReportDocument = new RSPUtility.Plantilla();
                //   rpt.Parameters.Add("paramCode", paramName);            

                //   break;


            }


            HRISViewer.ReportSource = rpt;


        }
    }
}