using DDNHRIS.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DDNHRIS.Controllers
{

    [SessionTimeout]
    [Authorize]
    public class WorkScheduleController : Controller
    {

        HRISDBEntities db = new HRISDBEntities();

        // GET: WorkSchedule
        public ActionResult Manager()
        {
            return View();
        }


        public class TempMonth
        {
            public string month { get; set; }
            public string monthValue { get; set; }
        }

        [HttpPost]
        public JsonResult ManagerInitData()
        {
            string userId = Session["_EIC"].ToString();

            tRSPWorkGroupAdmin wgAdm = db.tRSPWorkGroupAdmins.SingleOrDefault(e => e.EIC == userId && e.tag >= 1);

            if (wgAdm == null)
            {
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }

            string groupCode = wgAdm.workGroupCode;
            List<vRSPWorkGroupEmp> wrkGroup = new List<vRSPWorkGroupEmp>();

            if (wgAdm.tag == 2)
            {
                wrkGroup = db.vRSPWorkGroupEmps.Where(e => e.workGroupCode != null).OrderBy(o => o.fullNameLast).ToList();
            }
            else
            {
                wrkGroup = db.vRSPWorkGroupEmps.Where(e => e.workGroupCode == groupCode).OrderBy(o => o.fullNameLast).ToList();
            }


            List<TempMonth> months = new List<TempMonth>();
            DateTime dt = DateTime.Now.AddMonths(-1);
            for (int i = 1; i <= 5; i++)
            {
                months.Add(new TempMonth()
                {
                    month = dt.ToString("MMMM") + " " + dt.ToString("yyyy"),
                    monthValue = dt.ToString("MMMM") + " " + dt.ToString("yyyy")
                });
                dt = dt.AddMonths(1);
            }



            var schemes = db.tAttWorkSchedTemplates.Select(e => new
            {
                e.shiftTemplateId,
                e.shiftName
            }).ToList();

             

            IEnumerable<tAttWorkSched> list = db.tAttWorkScheds.Where(e => e.userEIC == userId).ToList();
            return Json(new { status = "success", monthList = months, schedules = list, schemes = schemes, workGroups = wrkGroup }, JsonRequestBehavior.AllowGet);

        }




        public class TempWorkSchedule
        {
            public string workSchedName { get; set; }
            public string details { get; set; }
            public string monthValue { get; set; }
            public int period { get; set; }
        }


        [HttpPost]
        public JsonResult SaveWorksSchedule(TempWorkSchedule data)
        {
            try
            {
                string userId = Session["_EIC"].ToString();
                DateTime dt = DateTime.Now;
                string id = "WS" + dt.ToString("yyMMddHHmm") + "SCH" + dt.ToString("ssfff") + userId.Substring(0, 5);

                tAttWorkSched s = new tAttWorkSched();
                s.workSchedId = id;
                s.workSchedName = data.workSchedName.Trim();
                s.details = data.details.Trim();
                s.workSchedMonth = Convert.ToDateTime("1 " + data.monthValue);
                s.period = data.period;
                s.tag = 1;
                s.remarks = "";
                s.transDT = dt;
                s.userEIC = userId;

                db.tAttWorkScheds.Add(s);
                db.SaveChanges();
                return Json(new { status = "success", schedule = s }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        public JsonResult SelectWorkSchedule(string id)
        {
            try
            {
                tAttWorkSched sched = db.tAttWorkScheds.Single(e => e.workSchedId == id);

                var temp = (from d in db.tAttWorkSchedDetails
                           join t in db.tAttWorkSchedTemplates on d.shiftTemplateId equals t.shiftTemplateId
                           select new 
                           {
                               workSchedId = d.workSchedId,
                               login = d.login,
                               logout = d.logout,
                               shiftHour =  d.shiftHour,
                               shiftName = t.shiftName
                           }).Where(e => e.workSchedId == id).ToList();

              

                return Json(new { status = "success", schedule = sched, scheduleSchemes = temp }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }


        public class TempSchemeData
        {
            public string workSchedId { get; set; }
            public string shiftTemplateId { get; set; }
            public DateTime dateFrom { get; set; }
            public DateTime dateTo { get; set; }
        }


        [HttpPost]
        public JsonResult SaveSchemeData(TempSchemeData data)
        {
            try
            {
                DateTime tempFrom = DateTime.Parse(data.dateFrom.ToString(), CultureInfo.CreateSpecificCulture("en-US"));
                DateTime tempTo = DateTime.Parse(data.dateTo.ToString(), CultureInfo.CreateSpecificCulture("en-US"));

                DateTime runDate = tempFrom;

                tAttWorkSchedTemplate t = db.tAttWorkSchedTemplates.Single(e => e.shiftTemplateId == data.shiftTemplateId);

                for (DateTime d = runDate; runDate <= tempTo; )
                {

                    DateTime loginDT = Convert.ToDateTime(runDate.ToString("MM/dd/yyyy") + " " + t.loginTime);
                    DateTime logoutDT = loginDT.AddHours(Convert.ToInt16(t.hours));

                    tAttWorkSchedDetail s = new tAttWorkSchedDetail();
                    s.workSchedId = data.workSchedId;
                    s.login = loginDT;
                    s.logout = logoutDT;
                    s.shiftHour = t.hours;
                    s.shiftTemplateId = t.shiftTemplateId;
                    db.tAttWorkSchedDetails.Add(s);
                    runDate = runDate.AddDays(1);
                }
                db.SaveChanges();


                IEnumerable<tAttWorkSchedDetail> schemes = db.tAttWorkSchedDetails.Where(e => e.workSchedId == data.workSchedId).OrderBy(o => o.login).ToList();



                return Json(new { status = "success", schemes = schemes }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {

                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }



        private class TempAttWorkSchedDetail
        {
            public string workSchedId { get; set; }
            public DateTime login { get; set; }
            public DateTime logout { get; set; }
            public string shiftName { get; set; }
            public int shiftHour { get; set; }
        }


        private List<TempAttWorkSchedDetail> GetSchemeById(string id)
        {

            List<TempAttWorkSchedDetail> result = new List<TempAttWorkSchedDetail>();

            result = (from d in db.tAttWorkSchedDetails
                                                 join t in db.tAttWorkSchedTemplates on d.shiftTemplateId equals t.shiftTemplateId
                                                 select new TempAttWorkSchedDetail
                                                 {
                                                     workSchedId = d.workSchedId,
                                                     login = Convert.ToDateTime(d.login),
                                                     logout = Convert.ToDateTime(d.logout),
                                                     shiftHour = Convert.ToInt16(d.shiftHour),
                                                     shiftName = t.shiftName
                                                 }).Where(e => e.workSchedId == id).ToList();

            return result;

                    
             

        }



     



    }
}