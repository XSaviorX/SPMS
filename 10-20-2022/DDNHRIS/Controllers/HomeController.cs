using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using DDNHRIS.Models;

namespace DDNHRIS.Controllers
{
    [SessionTimeout]
    [Authorize]
    public class HomeController : Controller
    {
        HRISDBEntities db = new HRISDBEntities();
        public ActionResult Index()
        {
            return RedirectToAction("Dashboard");
        }

        public ActionResult Dashboard()
        {


            string uEIC = Session["_EIC"].ToString();
            ////CLARA;
            //FHOBY;
            //NELDA
            //BAMBIE
            //GHAY
            //ANNA
            if (uEIC == "HC14737721042E4F04FC" || uEIC == "FN24251916852AF7C1C0" || uEIC == "NR1913947967D2CAED75" || uEIC == "JH1626699557462EC008" || uEIC == "LL7707420936197BF4AA" || uEIC == "AS1032558413E77D0B61" || uEIC == "RLBE754CFA4FD8448091") 
            {
                return RedirectToAction("APRDBoard");
            }
            else if (uEIC == "ML971777288BEC062716" || uEIC == "DS1070016970E3ACC02D" || uEIC == "EP1831954384C6C94D75")
            {
                return RedirectToAction("APRDBoard");
            }            
           return View();
        }

        public ActionResult APRDBoard()
        {

            return View();
        }

        public class WorkforceStat
        {
            public string groupName { get; set; }
            public int groupMale { get; set; }
            public int groupFemale { get; set; }
           public List<StatList>  statusList { get; set; }
        }


        public class StatList
        {
            public string statusName { get; set; }
            public int maleCount { get; set; }
            public int femaleCount { get; set; }
        }

        public class ListItem
        {
            public string x { get; set; }
            public int y { get; set; }
        }
     

        //public JsonResult APRDDashboardData()
        //{
        //    try
        //    {
        //        IEnumerable<vRSPPlantilla> plantilla = db.vRSPPlantillas.Where(e => e.EIC != null).ToList();
        //        IEnumerable<tRSPEmploymentStatu> empStat = db.tRSPEmploymentStatus.ToList();

        //        List<WorkforceStat> workforce = new List<WorkforceStat>();
        //        List<StatList> stat = new List<StatList>();

        //        List<ListItem> temp = new List<ListItem>();

        //        int[] chartDataPL = new int[8];
        //        string[] chartLabelPL = new string[8];

        //        int i = 0;
        //        foreach (tRSPEmploymentStatu item in empStat.Where(e => e.isPlantilla == 1))
        //        {
        //            StatList t = GetStatByEmpStat(item, plantilla);
        //            stat.Add(t);
        //            temp.Add(new ListItem()
        //            {
        //                x = item.employmentStatus,
                       
        //            });
        //            chartDataPL[i] = t.maleCount + t.femaleCount;
        //            chartLabelPL[i] = item.employmentStatusNameShort;
        //            i++;
        //        }

        //        workforce.Add(new WorkforceStat()
        //        {
        //            groupName = "PLANTILLA",
        //            statusList = stat
        //        });
                
        //        List<StatList> stat2 = new List<StatList>();

        //        IEnumerable<vRSPEmployeeList> nonplantilla = db.vRSPEmployeeLists.Where(e => e.isPlantilla == 0).ToList();
                
              
        //        foreach (tRSPEmploymentStatu item in empStat.Where(e => e.isPlantilla == 0))
        //        {
        //            StatList t = GetStatByEmpStatNP(item, nonplantilla);
        //            stat2.Add(t);
        //           chartDataPL[i] = t.maleCount + t.femaleCount;
        //            chartLabelPL[i] = item.employmentStatusNameShort;
        //            i++;
        //        }
        //        workforce.Add(new WorkforceStat()
        //        {
        //            groupName = "NON-PLANTILLA",
        //            statusList = stat2
        //        });


        //        IEnumerable<tRSPAppointment> list = db.tRSPAppointments.Where(e => e.tag >= 0).ToList();
        //        IEnumerable<tRSPAppointment> pending = list.Where(e => e.tag == 0).ToList();

        //        int incomingAppt = db.vRSPPSBApps.Count();


        //        return Json(new { status = "success", workforce = workforce, chartDataPL = chartDataPL, chartLabelPL = chartLabelPL, incomingAppt = incomingAppt, pendingAppt = pending.Count() }, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception)
        //    {
        //        return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
        //    }
        //}


        //public StatList GetStatByEmpStat(tRSPEmploymentStatu status, IEnumerable<vRSPPlantilla> plantilla)
        //{
        //    StatList s = new StatList();

        //    int m = plantilla.Where(e => e.sex == "MALE" && e.employmentStatusCode == status.employmentStatusCode).Count();
        //    int f = plantilla.Where(e => e.sex == "FEMALE" && e.employmentStatusCode == status.employmentStatusCode).Count();
            
        //    s.statusName = status.employmentStatus;
        //    s.maleCount = m;
        //    s.femaleCount = f;

        //    return s;

        //}


        //public StatList GetStatByEmpStatNP(tRSPEmploymentStatu status, IEnumerable<vRSPEmployeeList> plantilla)
        //{
        //    StatList s = new StatList();

        //    int m = plantilla.Where(e => e.sex == "MALE" && e.employmentStatusCode == status.employmentStatusCode).Count();
        //    int f = plantilla.Where(e => e.sex == "FEMALE" && e.employmentStatusCode == status.employmentStatusCode).Count();

        //    s.statusName = status.employmentStatus;
        //    s.maleCount = m;
        //    s.femaleCount = f;

        //    return s;

        //}



    }
}