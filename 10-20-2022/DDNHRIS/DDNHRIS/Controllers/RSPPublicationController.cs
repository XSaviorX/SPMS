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
    public class RSPPublicationController : Controller
    {

        HRISDBEntities db = new HRISDBEntities();

        // GET: RSPPublication
        public ActionResult Publication()
        {
            return View();
        }

        [HttpPost]
        public JsonResult PublicationInitData()
        {
            try
            {
                var pubList = db.tRSPPublications.Select(e => new
                {
                    e.recNo,
                    e.publicationCode,
                    e.publicationDate,
                    e.CSCPostedDate,
                    e.CSCClosingDate,
                    e.userEIC,
                    e.transDT,
                    tag = e.CSCClosingDate <= DateTime.Now ? 2 : e.tag,
                    Posnum = db.tRSPPublicationItems.Count(a => a.publicationCode == e.publicationCode)
                }).OrderByDescending(e => e.transDT).ThenBy(e => e.tag);
                return Json(new { status = "success", pubList = pubList }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult PublicationData(string id)
        {
            try
            {
                tRSPPublication data = db.tRSPPublications.SingleOrDefault(e => e.publicationCode == id);
                if (data == null)
                {
                    return Json(new { status = "HACKER ALERT" }, JsonRequestBehavior.AllowGet);
                }
                IEnumerable<vRSPPublicationItem> list = db.vRSPPublicationItems.Where(e => e.publicationCode == id).OrderBy(o => o.itemNo).ToList();
                string tmp = Convert.ToDateTime(data.publicationDate).ToString("MMMM dd, yyyy");
                return Json(new { status = "success", pubData = data, pubItemList = list, pubDate = tmp }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult UpdatePublicationData(tRSPPublication data)
        {
            try
            {
                tRSPPublication pub = db.tRSPPublications.SingleOrDefault(e => e.publicationCode == data.publicationCode);

                if (pub == null)
                {
                    return Json(new { status = "Hacker Alert!" }, JsonRequestBehavior.AllowGet);
                }
                if (data.CSCPostedDate != null && data.CSCPostedDate.Value.Year != 1)
                {
                    pub.CSCPostedDate = data.CSCPostedDate;
                }
                if (data.CSCClosingDate != null && data.CSCClosingDate.Value.Year != 1)
                {
                    pub.CSCClosingDate = data.CSCClosingDate;
                }
                db.SaveChanges();

                return Json(new { status = "success" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Create()
        {
            Session["PublicationList"] = null;
            return View();
        }

        public class TempPositionVacantList
        {
            public string plantillaCode { get; set; }
            public string itemNo { get; set; }
            public string positionTitle { get; set; }
            public string subPositionTitle { get; set; }
            public int salaryGrade { get; set; }
            public decimal salaryRate { get; set; }
            public string shortDepartmentName { get; set; }
            public string remarks { get; set; }
            public string placeOfAssignment { get; set; }
            public int daysLeftCount { get; set; }
            public int hasExpired { get; set; }

        }

        [HttpPost]
        public JsonResult PlantillaVacant()
        {
            try
            {
                DateTime dt = DateTime.Now.AddDays(-1);
                DateTime tmpDate = DateTime.Now.AddDays(-360);
                IEnumerable<vRSPPublicationItemLast> lastPub = db.vRSPPublicationItemLasts.Where(e => e.CSCClosingDate >= tmpDate).ToList();

                IEnumerable<vRSPPlantilla> plantilla = db.vRSPPlantillas.Where(e => e.isFunded == true).OrderBy(o => o.newItemNo).ToList();
                //VACANT LIST
                IEnumerable<vRSPPlantilla> vacantList = plantilla.Where(e => e.EIC == null).ToList();
                List<TempPositionVacantList> myList = new List<TempPositionVacantList>();

                foreach (vRSPPlantilla item in vacantList)
                {
                    string remarks = "";
                    vRSPPublicationItemLast lastData = lastPub.SingleOrDefault(e => e.plantillaCode == item.plantillaCode);
                    int expireTag = 1;
                    if (lastData != null)
                    {
                        remarks = Convert.ToDateTime(lastData.CSCClosingDate).ToString("MM/dd/yyyy");
                        if (lastData.CSCPostedDate == null)
                        {
                            expireTag = 2;
                        }
                        else
                        {
                            DateTime tmpD = lastData.CSCClosingDate.Value.Date;
                            if (tmpD < dt)
                            {
                                expireTag = 1;
                            }
                            else
                            {
                                expireTag = 0;
                            }
                        }
                    }


                    myList.Add(new TempPositionVacantList()
                    {
                        plantillaCode = item.plantillaCode,
                        itemNo = Convert.ToInt16(item.itemNo).ToString("0000"),
                        positionTitle = item.positionTitle,
                        subPositionTitle = item.subPositionTitle,
                        salaryGrade = Convert.ToInt16(item.salaryGrade),
                        salaryRate = Convert.ToDecimal(item.rateMonth),
                        shortDepartmentName = item.shortDepartmentName,
                        daysLeftCount = 0,
                        remarks = remarks,
                        hasExpired = expireTag,
                        placeOfAssignment = ""
                    });
                }


                //FORCASTED               
                IEnumerable<vRSPPublicationVacantForecast> forecasted = db.vRSPPublicationVacantForecasts.Where(e => e.effectiveDate > dt).ToList();

                foreach (vRSPPublicationVacantForecast item in forecasted)
                {
                    vRSPPlantilla p = plantilla.SingleOrDefault(e => e.plantillaCode == item.plantillaCode);
                    if (p != null && p.EIC == item.EIC)
                    {
                        myList.Add(new TempPositionVacantList()
                        {
                            plantillaCode = item.plantillaCode,
                            itemNo = Convert.ToInt16(item.itemNo).ToString("0000"),
                            positionTitle = item.positionTitle,
                            subPositionTitle = item.subPositionTitle,
                            salaryGrade = Convert.ToInt16(p.salaryGrade),
                            salaryRate = Convert.ToDecimal(p.rateMonth),
                            shortDepartmentName = p.shortDepartmentName,
                            daysLeftCount = 0,
                            remarks = "(F)",
                            hasExpired = 1,
                            placeOfAssignment = ""
                        });
                    }
                }

                //IEnumerable<vRSPPublicationItemLast> lastPub = db.vRSPPublicationItemLasts.ToList();
                //IEnumerable<vRSPPlantillaPublication> list = db.vRSPPlantillaPublications.Where(e => e.salaryGrade > 0).ToList();
                //foreach (vRSPPlantillaPublication item in list)
                //{
                //    string remarks = "";
                //    vRSPPublicationItemLast lastData = lastPub.SingleOrDefault(e => e.plantillaCode == item.plantillaCode);
                //    int expireTag = 0;
                //    if (lastData != null)
                //    {                      
                //        if (lastData.CSCPostedDate == null)
                //        {
                //            expireTag = 2;
                //        }
                //        else
                //        {
                //            DateTime dt = DateTime.Now;
                //            DateTime tmpD = lastData.CSCPostedDate.Value.Date.AddDays(90);
                //            remarks = tmpD.ToString("MM/dd/yyyy");
                //            if (tmpD < dt)
                //            {
                //                expireTag = 1;
                //            }                             
                //        }
                //    }

                //    myList.Add(new TempPositionVacantList()
                //    {
                //        plantillaCode = item.plantillaCode,
                //        itemNo = Convert.ToInt16(item.itemNo),
                //        positionTitle = item.positionTitle,
                //        subPositionTitle = item.subPositionTitle,
                //        salaryGrade = Convert.ToInt16(item.salaryGrade),
                //        salaryRate = Convert.ToDecimal(item.rateMonth),
                //        shortDepartmentName = item.shortDepartmentName,
                //        daysLeftCount = 10,
                //        remarks = remarks,
                //        hasExpired = expireTag
                //    });
                //}

                //myList = myList.Where(e => e.hasExpired != 0).ToList();
                int expCount = myList.Where(e => e.hasExpired == 1).Count();
                Session["_TempTable"] = myList.OrderBy(o => o.itemNo).ToList();
                return Json(new { status = "success", vacantList = myList, expiredCount = expCount }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string msg = ex.ToString();
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult SaveExpiredPublication(string id)
        {
            try
            {
                var tempList = Session["_TempTable"];
                List<TempPositionVacantList> list = tempList as List<TempPositionVacantList>;
                list = list.Where(e => e.hasExpired == 1).ToList();
                int counter = 1;
                foreach (TempPositionVacantList item in list)
                {
                    string tmpCode = id.Substring(0, 22) + Convert.ToInt16(item.itemNo).ToString("0000") + counter.ToString("000");
                    tRSPPublicationItem n = new tRSPPublicationItem();
                    n.publicationItemCode = tmpCode;
                    n.publicationCode = id;
                    n.plantillaCode = item.plantillaCode;
                    db.tRSPPublicationItems.Add(n);
                    counter = counter + 1;
                }
                db.SaveChanges();
                return Json(new { status = "success" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult AddPositionToPub(string id, string code)
        {
            try
            {
                string uEIC = Session["_EIC"].ToString();
                string tmpCode = "PUB" + DateTime.Now.ToString("yyMMddHHmmssfff") + code.Substring(code.Length - 4, 4) + uEIC.Substring(0, 3);

                tRSPPublicationItem p = new tRSPPublicationItem();
                p.publicationItemCode = tmpCode;
                p.publicationCode = id;
                p.plantillaCode = code;
                p.tag = 0;
                db.tRSPPublicationItems.Add(p);
                db.SaveChanges();
                return Json(new { status = "success" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string errMsg = ex.ToString();
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]

        public JsonResult DeletePublicationItem(string id)
        {
            try
            {
                string uEIC = Session["_EIC"].ToString();
                vRSPPublicationItem pubItem = db.vRSPPublicationItems.Single(e => e.publicationItemCode == id);
                tRSPPublicationItem del = db.tRSPPublicationItems.Single(e => e.publicationItemCode == id);
                db.tRSPPublicationItems.Remove(del);
                db.SaveChanges();
                IEnumerable<vRSPPublicationItem> list = db.vRSPPublicationItems.Where(e => e.publicationCode == pubItem.publicationCode).OrderBy(o => o.itemNo).ToList();
                return Json(new { status = "success", pubItemList = list }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string errMsg = ex.ToString();
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult ViewPositionQS(string id)
        {
            vRSPPositionQ qs = db.vRSPPositionQS.SingleOrDefault(e => e.plantillaCode == id);
            if (qs != null)
            {
                return Json(new { status = "success", QSData = qs }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { status = "Hacker Alert!" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult UpdatePositionQS(tRSPPositionQ data)
        {
            try
            {
                tRSPPositionQ qs = db.tRSPPositionQS.SingleOrDefault(e => e.plantillaCode == data.plantillaCode);
                if (qs != null)
                {
                    qs.QSEducation = data.QSEducation;
                    qs.QSExperience = data.QSExperience;
                    qs.QSTraining = data.QSTraining;
                    qs.QSEligibility = data.QSEligibility;
                    qs.QSEligibilityPub = data.QSEligibilityPub;
                    qs.QSNotation = data.QSNotation;
                    db.SaveChanges();
                    return Json(new { status = "success" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { status = "Invalid operation!" }, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex)
            {
                string errMsg = ex.ToString();
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }
        //[HttpPost]
        //public JsonResult AddPublicationItem(string id)
        //{
        //    try
        //    {

        //        var tempList = Session["PublicationList"];
        //        List<vRSPPublicationItem> list = tempList as List<vRSPPublicationItem>;

        //        vRSPPlantillaPublication pubItem = db.vRSPPlantillaPublications.Single(e => e.plantillaCode == id);
        //        tRSPPositionQ qs = db.tRSPPositionQS.Single(e => e.plantillaCode == id);

        //        string tmp = Convert.ToInt16(pubItem.itemNo).ToString("0000") + " - " + pubItem.positionTitle;
        //        if (pubItem.subPositionCode != null)
        //        {
        //            tmp = tmp + " (" + pubItem.subPositionTitle + ")";
        //        }

        //        List<vRSPPublicationItem> pubItemList = new List<vRSPPublicationItem>();

        //        if (list != null)
        //        {
        //            pubItemList = list.ToList();
        //        }

        //        int count = pubItemList.Where(e => e.plantillaCode == id).Count();
        //        if (count >= 1)
        //        {
        //            return Json(new { status = "Position already in the list!" }, JsonRequestBehavior.AllowGet);
        //        }

        //        pubItemList.Add(new vRSPPublicationItem()
        //        {
        //            plantillaCode = pubItem.plantillaCode,
        //            itemNo = pubItem.itemNo,
        //            positionTitle = tmp,
        //            salaryGrade = pubItem.salaryGrade,
        //            rateMonth = pubItem.rateMonth,
        //            QSEducation = qs.QSEducation,
        //            QSExperience = qs.QSExperience,
        //            QSTraining = qs.QSTraining,
        //            QSEligibility = qs.QSEligibility,
        //            placeOfAssignment = pubItem.placeOfAssignment
        //        });

        //        Session["PublicationList"] = pubItemList;

        //        return Json(new { status = "success", list = pubItemList }, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
        //    }
        //}


        [HttpPost]
        public JsonResult SavePublication(tRSPPublication data)
        {
            try
            {
                              
                string uEIC = Session["_EIC"].ToString();
                string tmpCode = "PUB" + DateTime.Now.ToString("yyMMddHHmmssfff");
                tmpCode = tmpCode + uEIC.Substring(0, 7);
                tRSPPublication pub = new tRSPPublication();
                pub.publicationCode = tmpCode;
                pub.publicationDate = Convert.ToDateTime(data.publicationDate);
                pub.branch = data.branch;
                pub.userEIC = uEIC;
                pub.transDT = DateTime.Now;
                pub.tag = 0;
                db.tRSPPublications.Add(pub);
                db.SaveChanges();

                var pubList = db.tRSPPublications.Select(e => new
                {
                    e.recNo,
                    e.publicationCode,
                    e.publicationDate,
                    e.CSCPostedDate,
                    e.CSCClosingDate,
                    e.userEIC,
                    e.transDT,
                    tag = e.CSCClosingDate <= DateTime.Now ? 2 : e.tag,
                    Posnum = db.tRSPPublicationItems.Count(a => a.publicationCode == e.publicationCode)
                }).OrderByDescending(e => e.transDT).ThenBy(e => e.tag);

                string tmp = Convert.ToDateTime(data.publicationDate).ToString("MMMM dd, yyyy");
                return Json(new { status = "success", pubDate = tmp }, JsonRequestBehavior.AllowGet);
                 
            }
            catch (Exception)
            {
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        public JsonResult SubmitPublication(tRSPPublication data)
        {
            try
            {

                tRSPPublication pub = new tRSPPublication();
                string uEIC = Session["_EIC"].ToString();


                var tempList = Session["PublicationList"];
                List<vRSPPublicationItem> list = tempList as List<vRSPPublicationItem>;

                string tmpCode = "PUB" + DateTime.Now.ToString("yyMMddHHmmssfff");

                tmpCode = tmpCode + uEIC.Substring(0, 7);

                pub.publicationCode = tmpCode;
                pub.publicationDate = data.publicationDate;
                if (data.CSCPostedDate != null)
                {
                    pub.CSCPostedDate = data.CSCPostedDate;
                }
                if (data.CSCClosingDate != null)
                {
                    pub.CSCClosingDate = data.CSCClosingDate;
                }
                pub.userEIC = uEIC;
                pub.transDT = DateTime.Now;
                pub.tag = 0;


                int counter = 0;

                foreach (vRSPPublicationItem itm in list)
                {
                    counter = counter + 1;
                    string code = tmpCode.Substring(0, 21);
                    code = code + String.Format("{0:0000}", counter);

                    tRSPPublicationItem p = new tRSPPublicationItem();
                    p.publicationItemCode = code;
                    p.publicationCode = tmpCode;
                    p.plantillaCode = itm.plantillaCode;
                    db.tRSPPublicationItems.Add(p);

                }

                db.tRSPPublications.Add(pub);
                db.SaveChanges();

                return Json(new { status = "success" }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception)
            {
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult PrintPublication(string id)
        {
            try
            {
                tRSPPublication pub = db.tRSPPublications.SingleOrDefault(e => e.publicationCode == id);
                if (pub != null)
                {
                    Session["ReportType"] = "PUBLICATION";
                    Session["PrintReport"] = id;
                    return Json(new { status = "success" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    //ADD -HACKERALERT-
                    return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception)
            {
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }

        
        [HttpPost]
        public JsonResult PrintPublicationJD(string id)
        {
            try
            {
                tRSPPublication pub = db.tRSPPublications.SingleOrDefault(e => e.publicationCode == id);
                if (pub != null)
                {
                    Session["ReportType"] = "PUBLICATION_JD";
                    Session["PrintReport"] = id;
                    return Json(new { status = "success" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    //ADD -HACKERALERT-
                    return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception)
            {
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }


        public class TempJobDescription
        {
            public string plantillaCode { get; set; }
            public string jobDescCode { get; set; }
            public string jobDesc { get; set; }
            public int percentage { get; set; }
            public int jobSeqNo { get; set; }
            public List<tRSPPositionJobDescSub> subJobDesc { get; set; }
        }



        [HttpPost]
        public JsonResult ViewPositionJobDesc(string id)
        {
            try
            {

                IEnumerable<tRSPPositionJobDesc> jobDesc = db.tRSPPositionJobDescs.Where(e => e.plantillaCode == id).ToList();


                foreach (tRSPPositionJobDesc item in jobDesc)
                {
                    List<tRSPPositionJobDescSub> sub = new List<tRSPPositionJobDescSub>();
                    if (item.jobDescCode != null)
                    {
                        //sub = db.tRSPPositionJobDescSubs.Where(e => e.jobDescCode == item.jobDescCode).ToList();

                    }
                }




                return Json(new { status = "success", list = jobDesc }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]

        public JsonResult SaveJobDescription(tRSPPositionJobDesc data)
        {
            try
            {
                string code = "JD" + DateTime.Now.ToString("yyMMddHHmmssfff");
                tRSPPositionJobDesc jd = new tRSPPositionJobDesc();
                jd.plantillaCode = data.plantillaCode;
                jd.jobDescCode = code;
                jd.jobDesc = data.jobDesc;
                jd.percentage = data.percentage;
                db.tRSPPositionJobDescs.Add(jd);
                //db.SaveChanges();                                

                IEnumerable<tRSPPositionJobDesc> jobDesc = db.tRSPPositionJobDescs.Where(e => e.plantillaCode == data.plantillaCode).ToList();

                return Json(new { status = "success", list = jobDesc }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }

      



    }
}