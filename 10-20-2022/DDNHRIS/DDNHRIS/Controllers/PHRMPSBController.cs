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
    //[Authorize(Roles = "PSBPANEL,PSBSEC")]
    public class PHRMPSBController : Controller
    {
        HRISDBEntities db = new HRISDBEntities();
        // GET: PHRMPSB
        public ActionResult BEI()
        {
            return View();
        }

        private class TempPSBScreening
        {
            public string transCode { get; set; }
            public string publicationItemCode { get; set; }
            public int itemCount { get; set; }
            public string itemText { get; set; }
            public string positionTitle { get; set; }
            public int salaryGrade { get; set; }
            public decimal? rateMonth { get; set; }
            public DateTime PSBDate { get; set; }
            public string PSBVenue { get; set; }
            public string departmentName { get; set; }
            public IEnumerable<ApplicantRating> applicantList { get; set; }
            //public IEnumerable<vRSPApplication> applicantList { get; set; }
        }

        public class ApplicantRating
        {
            public string applicationCode { get; set; }
            public string applicantName { get; set; }
            public string applicantNameLast { get; set; }
            public string applicantPositionTitle { get; set; }
            public int applicantSalaryGrade { get; set; }
            public int applicantStep { get; set; }
            public string employmentStatusTag { get; set; }
            public string employmentStatusNameShort { get; set; }
            public int appTypeCode { get; set; }

            public int personalRate { get; set; }
            public int personalMax { get; set; }
            
            public int clarityRate { get; set; }
            public int clarityMax { get; set; }
            public int vividnessRate { get; set; }
            public int vividnessMax { get; set; }
            public int alertnessRate { get; set; }
            public int alertnessMax { get; set; }

            public int projectionRate { get; set; }
            public int projectionMax { get; set; }
            public int totalRating { get; set; }

        }

        
        [HttpPost]
        [Authorize(Roles = "PSBPANEL")]
        public JsonResult GetPSBSchedule()
        {
            try
            {
                DateTime dt = DateTime.Now;                
                var positionList = db.vRSPPSBSchedules.Select(e => new
                {
                    e.publicationItemCode,
                    e.transCode,
                    e.itemCount,
                    e.itemText,
                    e.positionTitle,
                    e.salaryGrade,
                    e.rateMonth,
                    e.departmentName,
                    e.PSBDate,
                    e.venue,
                    e.tag,
                    e.userEIC
                }).Where(e => e.tag == 1  && e.PSBDate >= dt).OrderBy(o => o.PSBDate).ToList();

                List<TempPSBScreening> myList = new List<TempPSBScreening>();
                foreach (var item in positionList)
                {
                    IEnumerable<vRSPApplication> lst = db.vRSPApplications.Where(e => e.publicationItemCode == item.publicationItemCode).OrderByDescending(o => o.appTypeCode).ThenBy(o => o.applicantNameLast).ToList();

                    List<ApplicantRating> tempList = RatingList(lst);
                    
                    myList.Add(new TempPSBScreening
                    {
                        transCode = item.transCode,
                        publicationItemCode = item.publicationItemCode,
                        itemText = item.itemText,
                        itemCount = Convert.ToInt16(item.itemCount),
                        positionTitle = item.positionTitle,
                        salaryGrade = Convert.ToInt16(item.salaryGrade),
                        rateMonth = item.rateMonth,
                        PSBDate = Convert.ToDateTime(item.PSBDate),
                        PSBVenue = item.venue,
                        departmentName = item.departmentName,
                        applicantList = tempList
                    });
                }
                return Json(new { status = "success", list = myList }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string errMgs = ex.ToString();
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }


        public List<ApplicantRating> RatingList(IEnumerable<vRSPApplication> list)
        {

            List<ApplicantRating> myList = new List<ApplicantRating>();

            foreach (vRSPApplication item in list)
            {
                myList.Add(new ApplicantRating()
                {
                    applicationCode = item.applicationCode,
                    applicantName = item.applicantName,
                    applicantNameLast = item.applicantNameLast,
                    applicantSalaryGrade = Convert.ToInt16(item.applicantSalaryGrade),
                    applicantStep =  Convert.ToInt16(item.applicantStep),
                    appTypeCode =  Convert.ToInt16(item.appTypeCode),
                    personalRate = 0,
                    personalMax = 30,
                    clarityRate  =0,
                    clarityMax = 20,
                    vividnessRate = 0,
                    vividnessMax = 25,
                    alertnessRate = 0,
                    alertnessMax = 15,
                    projectionRate = 0,
                    projectionMax = 10,
                    totalRating = 0
                });
            }

            return myList;


        }



    }
}