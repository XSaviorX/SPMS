using DDNHRIS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DDNHRIS.Controllers
{
    [SessionTimeout]
    [Authorize(Roles = "APRD")]

    public class RSPSeparationController : Controller
    {
        HRISDBEntities db = new HRISDBEntities();
        // GET: RSPSeparation
        public ActionResult List()
        {
            return View();
        }
        
        public class SeparationMonthly
        {
            public string monthGroup { get; set; }
            public int count { get; set; }
            public List<tRSPSeparation> list { get; set; }
        }
        
        public JsonResult EmployeeList()
        {
            try
            {
               
                var list = (from e in db.vRSPEmployeeLists
                            select new
                            {
                                e.EIC,
                                e.fullNameLast,
                                e.isPlantilla
                            }).OrderBy(o => o.fullNameLast).ToList();

            
                return Json(new { status = "success", list = list }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string msg = ex.ToString();
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult GetEmpListByStatusGroup(string id)
        {
            try
            {

                var list = (from e in db.vRSPEmployeeLists
                            where e.tag == 1
                            select new
                            {
                                e.EIC,
                                e.fullNameLast,
                                e.isPlantilla
                            }).OrderBy(o => o.fullNameLast).ToList();
                return Json(new { status = "success", list = list }, JsonRequestBehavior.AllowGet);

                //if (id == "PLANTILLA")
                //{
                //    var list = (from e in db.vRSPEmployeeLists
                //                where e.isPlantilla == 1
                //                select new
                //                {
                //                    e.EIC,
                //                    e.fullNameLast,
                //                    e.isPlantilla
                //                }).OrderBy(o => o.fullNameLast).ToList();
                //    return Json(new { status = "success", list = list }, JsonRequestBehavior.AllowGet);
                //}
                //else
                //{
                //    var list = (from e in db.vRSPEmployeeLists
                //                where e.isPlantilla != 1
                //                select new
                //                {
                //                    e.EIC,
                //                    e.fullNameLast
                //                }).OrderBy(o => o.fullNameLast).ToList();
                //    return Json(new { status = "success", list = list }, JsonRequestBehavior.AllowGet);
                //}
            }
            catch (Exception ex)
            {
                string msg = ex.ToString();
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        public JsonResult SeparationList()
        {
            try
            {
                IEnumerable<tRSPSeparation> list = db.tRSPSeparations.Where(e => e.recNo > 0).OrderByDescending(o => o.effectiveDate).ToList();

                List<SeparationMonthly> sep = new List<SeparationMonthly>();

                foreach (tRSPSeparation item in list)
                {
                    string tmp = Convert.ToDateTime(item.effectiveDate).ToString("MMMM yyyy");
                    sep.Add(new SeparationMonthly()
                    {
                        monthGroup = tmp
                    });
                }

                var myGroup = sep.GroupBy(e => e.monthGroup).Select(g => new { g.Key });
                List<SeparationMonthly> separation = new List<SeparationMonthly>();
                foreach (var item in myGroup)
                {
                    DateTime tmpDate = Convert.ToDateTime("1 " + item.Key);

                    List<tRSPSeparation> myList = list.Where(e => e.effectiveDate.Year == tmpDate.Year && e.effectiveDate.Month == tmpDate.Month).OrderBy(o => o.effectiveDate).ThenBy(o => o.fullNameLast).ToList();

                    if (myList.Count > 0)
                    {
                        separation.Add(new SeparationMonthly()
                          {
                              monthGroup = item.Key.ToString(),
                              count = myList.Count(),
                              list = myList
                          });
                    }
                }

                return Json(new { status = "success", list = separation }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        public JsonResult SubmitSeparation(tRSPSeparation data, int tag)
        {
            if (data.EIC == null || data.EIC == "" || data.effectiveDate == null || data.separationType == null || data.separationType == "")
            {
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
            try
            {
                string uEIC = Session["_EIC"].ToString();
                vRSPEmployeeList employee = db.vRSPEmployeeLists.SingleOrDefault(e => e.EIC == data.EIC);

                if (employee == null)
                {
                    return Json(new { status = "Invalid data!" }, JsonRequestBehavior.AllowGet);
                }
                                   
                if (tag == 1)
                {
                    vRSPPlantillaPersonnel plan = db.vRSPPlantillaPersonnels.Where(e => e.EIC == data.EIC).OrderByDescending(o => o.recNo).FirstOrDefault();
                    if (plan != null)
                    {
                        //tRSPEmployee employee = db.tRSPEmployees.Single(e => e.EIC == plan.EIC);
                        tRSPSeparation sep = new tRSPSeparation();
                        sep.recordTransCode = plan.transCode;
                        sep.EIC = plan.EIC;
                        sep.fullNameLast = employee.fullNameLast;
                        sep.dateOfBirth = employee.birthDate;
                        sep.positionTitle = plan.positionTitle;
                        sep.statusOfAppt = plan.employmentStatus;
                        sep.salaryGrade = plan.salaryGrade;
                        sep.separationType = data.separationType;
                        sep.effectiveDate = Convert.ToDateTime(data.effectiveDate);
                        sep.remarks = data.remarks;
                        sep.userEIC = uEIC;
                        sep.statusGroup = "P";
                        sep.dateEncoded = DateTime.Now;
                        sep.tag = 1;
                        db.tRSPSeparations.Add(sep);

                   
                        int tempTag = 1;
                        if (data.separationType == "TRANSFERED" || data.separationType == "RETIRED" || data.separationType == "DEATH")
                        {
                            tempTag = 0;
                        }

                        tRSPEmployee emp = db.tRSPEmployees.Single(e => e.EIC == plan.EIC);
                        emp.tag = tempTag;

                        db.SaveChanges();
                    }
                    return Json(new { status = "success" }, JsonRequestBehavior.AllowGet);
                }
                else if (tag == 0)
                {

                    if (employee.transCode == null || employee.transCode  == "")
                    {
                        return Json(new { status = "Invalid employee position code!" }, JsonRequestBehavior.AllowGet);
                    }
                     
                    if (employee.employmentStatusCode == "05")
                    {
                        // UPDATE EMPLOYEE SERVICE RECORD
                        // - GET LAST SR THEN UPDATE
                        // - GET [tRSPEmployeePosition] record & Update [periodTo]
                                              
                        //a.positionTitle = position.po

                    }
                    else if (employee.employmentStatusCode == "06" || employee.employmentStatusCode == "07" || employee.employmentStatusCode == "08")
                    {   // JO;COS;HON;
                        // UPDATE EMPLOYEE WORK RECORD
                        // -
                        // - 
                    }
                    else
                    {
                        return Json(new { status = "Unable to save separation!" }, JsonRequestBehavior.AllowGet);
                    }



                    string tempTransCode = employee.transCode;
                    vRSPEmployeePosition position = db.vRSPEmployeePositions.SingleOrDefault(e => e.transCode == tempTransCode);
                   

                    position.isActivePosition = 0;
                    position.periodTo = data.effectiveDate;
                    
                    DateTime dt = DateTime.Now;
                    string tmpCode = "T" + dt.ToString("yyMMddHHmm") + "SEP" + data.EIC.Substring(0,4) + dt.ToString("ssfff");

                    tRSPSeparation sep = new tRSPSeparation();
                    sep.recordTransCode = tmpCode;
                    sep.EIC = employee.EIC;
                    sep.fullNameLast = employee.fullNameLast;
                    sep.dateOfBirth = employee.birthDate;
                    sep.positionTitle = employee.positionTitle;
                    sep.statusOfAppt = employee.employmentStatus;
                    sep.salaryGrade = employee.salaryGrade;
                    sep.separationType = data.separationType;
                    sep.effectiveDate = Convert.ToDateTime(data.effectiveDate);
                    sep.remarks = data.remarks;
                    sep.userEIC = uEIC;
                    sep.dateEncoded = DateTime.Now;
                    sep.statusGroup = "NP";
                    sep.tag = 1;
                    db.tRSPSeparations.Add(sep);
                    tRSPEmployee record = db.tRSPEmployees.Single(e => e.EIC == employee.EIC);
                    record.tag = 0;

                    //if (employee.employmentStatusCode == "05")
                    //{
                    //    tRSPRPTARA a = new tRSPRPTARA();
                    //    a.lastName = employee.lastName;
                    //    a.firstName = employee.firstName;
                    //    a.middleName = employee.middleName;
                    //    a.extName = employee.extName;
                    //    a.salary = position.salaryRate;
                    //    a.effectiveDate = Convert.ToDateTime(data.effectiveDate);
                    //    a.assumptionDate = Convert.ToDateTime(data.effectiveDate);
                        
                    //    a.positionTitle = position.positionTitle;
                    //    if (position.subPositionCode != null)
                    //    {
                    //        a.positionTitle = position.positionTitle + " (" + position.subPositionTitle + ")";
                    //    }
                         
                    //    a.employmentStatus = "CASUAL";
                    //    a.natureOfAppt = data.separationType;
                    //    a.remarks = data.remarks;
                    //    db.tRSPRPTARAs.Add(a);
                    //}

                    db.SaveChanges();
                    return Json(new { status = "success" }, JsonRequestBehavior.AllowGet);
                }                
                return Json(new { status = "Unable to save separation!" }, JsonRequestBehavior.AllowGet);              
            }
            catch (Exception ex)
            {
                string stat = ex.ToString();
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }


        //FORECASTED
        [HttpPost]
        public JsonResult GetForecastData()
        {
            try
            {
                DateTime dt = DateTime.Now.AddDays(-1);
                IEnumerable<vRSPPublicationVacantForecast> list = db.vRSPPublicationVacantForecasts.Where(e => e.effectiveDate > dt).ToList();
                return Json(new { status = "success", list = list }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                string stat = ex.ToString();
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult SubmitForecast(tRSPPublicationVacantForecast data)
        {
            try
            {                
                if (data.effectiveDate <= DateTime.Now)
                {
                    return Json(new { status = "Invalid Date" }, JsonRequestBehavior.AllowGet);
                }

                string uEIC = Session["_EIC"].ToString();
                vRSPPlantilla plantilla = db.vRSPPlantillas.SingleOrDefault(e => e.EIC == data.EIC);

                if (plantilla != null)
                {
                    tRSPPublicationVacantForecast f = new tRSPPublicationVacantForecast();
                    f.plantillaCode = plantilla.plantillaCode;
                    f.EIC = data.EIC = plantilla.EIC;
                    f.itemNo = plantilla.itemNo;
                    f.separationType = data.separationType;
                    f.effectiveDate = data.effectiveDate;
                    f.remarks = data.remarks;
                    f.userEIC = uEIC;
                    f.transDT = DateTime.Now;
                    db.tRSPPublicationVacantForecasts.Add(f);
                    db.SaveChanges();

                    DateTime dt = DateTime.Now.AddDays(-1);
                    IEnumerable<vRSPPublicationVacantForecast> list = db.vRSPPublicationVacantForecasts.Where(e => e.effectiveDate > dt).ToList();

                    return Json(new { status = "success", list = list }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { status = "Invalid data!" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                string stat = ex.ToString();
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }



    }
}