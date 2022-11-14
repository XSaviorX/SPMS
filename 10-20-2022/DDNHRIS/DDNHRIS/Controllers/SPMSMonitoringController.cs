using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using DDNHRIS.Models;

namespace DDNHRIS.Controllers
{


    [SessionTimeout]
    [Authorize(Roles = "SPMSMASTER")]

    public class SPMSMonitoringController : Controller
    {

        HRISDBEntities db = new HRISDBEntities();

        // GET: SPMSMonitoring
        public ActionResult WorkGroup()
        {
            return View();
        }


        [HttpPost]
        public JsonResult GetWorkGroup()
        {
            try
            {

                string groupCode = "WRKGRPPGO";

                var list = db.tRSPWorkGroups.Select(e => new
                  {
                      e.workGroupCode, 
                      e.workGroupName
                }).OrderBy(o => o.workGroupName).ToList();


                IEnumerable<tSPMSIPRCRating> rating = db.tSPMSIPRCRatings.Where(e => e.semester == 2 && e.year == 2020).ToList();

                IEnumerable<vRSPEmployeeList> employee = db.vRSPEmployeeLists.Where(e => e.workGroupCode == groupCode).OrderBy(o => o.fullNameLast).ToList();
                
                List<TempRatingList> myList = _GetPerformanceRating(employee, rating);

                //tRSPWorkGroup g = list.Single(e => e.workGroupCode == "WRKGRPPGO");
                //var emp =  db.vRSPEmployeeLists.Where(e => e.workGroupCode == "WRKGRPPGO")
                                

                return Json(new { status = "success", groupList = list, ratingList = myList }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        public JsonResult GetRatingGroupByID(string id)
        {
            IEnumerable<tSPMSIPRCRating> rating = db.tSPMSIPRCRatings.Where(e => e.semester == 2 && e.year == 2020).ToList();
            IEnumerable<vRSPEmployeeList> employee = db.vRSPEmployeeLists.Where(e => e.workGroupCode == id).OrderBy(o => o.fullNameLast).ToList();
            List<TempRatingList> myList = _GetPerformanceRating(employee, rating);
            return Json(new { status = "success", ratingList = myList }, JsonRequestBehavior.AllowGet);
        }

        private class TempRatingList
        {
           
            public string EIC { get; set; }
            public string fullNameLast { get; set; }
            public string positionTitle { get; set; }
            public string statusName { get; set; }
            
            public string controlNo { get; set; }
            public decimal ratingNum { get; set; }
            public string ratingAdj { get; set; }
            public int semester { get; set; }
            public string remarks { get; set; }
            public int year { get; set; }

        }


        private List<TempRatingList> _GetPerformanceRating(IEnumerable<vRSPEmployeeList> employee, IEnumerable<tSPMSIPRCRating> rating)
        {

            List<TempRatingList> myList = new List<TempRatingList>();

            foreach (vRSPEmployeeList item in employee)
            {

                tSPMSIPRCRating rate = rating.SingleOrDefault(e => e.EIC == item.EIC);
                
                if (rate == null)
                {
                    tSPMSIPRCRating r = new tSPMSIPRCRating();
                    r.controlNo = "";
                    r.EIC = "";
                    r.ratingAdj = "";
                    r.ratingNum = 0;
                    r.remarks = "";
                    rate = r;
                }
                                
                myList.Add(new TempRatingList()
                {
                    EIC =  item.EIC,
                    fullNameLast = item.fullNameLast,
                    positionTitle = item.positionTitle,
                    statusName = item.employmentStatusNameShort,
                    ratingAdj = rate.ratingAdj,
                    ratingNum = Convert.ToDecimal(rate.ratingNum),
                    remarks = rate.remarks
                });
            }


            return myList;

        }



    }
}