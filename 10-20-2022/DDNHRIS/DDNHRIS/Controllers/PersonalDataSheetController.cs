using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using DDNHRIS.Models;

namespace DDNHRIS.Controllers
{
    public class PersonalDataSheetController : Controller
    {

        HRISDBEntities db = new HRISDBEntities();
        HRISEntities dbp = new HRISEntities();

        // ********************* CIVIL SERVICE ELIGIBILITY ********************* //

        public JsonResult EligibilityInitData(string id)
        {
            try
            {
                //string id = Session["CurSelEIC"].ToString();

                var empElig = db.tPDSEligibilities.Select(e => new
                {

                    eligibilityName = db.tRSPEligibilities.FirstOrDefault(a => a.eligibilityCode == e.eligibilityCode).eligibilityName,
                    e.eligibilityCode,
                    e.controlNo,
                    e.rating,
                    e.examDate,
                    e.examPlace,
                    e.licenseNo,
                    e.validityDate,
                    e.isVerified,
                    e.EIC
                }).Where(e => e.EIC == id).ToList();

                var eligType = db.tRSPEligibilities.Select(e => new
                {
                    e.eligibilityCode,
                    e.eligibilityName,
                    e.orderNo
                }).Where(e => e.orderNo > 0).OrderBy(o => o.orderNo).ToList();

                return Json(new { status = "success", empElig = empElig, eligType = eligType }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        //SAVE ELIGIBILITY
        public JsonResult SaveEligibility(tPDSEligibility data)
        {
            try
            {
                string eEIC = data.EIC;
                tPDSEligibility temp = db.tPDSEligibilities.SingleOrDefault(e => e.EIC == eEIC && e.eligibilityCode == data.eligibilityCode && e.isVerified == 1);

                if (temp != null)
                {

                    if (data.validityDate != null || data.validityDate.Value.Year != 1)
                    {
                        if (Convert.ToDateTime(data.validityDate) == Convert.ToDateTime(temp.validityDate))
                        {
                            return Json(new { status = "Unable to save duplicate data!" }, JsonRequestBehavior.AllowGet);
                        }
                    }

                    if (data.examDate != null || temp.examDate.Value.Year != 1)
                    {
                        if (Convert.ToDateTime(data.examDate) == Convert.ToDateTime(temp.examDate))
                        {
                            return Json(new { status = "Unable to save duplicate data!" }, JsonRequestBehavior.AllowGet);
                        }
                    }


                }


                string uEIC = Session["_EIC"].ToString();
                string tempCode = DateTime.Now.ToString("yyMMddHHmmss") + eEIC.Substring(0, 7) + data.eligibilityCode.Substring(0, 5);

                tPDSEligibility n = new tPDSEligibility();
                n.controlNo = tempCode;
                n.eligibilityCode = data.eligibilityCode;
                n.rating = data.rating;
                n.examDate = data.examDate;
                n.examPlace = data.examPlace;
                n.licenseNo = data.licenseNo;
                n.validityDate = data.validityDate;
                n.EIC = eEIC;
                n.transDT = DateTime.Now;
                n.isVerified = 1;
                n.verifiedByEIC = uEIC;
                db.tPDSEligibilities.Add(n);
                db.SaveChanges();

                var empElig = db.tPDSEligibilities.Select(e => new
                {
                    eligibilityName = db.tRSPEligibilities.FirstOrDefault(a => a.eligibilityCode == e.eligibilityCode).eligibilityName,
                    e.eligibilityCode,
                    e.controlNo,
                    e.rating,
                    e.examDate,
                    e.examPlace,
                    e.licenseNo,
                    e.validityDate,
                    e.isVerified,
                    e.EIC
                }).Where(e => e.EIC == eEIC && e.isVerified >= 0).ToList();

                return Json(new { status = "success", empElig = empElig }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        //UPDATE ELIGIBILITY
        public JsonResult UpdateEligibility(tPDSEligibility data)
        {
            try
            {
                string eEIC = data.EIC;
                tPDSEligibility temp = db.tPDSEligibilities.SingleOrDefault(e => e.EIC == eEIC && e.eligibilityCode == data.eligibilityCode && e.isVerified == 1 && e.controlNo != data.controlNo);

                if (temp != null)
                {
                    if (Convert.ToDateTime(temp.examDate) == Convert.ToDateTime(data.examDate).Date || Convert.ToDateTime(temp.validityDate) == Convert.ToDateTime(data.validityDate).Date)
                    {
                        return Json(new { status = "Unable to save duplicate data!" }, JsonRequestBehavior.AllowGet);
                    }
                }

                tPDSEligibility n = db.tPDSEligibilities.SingleOrDefault(e => e.controlNo == data.controlNo && e.EIC == data.EIC);
                if (n != null)
                {
                    n.eligibilityCode = data.eligibilityCode;
                    n.rating = data.rating;
                    n.examDate = data.examDate;
                    n.examPlace = data.examPlace;
                    n.licenseNo = data.licenseNo;
                    n.validityDate = data.validityDate;
                    db.SaveChanges();

                    var empElig = db.tPDSEligibilities.Select(e => new
                    {
                        eligibilityName = db.tRSPEligibilities.FirstOrDefault(a => a.eligibilityCode == e.eligibilityCode).eligibilityName,
                        e.eligibilityCode,
                        e.controlNo,
                        e.rating,
                        e.examDate,
                        e.examPlace,
                        e.licenseNo,
                        e.validityDate,
                        e.isVerified,
                        e.EIC
                    }).Where(e => e.EIC == eEIC && e.isVerified >= 0).ToList();

                    return Json(new { status = "success", empElig = empElig }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { status = "Hacker Alert" }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception)
            {
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }


        //VALIDATE ELIGIBILITY
        public JsonResult ValidateEligibility(tPDSEligibility data)
        {
            string uEIC = Session["_EIC"].ToString();
            
            tPDSEligibility elig = db.tPDSEligibilities.SingleOrDefault(e => e.controlNo == data.controlNo && e.isVerified == null);
            if (elig == null)
            {
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }

            if (data.isVerified == 1)
            {
                elig.isVerified = 1;
                elig.remarks = "";
            }
            else
            {
                elig.isVerified = 0;
                elig.remarks = data.remarks;
            }

            //elig.verifiedDate = DateTime.Now;
            elig.verifiedByEIC = uEIC;
            db.SaveChanges();


            return Json(new { status = "success" }, JsonRequestBehavior.AllowGet);
        }

        //DELETE
        //DeleteEligibility

        [HttpPost]
        //DELETE ELIGIBILITY
        public JsonResult DeleteEligibility(tPDSEligibility data)
        {
            try
            {
                string eEIC = data.EIC;
                tPDSEligibility n = db.tPDSEligibilities.SingleOrDefault(e => e.controlNo == data.controlNo && e.EIC == data.EIC);
                if (n != null)
                {
                    n.isVerified = -2;
                    db.SaveChanges();

                    var empElig = db.tPDSEligibilities.Select(e => new
                    {
                        eligibilityName = db.tRSPEligibilities.FirstOrDefault(a => a.eligibilityCode == e.eligibilityCode).eligibilityName,
                        e.eligibilityCode,
                        e.controlNo,
                        e.rating,
                        e.examDate,
                        e.examPlace,
                        e.licenseNo,
                        e.validityDate,
                        e.isVerified,
                        e.EIC
                    }).Where(e => e.EIC == eEIC && e.isVerified >= 0).ToList();

                    return Json(new { status = "success", empElig = empElig }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { status = "Hacker Alert" }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception)
            {
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }


        //Get Employee PDS Training
        [HttpPost]
        public JsonResult GetEmpPDSTraining(string id)        
        {
            try
            {
                IEnumerable<tPDSTraining> trainingList = db.tPDSTrainings.Where(e => e.EIC == id).OrderBy(o => o.dateFrom).ToList();

                return Json(new { status = "success", trainingList = trainingList }, JsonRequestBehavior.AllowGet);
             }
            catch (Exception)
            {              
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>

        //[HttpPost]
        //public JsonResult GetTrainings()
        //{
        //    try
        //    {
        //        return Json(new { status = "success" }, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        string errMsg = ex.ToString();
        //        return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
        //    }
        //}


        //private JsonResult PDSTrainings()
        //{
        //    try
        //    {
                
        //        return Json(new { status = "success" }, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        string errMsg = ex.ToString();
        //        return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
        //    }
        //}


        [HttpPost]
        public JsonResult ApplicationProfileEducation(string id)
        {
            try
            {
                tRSPApplication app = db.tRSPApplications.Single(e => e.applicationCode == id);
                return Json(new { status = "success" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string errMsg = ex.ToString();
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        public JsonResult MyDataID(string id)
        {
            try
            {
                tRSPEmployee emp = db.tRSPEmployees.SingleOrDefault(e => e.EIC == id);
                if (emp != null)
                {

                    string idNo = Convert.ToInt16(emp.idNo).ToString("0000");
                    aaTable dataId = db.aaTables.SingleOrDefault(e => e.emp_idno == idNo && e.lastname == emp.lastName && e.firstname == emp.firstName);
                    if (dataId != null)
                    {

                        return Json(new { status = "success", dataId = dataId }, JsonRequestBehavior.AllowGet);
                    }
                }

                return Json(new { status = "none" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string errMsg = ex.ToString();
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }




    }
}